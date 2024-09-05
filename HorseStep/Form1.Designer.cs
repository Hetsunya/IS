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
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)delayTrackBar).BeginInit();
            SuspendLayout();
            // 
            // sizeN
            // 
            sizeN.Location = new Point(798, 22);
            sizeN.Name = "sizeN";
            sizeN.Size = new Size(194, 31);
            sizeN.TabIndex = 0;
            sizeN.Text = "Введите N";
            // 
            // sizeM
            // 
            sizeM.Location = new Point(798, 72);
            sizeM.Name = "sizeM";
            sizeM.Size = new Size(194, 31);
            sizeM.TabIndex = 1;
            sizeM.Text = "Введите M";
            // 
            // StartPointX
            // 
            StartPointX.Location = new Point(798, 120);
            StartPointX.Name = "StartPointX";
            StartPointX.Size = new Size(194, 31);
            StartPointX.TabIndex = 2;
            StartPointX.Text = "Координата X";
            // 
            // StartPointY
            // 
            StartPointY.Location = new Point(798, 172);
            StartPointY.Name = "StartPointY";
            StartPointY.Size = new Size(194, 31);
            StartPointY.TabIndex = 3;
            StartPointY.Text = "Координата Y";
            // 
            // panel
            // 
            panel.Location = new Point(12, 22);
            panel.Name = "panel";
            panel.Size = new Size(780, 600);
            panel.TabIndex = 5;
            // 
            // StartButton
            // 
            StartButton.Location = new Point(855, 284);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(90, 42);
            StartButton.TabIndex = 6;
            StartButton.Text = "Начать";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += button1_Click;
            // 
            // delayTrackBar
            // 
            delayTrackBar.Location = new Point(798, 209);
            delayTrackBar.Name = "delayTrackBar";
            delayTrackBar.Size = new Size(194, 69);
            delayTrackBar.TabIndex = 7;
            delayTrackBar.Scroll += delayTrackBar_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(814, 256);
            label1.Name = "label1";
            label1.Size = new Size(178, 25);
            label1.TabIndex = 8;
            label1.Text = "Время задержки, мс";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(1011, 635);
            Controls.Add(label1);
            Controls.Add(delayTrackBar);
            Controls.Add(StartButton);
            Controls.Add(panel);
            Controls.Add(StartPointY);
            Controls.Add(StartPointX);
            Controls.Add(sizeM);
            Controls.Add(sizeN);
            Name = "Form1";
            Text = "Form1";
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
        private Label label1;
    }
}
