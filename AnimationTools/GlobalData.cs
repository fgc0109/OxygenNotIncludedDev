using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationTools
{
    class GlobalData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> mCurrentFileList = new ObservableCollection<string>();

        private bool mGeneratePNG = false;
        private bool mGenerateXML = true;

        private bool mGenerateTileTexture = false;
        private bool mGeneratePowersOfTwo = true;
        private bool mGenerateCropTexture = true;

        private bool mIsBtnPackedNineSliceEnable = false;
        private bool mIsBtnPackedNormalPicEnable = false;

        private string mFilePathPNG = Environment.CurrentDirectory + @"\temp.tex";
        private string mFilePathXML = Environment.CurrentDirectory + @"\temp.xml";

        private string mTipContents = "";

        private double mProgressValue = 0.0;
        private double mProgressMax = 100.0;

        //用于进程间传递生成的图片数据
        public int buildImageW = 0;
        public int buildImageH = 0;
        public Bitmap buildImageBitmap = null;

        /// <summary>
        /// 当前文件列表
        /// </summary>
        public ObservableCollection<string> CurrentFileList
        {
            get { return mCurrentFileList; }
            set
            {
                mCurrentFileList = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentFileList)));
            }
        }

        /// <summary>
        /// PNG文件路径
        /// </summary>
        public bool Reserved1
        {
            get { return mGenerateTileTexture; }
            set
            {
                mGenerateTileTexture = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Reserved1)));
            }
        }

        /// <summary>
        /// 生成2的幂次方大小的图片
        /// </summary>
        public bool Reserved2
        {
            get { return mGeneratePowersOfTwo; }
            set
            {
                mGeneratePowersOfTwo = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Reserved2)));
            }
        }

        /// <summary>
        /// 裁切透明像素
        /// </summary>
        public bool Reserved3
        {
            get { return mGenerateCropTexture; }
            set
            {
                mGenerateCropTexture = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Reserved3)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBtnPackedNormalPicEnable
        {
            get { return mIsBtnPackedNormalPicEnable; }
            set
            {
                mIsBtnPackedNormalPicEnable = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsBtnPackedNormalPicEnable)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBtnPackedNineSliceEnable
        {
            get { return mIsBtnPackedNineSliceEnable; }
            set
            {
                mIsBtnPackedNineSliceEnable = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsBtnPackedNineSliceEnable)));
            }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string TipContents
        {
            get { return mTipContents; }
            set
            {
                mTipContents = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(TipContents)));
            }
        }

        /// <summary>
        /// 进度条进度
        /// </summary>
        public double ProgressValue
        {
            get { return mProgressValue; }
            set
            {
                mProgressValue = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressValue)));
            }
        }
        /// <summary>
        /// 进度条最大值
        /// </summary>
        public double ProgressMax
        {
            get { return mProgressMax; }
            set
            {
                mProgressMax = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressMax)));
            }
        }

    }
}
