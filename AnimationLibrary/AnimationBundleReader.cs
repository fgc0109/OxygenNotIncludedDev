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

    #region ============================== ANIM STRUCT ==============================

    public struct Anim
    {
        public int version;
        public int elements;
        public int frames;
        public int anims;

        public List<AnimBank> animList;
    }

    public struct AnimBank
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
        public int image;
        public int index;
        public int layer;
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

        public float order;
        public float repeat;//额外添加 用于区分物体在场景中复用而没有区分的情况
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
                version = animReader.ReadInt32(),                                   //版本号
                elements = animReader.ReadInt32(),                                  //
                frames = animReader.ReadInt32(),                                    //
                anims = animReader.ReadInt32(),                                     //

                animList = new List<AnimBank>(),
            };

            for (int i = 0; i < AnimData.anims; i++)
            {
                AnimBank anim = new AnimBank
                {
                    name = ReadKleiString(animReader),                              //动画名
                    hash = animReader.ReadInt32(),                                  //
                    rate = animReader.ReadSingle(),                                 //
                    frames = animReader.ReadInt32(),                                //动画帧数

                    framesList = new List<AnimFrame>(),
                };

                for (int j = 0; j < anim.frames; j++)
                {
                    AnimFrame frame = new AnimFrame
                    {
                        num3 = animReader.ReadSingle(),                             //X
                        num4 = animReader.ReadSingle(),                             //Y
                        num5 = animReader.ReadSingle(),                             //W
                        num6 = animReader.ReadSingle(),                             //H

                        elements = animReader.ReadInt32(),

                        elementsList = new List<AnimElement>(),
                    };

                    for (int k = 0; k < frame.elements; k++)
                    {
                        AnimElement element = new AnimElement
                        {
                            image = animReader.ReadInt32(),
                            index = animReader.ReadInt32(),
                            layer = animReader.ReadInt32(),
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

                            order = animReader.ReadSingle(),
                            repeat = 0,
                        };
                        frame.elementsList.Add(element);
                    }
                    anim.framesList.Add(frame);
                }
                AnimData.animList.Add(anim);
            }
            int maxVisSymbolFrames = animReader.ReadInt32();
            AnimHash = ParseHashTable(animReader);
        }

        public void ParseAnimDataTable()
        {
            animTable.Columns.Add("ANIM_Name", typeof(string));                     //
            animTable.Columns.Add("ANIM_Hash", typeof(string));                     //
            animTable.Columns.Add("ANIM_Rate", typeof(float));                      //

            //
            animTable.Columns.Add("FRAM_Num3", typeof(float));                      //
            animTable.Columns.Add("FRAM_Num4", typeof(float));                      //
            animTable.Columns.Add("FRAM_Num5", typeof(float));                      //
            animTable.Columns.Add("FRAM_Num6", typeof(float));                      //

            //场景物体的文件和标志信息,用于区分使用了哪个文件以及文件在场景中复用的后缀
            animTable.Columns.Add("ELEM_Image", typeof(string));                    //文件名
            animTable.Columns.Add("ELEM_Layer", typeof(string));                    //文件复用图层后缀(一个文件可以在场景中引用多次,每个引用的名字不同)
            animTable.Columns.Add("image", typeof(int));                            //文件名Hash数值
            animTable.Columns.Add("index", typeof(int));                            //文件名后缀编号
            animTable.Columns.Add("layer", typeof(int));                            //文件图层Hash数值
            animTable.Columns.Add("flags", typeof(int));                            //文件标记(总是0)

            //场景中每个物体的额外信息,用于计算动画主线和物体时间线
            animTable.Columns.Add("idanim", typeof(int));                           //所在动画Bank
            animTable.Columns.Add("idframe", typeof(int));                          //所在动画主线的关键帧位置
            animTable.Columns.Add("idelement", typeof(int));                        //所在动画主线的关键帧中的排序
            animTable.Columns.Add("timeline", typeof(int));                         //所在时间线
            animTable.Columns.Add("line_key", typeof(int));                         //所在时间线中的关键帧的位置

            //
            animTable.Columns.Add("m1", typeof(float));                             //
            animTable.Columns.Add("m2", typeof(float));                             //
            animTable.Columns.Add("m3", typeof(float));                             //
            animTable.Columns.Add("m4", typeof(float));                             //
            animTable.Columns.Add("m5", typeof(float));                             //
            animTable.Columns.Add("m6", typeof(float));                             //

            animTable.Columns.Add("order", typeof(float));                          //文件关键帧排序(总为0,无效)
            animTable.Columns.Add("repeat", typeof(float));                         //额外添加

            int idanim = 0;
            foreach (var anim in AnimData.animList)
            {
                //if (idanim != 4)
                //{
                //    idanim++;
                //    continue;
                //}

                int idframe = 0;
                Dictionary<string, int> timelines = new Dictionary<string, int>(); //每个动画建立一组时间线
                foreach (var frame in anim.framesList)
                {
                    for (int idelement = 0; idelement < frame.elements; idelement++)
                    {
                        var element = AnimData.animList[idanim].framesList[idframe].elementsList[idelement];

                        int timelineid = 0;

                        string timeline = element.image + "_" + element.index + "_" + element.layer; //用这三个数据来区分是否属于一个时间线
                        if (!timelines.ContainsKey(timeline))
                        {
                            timelines.Add(timeline, 0);
                        }
                        else
                        {
                            //同一帧添加两个相同的时间线是不可能的,表明这两个时间线的数据没有区分
                            if (timelines[timeline] >= idframe)
                            {
                                for (int special = 0; special < frame.elements * idframe + 1; special++)
                                {
                                    string timeline_special = timeline + "_" + special.ToString();

                                    if (timelines.ContainsKey(timeline_special) && timelines[timeline_special] >= idframe)
                                    {
                                        continue;
                                    }
                                    else if (timelines.ContainsKey(timeline_special))
                                    {
                                        timeline = timeline_special;
                                        timelines[timeline]++;

                                        element.repeat = special + 1;
                                        var ELEMENT = AnimData.animList[idanim].framesList[idframe].elementsList[idelement];
                                        ELEMENT.repeat = special + 1;
                                        AnimData.animList[idanim].framesList[idframe].elementsList[idelement] = ELEMENT;

                                        break;
                                    }
                                    else
                                    {
                                        timeline = timeline_special;
                                        timelines.Add(timeline, 0);

                                        element.repeat = special + 1;
                                        var ELEMENT = AnimData.animList[idanim].framesList[idframe].elementsList[idelement];
                                        ELEMENT.repeat = special + 1;
                                        AnimData.animList[idanim].framesList[idframe].elementsList[idelement] = ELEMENT;

                                        break;
                                    }
                                }

                            }
                            else
                            {
                                timelines[timeline]++;

                            }
                        }

                        List<string> timelinesKeys = new List<string>();
                        foreach (var item in timelines.Keys) { timelinesKeys.Add(item); }//获取时间线的ID表
                        for (int lineid = 0; lineid < timelinesKeys.Count; lineid++)
                        {
                            if (timelinesKeys[lineid] == timeline)
                            {
                                timelineid = lineid;
                                break;
                            }
                        }

                        animTable.Rows.Add(
                           anim.name, AnimHash[anim.hash], anim.rate,
                           frame.num3, frame.num4, frame.num5, frame.num6,
                           AnimHash[element.image], AnimHash[element.layer],
                           element.image, element.index, element.layer, element.flags,
                           idanim, idframe, idelement,
                           timelineid, timelines[timelinesKeys[timelineid]],
                           element.m1, element.m2, element.m3, element.m4, element.m5, element.m6, element.order, element.repeat
                           );
                    }
                    idframe++;
                }

                if (true)
                {
                    //Math.Max()
                    //timelines.Count
                }
                idanim++;
            }
        }
    }
    #endregion
}

