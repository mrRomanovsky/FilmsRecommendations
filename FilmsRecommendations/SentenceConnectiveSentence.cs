using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmsRecommendations
{
    public class SentenceConnectiveSentence : ISentence
    { 
        public SentenceConnectiveSentence() { }

        public SentenceConnectiveSentence(ISentence sentence, ISentence s, string v)
        {
            this.Sentence1 = sentence;
            this.Sentence2 = s;
            this.Connective = v;
        }

        public SentenceType GetSentenceType()
        {
            return SentenceType.SentenceConnectiveSentence;
        }

        public ISentence Substitute(Substitution substitution)
        {
            var sentenceConnectiveSentenceSubst = new SentenceConnectiveSentence();
            sentenceConnectiveSentenceSubst.Connective = Connective;
            sentenceConnectiveSentenceSubst.Sentence1 = Sentence1.Substitute(substitution);
            sentenceConnectiveSentenceSubst.Sentence2 = Sentence2.Substitute(substitution);
            return sentenceConnectiveSentenceSubst;
        }

        public static List<ISentence> GetAnticedents(SentenceConnectiveSentence implSentence)
        {
            if (implSentence.Connective != "->")
                return new List<ISentence>(); 
            return GetConjunctions(implSentence.Sentence1);
        }

        public static List<ISentence> GetConjunctions(ISentence sentence)
        {
            if (sentence.GetSentenceType() == SentenceType.SentenceConnectiveSentence)
            {
                var connectiveSentence = sentence as SentenceConnectiveSentence;
                if (connectiveSentence.Connective != "^")
                    return new List<ISentence>();

                var conjuncts = GetConjunctions(connectiveSentence.Sentence1);
                conjuncts.AddRange(GetConjunctions(connectiveSentence.Sentence2));
                return conjuncts;
            }
            return new List<ISentence>();
        }

        public override string ToString()
        {
            return Sentence1.ToString() + Connective + Sentence2.ToString();
        }
        public string Connective { get; set; }

        public ISentence Sentence1 { get; set; }

        public ISentence Sentence2 { get; set; }

    }
}
