namespace MergeSubtitles
{
    partial class MergeSubForm
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
            this.btnSelectFFMpeg = new System.Windows.Forms.Button();
            this.btnSelectVideo = new System.Windows.Forms.Button();
            this.btnSelectSRT = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnSelectFFMpeg
            // 
            this.btnSelectFFMpeg.Location = new System.Drawing.Point(15, 36);
            this.btnSelectFFMpeg.Name = "btnSelectFFMpeg";
            this.btnSelectFFMpeg.Size = new System.Drawing.Size(116, 81);
            this.btnSelectFFMpeg.TabIndex = 0;
            this.btnSelectFFMpeg.Text = "Select FFMpeg Directory";
            this.btnSelectFFMpeg.UseVisualStyleBackColor = true;
            this.btnSelectFFMpeg.Click += new System.EventHandler(this.btnSelectFFMpeg_Click);
            // 
            // btnSelectVideo
            // 
            this.btnSelectVideo.Location = new System.Drawing.Point(15, 123);
            this.btnSelectVideo.Name = "btnSelectVideo";
            this.btnSelectVideo.Size = new System.Drawing.Size(116, 81);
            this.btnSelectVideo.TabIndex = 1;
            this.btnSelectVideo.Text = "Select Video";
            this.btnSelectVideo.UseVisualStyleBackColor = true;
            this.btnSelectVideo.Click += new System.EventHandler(this.btnSelectVideo_Click);
            // 
            // btnSelectSRT
            // 
            this.btnSelectSRT.Location = new System.Drawing.Point(15, 210);
            this.btnSelectSRT.Name = "btnSelectSRT";
            this.btnSelectSRT.Size = new System.Drawing.Size(116, 80);
            this.btnSelectSRT.TabIndex = 2;
            this.btnSelectSRT.Text = "Select SRT File";
            this.btnSelectSRT.UseVisualStyleBackColor = true;
            this.btnSelectSRT.Click += new System.EventHandler(this.btnSelectSRT_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.Location = new System.Drawing.Point(15, 296);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(116, 80);
            this.btnMerge.TabIndex = 3;
            this.btnMerge.Text = "Merge Subtitles";
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(146, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "<FFMpeg directory is not selected>";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(146, 157);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "<video is not selected>";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(146, 244);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "<srt file is not selected>";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(145, 330);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "<output>";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(12, 9);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(97, 13);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Download FFMpeg";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // MergeSubForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 391);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.btnSelectSRT);
            this.Controls.Add(this.btnSelectVideo);
            this.Controls.Add(this.btnSelectFFMpeg);
            this.Name = "MergeSubForm";
            this.Text = "Merge Subtitles";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFFMpeg;
        private System.Windows.Forms.Button btnSelectVideo;
        private System.Windows.Forms.Button btnSelectSRT;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

