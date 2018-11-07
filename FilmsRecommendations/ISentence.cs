using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmsRecommendations
{
    public enum SentenceType { Predicate, SentenceConnectiveSentence, QuantifierVariableSentence }

    public interface ISentence
    {
        SentenceType GetSentenceType();

        ISentence Substitute(Substitution substitution);
    }
}
