using System;
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

        public FilmKnowledgeBase parent;
        public List<FilmKnowledgeBase> next;
        public Tuple<Type, string> data;

        public static SortedDictionary<string, List<string>> store;
        public FilmKnowledgeBase(Type type, string sentence)
        {
            parent = null;
            data = new Tuple<Type, string>(type, sentence);
            next = new List<FilmKnowledgeBase>();
        }

        public FilmKnowledgeBase(string pathToKnowledgeBase, Type type, string sentence)
        {
            store = new SortedDictionary<string, List<string>>();
            var text = System.IO.File.ReadAllText(pathToKnowledgeBase)
                .Replace("\r\n", String.Empty)
                .Split('@')
                .Reverse()
                .Skip(1) //To remove last empty line
                .Reverse()
                .ToList();
            foreach (var item in text)
            {
                var keyValue = item.Split(':').ToList();
                store[keyValue.First().Replace(" ", String.Empty)] = keyValue
                                                                             .Last()
                                                                             .Split('|')
                                                                             .Select(x => x.Replace(" ", String.Empty))
                                                                             .ToList();
            }

            parent = null;
            data = new Tuple<Type, string>(type, sentence);
            next = new List<FilmKnowledgeBase>();
        }

        public FilmKnowledgeBase(string pathToKnowledgeBase)
        {
            store = new SortedDictionary<string, List<string>>();
            var text = System.IO.File.ReadAllText(pathToKnowledgeBase)
                .Replace("\r\n", String.Empty)
                .Split('@')
                .Reverse()
                .Skip(1) //To remove last empty line
                .Reverse()
                .ToList();
            foreach (var item in text)
            {
                var keyValue = item.Split(':').ToList();
                store[keyValue.First().Replace(" ", String.Empty)] = keyValue
                                                                             .Last()
                                                                             .Split('|')
                                                                             .Select(x => x.Replace(" ", String.Empty))
                                                                             .ToList();
            }

            parent = null;
            data = new Tuple<Type, string>(Type.None, "");
            next = new List<FilmKnowledgeBase>();
        }

        private static List<char> quantifier = new List<char> {'V', 'E' };
        private static List<char> connective = new List<char> { '-', '/', '\\', '<'};
        private static List<Type> terminal = new List<Type> { Type.Variable, Type.Constant, Type.Connective, Type.Quantifier, Type.Predicate};

        private static bool isAtomicSentence(string str)
        {
            var atomicSent = true;
            foreach (var item in connective)
            {
                if (str.Contains(item) || str.Contains('.'))
                {
                    atomicSent = false;
                    break;
                }
            }
            return atomicSent;
        }

        public static void ParseSentence(FilmKnowledgeBase KB, string strToParse)
        {
            if (terminal.Contains(KB.data.Item1))
                return;
            if(KB.parent == null)
                KB.data = new Tuple<Type, string>(Type.Sentence, strToParse);
            //Parse Quantifier, plz use one symbol variables (maybe fix it later)
            if (quantifier.Contains(strToParse[0]))
            {
                var dotInd = strToParse.IndexOf('.');
                if (dotInd == -1)
                {
                    var nodeQuant = new FilmKnowledgeBase(Type.Quantifier, strToParse.Substring(0, 1));
                    nodeQuant.parent = KB;
                    KB.next.Add(nodeQuant);
                    var nodeVar = new FilmKnowledgeBase(Type.Variable, strToParse.Substring(1));
                    nodeVar.parent = KB;
                    KB.next.Add(nodeVar);
                    ParseSentence(nodeVar, strToParse.Substring(1));
                    ParseSentence(nodeQuant, strToParse.Substring(0, 1));
                }
                else
                {
                    var node1 = new FilmKnowledgeBase(Type.QuantVar, strToParse.Substring(0, dotInd));
                    node1.parent = KB;
                    KB.next.Add(node1);
                    FilmKnowledgeBase node2;

                    if (isAtomicSentence(strToParse))
                    {
                        node2 = new FilmKnowledgeBase(Type.Sentence, strToParse.Substring(dotInd + 1));
                        node2.parent = KB;
                        KB.next.Add(node2);
                        ParseSentence(node2, strToParse.Substring(dotInd + 1));
                    }
                    else
                    {
                        node2 = new FilmKnowledgeBase(Type.AtomicSent, strToParse.Substring(dotInd + 1));
                        node2.parent = KB;
                        KB.next.Add(node2);
                        ParseSentence(node2, strToParse.Substring(dotInd + 1));
                    }
                    ParseSentence(node1, strToParse.Substring(0, dotInd));
                }
            }

            //Parse Predicate
            else if (KB.data.Item1 == Type.AtomicSent)
            {
                foreach (var item in store["Predicate"])
                {
                    var predInd = strToParse.IndexOf(item);
                    if (predInd == 0)
                    {
                        var node1 = new FilmKnowledgeBase(Type.Predicate, strToParse.Substring(0, item.Length));
                        node1.parent = KB;
                        KB.next.Add(node1);
                        var node2 = new FilmKnowledgeBase(Type.Arg, strToParse.Substring(item.Length));
                        node2.parent = KB;
                        KB.next.Add(node2);
                        ParseSentence(node1, strToParse.Substring(0, item.Length));
                        ParseSentence(node2, strToParse.Substring(item.Length));
                        break;
                    }
                }
            }

            //Parse Arg
            else if(KB.data.Item1 == Type.Arg)
            {
                strToParse = strToParse.Replace(" ", String.Empty);
                var args = strToParse.Substring(1, strToParse.Length - 2).Split(',');
                foreach (var item in args)
                {
                    if (store["Constant"].Contains(item))
                    {
                        var node1 = new FilmKnowledgeBase(Type.Constant, item);
                        node1.parent = KB;
                        KB.next.Add(node1);
                        ParseSentence(node1, item);
                    }
                    else if(store["Variable"].Contains(item)){
                        var node1 = new FilmKnowledgeBase(Type.Variable, item);
                        node1.parent = KB;
                        KB.next.Add(node1);
                        ParseSentence(node1, item);
                    }
                }
            }

            //Parse Sentence Connective Sentence
            else if (strToParse[0] == '(')
            {
                strToParse = strToParse.Substring(1, strToParse.Length - 2);
                var bracketsCount = 0;
                var i = 0;
                var flag = true;
                while(i != strToParse.Length - 1 && flag)
                {
                    if(strToParse[i] == ')' || strToParse[i] == '(')
                    {
                        bracketsCount++;
                        i++;
                        continue;
                    }
                    if (bracketsCount % 2 == 0)
                    {
                        foreach (var item in connective)
                        {
                            if (strToParse[i] == item)
                            {
                                flag = false;
                                var str = strToParse.Substring(0, i);

                                if (isAtomicSentence(str))
                                {
                                    var node1 = new FilmKnowledgeBase(Type.AtomicSent, str);
                                    node1.parent = KB;
                                    KB.next.Add(node1);
                                    ParseSentence(node1, str);
                                }
                                else
                                {
                                    var node1 = new FilmKnowledgeBase(Type.Sentence, str);
                                    node1.parent = KB;
                                    KB.next.Add(node1);
                                    ParseSentence(node1, str);
                                }

                                var node2 = new FilmKnowledgeBase(Type.Connective, strToParse.Substring(i, 2));
                                node2.parent = KB;
                                KB.next.Add(node2);
                                ParseSentence(node2, strToParse.Substring(i, 2));

                                str = strToParse.Substring(i + 2);
                                if (isAtomicSentence(str))
                                {
                                    var node3 = new FilmKnowledgeBase(Type.AtomicSent, str);
                                    node3.parent = KB;
                                    KB.next.Add(node3);
                                    ParseSentence(node3, str);
                                }
                                else
                                {
                                    var node3 = new FilmKnowledgeBase(Type.Sentence, str);
                                    node3.parent = KB;
                                    KB.next.Add(node3);
                                    ParseSentence(node3, str);
                                }

                                break;
                            }
                        }
                    }
                    i++;
                }
            }
        }
    }
}
