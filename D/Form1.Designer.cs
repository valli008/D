namespace D
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            training = new Button();
            PasswordWord1 = new TextBox();
            PathToTheFolder = new TextBox();
            label2 = new Label();
            label1 = new Label();
            tabPage2 = new TabPage();
            selection = new Button();
            MinimumExecutionTime = new RadioButton();
            MaximumCompressionRatio = new RadioButton();
            FilePath1 = new TextBox();
            tabPage3 = new TabPage();
            PasswordWord2 = new TextBox();
            FilePath2 = new TextBox();
            change = new Button();
            save = new Button();
            arkhivuvannya = new ComboBox();
            shyfruvannya = new ComboBox();
            unzipping = new CheckBox();
            archiving = new CheckBox();
            decoding = new CheckBox();
            encryption = new CheckBox();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 451);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(training);
            tabPage1.Controls.Add(PasswordWord1);
            tabPage1.Controls.Add(PathToTheFolder);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 418);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Навчання";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // training
            // 
            training.Location = new Point(275, 246);
            training.Name = "training";
            training.Size = new Size(94, 29);
            training.TabIndex = 9;
            training.Text = "Навчити";
            training.UseVisualStyleBackColor = true;
            training.Click += training_Click;
            // 
            // PasswordWord1
            // 
            PasswordWord1.Location = new Point(275, 166);
            PasswordWord1.Name = "PasswordWord1";
            PasswordWord1.Size = new Size(274, 27);
            PasswordWord1.TabIndex = 8;
            // 
            // PathToTheFolder
            // 
            PathToTheFolder.Location = new Point(275, 77);
            PathToTheFolder.Name = "PathToTheFolder";
            PathToTheFolder.Size = new Size(274, 27);
            PathToTheFolder.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(72, 166);
            label2.Name = "label2";
            label2.Size = new Size(169, 20);
            label2.TabIndex = 6;
            label2.Text = "Введіть ключове слово";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(72, 77);
            label1.Name = "label1";
            label1.Size = new Size(166, 20);
            label1.TabIndex = 5;
            label1.Text = "Введіть шлях до папки";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(selection);
            tabPage2.Controls.Add(MinimumExecutionTime);
            tabPage2.Controls.Add(MaximumCompressionRatio);
            tabPage2.Controls.Add(FilePath1);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 418);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Підбір алгоритму";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // selection
            // 
            selection.Location = new Point(316, 283);
            selection.Name = "selection";
            selection.Size = new Size(143, 29);
            selection.TabIndex = 3;
            selection.Text = "Підібрати";
            selection.UseVisualStyleBackColor = true;
            selection.Click += selection_Click;
            // 
            // MinimumExecutionTime
            // 
            MinimumExecutionTime.AutoSize = true;
            MinimumExecutionTime.Location = new Point(249, 218);
            MinimumExecutionTime.Name = "MinimumExecutionTime";
            MinimumExecutionTime.Size = new Size(229, 24);
            MinimumExecutionTime.TabIndex = 2;
            MinimumExecutionTime.TabStop = true;
            MinimumExecutionTime.Text = "Мінімальний час виконання";
            MinimumExecutionTime.UseVisualStyleBackColor = true;
            // 
            // MaximumCompressionRatio
            // 
            MaximumCompressionRatio.AutoSize = true;
            MaximumCompressionRatio.Location = new Point(249, 169);
            MaximumCompressionRatio.Name = "MaximumCompressionRatio";
            MaximumCompressionRatio.Size = new Size(289, 24);
            MaximumCompressionRatio.TabIndex = 1;
            MaximumCompressionRatio.TabStop = true;
            MaximumCompressionRatio.Text = "Максимальний коєфіціент стиснення";
            MaximumCompressionRatio.UseVisualStyleBackColor = true;
            // 
            // FilePath1
            // 
            FilePath1.Location = new Point(249, 111);
            FilePath1.Name = "FilePath1";
            FilePath1.Size = new Size(283, 27);
            FilePath1.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(PasswordWord2);
            tabPage3.Controls.Add(FilePath2);
            tabPage3.Controls.Add(change);
            tabPage3.Controls.Add(save);
            tabPage3.Controls.Add(arkhivuvannya);
            tabPage3.Controls.Add(shyfruvannya);
            tabPage3.Controls.Add(unzipping);
            tabPage3.Controls.Add(archiving);
            tabPage3.Controls.Add(decoding);
            tabPage3.Controls.Add(encryption);
            tabPage3.Controls.Add(label6);
            tabPage3.Controls.Add(label5);
            tabPage3.Controls.Add(label4);
            tabPage3.Controls.Add(label3);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(792, 418);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Шифрування/архівування";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // PasswordWord2
            // 
            PasswordWord2.Location = new Point(200, 126);
            PasswordWord2.Name = "PasswordWord2";
            PasswordWord2.Size = new Size(159, 27);
            PasswordWord2.TabIndex = 13;
            // 
            // FilePath2
            // 
            FilePath2.Location = new Point(200, 63);
            FilePath2.Name = "FilePath2";
            FilePath2.Size = new Size(159, 27);
            FilePath2.TabIndex = 12;
            // 
            // change
            // 
            change.Location = new Point(486, 12);
            change.Name = "change";
            change.Size = new Size(94, 29);
            change.TabIndex = 11;
            change.Text = "Змінити";
            change.UseVisualStyleBackColor = true;
            change.Click += change_Click;
            // 
            // save
            // 
            save.Location = new Point(511, 246);
            save.Name = "save";
            save.Size = new Size(94, 29);
            save.TabIndex = 10;
            save.Text = "Зберегти";
            save.UseVisualStyleBackColor = true;
            save.Click += save_Click;
            // 
            // arkhivuvannya
            // 
            arkhivuvannya.FormattingEnabled = true;
            arkhivuvannya.Location = new Point(620, 178);
            arkhivuvannya.Name = "arkhivuvannya";
            arkhivuvannya.Size = new Size(151, 28);
            arkhivuvannya.TabIndex = 9;
            // 
            // shyfruvannya
            // 
            shyfruvannya.FormattingEnabled = true;
            shyfruvannya.Location = new Point(387, 178);
            shyfruvannya.Name = "shyfruvannya";
            shyfruvannya.Size = new Size(151, 28);
            shyfruvannya.TabIndex = 8;
            // 
            // unzipping
            // 
            unzipping.AutoSize = true;
            unzipping.Location = new Point(620, 126);
            unzipping.Name = "unzipping";
            unzipping.Size = new Size(140, 24);
            unzipping.TabIndex = 7;
            unzipping.Text = "Розархівування";
            unzipping.UseVisualStyleBackColor = true;
            // 
            // archiving
            // 
            archiving.AutoSize = true;
            archiving.Location = new Point(620, 63);
            archiving.Name = "archiving";
            archiving.Size = new Size(118, 24);
            archiving.TabIndex = 6;
            archiving.Text = "Архівування";
            archiving.UseVisualStyleBackColor = true;
            // 
            // decoding
            // 
            decoding.AutoSize = true;
            decoding.Location = new Point(387, 126);
            decoding.Name = "decoding";
            decoding.Size = new Size(144, 24);
            decoding.TabIndex = 5;
            decoding.Text = "Розшифрування";
            decoding.UseVisualStyleBackColor = true;
            // 
            // encryption
            // 
            encryption.AutoSize = true;
            encryption.Location = new Point(387, 63);
            encryption.Name = "encryption";
            encryption.Size = new Size(122, 24);
            encryption.TabIndex = 4;
            encryption.Text = "Шифрування";
            encryption.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(620, 16);
            label6.Name = "label6";
            label6.Size = new Size(50, 20);
            label6.TabIndex = 3;
            label6.Text = "label6";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(387, 16);
            label5.Name = "label5";
            label5.Size = new Size(50, 20);
            label5.TabIndex = 2;
            label5.Text = "label5";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 126);
            label4.Name = "label4";
            label4.Size = new Size(177, 20);
            label4.TabIndex = 1;
            label4.Text = "Введіть парольне слово";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 63);
            label3.Name = "label3";
            label3.Size = new Size(166, 20);
            label3.TabIndex = 0;
            label3.Text = "Введіть шлях до файлу";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private Button training;
        private TextBox PasswordWord1;
        private TextBox PathToTheFolder;
        private Label label2;
        private Label label1;
        private TabPage tabPage2;
        private Button selection;
        private RadioButton MinimumExecutionTime;
        private RadioButton MaximumCompressionRatio;
        private TextBox FilePath1;
        private TabPage tabPage3;
        private CheckBox unzipping;
        private CheckBox archiving;
        private CheckBox decoding;
        private CheckBox encryption;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Button change;
        private Button save;
        private ComboBox arkhivuvannya;
        private ComboBox shyfruvannya;
        private TextBox PasswordWord2;
        private TextBox FilePath2;
    }
}
