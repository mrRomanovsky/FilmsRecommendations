namespace FilmsRecommendations
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.genreLabel = new System.Windows.Forms.Label();
            this.countryLabel = new System.Windows.Forms.Label();
            this.actorsLabel = new System.Windows.Forms.Label();
            this.recommendationsLabel = new System.Windows.Forms.Label();
            this.genreTextBox = new System.Windows.Forms.TextBox();
            this.countryTextBox = new System.Windows.Forms.TextBox();
            this.actorsTextBox = new System.Windows.Forms.TextBox();
            this.recommendationsTextBox = new System.Windows.Forms.TextBox();
            this.recommendationsButton = new System.Windows.Forms.Button();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.SuspendLayout();
            // 
            // genreLabel
            // 
            this.genreLabel.AutoSize = true;
            this.genreLabel.Location = new System.Drawing.Point(12, 77);
            this.genreLabel.Name = "genreLabel";
            this.genreLabel.Size = new System.Drawing.Size(36, 13);
            this.genreLabel.TabIndex = 0;
            this.genreLabel.Text = "Жанр";
            // 
            // countryLabel
            // 
            this.countryLabel.AutoSize = true;
            this.countryLabel.Location = new System.Drawing.Point(12, 161);
            this.countryLabel.Name = "countryLabel";
            this.countryLabel.Size = new System.Drawing.Size(43, 13);
            this.countryLabel.TabIndex = 1;
            this.countryLabel.Text = "Страна";
            // 
            // actorsLabel
            // 
            this.actorsLabel.AutoSize = true;
            this.actorsLabel.Location = new System.Drawing.Point(12, 265);
            this.actorsLabel.Name = "actorsLabel";
            this.actorsLabel.Size = new System.Drawing.Size(45, 13);
            this.actorsLabel.TabIndex = 2;
            this.actorsLabel.Text = "Актёры";
            // 
            // recommendationsLabel
            // 
            this.recommendationsLabel.AutoSize = true;
            this.recommendationsLabel.Location = new System.Drawing.Point(403, 206);
            this.recommendationsLabel.Name = "recommendationsLabel";
            this.recommendationsLabel.Size = new System.Drawing.Size(82, 13);
            this.recommendationsLabel.TabIndex = 3;
            this.recommendationsLabel.Text = "Рекомендации";
            // 
            // genreTextBox
            // 
            this.genreTextBox.Location = new System.Drawing.Point(106, 70);
            this.genreTextBox.Name = "genreTextBox";
            this.genreTextBox.Size = new System.Drawing.Size(100, 20);
            this.genreTextBox.TabIndex = 4;
            // 
            // countryTextBox
            // 
            this.countryTextBox.Location = new System.Drawing.Point(106, 161);
            this.countryTextBox.Name = "countryTextBox";
            this.countryTextBox.Size = new System.Drawing.Size(100, 20);
            this.countryTextBox.TabIndex = 5;
            // 
            // actorsTextBox
            // 
            this.actorsTextBox.Location = new System.Drawing.Point(106, 262);
            this.actorsTextBox.Name = "actorsTextBox";
            this.actorsTextBox.Size = new System.Drawing.Size(100, 20);
            this.actorsTextBox.TabIndex = 6;
            // 
            // recommendationsTextBox
            // 
            this.recommendationsTextBox.Location = new System.Drawing.Point(560, 203);
            this.recommendationsTextBox.Multiline = true;
            this.recommendationsTextBox.Name = "recommendationsTextBox";
            this.recommendationsTextBox.Size = new System.Drawing.Size(100, 20);
            this.recommendationsTextBox.TabIndex = 7;
            // 
            // recommendationsButton
            // 
            this.recommendationsButton.Location = new System.Drawing.Point(69, 373);
            this.recommendationsButton.Name = "recommendationsButton";
            this.recommendationsButton.Size = new System.Drawing.Size(172, 23);
            this.recommendationsButton.TabIndex = 8;
            this.recommendationsButton.Text = "Получить рекомендации";
            this.recommendationsButton.UseVisualStyleBackColor = true;
            this.recommendationsButton.Click += new System.EventHandler(this.recommendationsButton_Click);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.recommendationsButton);
            this.Controls.Add(this.recommendationsTextBox);
            this.Controls.Add(this.actorsTextBox);
            this.Controls.Add(this.countryTextBox);
            this.Controls.Add(this.genreTextBox);
            this.Controls.Add(this.recommendationsLabel);
            this.Controls.Add(this.actorsLabel);
            this.Controls.Add(this.countryLabel);
            this.Controls.Add(this.genreLabel);
            this.Name = "Form1";
            this.Text = "Прямой поиск";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label genreLabel;
        private System.Windows.Forms.Label countryLabel;
        private System.Windows.Forms.Label actorsLabel;
        private System.Windows.Forms.Label recommendationsLabel;
        private System.Windows.Forms.TextBox genreTextBox;
        private System.Windows.Forms.TextBox countryTextBox;
        private System.Windows.Forms.TextBox actorsTextBox;
        private System.Windows.Forms.TextBox recommendationsTextBox;
        private System.Windows.Forms.Button recommendationsButton;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
    }
}

