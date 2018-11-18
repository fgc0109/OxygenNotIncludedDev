using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace AnimationLibrary
{
    #region ============================== BILD STRUCT ==============================

    public struct Bild
    {
        public int version;
        public int symbols;
        public int frames;
        public string name;

        public List<BildSymbol> symbolsList;
    }

    public struct BildSymbol
    {
        public int hash;
        public int path;

        public int color;
        public int flags;
        public int numFrames;

        public List<BildFrame> framesList;
    }

    public struct BildFrame
    {
        public int sourceFrameNum;
        public int duration;
        public int buildImageIdx;

        public float num6;
        public float num7;
        public float num8;
        public float num9;

        public float x1;
        public float y1;
        public float x2;
        public float y2;

        public int time;
    }

    #endregion

    #region ============================== BILD STRUCT ==============================

    public struct Anim
    {
        public int version;
        public int space1;
        public int space2;
        public int symbols;

        public List<AnimSymbol> symbolsList;
    }

    public struct AnimSymbol
    {
        public string name;
        public int hash;

        public float rate;
        public int frames;

        public List<AnimFrame> framesList;
    }

    public struct AnimFrame
    {
        public float num3;
        public float num4;
        public float num5;
        public float num6;

        public int elements;

        public List<AnimElement> elementsList;
    }

    public struct AnimElement
    {
        public int symbol;
        public int frame;
        public int folder;
        public int flags;

        public float a;
        public float b;
        public float g;
        public float r;

        public float m1;
        public float m2;
        public float m3;
        public float m4;
        public float m5;
        public float m6;

        public float space;
    }

    #endregion

    public class AnimationBundleReader
    {
        private FileStream bildStream = null;
        private FileStream animStream = null;
        private BinaryReader bildReader = null;
        private BinaryReader animReader = null;
        private DataTable bildTable = null;
        private DataTable animTable = null;

        public Bild BildData { get; private set; } = new Bild();
        public Anim AnimData { get; private set; } = new Anim();
        public Dictionary<int, string> BildHash { get; private set; }
        public Dictionary<int, string> AnimHash { get; private set; }

        public int ImageH { get; private set; }
        public int ImageW { get; private set; }

        /// <summary>
        /// 检查文件头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="reader"></param>
        private static void CheckHeader(string header, BinaryReader reader)
        {
            char[] array = reader.ReadChars(header.Length);
            for (int i = 0; i < header.Length; i++)
            {
                if (array[i] != header[i]) { throw new Exception("Expected " + header); }
            }
        }

        /// <summary>
        /// 读取序列化字符串
        /// </summary>
        /// <param name="reader">流读写器</param>
        /// <returns></returns>
        public static string ReadKleiString(BinaryReader reader)
        {
            int num = reader.ReadInt32();

            if (num < 0) return null;
            return Encoding.UTF8.GetString(reader.ReadBytes(num));
        }

        /// <summary>
        /// 读取哈希表
        /// </summary>
        /// <param name="reader">流读写器</param>
        /// <returns></returns>
        private static Dictionary<int, string> ParseHashTable(BinaryReader reader)
        {
            int num = reader.ReadInt32();
            Dictionary<int, string> hashTable = new Dictionary<int, string>();

            for (int i = 0; i < num; i++)
            {
                int hash = reader.ReadInt32();
                string text = ReadKleiString(reader);
                hashTable.Add(hash, text);
            }
            return hashTable;
        }

        public DataSet ReadFile(string path, int winth, int height)
        {
            DataSet data = new DataSet();

            ImageW = winth;
            ImageH = height;

            bildTable = new DataTable();
            bildTable.TableName = nameof(bildTable);

            animTable = new DataTable();
            animTable.TableName = nameof(animTable);

            bildStream = new FileStream(path + "_build.txt", FileMode.Open);
            animStream = new FileStream(path + "_anim.txt", FileMode.Open);

            bildReader = new BinaryReader(bildStream, Encoding.UTF8);
            animReader = new BinaryReader(animStream, Encoding.UTF8);

            ParseBildData();
            ParseBildDataTable();
            ParseAnimData();
            ParseAnimDataTable();

            data.Tables.Add(bildTable);
            data.Tables.Add(animTable);

            bildReader.Close();
            animReader.Close();

            return data;
        }

        #region ============================== BILD ==============================

        public void ParseBildData()
        {
            CheckHeader("BILD", bildReader);

            BildData = new Bild
            {
                version = bildReader.ReadInt32(),                                   //BILD文件版本
                symbols = bildReader.ReadInt32(),                                   //文件类型数量(文件夹数量)
                frames = bildReader.ReadInt32(),                                    //文件数量
                name = ReadKleiString(bildReader),                                  //BANK名称

                symbolsList = new List<BildSymbol>(),
            };

            for (int i = 0; i < BildData.symbols; i++)
            {
                BildSymbol symbol = new BildSymbol
                {
                    hash = bildReader.ReadInt32(),                                  //类型哈希
                    path = BildData.version > 9 ? bildReader.ReadInt32() : 0,       //类型路径哈希
                    color = bildReader.ReadInt32(),                                 //类型颜色
                    flags = bildReader.ReadInt32(),                                 //类型标志
                    numFrames = bildReader.ReadInt32(),                             //类型文件数量

                    framesList = new List<BildFrame>(),
                };

                bool isFirstFrame = true;
                int lastTime = 0;
                for (int j = 0; j < symbol.numFrames; j++)
                {
                    BildFrame frame = new BildFrame
                    {
                        sourceFrameNum = bildReader.ReadInt32(),
                        duration = bildReader.ReadInt32(),
                        buildImageIdx = bildReader.ReadInt32(),

                        num6 = bildReader.ReadSingle(),
                        num7 = bildReader.ReadSingle(),
                        num8 = bildReader.ReadSingle(),
                        num9 = bildReader.ReadSingle(),

                        x1 = bildReader.ReadSingle(),
                        y1 = bildReader.ReadSingle(),
                        x2 = bildReader.ReadSingle(),
                        y2 = bildReader.ReadSingle(),
                    };
                    frame.time = isFirstFrame ? 0 : lastTime + frame.duration;
                    lastTime = frame.time;
                    symbol.framesList.Add(frame);
                }
                BildData.symbolsList.Add(symbol);
            }
            BildHash = ParseHashTable(bildReader);
        }

        private void ParseBildDataTable()
        {
            bildTable.Columns.Add("bild", typeof(Bild));
            bildTable.Columns.Add("name", typeof(string));
            bildTable.Columns.Add("index", typeof(float));
            bildTable.Columns.Add("hash", typeof(int));
            bildTable.Columns.Add("time", typeof(int));
            bildTable.Columns.Add("duration", typeof(int));
            bildTable.Columns.Add("x1", typeof(float));
            bildTable.Columns.Add("y1", typeof(float));
            bildTable.Columns.Add("x2", typeof(float));
            bildTable.Columns.Add("y2", typeof(float));
            bildTable.Columns.Add("w", typeof(float));
            bildTable.Columns.Add("h", typeof(float));
            bildTable.Columns.Add("num6", typeof(float));
            bildTable.Columns.Add("num7", typeof(float));
            bildTable.Columns.Add("num8", typeof(float));
            bildTable.Columns.Add("num9", typeof(float));

            foreach (var symbol in BildData.symbolsList)
            {
                foreach (var frame in symbol.framesList)
                {
                    bildTable.Rows.Add(BildData,
                        BildHash[symbol.hash], frame.sourceFrameNum, symbol.hash,
                        frame.time, frame.duration,
                        frame.x1 * ImageW, (1 - frame.y1) * ImageH, frame.x2 * ImageW, (1 - frame.y2) * ImageH, (frame.x2 - frame.x1) * ImageW, (frame.y2 - frame.y1) * ImageH,
                        frame.num6, frame.num7, frame.num8, frame.num9
                        );
                }
            }
        }

        #endregion

        #region ============================== ANIM ==============================


        public void ParseAnimData()
        {
            CheckHeader("ANIM", animReader);
            AnimData = new Anim
            {
                version = animReader.ReadInt32(),
                space1 = animReader.ReadInt32(),
                space2 = animReader.ReadInt32(),
                symbols = animReader.ReadInt32(),

                symbolsList = new List<AnimSymbol>(),
            };

            for (int i = 0; i < AnimData.symbols; i++)
            {
                AnimSymbol symbol = new AnimSymbol
                {
                    name = ReadKleiString(animReader),                              //动画名
                    hash = animReader.ReadInt32(),                                  //
                    rate = animReader.ReadSingle(),                                 //
                    frames = animReader.ReadInt32(),                                //动画帧数

                    framesList = new List<AnimFrame>(),
                };

                for (int j = 0; j < symbol.frames; j++)
                {
                    AnimFrame frame = new AnimFrame
                    {
                        num3 = animReader.ReadSingle(),
                        num4 = animReader.ReadSingle(),
                        num5 = animReader.ReadSingle(),
                        num6 = animReader.ReadSingle(),

                        elements = animReader.ReadInt32(),

                        elementsList = new List<AnimElement>(),
                    };

                    for (int k = 0; k < frame.elements; k++)
                    {
                        AnimElement element = new AnimElement
                        {
                            symbol = animReader.ReadInt32(),
                            frame = animReader.ReadInt32(),
                            folder = animReader.ReadInt32(),
                            flags = animReader.ReadInt32(),

                            a = animReader.ReadSingle(),
                            b = animReader.ReadSingle(),
                            g = animReader.ReadSingle(),
                            r = animReader.ReadSingle(),

                            m1 = animReader.ReadSingle(),
                            m2 = animReader.ReadSingle(),
                            m3 = animReader.ReadSingle(),
                            m4 = animReader.ReadSingle(),
                            m5 = animReader.ReadSingle(),
                            m6 = animReader.ReadSingle(),


                            space = animReader.ReadSingle(),
                        };
                        frame.elementsList.Add(element);
                    }
                    symbol.framesList.Add(frame);
                }
                AnimData.symbolsList.Add(symbol);
            }
            int maxVisSymbolFrames = animReader.ReadInt32();
            AnimHash = ParseHashTable(animReader);
        }

        public void ParseAnimDataTable()
        {
            animTable.Columns.Add("anim", typeof(Anim));
            animTable.Columns.Add("name", typeof(string));
            animTable.Columns.Add("hashname", typeof(string));
            //animTable.Columns.Add("index", typeof(float), "");
            //animTable.Columns.Add("hash", typeof(int), "");
            animTable.Columns.Add("num3", typeof(float));
            animTable.Columns.Add("num4", typeof(float));
            animTable.Columns.Add("num5", typeof(float));
            animTable.Columns.Add("num6", typeof(float));
            //animTable.Columns.Add("w", typeof(float));
            //animTable.Columns.Add("h", typeof(float));
            animTable.Columns.Add("elemnetHashSymbol", typeof(string));
            animTable.Columns.Add("elemnetFrame", typeof(int));
            animTable.Columns.Add("elemnetHashFolder", typeof(string));
            animTable.Columns.Add("elemnetFlags", typeof(int));

            animTable.Columns.Add("m1", typeof(float));
            animTable.Columns.Add("m2", typeof(float));
            animTable.Columns.Add("m3", typeof(float));
            animTable.Columns.Add("m4", typeof(float));
            animTable.Columns.Add("m5", typeof(float));
            animTable.Columns.Add("m6", typeof(float));

            foreach (var symbol in AnimData.symbolsList)
            {
                foreach (var frame in symbol.framesList)
                {
                    foreach (var element in frame.elementsList)
                    {
                        animTable.Rows.Add(AnimData,
                            symbol.name, AnimHash[symbol.hash],
                            frame.num3, frame.num4, frame.num5, frame.num6,
                            AnimHash[element.symbol], element.frame, AnimHash[element.folder], element.flags,
                            element.m1, element.m2, element.m3, element.m4, element.m5, element.m6
                            );
                    }
                }
            }
            //foreach (var element in AnimData.symbolsList[1].framesList[1].elementsList)
            //{
            //    animTable.Rows.Add(AnimData,
            //        AnimData.symbolsList[1].name, AnimHash[AnimData.symbolsList[1].hash],
            //        AnimData.symbolsList[1].framesList[1].num3, AnimData.symbolsList[1].framesList[1].num4, AnimData.symbolsList[1].framesList[1].num5, AnimData.symbolsList[1].framesList[1].num6,
            //        AnimHash[element.symbol], element.frame, AnimHash[element.folder], element.flags,
            //        element.m1, element.m2, element.m3, element.m4, element.m5, element.m6
            //        );
            //}
        }
    }
    #endregion
}

