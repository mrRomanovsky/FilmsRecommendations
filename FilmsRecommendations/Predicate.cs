using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmsRecommendations
{
    public class Predicate : ISentence
    {
        public Predicate() { }

        public Predicate(string name, List<Term> terms)
        {
            PredicateName = name;
            Terms = terms;
        }

        public SentenceType GetSentenceType()
        {
            return SentenceType.Predicate;
        }

        public ISentence Substitute(Substitution substitution)
        {
            var newPredicate = new Predicate();
            newPredicate.PredicateName = PredicateName;
            foreach (var term in Terms)
                newPredicate.Terms.Add(new Term(term.Value));

            foreach (var term in newPredicate.Terms)
            {
                if (!term.IsConstant)
                    if (substitution.SubsitutionDict.ContainsKey(term.Value))
                        term.Value = substitution.SubsitutionDict[term.Value];
            }

            return newPredicate;
        }

        public override string ToString()
        {
            return PredicateName + "(" + Terms.Select(t => t.Value + ",") + ")";
        }

        public string PredicateName { get; set; }

        public List<Term> Terms { get; set; }
    }

    public class Term
    {
        public Term(string val)
        {
            Value = val;
        }

        public string Value { get; set; }

        public bool IsConstant { get { return Char.IsUpper(Value[0]); } }
    }
}
