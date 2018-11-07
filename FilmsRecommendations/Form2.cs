using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilmsRecommendations
{
    public partial class Form2 : Form
    {
        //private string pathToKB;
        private FilmKnowledgeBase knowledgeBase;
        private ISentence sentence;

        public Form2(FilmKnowledgeBase kb)//string path)
        {
            InitializeComponent();
            //  pathToKB = path;
            knowledgeBase = kb;// new FilmKnowledgeBase(pathToKB);
            sentence = null;
        }

        private void buttonAddParametrs_Click(object sender, EventArgs e)
        {

        }

        private void buttonGetAnswer_Click(object sender, EventArgs e)
        {
            if (listBoxAnswerType.SelectedItem == null)
            {
                //распознать 
               // sentence = knowledgeBase.ParseSentence(knowledgeBase, textBoxAnwerAsString.Text);
            }
            else
            {
                var s = ReadSentenceFromPanel();
                if (sentence == null)
                {
                    sentence = s;
                }
                else
                    sentence = new SentenceConnectiveSentence(sentence, s, "^");
            }
            var answer = knowledgeBase.BackwardChain(sentence);
            var m = MessageBox.Show(sentence.ToString(), answer.Successful ? "True" : "False", MessageBoxButtons.OK);
        }

        private ISentence ReadSentenceFromPanel()
        {
                switch (listBoxAnswerType.SelectedIndex)
                {
                    case 0:
                        return new Predicate("HasActor", new List<Term> { new Term(textBoxFilm.Text), new Term(textBoxActor.Text) });
                    case 1:
                        return new Predicate("HasOscar", new List<Term> { new Term(textBoxActor.Text) });
                    case 2:
                        return new Predicate("HasJanre", new List<Term> { new Term(textBoxFilm.Text), new Term(textBoxJanre.Text) });
                    case 3:
                        return new Predicate("HasCountry", new List<Term> { new Term(textBoxFilm.Text), new Term(textBoxCountry.Text) });
                    case 4:
                        return new Predicate("IsGood", new List<Term> { new Term(textBoxFilm.Text) });
                    case 5:
                        return new Predicate("IsBad", new List<Term> { new Term(textBoxFilm.Text) });
                    case 6:
                        return new Predicate("IsAwesome", new List<Term> { new Term(textBoxFilm.Text) });
                    case 7:
                        return new Predicate("IsCommon", new List<Term> { new Term(textBoxFilm.Text) });
                    default:
                    return null;
                }

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            sentence = null;
        }
    }
}
