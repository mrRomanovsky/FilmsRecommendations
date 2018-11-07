using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmsRecommendations
{
    public enum Quantifier { All, Exists}

    public class QuantifierVariableSentence : ISentence
    {
        public SentenceType GetSentenceType()
        {
            return SentenceType.QuantifierVariableSentence;
        }

        public ISentence Substitute(Substitution substitution)
        {
            var quantifierVariableSentenceSubst = new QuantifierVariableSentence();
            quantifierVariableSentenceSubst.Variable = Variable;
            quantifierVariableSentenceSubst.Quantifier = Quantifier;
            quantifierVariableSentenceSubst.Sentence = Sentence.Substitute(substitution);

            if (substitution.SubsitutionDict.ContainsKey(Variable))
            {
                if (Char.IsLower(substitution.SubsitutionDict[Variable][0]))
                {
                    quantifierVariableSentenceSubst.Variable = substitution.SubsitutionDict[Variable];
                    return quantifierVariableSentenceSubst;
                }
                else // drop the quantifier
                    return quantifierVariableSentenceSubst.Sentence;
            }

            return quantifierVariableSentenceSubst;
        }

        override public string ToString()
        {
            var q = Quantifier == Quantifier.All ? "V" : "E";
            return q + Variable + "." + Sentence.ToString();
        }

        public Quantifier Quantifier { get; set; }
        public string Variable { get; set; }
        public ISentence Sentence { get; set; }
    }
}
