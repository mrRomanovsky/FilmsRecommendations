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
        public Form1()
        {
            InitializeComponent();
        }

        private void recommendationsButton_Click(object sender, EventArgs e)
        {
            var flm = new FilmKnowledgeBase(@"D:\KnowledgeBase.txt");
            FilmKnowledgeBase.ParseSentence(flm, "(Vx.HasActor(x,ACTOR_EDWARD_NORTON)<>IsAwesome(x))");
            Console.WriteLine(flm); //Console doesn't work, just for breakpoint :D
        }
    }
}
