using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MergeSubtitles
{
    public partial class MergeSubForm : Form
    {
        string ffmpeg = "";
        string video = "";
        string videoPath = "";
        string srt = "";
        string videoOut = "";

        public MergeSubForm()
        {
            InitializeComponent();
        }

        private void btnSelectFFMpeg_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if(fb.ShowDialog() == DialogResult.OK)
            {
                ffmpeg = fb.SelectedPath + @"\bin\ffmpeg.exe";
                label1.Text = ffmpeg;
            }
        }

        public void ExecuteCommandSync(string command, string arguments)
        {
            try
            {

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = command;
                startInfo.Arguments = arguments;
                
                Process.Start(startInfo);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        private void btnSelectVideo_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if(fd.ShowDialog() == DialogResult.OK)
            {
                video = fd.FileName;
                videoPath = Path.GetDirectoryName(video);
                video = Path.GetFileName(video);
                label2.Text = videoPath + @"\" + video;
                videoOut = Path.GetFileNameWithoutExtension(video) + "-OUTPUT." + Path.GetExtension(video);
            }
        }

        private void btnSelectSRT_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                srt = Path.GetFileName(fd.FileName);
                label3.Text = srt;
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            Directory.SetCurrentDirectory(videoPath);
            var ass = srt.Replace(".srt", ".ass");
            var args = "-i " + srt + " " + ass;
            // ffmpeg -i test.mp4 -vf "ass=test.ass" test-output.mp4
            ExecuteCommandSync(ffmpeg, args);
            
            args = "-i " + video + " -vf \"ass=" + ass + "\" " + videoOut;
            ExecuteCommandSync(ffmpeg, args);
            label4.Text = videoPath + @"\" + videoOut;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-20160924-267da70-win64-static.zip");
            Process.Start(sInfo);
        }
    }
}
