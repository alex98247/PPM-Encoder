using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace PPM_Encoder
{
    public partial class Form1 : Form
    {
        private PpmCompressor ppmCompressor;
        private string saveFileName;

        public Form1()
        {
            ppmCompressor = new PpmCompressor();
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    var filePath = openFileDialog.FileName;
                    textBox1.Text = filePath;
                }
            }
        }

        private void Encode_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    saveFileName = saveFileDialog.FileName;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var filePath = textBox1.Text;
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var reader = new FileStream(filePath, FileMode.Open);
            var writer = new FileStream(saveFileName, FileMode.OpenOrCreate);
            ppmCompressor.Compress(reader, new BitOutputStream(writer));
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Сжатие завершено");
        }
    }
}