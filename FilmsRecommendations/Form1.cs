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
            //var flm = new FilmKnowledgeBase(pathToKB);
            //FilmKnowledgeBase.ParseSentence(knowledgeBase, "HasActor(FILM_TOP_GUN,ACTOR_EDWARD_NORTON");
            //FilmKnowledgeBase.ParseSentence(knowledgeBase, "(Vx.HasActor(x,ACTOR_EDWARD_NORTON)<>IsAwesome(x))");
            //FilmKnowledgeBase.ParseSentence(knowledgeBase, "HasActor(FILM_TITANIC,ACTOR_BRAD_PITT");
            //FilmKnowledgeBase.ParseSentence(flm, "(Vx.HasActor(x, ACTOR_BRAD_PITT) <> IsGood(x))");
            Console.WriteLine(knowledgeBase); //Console doesn't work, just for breakpoint :D
            //C:\Users\Andrew\intellectual_systems\FilmsRecommendations\KnowledgeBase.txt
        }
    }
}
