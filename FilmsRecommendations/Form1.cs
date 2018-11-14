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
    public partial class Form1 : Form
    {
        // private string pathToKB; 
        private FilmKnowledgeBase knowledgeBase;
        public Form1(FilmKnowledgeBase kb)//string path)
        {
            InitializeComponent();
            knowledgeBase = kb;
            //pathToKB = path;
        }

        private List<ISentence> answers = new List<ISentence>();

        private void recommendationsButton_Click(object sender, EventArgs e)
        {
            var genres = genreTextBox.Text.Split(';');
            var qualities = qualityTextBox.Text.Split(';');
            var actors = actorsTextBox.Text.Split(';');
            var countries = countryTextBox.Text.Split(';');
            //var kb = new FilmKnowledgeBase("");
            var userSentences = new List<ISentence>();
            foreach (var genre in genres)
                if (genre != "")
                    userSentences.Add(knowledgeBase.ParseSentence("UserLikesGenre("+ genre + ") 1"));
            foreach (var quality in qualities)
                if (quality != "")
                    userSentences.Add(knowledgeBase.ParseSentence("UserLikesQuality(" + quality + ") 1"));
            foreach (var actor in actors)
                if (actor != "")
                    userSentences.Add(knowledgeBase.ParseSentence("UserLikesActor(" + actor + ") 1"));
            foreach (var country in countries)
                if (country != "")
                    userSentences.Add(knowledgeBase.ParseSentence("UserLikesCountry(" + country + ") 1"));
            for (int i = 1; i < userSentences.Count; ++i)
                knowledgeBase.AddSentence(userSentences[i]);

            FilmKnowledgeBase.ForwardChain(knowledgeBase, userSentences[0]);

            var res = knowledgeBase.GetFilmsForUser();
            answers = res.Item2;
            foreach (var filmForUser in res.Item1)
                recommendationsTextBox.Text += filmForUser + "\r\n";
            //FilmKnowledgeBase.ForwardChain(kb, kb.ParseSentence("HasOscar(DI_CAPRIO)"));
        }

        private void filmInfoButton_Click(object sender, EventArgs e)
        {
            var filmName = filmNameTextBox.Text;
            var genres = genreTextBox.Text.Split(';');
            var qualities = qualityTextBox.Text.Split(';');
            var actors = actorsTextBox.Text.Split(';');
            var countries = countryTextBox.Text.Split(';');
            //var kb = new FilmKnowledgeBase("");
            var userSentences = new List<ISentence>();
            foreach (var genre in genres)
                if (genre != "")
                {
                    var terms = filmName + "," + genre;
                    var userSentence = "HasGenre(" + terms + ") 1";
                    userSentences.Add(knowledgeBase.ParseSentence(userSentence));
                }
            foreach (var quality in qualities)
                if (quality != "")
                {
                    var userSentence = GetQualitySentence(filmName, quality);
                    userSentences.Add(knowledgeBase.ParseSentence(userSentence));
                }
            foreach (var actor in actors)
                if (actor != "")
                {
                    var terms = filmName + "," + actor;
                    var userSentence = "HasActor(" + terms + ") 1";
                    userSentences.Add(knowledgeBase.ParseSentence(userSentence));
                }
            foreach (var country in countries)
            {
                if (country != "")
                {
                    var terms = filmName + "," + country;
                    var userSentence = "HasCountry(" + terms + ") 1";
                    userSentences.Add(knowledgeBase.ParseSentence(userSentence));
                }
            }
            for (int i = 1; i < userSentences.Count; ++i)
                knowledgeBase.AddSentence(userSentences[i]);

            FilmKnowledgeBase.ForwardChain(knowledgeBase, userSentences[0]);

            var res = knowledgeBase.GetFilmsForUser();
            answers = res.Item2;

            foreach (var filmForUser in res.Item1)
                recommendationsTextBox.Text += filmForUser + "\r\n";
        }

        private string GetQualitySentence(string filmName, string Quality)
        {
            switch (Quality)
            {
                case "AWESOME":
                    return "IsAwesome(" + filmName + ") 1";
                case "GOOD":
                    return "IsGood(" + filmName + ") 1";
                default:
                    return "IsBad(" + filmName + ") 1";
            }
        }

        private void whyButton_Click(object sender, EventArgs e)
        {
            var explanation = answers.Select(sentence => FilmKnowledgeBase.proofs[sentence]).ToArray();
            System.IO.File.WriteAllLines("explanation.txt", explanation);
        }
    }
}
