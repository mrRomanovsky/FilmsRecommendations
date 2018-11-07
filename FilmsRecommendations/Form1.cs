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

        private void recommendationsButton_Click(object sender, EventArgs e)
        { 
            var kb = new FilmKnowledgeBase("");
            kb.AddSentence(kb.ParseSentence("HasActor(THE_GREAT_GATSBY,DI_CAPRIO)"));
            kb.AddSentence(kb.ParseSentence("HasActor(INCEPTION,DI_CAPRIO)"));
            kb.AddSentence(kb.ParseSentence("Vy.(Vx.(((HasOscar(x))^(HasActor(y,x)))->(IsAwesome(y))))"));
            FilmKnowledgeBase.ForwardChain(kb, kb.ParseSentence("HasOscar(DI_CAPRIO)"));
        }
    }
}
