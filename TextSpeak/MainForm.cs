using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Threading;

namespace TextSpeak
{
    public partial class MainForm : Form
    {
        SpeechSynthesizer speech = null;
        private int value = 100; //音量
        private int rate=50; //语速
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            speech = new SpeechSynthesizer();
            speech.Rate = rate;
            speech.SelectVoice("Microsoft Lili");//设置播音员（中文）
            //speech.SelectVoice("Microsoft Anna"); //英文
            speech.Volume = value;
        }

        private void Speak()
        {
            btRead.Enabled = true;
            
            speech.SpeakAsync(tbContent.Text);//语音阅读方法
            speech.SpeakCompleted += speech_SpeakCompleted;//绑定事件
        }

        void speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            btRead.Enabled = true;
        }

        /// <summary>
        /// 生成语音文件的方法
        /// </summary>
        /// <param name="text"></param>
        private void SaveFile(string text)
        {
            speech = new SpeechSynthesizer();
            var dialog = new SaveFileDialog();
            dialog.Filter = "*.wav|*.wav|*.mp3|*.mp3";
            dialog.ShowDialog();

            string path = dialog.FileName;
            if (path.Trim().Length == 0)
            {
                return;
            }
            speech.SetOutputToWaveFile(path);
            speech.Volume = value;
            speech.Rate = rate;
            speech.Speak(text);
            speech.SetOutputToNull();
            MessageHelper.ShowInfo("生成成功!");

        }

        private void btRead_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbContent.Text))
            {
                MessageHelper.ShowError("内容不能为空！");
            }
            else
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        Speak();
                    }));
                });
            }
        }
    }
}
