using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageMagick;

namespace ImageTransformer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text.Trim();
            // 需要对路径进行验证
            if (Directory.Exists(path))
            {
                TransformToJpg(path, isDirectory:true);
            }
            else if (File.Exists(path))
            {
                if (path.EndsWith(".webp"))
                {
                    TransformToJpg(path);
                }
                else
                {
                    MessageBox.Show(
                        "不支持这种格式的文件",
                        "提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            else
            {
                MessageBox.Show(
                    "文件或文件夹不存在",
                    "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void TransformOneImageToJpg(string path, string outputDirectory=null, bool showMessage=true)
        {
            using (MagickImage image = new MagickImage(path))
            {
                image.Format = MagickFormat.Jpeg;
                if (outputDirectory == null)
                {
                    outputDirectory = Path.GetDirectoryName(path);
                }
                string filename = Path.GetFileName(path);
                filename = filename.Split('.')[0] + ".jpg";
                string filepath = Path.Combine(outputDirectory, filename);
                image.Write(filepath);
                if (showMessage)
                {
                    MessageBox.Show($"{filepath} 保存成功。", "提示");
                }
            }
        }

        private void TransformToJpg(string path, bool isDirectory=false, bool showMessage=true)
        {
            if (!isDirectory)
            {
                TransformOneImageToJpg(path);
            }
            else
            {
                List<string> images = new List<string>();
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.EndsWith(".webp"))
                    {
                        images.Add(file);
                    }
                }
                if (images.Count == 0) 
                {
                    MessageBox.Show($"在[{path}]路径下没有找到.webp后缀的图片。", "提示");
                }
                else
                {
                    string outputDirectory = path + "_transform";
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                    foreach (string image in images) 
                    {
                        TransformOneImageToJpg(image, outputDirectory, showMessage: false);
                    }
                    MessageBox.Show($"{outputDirectory} 保存成功。", "提示");
                }
            }
        }

        private void ViewHelpMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm helpform = new HelpForm();
            helpform.ShowDialog();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0) 
            {
                textBox1.Text = files[0];
                textBox1.SelectionStart = textBox1.Text.Length;
            }
        }
    }
}
