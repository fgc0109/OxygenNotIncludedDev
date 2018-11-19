using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnimationLibrary;

namespace AnimationConverter
{
    public partial class AnimationDataViewer : Form
    {
        public AnimationDataViewer()
        {
            InitializeComponent();
        }

        DataSet dataset = null;
        AnimationBundleReader bundles = null;
        string file = "";
        string path = "";
        string name = "";

        /// <summary>
        /// 导出图片
        /// </summary>
        private void ExportTexture()
        {
            Directory.CreateDirectory(path);
            for (int frame = 0; frame < dataset.Tables["bildTable"].Rows.Count; frame++)
            {
                var row = dataset.Tables["bildTable"].Rows[frame];
                var img = AnimationTextureExporter.TextureSlicing(
                    file,
                    Convert.ToInt32(row["x1"]), Convert.ToInt32(row["y2"]), Convert.ToInt32(row["x2"]), Convert.ToInt32(row["y1"])
                    );
                img.Save(path + "\\" + row["name"] + "_" + row["index"] + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            file = textBoxPath.Text + textBoxFile.Text;
            path = file.Remove(file.LastIndexOf('_'));
            name = file.Replace("/", "\\");
            name = name.Substring(name.LastIndexOf("\\") + 1, (name.LastIndexOf(".") - name.LastIndexOf("\\") - 1));

            Image source = Image.FromFile(file);

            bundles = new AnimationBundleReader();
            dataset = bundles.ReadFile(path, source.Width, source.Height);
            dataGridView1.DataSource = dataset.Tables["animTable"];

            ExportTexture();
        }

        private void button2_Click(object sender, EventArgs e)

        {
            AnimationConfigExporter animationConfigExporter = new AnimationConfigExporter();
            animationConfigExporter.InitData(dataset, bundles, path, name);
        }
    }
}
