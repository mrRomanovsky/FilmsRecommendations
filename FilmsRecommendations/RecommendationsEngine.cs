using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmsRecommendations
{
    public class FilmRecommendationsEngine
    {
        public FilmRecommendationsEngine(string pathToKnowledgeBase)
        {
            knowledgeBase = new FilmKnowledgeBase(pathToKnowledgeBase);
        }

        public FilmRecommendationsEngine(FilmKnowledgeBase knowledgeBase)
        {
            this.knowledgeBase = knowledgeBase;
        }


        FilmKnowledgeBase knowledgeBase;
    }
}
