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
        public static Dictionary<ISentence, double> sentenceConfidence;
        //public static Dictionary<ISentence, Tuple<List<ISentence>, ISentence>> inferenceChain;
        public static Dictionary<ISentence, string> proofs = new Dictionary<ISentence, string>();
        public static double minSentence;

        public FilmKnowledgeBase(string pathToKB)
        {
            sentenceConfidence = new Dictionary<ISentence, double>();
            //inferenceChain = new Dictionary<ISentence, Tuple<List<ISentence>, ISentence>>();
            minSentence = 1;

            this.pathToKB = pathToKB;
            Sentences = new List<ISentence>();
            foreach (var sent in System.IO.File.ReadAllLines(pathToKB))
                AddSentence(ParseSentence(sent));
        }

        public List<ISentence> Sentences { get; set; } 

        public ISentence ParseSentence(string sentence, double coef = -1)
        {
            var splitedSentence = sentence.Split(' ');
            if (coef < 0)
                coef = double.Parse(splitedSentence[1]);
            if (sentence[0] == 'V' || sentence[0] == 'E')
                return ParseQuantifiedSentence(splitedSentence[0], coef);
            if (sentence[0] == '(')
                return ParseSentenceConnectiveSentence(splitedSentence[0], coef);
            return ParsePredicate(splitedSentence[0], coef);
        }

        public void AddSentence(ISentence sentence)
        {
            Sentences.Add(sentence);
        }

        #region parsingSentences
        private ISentence ParseQuantifiedSentence(string sentence, double coefficient)
        {
            var quantifiedSentence = new QuantifierVariableSentence();
            if (sentence[0] == 'V')
                quantifiedSentence.Quantifier = Quantifier.All;
            else
                quantifiedSentence.Quantifier = Quantifier.Exists;
            int openBracketIdx = 3;
            int closingBracketIdx = sentence.LastIndexOf(')');
            quantifiedSentence.Variable = sentence[1].ToString();
            int innerSentenceStart = openBracketIdx + 1;
            quantifiedSentence.Sentence = ParseSentence(sentence.Substring(innerSentenceStart, closingBracketIdx - innerSentenceStart), coefficient);

            sentenceConfidence[quantifiedSentence] = coefficient;
            return quantifiedSentence;
        }

        private SentenceConnectiveSentence ParseSentenceConnectiveSentence(string sentence, double coefficient)
        {
            if (sentence[0] != '(')
                throw new Exception("Trying to parse SentenceConnectiveSentence: '(' expected!");
            var sentenceConnSentence = new SentenceConnectiveSentence();
            int openBracketsCount = 1;
            int closingBracketsCount = 0;
            int currPos = 1;
            while (true)
            {
                if (sentence[currPos] == '(')
                    ++openBracketsCount;
                else if (sentence[currPos] == ')')
                    ++closingBracketsCount;
                ++currPos;
                if (openBracketsCount == closingBracketsCount)
                    break;
            }

            sentenceConnSentence.Sentence1 = ParseSentence(sentence.Substring(1, currPos - 1 - 1), coefficient);
            int connectiveEndIdx = sentence.IndexOf('(', currPos);
            sentenceConnSentence.Connective = sentence.Substring(currPos, connectiveEndIdx - currPos);//sentence[currPos].ToString();
            sentenceConnSentence.Sentence2 =
                ParseSentence(sentence.Substring(connectiveEndIdx + 1, sentence.Length - (connectiveEndIdx + 1) - 1), coefficient);

            sentenceConfidence[sentenceConnSentence] = coefficient;
            return sentenceConnSentence;
        }

        private Predicate ParsePredicate(string sentence, double coefficient)
        {
            var pred = new Predicate();
            int openBracketIdx = sentence.IndexOf('(');
            int closingBracketIdx = openBracketIdx + 1;
            while (sentence[closingBracketIdx] != ')')
                ++closingBracketIdx;

            var argsStartIdx = openBracketIdx + 1;
            pred.PredicateName = sentence.Substring(0, openBracketIdx);
            foreach (var argument in sentence.Substring(argsStartIdx, closingBracketIdx - argsStartIdx).Split(','))
                pred.Terms.Add(new Term(argument));

            sentenceConfidence[pred] = coefficient;
            return pred;
        }
        #endregion

        #region Unify

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
                    var scs2 = sentence2 as SentenceConnectiveSentence;
                    if (scs1.Connective != scs2.Connective)
                        break;
                    return Unify(scs1.Sentence1, scs2.Sentence1).Compose(Unify(scs1.Sentence2, scs2.Sentence2));

                case SentenceType.QuantifierVariableSentence:
                    var qvs1 = sentence1 as QuantifierVariableSentence;
                    if (qvs1.Quantifier == Quantifier.Exists)
                        return Unify(qvs1.Sentence, sentence2);
                    if (sentence2.GetSentenceType() == SentenceType.QuantifierVariableSentence)
                    {
                        var qvs2 = sentence2 as QuantifierVariableSentence;
                        if (qvs1.Quantifier == qvs2.Quantifier)
                            return Unify(qvs1.Sentence, qvs2.Sentence);
                        break;
                    }

                    break;

                default:
                    return res;
            }
            return res;
        }
        #endregion 

        #region ForwardChain
        public static void ForwardChain(FilmKnowledgeBase kb, ISentence sentence, double coefficientConfidence = 1, string prove = "")
        {
            foreach (var kbSentence in kb.Sentences)
                if (IsRenaiming(kbSentence, sentence))
                    return;

            if (prove == "")
                prove = "proven " + sentence.ToString();
            System.IO.File.AppendAllText("explanationPath.txt", "proven: " + sentence + ":\n)");
            proofs[sentence] = prove;
            kb.Sentences.Add(sentence);
            sentenceConfidence[sentence] = coefficientConfidence;

            int sentIdx = 0;
            while (sentIdx < kb.Sentences.Count)
            {
                var kbSentence = kb.Sentences[sentIdx];
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
                            {
                                FindAndInfer(kb, anticedents.Take(i).Concat(anticedents.Skip(i + 1)).ToList(), //dropping unified sentence
    sentenceConnectiveSentence.Sentence2, unificationResult, 1, sentenceConfidence[sentenceConnectiveSentence]);
                                //inferenceChain[sentenceConnectiveSentence.Sente2] = new Tuple<List<ISentence>, ISentence>(new List<ISentence>(), );
                                //inferenceChain[]
                            }

                        }
                    }
                }
                ++sentIdx;
            }
        }

        public static void FindAndInfer(FilmKnowledgeBase kb, List<ISentence> premises, ISentence conclusion, Substitution s, double minPremiseConf, double ruleFC, string prove = "")
        {
            System.IO.File.AppendAllText("explanationPath.txt", "##########################\n\n\n");
            System.IO.File.AppendAllText("explanationPath.txt", "Trying to prove: " + conclusion.Substitute(s).ToString() + ":\n)");
            if (premises.Count == 0)
            {
                //словарик: <Isentence -> <List<Isentences>(посылки), ISentence(правило)>
                //добавить в словарик правило и все доказанные (с подстановками) посылки
                //при восстановлении будем проходить по всем посылкам и рекурсивно выписывать всё для них
                prove = prove + " | proven " + conclusion.Substitute(s).ToString();
                ForwardChain(kb, conclusion.Substitute(s), minPremiseConf * ruleFC, prove);
                return;
            }
            int sentIdx = 0;
            while (sentIdx < kb.Sentences.Count)
            {
                var kbSentence = kb.Sentences[sentIdx];
                var unifyResult = Unify(kbSentence, premises.First().Substitute(s));
                if (unifyResult.Successful)
                {
                    var premise = kbSentence;
                    var currentPremiseConf = sentenceConfidence[premise];
                    var minConf = Math.Min(currentPremiseConf, minPremiseConf);
                    //inferenceChain[conclusion].Item1.Add(premises.First().Substitute(s));
                    prove = prove + " | proven anticedent: " + kbSentence.Substitute(unifyResult).ToString();
                    FindAndInfer(kb, premises.Skip(1).ToList(), conclusion, s.Compose(unifyResult), minConf, ruleFC, prove);
                }
                ++sentIdx;
            }
        }

        /*public void FixInferenceChain()
        {
            foreach (var chains in inferenceChain)
                foreach (var chains2 in inferenceChain)
                    if (Unify(chains.Key, chains2.Key).Successful)
                    {
                        if (chains.Value.Item1.Count < chains2.Value.Item1.Count)
                        {
                            inferenceChain[chains.Key] = new Tuple<List<ISentence>, ISentence>(chains2.Value.Item1, chains2.Value.Item2);
                            //chains.Value.Item1.AddRange(chains2.Value.Item1);
                            
                            //chains.Value.Item2 = chains2.Value.Item2
                        }
                        else
                            inferenceChain[chains2.Key] = new Tuple<List<ISentence>, ISentence>(chains.Value.Item1, chains.Value.Item2);
                        //chains2.Value.Item1.AddRange(chains.Value.Item1);
                    }

        }*/

        public Tuple<List<string>, List<ISentence>> GetFilmsForUser()
        {
            var filmsForUser = new Tuple<List<string>, List<ISentence>>(new List<string>(), new List<ISentence>());
            var selectedFilms = sentenceConfidence.Where(x => x.Key.GetSentenceType() == SentenceType.Predicate).ToList();
            selectedFilms = selectedFilms.Where(x => (x.Key as Predicate).PredicateName == "UserLikesFilm").ToList();
            selectedFilms.Sort((p1, p2) => p2.Value.CompareTo(p1.Value));

            foreach(var s in selectedFilms)
            {
                if ((s.Key as Predicate).Terms.Any(t => !t.IsConstant))
                    continue;
                else
                {
                    filmsForUser.Item1.Add((s.Key as Predicate).Terms[0].Value + " " + s.Value);
                    filmsForUser.Item2.Add((s.Key as Predicate));
                }
            }
            return filmsForUser;
        }

        public Tuple<List<string>, List<ISentence>> GetInfoAboutFilm(string film)
        {
            var filmInfo = new Tuple<List<string>, List<ISentence>>(new List<string>(), new List<ISentence>());
            var selectedFilms = sentenceConfidence.Where(x => x.Key.GetSentenceType() == SentenceType.Predicate).ToList();
            selectedFilms = selectedFilms.Where(x => (x.Key as Predicate).Terms.Any(t => t.Value == film)).ToList();
            selectedFilms.Sort((p1, p2) => p2.Value.CompareTo(p1.Value));

            foreach (var s in selectedFilms)
            {
                var pred = s.Key as Predicate;
                if (pred.Terms.Any(t => !t.IsConstant))
                    continue;
                else
                {
                    filmInfo.Item1.Add(pred.PredicateName + "(" + String.Join(", ", pred.Terms.Select(t => t.Value).ToArray()) + ") " + s.Value);//", ".Join(pred.Terms[0].Value + " " + s.Value);
                    filmInfo.Item2.Add(s.Key as Predicate);
                }
            }
            return filmInfo;
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
        public Substitution BackwardChain(ISentence sentence, ref List<Tuple<string, List<string>>> listInference)
        {
            listInference = new List<Tuple<string, List<string>>>();
            var q = new Queue<ISentence>();
            q.Enqueue(sentence);
            return BackChainList(q, new Substitution(), listInference);
        }

        private Substitution BackChainList(Queue<ISentence> sentences, Substitution substitution, List<Tuple<string, List<string>>> listInference)
        {
            Substitution answer = new Substitution();
            answer.Successful = false;
            if (sentences.Count == 0)
            {
                return substitution;
            }
            var q = sentences.Dequeue();
            Tuple<string, List<string>> inferenceItem = new Tuple<string, List<string>>(q.ToString(), new List<string>());
            

            while (q.GetSentenceType() == SentenceType.SentenceConnectiveSentence && (q as SentenceConnectiveSentence).Connective == "^")
            {
                var sen1 = (q as SentenceConnectiveSentence).Sentence1;
                var sen2 = (q as SentenceConnectiveSentence).Sentence2;

                sentences.Enqueue(sen1);
                sentences.Enqueue(sen2);
                inferenceItem.Item2.Add(sen1.ToString());
                inferenceItem.Item2.Add(sen2.ToString());
                q = sentences.Dequeue();
            }

            if (inferenceItem.Item2.Count() != 0)
            {
                listInference.Add(inferenceItem);
                inferenceItem = new Tuple<string, List<string>>(q.ToString(), new List<string>());
            }

            foreach (var sen in Sentences)
            {
                var sub = Unify(q, sen);
                if (sub.Successful)
                {
                    answer.Successful = true;
                    substitution.Successful = true;
                    substitution.Compose(sub);
                    answer = answer.Compose(BackChainList(sentences, substitution, listInference));
                    //if (answer.Successful)
                    //{
                        inferenceItem.Item2.Add(sen.ToString());
                        listInference.Add(inferenceItem);
                   // }
                    return answer;
                }
            }
            if (q.GetSentenceType() == SentenceType.SentenceConnectiveSentence && (q as SentenceConnectiveSentence).Connective == "->")
            {
                var sen1 = (q as SentenceConnectiveSentence).Sentence1;

                sentences.Enqueue(sen1);
                inferenceItem.Item2.Add(sen1.ToString());
                return BackChainList(sentences, substitution, listInference);
            }
            foreach (var sen in Sentences
                .Select(s => DropOuterQuantifiers(s))
                .Where(s => s.GetSentenceType() == SentenceType.SentenceConnectiveSentence && (s as SentenceConnectiveSentence).Connective == "->")
                .Select(s => s as SentenceConnectiveSentence))
            {
                var sub2 = Unify(q, sen.Sentence2);
                if (sub2.Successful)
                {
                    answer.Successful = true;
                    var qq = new Queue<ISentence>( SentenceConnectiveSentence.GetAnticedents(sen));
                    answer = answer.Compose(BackChainList(new Queue<ISentence>(
                        qq.Select(x => x.Substitute(sub2))),
                        substitution.Compose(sub2), listInference));
                    if (answer.Successful)
                    {
                        inferenceItem.Item2.Add(sen.ToString());
                        listInference.Add(inferenceItem);
                        substitution.Successful = true;
                    }
                    return answer
                    .Compose(BackChainList(sentences, substitution, listInference));
                }
            }
            inferenceItem.Item2.Add("Нет вывода");
            listInference.Add(inferenceItem);
            return answer.Compose(BackChainList(sentences, substitution, listInference));
        }
        #endregion
    }

    public class Substitution
    {
        public Substitution(bool s)
        {
            SubsitutionDict = new Dictionary<string, string>();
            Successful = s;
        }

        public Substitution()
        {
            SubsitutionDict = new Dictionary<string, string>();
            Successful = false;
        }

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
