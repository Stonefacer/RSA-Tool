namespace WindowsFormsApplication2
{
    partial class TestRSAInterval
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.tbFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbN = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbE = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbD = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbCur = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPocessed = new System.Windows.Forms.TextBox();
            this.lbTotal = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.lbFails = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "From:";
            // 
            // tbFrom
            // 
            this.tbFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrom.Location = new System.Drawing.Point(79, 10);
            this.tbFrom.Name = "tbFrom";
            this.tbFrom.ReadOnly = true;
            this.tbFrom.Size = new System.Drawing.Size(844, 20);
            this.tbFrom.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "To:";
            // 
            // tbTo
            // 
            this.tbTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTo.Location = new System.Drawing.Point(79, 36);
            this.tbTo.Name = "tbTo";
            this.tbTo.ReadOnly = true;
            this.tbTo.Size = new System.Drawing.Size(844, 20);
            this.tbTo.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "N:";
            // 
            // tbN
            // 
            this.tbN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbN.Location = new System.Drawing.Point(79, 62);
            this.tbN.Name = "tbN";
            this.tbN.ReadOnly = true;
            this.tbN.Size = new System.Drawing.Size(844, 20);
            this.tbN.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "E:";
            // 
            // tbE
            // 
            this.tbE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbE.Location = new System.Drawing.Point(79, 88);
            this.tbE.Name = "tbE";
            this.tbE.ReadOnly = true;
            this.tbE.Size = new System.Drawing.Size(844, 20);
            this.tbE.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "D:";
            // 
            // tbD
            // 
            this.tbD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbD.Location = new System.Drawing.Point(79, 114);
            this.tbD.Name = "tbD";
            this.tbD.ReadOnly = true;
            this.tbD.Size = new System.Drawing.Size(844, 20);
            this.tbD.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Current:";
            // 
            // tbCur
            // 
            this.tbCur.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCur.Location = new System.Drawing.Point(79, 140);
            this.tbCur.Name = "tbCur";
            this.tbCur.ReadOnly = true;
            this.tbCur.Size = new System.Drawing.Size(844, 20);
            this.tbCur.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 169);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Processed:";
            // 
            // tbPocessed
            // 
            this.tbPocessed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPocessed.Location = new System.Drawing.Point(79, 166);
            this.tbPocessed.Name = "tbPocessed";
            this.tbPocessed.ReadOnly = true;
            this.tbPocessed.Size = new System.Drawing.Size(788, 20);
            this.tbPocessed.TabIndex = 1;
            // 
            // lbTotal
            // 
            this.lbTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTotal.AutoSize = true;
            this.lbTotal.Location = new System.Drawing.Point(873, 169);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(28, 13);
            this.lbTotal.TabIndex = 2;
            this.lbTotal.Text = "Left:";
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Location = new System.Drawing.Point(12, 219);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(911, 128);
            this.tbLog.TabIndex = 3;
            // 
            // lbFails
            // 
            this.lbFails.AutoSize = true;
            this.lbFails.Location = new System.Drawing.Point(12, 200);
            this.lbFails.Name = "lbFails";
            this.lbFails.Size = new System.Drawing.Size(35, 13);
            this.lbFails.TabIndex = 5;
            this.lbFails.Text = "label8";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(848, 353);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestRSAInterval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 388);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbFails);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbPocessed);
            this.Controls.Add(this.tbCur);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbD);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbE);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbN);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbTo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbFrom);
            this.Controls.Add(this.label1);
            this.Name = "TestRSAInterval";
            this.Text = "TestRSAInterval";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestRSAInterval_FormClosing);
            this.Load += new System.EventHandler(this.TestRSAInterval_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbN;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbE;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbD;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbCur;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPocessed;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label lbFails;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
    }
}