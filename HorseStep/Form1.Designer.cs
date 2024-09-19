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
        /// <param name="disposing">true if managed resources should be disposed; otherwise,</param>
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
            timeLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)delayTrackBar).BeginInit();
            SuspendLayout();
            // 
            // sizeN
            // 
            sizeN.Anchor = AnchorStyles.Bottom;
            sizeN.Location = new Point(58, 684);
            sizeN.Name = "sizeN";
            sizeN.Size = new Size(150, 31);
            sizeN.TabIndex = 0;
            sizeN.Text = "Введите N";
            // 
            // sizeM
            // 
            sizeM.Anchor = AnchorStyles.Bottom;
            sizeM.Location = new Point(218, 684);
            sizeM.Name = "sizeM";
            sizeM.Size = new Size(150, 31);
            sizeM.TabIndex = 1;
            sizeM.Text = "Введите M";
            // 
            // StartPointX
            // 
            StartPointX.Anchor = AnchorStyles.Bottom;
            StartPointX.Location = new Point(378, 684);
            StartPointX.Name = "StartPointX";
            StartPointX.Size = new Size(150, 31);
            StartPointX.TabIndex = 2;
            StartPointX.Text = "Координата X";
            // 
            // StartPointY
            // 
            StartPointY.Anchor = AnchorStyles.Bottom;
            StartPointY.Location = new Point(538, 684);
            StartPointY.Name = "StartPointY";
            StartPointY.Size = new Size(150, 31);
            StartPointY.TabIndex = 3;
            StartPointY.Text = "Координата Y";
            // 
            // panel
            // 
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel.BackColor = SystemColors.ControlLight;
            panel.Location = new Point(20, 20);
            panel.Name = "panel";
            panel.Size = new Size(1281, 600);
            panel.TabIndex = 5;
            // 
            // StartButton
            // 
            StartButton.Anchor = AnchorStyles.Bottom;
            StartButton.Location = new Point(698, 684);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(150, 31);
            StartButton.TabIndex = 6;
            StartButton.Text = "Начать";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += button1_Click;
            // 
            // delayTrackBar
            // 
            delayTrackBar.Anchor = AnchorStyles.Bottom;
            delayTrackBar.Location = new Point(858, 684);
            delayTrackBar.Name = "delayTrackBar";
            delayTrackBar.Size = new Size(200, 69);
            delayTrackBar.TabIndex = 7;
            delayTrackBar.Scroll += delayTrackBar_Scroll;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom;
            label1.AutoSize = true;
            label1.Location = new Point(858, 664);
            label1.Name = "label1";
            label1.Size = new Size(178, 25);
            label1.TabIndex = 8;
            label1.Text = "Время задержки, мс";
            // 
            // timeLabel
            // 
            timeLabel.Anchor = AnchorStyles.Bottom;
            timeLabel.AutoSize = true;
            timeLabel.Location = new Point(1058, 684);
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new Size(210, 25);
            timeLabel.TabIndex = 9;
            timeLabel.Text = "Прошло времени: ... сек";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(1313, 721);
            Controls.Add(timeLabel);
            Controls.Add(label1);
            Controls.Add(delayTrackBar);
            Controls.Add(StartButton);
            Controls.Add(panel);
            Controls.Add(StartPointY);
            Controls.Add(StartPointX);
            Controls.Add(sizeM);
            Controls.Add(sizeN);
            Name = "Form1";
            Text = "Horse Step";
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
        private Label timeLabel;
    }
}
