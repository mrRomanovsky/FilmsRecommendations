﻿using System;
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

        private void recommendationsButton_Click(object sender, EventArgs e)
        {
            var genre = genreTextBox.Text;
            var quality = qualityTextBox.Text;
            var actors = actorsTextBox.Text.Split(';');
            //var kb = new FilmKnowledgeBase("");
            var userSentences = new List<ISentence>();
            userSentences.Add(knowledgeBase.ParseSentence("UserLikesGenre("+ genre + ")"));
            userSentences.Add(knowledgeBase.ParseSentence("UserLikesQuality(" + quality + ")"));
            foreach (var actor in actors)
                userSentences.Add(knowledgeBase.ParseSentence("UserLikesActor(" + actor + ")"));

            for (int i = 1; i < userSentences.Count; ++i)
                knowledgeBase.AddSentence(userSentences[i]);

            knowledgeBase.AddSentence(knowledgeBase.ParseSentence("HasActor(THE_GREAT_GATSBY,DI_CAPRIO)"));
            knowledgeBase.AddSentence(knowledgeBase.ParseSentence("HasActor(INCEPTION,DI_CAPRIO)"));
            knowledgeBase.AddSentence(knowledgeBase.ParseSentence("Vy.(Vx.(((HasOscar(x))^(HasActor(y,x)))->(IsAwesome(y))))"));
            FilmKnowledgeBase.ForwardChain(knowledgeBase, userSentences[0]);

            foreach (var filmForUser in knowledgeBase.GetFilmsForUser())
                recommendationsTextBox.Text += filmForUser + "\n";
            //FilmKnowledgeBase.ForwardChain(kb, kb.ParseSentence("HasOscar(DI_CAPRIO)"));
        }
    }
}
