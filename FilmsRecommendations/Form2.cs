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
            if (listBoxAnswerType.SelectedItem == null)
            {
                //распознать 
                var s = knowledgeBase.ParseSentence(textBoxAnwerAsString.Text);
                if (sentence == null)
                {
                    sentence = s;
                }
                else
                    sentence = new SentenceConnectiveSentence(sentence, s, "^");
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
        }

        private void buttonGetAnswer_Click(object sender, EventArgs e)
        {
            if (listBoxAnswerType.SelectedItem == null)
            {
                //распознать 
                sentence = knowledgeBase.ParseSentence(textBoxAnwerAsString.Text);
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
            var listInfersece = new List<Tuple<string, List<string>>>();
            var answer = knowledgeBase.BackwardChain(sentence,ref listInfersece);
            var infer = BuiildInferce(listInfersece);
            var m = MessageBox.Show(sentence.ToString() + "\n Ответ : " + answer.Successful.ToString(), answer.Successful ? "True" : "False", MessageBoxButtons.OK);
            var m1 = MessageBox.Show("Вывод: \n" + infer, "Вывод", MessageBoxButtons.OK);

            sentence = null;
        }

        private string BuiildInferce(List<Tuple<string, List<string>>>  listInferce)
        {
            string res = "";
            res = string.Join("\n-----------------------------------\n", listInferce.Select(item => item.Item1 + "<=" + string.Join(",", item.Item2)));
            foreach (var item in listInferce)
            {

            }
            return res;
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
                        return new Predicate("HasGenre", new List<Term> { new Term(textBoxFilm.Text), new Term(textBoxGenre.Text) });
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
