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
using System.Text.RegularExpressions;
using ImageMagick;

namespace ImageTransformer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //string[] outputType = { 
            //    ".jpg", ".jepg", ".png", ".bmp", ".pdf", ".webp" 
            //};
            comboBox1.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text.Trim();
            string outputType = comboBox1.Text.Trim();
            // 需要对路径进行验证
            if (Directory.Exists(path))
            {
                TransformToJpg(path, isDirectory:true, outputType: outputType);
            }
            else if (File.Exists(path))
            {
                bool checkResult = CheckInputImageType(path);
                if (checkResult)
                {

                    TransformToJpg(path, outputType: outputType);
                }
                else
                {
                    MessageBox.Show(
                        $"当前程序不支持这种格式的文件：[{path}]",
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

        private bool CheckInputImageType(string path)
        {
            string pattern = @"\.(jpg|jpeg|png|gif|tiff|tif|bmp|webp|heic|heif|ico|svg)$";
            Match match = Regex.Match(path, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TransformOneImageToJpg(string path, string outputType=".jpg", string outputDirectory=null, bool showMessage=true)
        {
            using (MagickImage image = new MagickImage(path))
            {
                switch (outputType)
                {
                    case ".jpg":
                        image.Format = MagickFormat.Jpeg;
                        break;
                    case ".jpeg":
                        image.Format = MagickFormat.Jpeg;
                        break;
                    case ".png":
                        image.Format = MagickFormat.Png;
                        break;
                    case ".bmp":
                        image.Format = MagickFormat.Bmp;
                        break;
                    case ".pdf":
                        image.Format = MagickFormat.Pdf;
                        break;
                    case ".webp":
                        image.Format = MagickFormat.WebP;
                        break;
                    case ".html":
                        image.Format = MagickFormat.Html;
                        break;
                    default:
                        return;
                }
                if (outputDirectory == null)
                {
                    outputDirectory = Path.GetDirectoryName(path);
                }
                string filename = Path.GetFileName(path);
                filename = filename.Split('.')[0] + outputType;
                string filepath = Path.Combine(outputDirectory, filename);
                image.Write(filepath);
                if (showMessage)
                {
                    MessageBox.Show($"{filepath} 保存成功。", "提示");
                }
            }
        }

        private void TransformToJpg(
            string path,
            bool isDirectory = false,
            string outputType = ".jpg",
            bool showMessage=true
        )
        {
            if (!isDirectory)
            {
                TransformOneImageToJpg(path, outputType:outputType);
            }
            else
            {
                List<string> images = new List<string>();
                foreach (string file in Directory.GetFiles(path))
                {
                    if (CheckInputImageType(file))
                    {
                        images.Add(file);
                    }
                }
                if (images.Count == 0) 
                {
                    MessageBox.Show($"在[{path}]路径下没有找到符合规定格式的图片。", "提示");
                }
                else
                {
                    string outputDirectory = path + "_" + outputType.Trim('.');
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                    foreach (string image in images) 
                    {
                        TransformOneImageToJpg(image, outputType, outputDirectory, showMessage: false);
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
