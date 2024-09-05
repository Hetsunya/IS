namespace HorseStep
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
            sizeN = new TextBox();
            sizeM = new TextBox();
            StartPointX = new TextBox();
            StartPointY = new TextBox();
            panel = new Panel();
            StartButton = new Button();
            delayTrackBar = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)delayTrackBar).BeginInit();
            SuspendLayout();
            // 
            // sizeN
            // 
            sizeN.Location = new Point(1052, 22);
            sizeN.Name = "sizeN";
            sizeN.Size = new Size(194, 31);
            sizeN.TabIndex = 0;
            sizeN.Text = "Введите N";
            sizeN.TextChanged += textBox1_TextChanged;
            // 
            // sizeM
            // 
            sizeM.Location = new Point(1052, 72);
            sizeM.Name = "sizeM";
            sizeM.Size = new Size(194, 31);
            sizeM.TabIndex = 1;
            sizeM.Text = "Введите M";
            sizeM.TextChanged += textBox2_TextChanged;
            // 
            // StartPointX
            // 
            StartPointX.Location = new Point(1052, 120);
            StartPointX.Name = "StartPointX";
            StartPointX.Size = new Size(194, 31);
            StartPointX.TabIndex = 2;
            StartPointX.Text = "Координата X";
            StartPointX.TextChanged += textBox3_TextChanged;
            // 
            // StartPointY
            // 
            StartPointY.Location = new Point(1052, 172);
            StartPointY.Name = "StartPointY";
            StartPointY.Size = new Size(194, 31);
            StartPointY.TabIndex = 3;
            StartPointY.Text = "Координата Y";
            StartPointY.TextChanged += textBox4_TextChanged;
            // 
            // panel
            // 
            panel.Location = new Point(28, 22);
            panel.Name = "panel";
            panel.Size = new Size(600, 600);
            panel.TabIndex = 5;
            panel.Paint += panel2_Paint;
            // 
            // StartButton
            // 
            StartButton.Location = new Point(1109, 284);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(90, 42);
            StartButton.TabIndex = 6;
            StartButton.Text = "Начать";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += button1_Click;
            // 
            // delayTrackBar
            // 
            delayTrackBar.Location = new Point(1052, 209);
            delayTrackBar.Name = "delayTrackBar";
            delayTrackBar.Size = new Size(194, 69);
            delayTrackBar.TabIndex = 7;
            delayTrackBar.Scroll += delayTrackBar_Scroll;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1258, 664);
            Controls.Add(delayTrackBar);
            Controls.Add(StartButton);
            Controls.Add(panel);
            Controls.Add(StartPointY);
            Controls.Add(StartPointX);
            Controls.Add(sizeM);
            Controls.Add(sizeN);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)delayTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox sizeN;
        private TextBox sizeM;
        private TextBox StartPointX;
        private TextBox StartPointY;
        private Panel panel;
        private Button StartButton;
        private TrackBar delayTrackBar;
    }
}
