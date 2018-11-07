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
    public partial class Main : Form
    {
        private string pathToKB;
        private FilmKnowledgeBase knowledgeBase;

        public Main()
        {
            InitializeComponent();
            pathToKB = pathToKBTextBox.Text;
            //knowledgeBase = new FilmKnowledgeBase(pathToKB);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            knowledgeBase = new FilmKnowledgeBase(this.pathToKBTextBox.Text);
            var f = new Form1(knowledgeBase);
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f = new Form2(knowledgeBase);
            f.ShowDialog();
        }
    }
}
