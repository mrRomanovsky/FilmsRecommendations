﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FilmsRecommendations
{
    public enum Type {None, Sentence, SentConnSent, AtomicSent,
                      QuantVar, QuantSent, PredTerm,
                      Term, Constant, Variable, Arg,
                      Connective, Quantifier, Predicate };

    public class FilmKnowledgeBase
    {
        private string pathToKB;

        public FilmKnowledgeBase(string pathToKB)
        {
            this.pathToKB = pathToKB;
          //  Sentences = ParseSentencesFromFile(pathToKB);
        }

        public List<ISentence> Sentences { get; set; } 

        private static Substitution PredicateUnify(Predicate pred1, Predicate pred2)
        {
            var res = new Substitution();
            res.Successful = false;
            
            if (pred1.PredicateName != pred2.PredicateName)
                return res;

            for (int i = 0; i < pred1.Terms.Count; i++)
            {
                var t1 = pred1.Terms[i];
                var t2 = pred2.Terms[i];
                if (t1.IsConstant && t2.IsConstant)
                {
                    if (t1.Value != t2.Value)
                        return res;
                    else
                        continue;
                }

                if (t1.IsConstant && !t2.IsConstant)
                {
                    //на самом деле надо идти по словарю по предкам до первой встреченной консстанты или пустоты
                    if (!res.SubsitutionDict.ContainsKey(t2.Value))
                    {
                        res.SubsitutionDict.Add(t2.Value, t1.Value);
                        continue;
                    }
                    else
                        return res;
                }

                if (!t1.IsConstant)
                {
                    if (t2.IsConstant)
                    {
                        if (!res.SubsitutionDict.ContainsKey(t1.Value))
                        {
                            res.SubsitutionDict.Add(t1.Value, t2.Value);
                            continue;
                        }
                        else
                            return res;
                    }
                    else
                    {
                        if (!res.SubsitutionDict.ContainsKey(t1.Value))
                        {
                            res.SubsitutionDict.Add(t1.Value, t2.Value);
                            continue;
                        }
                        if (!res.SubsitutionDict.ContainsKey(t2.Value))
                        {
                            res.SubsitutionDict.Add(t2.Value, t1.Value);
                            continue;
                        }
                    }
                    //if (char.IsUpper(res.SubsitutionDict[t1.Value][0]))
                    //        //if (t2.Value != res.SubsitutionDict[t1.Value])
                    //        //    return res;
                    //        //else
                    return res;
                }
            }
            res.Successful = true;
            return res;
        }

        public static Substitution Unify(ISentence sentence1, ISentence sentence2)
        {
            var res = new Substitution();
            res.Successful = false;
            switch (sentence1.GetSentenceType())
            {
                case SentenceType.Predicate:
                    if (sentence2.GetSentenceType() == SentenceType.QuantifierVariableSentence)
                    {
                        var qvs2 = sentence2 as QuantifierVariableSentence;
                        if (qvs2.Quantifier != Quantifier.Exists)
                            break;
                        return Unify(sentence1, qvs2.Sentence);
                    }
                    if (sentence2.GetSentenceType() == SentenceType.SentenceConnectiveSentence)
                        break;
                    var pred1 = sentence1 as Predicate;
                    var pred2 = sentence2 as Predicate;
                    return PredicateUnify(pred1, pred2);

                case SentenceType.SentenceConnectiveSentence:
                    if (sentence1.GetSentenceType() != sentence2.GetSentenceType())
                        break;
                    var scs1 = sentence1 as SentenceConnectiveSentence;
                    var scs2 = sentence1 as SentenceConnectiveSentence;
                    return Unify(scs1.Sentence1, scs2.Sentence1).Compose(Unify(scs1.Sentence2, scs2.Sentence2));

                case SentenceType.QuantifierVariableSentence:
                    var qvs1 = sentence1 as QuantifierVariableSentence;
                    if (qvs1.Quantifier == Quantifier.Exists)
                        return Unify(qvs1.Sentence, sentence2);
                    break;

                default:
                    return res;


            }
            return res;
        }

        #region ForwardChain
        public static void ForwardChain(FilmKnowledgeBase kb, ISentence sentence)
        {
            foreach (var kbSentence in kb.Sentences)
                if (IsRenaiming(kbSentence, sentence))
                    return;
            kb.Sentences.Add(sentence);
            foreach (var kbSentence in kb.Sentences)
            {
                var innerSentence = DropOuterQuantifiers(kbSentence);
                if (innerSentence.GetSentenceType() == SentenceType.SentenceConnectiveSentence)
                {
                    var sentenceConnectiveSentence = innerSentence as SentenceConnectiveSentence;
                    if (sentenceConnectiveSentence.Connective == "->")
                    {
                        var anticedents = SentenceConnectiveSentence.GetAnticedents(sentenceConnectiveSentence);
                        for (int i = 0; i < anticedents.Count; ++i)
                        {
                            var unificationResult = Unify(anticedents[i], sentence);
                            if (unificationResult.Successful)
                                FindAndInfer(kb, anticedents.Take(i).Concat(anticedents.Skip(i + 1)).ToList(), //dropping unified sentence
                                    sentenceConnectiveSentence.Sentence2, unificationResult);
                        }
                    }
                }
            }
        }

        public static void FindAndInfer(FilmKnowledgeBase kb, List<ISentence> premises, ISentence conclusion, Substitution s)
        {
            if (premises.Count == 0)
                ForwardChain(kb, conclusion.Substitute(s));
            foreach (var kbSentence in kb.Sentences)
            {
                var unifyResult = Unify(kbSentence, premises.First().Substitute(s));
                if (unifyResult.Successful)
                {
                    FindAndInfer(kb, premises.Skip(1).ToList(), conclusion, s.Compose(unifyResult));
                }
            }
        }

        public static ISentence DropOuterQuantifiers(ISentence sentence)
        {
            switch (sentence.GetSentenceType())
            {
                case SentenceType.Predicate:
                    return sentence;
                case SentenceType.SentenceConnectiveSentence:
                    return sentence;
                default: //QuantifierVariableSentence
                    var quantifiedSentence = sentence as QuantifierVariableSentence;
                    if (quantifiedSentence.Quantifier == Quantifier.All)
                        return DropOuterQuantifiers(quantifiedSentence.Sentence);
                    else
                        return sentence;
            }
        }

        public static bool IsRenaiming(ISentence sentence1, ISentence sentence2)
        {
            var unification = Unify(sentence1, sentence2);
            if (!unification.Successful)
                return false;
            foreach (var val in unification.SubsitutionDict.Values)
                if (Char.IsUpper(val[0]))
                    return false;
            return true;
        }
        #endregion

        #region BackwardChain
        public Substitution BackwardChain(ISentence sentence)
        {
            var q = new Queue<ISentence>();
            q.Enqueue(sentence);
            return BackChainList(q, new Substitution());
        }

        private Substitution BackChainList(Queue<ISentence> sentences, Substitution substitution)
        {
            Substitution answer = new Substitution();
            if (sentences.Count == 0)
                return substitution;
            var q = sentences.Dequeue();
            foreach (var sen in Sentences)
            {
                var sub = Unify(q, sen);
                if (sub.Successful)
                    substitution.Compose(sub);
            }
            foreach (var sen in Sentences
                .Select(s => DropOuterQuantifiers(s))
                .Where(s => s.GetSentenceType() == SentenceType.SentenceConnectiveSentence && (s as SentenceConnectiveSentence).Connective == "->")
                .Select(s => s as SentenceConnectiveSentence))
            {
                var sub2 = Unify(q, sen.Sentence2);
                if (sub2.Successful)
                {
                    var qq = new Queue<ISentence>( SentenceConnectiveSentence.GetAnticedents(sen));
                    answer.Compose(BackChainList(new Queue<ISentence>(qq.Select(x => x.Substitute(sub2))), substitution.Compose(sub2)));
                }
            }
            return answer.Compose(BackChainList(sentences, substitution));
        }
        #endregion
    }

    public class Substitution
    {
        public Dictionary<string, string> SubsitutionDict { get; set; }

        public bool Successful { get; set; }

        public Substitution Compose(Substitution other)
        {
            var substitutionComposed = new Substitution();
            foreach (var dictElem in SubsitutionDict)
                substitutionComposed.SubsitutionDict[dictElem.Key] = dictElem.Value;

            if (!Successful || !other.Successful)
            {
                substitutionComposed.Successful = false;
                return substitutionComposed;
            }

            foreach (var otherDictElem in other.SubsitutionDict)
            {
                if (SubsitutionDict.ContainsKey(otherDictElem.Key))
                {
                    var thisValue = SubsitutionDict[otherDictElem.Key];
                    if (Char.IsUpper(thisValue[0]) && otherDictElem.Value != thisValue)
                    {
                        substitutionComposed.Successful = false;
                        return substitutionComposed;
                    }
                    substitutionComposed.SubsitutionDict[otherDictElem.Key] = otherDictElem.Value;
                }
                else
                    substitutionComposed.SubsitutionDict[otherDictElem.Key] = otherDictElem.Value;
            }

            substitutionComposed.Successful = true;
            return substitutionComposed;
        }
    }

}
