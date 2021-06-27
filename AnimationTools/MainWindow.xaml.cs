using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AnimationLibrary;
using System.Drawing;

namespace AnimationTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BindingVariableData();

        }

        #region ==================== 成员变量 ====================

        private GlobalData mVariableData = new GlobalData();

        private System.Windows.Forms.OpenFileDialog PNGOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
        private System.Windows.Forms.FolderBrowserDialog PNGFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

        private System.Windows.Forms.SaveFileDialog TEXSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
        private System.Windows.Forms.SaveFileDialog XMLSaveFileDialog = new System.Windows.Forms.SaveFileDialog();

        DataSet dataset = null;
        AnimationBundleReader bundles = null;
        string file = "";
        string path = "";
        string name = "";

        #endregion


        #region ==================== 成员函数 ====================
        /// <summary>
        /// 绑定变量数据
        /// </summary>
        private void BindingVariableData()
        {
            ListBoxFileList.SetBinding(ListBox.ItemsSourceProperty, new Binding(nameof(GlobalData.CurrentFileList)) { Source = mVariableData, Mode = BindingMode.TwoWay });

            Reserved1.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(GlobalData.Reserved1)) { Source = mVariableData, Mode = BindingMode.TwoWay });
            Reserved2.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(GlobalData.Reserved2)) { Source = mVariableData, Mode = BindingMode.TwoWay });
            Reserved3.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(GlobalData.Reserved3)) { Source = mVariableData, Mode = BindingMode.TwoWay });

            LabelTipContents.SetBinding(Label.ContentProperty, new Binding(nameof(GlobalData.TipContents)) { Source = mVariableData, Mode = BindingMode.TwoWay });

            ProgressBarConvert.SetBinding(ProgressBar.ValueProperty, new Binding(nameof(GlobalData.ProgressValue)) { Source = mVariableData, Mode = BindingMode.TwoWay });
            ProgressBarConvert.SetBinding(ProgressBar.MaximumProperty, new Binding(nameof(GlobalData.ProgressMax)) { Source = mVariableData, Mode = BindingMode.TwoWay });

            BtnPackedNormalPic.SetBinding(Button.IsEnabledProperty, new Binding(nameof(GlobalData.IsBtnPackedNormalPicEnable)) { Source = mVariableData, Mode = BindingMode.TwoWay });
        }

        /// <summary>
        /// 添加文件到列表
        /// </summary>
        /// <param name="paths"></param>
        private void AddFiles(IEnumerable<string> paths)
        {
            foreach (string current in paths)
            {
                if (Directory.Exists(current))
                {
                    AddFiles(Directory.GetFiles(current, "*", SearchOption.AllDirectories));
                }
                else if (AnimationTextureExporter.IsImageFile(current) && !mVariableData.CurrentFileList.Contains(current))
                {
                    mVariableData.CurrentFileList.Add(current);
                    if (mVariableData.CurrentFileList.Count > 0)
                    {
                        //BtnPackedNormalPic.IsEnabled = true;
                        //BtnUnPackNormal.IsEnabled = true;

                        BtnClearAllList.IsEnabled = true;
                    }
                }
            }
        }

        private static bool DirectoryContainsImages(string directory)
        {
            string[] array = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            for (int i = 0; i < array.Length; i++)
            {
                //if (MiscHelper.IsImageFile(array[i]))
                //{
                //    return true;
                //}
            }
            return false;
        }

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

        #endregion

        private void ListBoxFileList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) { return; }

            bool flag = false;
            string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if (Directory.Exists(text) && DirectoryContainsImages(text))
                {
                    flag = true;
                    break;
                }
                if (AnimationTextureExporter.IsImageFile(text))
                {
                    flag = true;
                    break;
                }
            }
            e.Effects = (flag ? DragDropEffects.Copy : DragDropEffects.None);
        }


        private void ListBoxFileList_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void ListBox_FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != ListBoxFileList.SelectedItem)
            {
                BtnRemoveSelect.IsEnabled = true;
                BtnUnPackNormal.IsEnabled = true;
                //mVariableData.selectPictureIndex = ListBoxFileList.SelectedIndex;

                if (!System.IO.Path.GetExtension(ListBoxFileList.SelectedItem.ToString()).Equals(".tex"))
                {
                    BitmapImage bmi = new BitmapImage(new Uri(ListBoxFileList.SelectedItem.ToString()));
                    ImageCurrentFile.Source = bmi;
                }
                else
                {
                    ImageCurrentFile.Source = null;
                }
            }
            else
            {
                BtnRemoveSelect.IsEnabled = false;
                ImageCurrentFile.Source = null;
            }
        }

        private void BtnPacked_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUnPack_Click(object sender, RoutedEventArgs e)
        {
            bundles = new AnimationBundleReader();
            file = ListBoxFileList.SelectedItem.ToString();
            try
            {
                path = file.Remove(file.LastIndexOf('_'));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                mVariableData.TipContents = "解包" + name + "失败：图片文件名格式错误";
                return;
            }
            name = file.Replace("/", "\\");
            name = name.Substring(name.LastIndexOf("\\") + 1, (name.LastIndexOf(".") - name.LastIndexOf("\\") - 1));
            Directory.CreateDirectory(path);

            System.Drawing.Image source = System.Drawing.Image.FromFile(file);
            try
            {
                dataset = bundles.ReadFile(path, source.Width, source.Height);
            }
            catch (System.IO.FileNotFoundException)
            {
                mVariableData.TipContents = "解包" + name + "失败：找不到对应 build/anim 文件";
                return;
            }
            //dataGridView1.DataSource = ds.Tables["animTable"];

            ExportTexture();

            AnimationConfigExporter animationConfigExporter = new AnimationConfigExporter();
            animationConfigExporter.InitData(dataset, bundles, path, name);

            mVariableData.TipContents = "解包" + name + "成功";
        }

        private void BtnRemoveSelect_Click(object sender, RoutedEventArgs e)
        {
            mVariableData.CurrentFileList.Remove(ListBoxFileList.SelectedItem.ToString());

            if (null != ListBoxFileList.SelectedItem)
            {
                BtnRemoveSelect.IsEnabled = true;
            }
            else
            {
                BtnRemoveSelect.IsEnabled = false;
            }

            if (mVariableData.CurrentFileList.Count > 0)
            {
                BtnPackedNormalPic.IsEnabled = true;
                BtnUnPackNormal.IsEnabled = true;

                BtnClearAllList.IsEnabled = true;
            }
            else
            {
                BtnPackedNormalPic.IsEnabled = false;
                BtnUnPackNormal.IsEnabled = false;

                BtnClearAllList.IsEnabled = false;
            }
        }

        private void BtnClearAllList_Click(object sender, RoutedEventArgs e)
        {
            mVariableData.CurrentFileList.Clear();

            BtnPackedNormalPic.IsEnabled = false;
            BtnUnPackNormal.IsEnabled = false;

            BtnRemoveSelect.IsEnabled = false;
            BtnClearAllList.IsEnabled = false;
        }

        private void MemuItemAddFile_Click(object sender, RoutedEventArgs e)
        {
            PNGOpenFileDialog.Filter = "(*.png,*.jpg)|*.png;*.jpg|All Files(*.*)|*.*";

            if (PNGOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddFiles(PNGOpenFileDialog.FileNames);
            }
        }

        private void MemuAddFolder_Click(object sender, RoutedEventArgs e)
        {
            if (PNGFolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddFiles(new List<string> { PNGFolderBrowserDialog.SelectedPath });
            }
        }

    }
}
