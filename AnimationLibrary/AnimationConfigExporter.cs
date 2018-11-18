using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace AnimationLibrary
{
    public class AnimationConfigExporter
    {
        XmlDocument scml = null;
        DataSet dataSet = null;
        DataTable bildTable = null;
        DataTable animTable = null;
        AnimationBundleReader bundleData = null;
        Dictionary<string, string> fileNameIndex = null;

        public string FilePath { get; private set; }
        public string FileName { get; private set; }

        public void InitData(DataSet data, AnimationBundleReader bundle, string path, string name)
        {
            dataSet = data;
            bildTable = data.Tables["bildTable"];
            animTable = data.Tables["animTable"];

            bundleData = bundle;
            FilePath = path;
            FileName = name;

            InitFile();
            InitFolderInfo();
            InitEntityInfo();
            InitAnimationInfo();
        }

        public void InitFile()
        {
            scml = new XmlDocument();

            XmlDeclaration xmldecl;
            xmldecl = scml.CreateXmlDeclaration("1.0", "UTF-8", null);
            scml.AppendChild(xmldecl);

            XmlElement spriter_data = scml.CreateElement("spriter_data");
            scml.AppendChild(spriter_data);

            spriter_data.SetAttribute("scml_version", "1.0");
            spriter_data.SetAttribute("generator", "BrashMonkey Spriter");
            spriter_data.SetAttribute("generator_version", "r11");
        }

        public void InitFolderInfo()
        {
            XmlNode root = scml.SelectSingleNode("spriter_data");

            int floders = 1;
            for (int i = 0; i < floders; i++)
            {
                XmlElement folder = scml.CreateElement("folder");
                folder.SetAttribute("id", i.ToString());
                //folder.SetAttribute("name", "testfolder");
                root.AppendChild(folder);

                fileNameIndex = new Dictionary<string, string>();
                for (int fileIndex = 0; fileIndex < bundleData.BildData.frames; fileIndex++)
                {
                    fileNameIndex.Add(bildTable.Rows[fileIndex]["name"] + "_" + bildTable.Rows[fileIndex]["index"], fileIndex.ToString());

                    float x = (float)bildTable.Rows[fileIndex]["num6"] - (float)bildTable.Rows[fileIndex]["num8"] / 2;
                    float y = (float)bildTable.Rows[fileIndex]["num7"] - (float)bildTable.Rows[fileIndex]["num9"] / 2;
                    float pivot_x = 0 - x / (float)bildTable.Rows[fileIndex]["num8"];
                    float pivot_y = 1 + y / (float)bildTable.Rows[fileIndex]["num9"];

                    XmlElement file = scml.CreateElement("file");
                    file.SetAttribute("id", fileIndex.ToString());
                    file.SetAttribute("name", bildTable.Rows[fileIndex]["name"] + "_" + bildTable.Rows[fileIndex]["index"]);
                    file.SetAttribute("width", bildTable.Rows[fileIndex]["w"].ToString());
                    file.SetAttribute("height", bildTable.Rows[fileIndex]["h"].ToString());
                    file.SetAttribute("pivot_x", pivot_x.ToString());
                    file.SetAttribute("pivot_y", pivot_y.ToString());

                    folder.AppendChild(file);
                }
            }
        }

        public void InitEntityInfo()
        {
            XmlNode root = scml.SelectSingleNode("spriter_data");

            XmlElement entity = scml.CreateElement("entity");
            entity.SetAttribute("id", "0");
            entity.SetAttribute("name", ((Bild)bildTable.Rows[0]["bild"]).name);
            root.AppendChild(entity);
        }

        public void InitAnimationInfo()
        {
            XmlNode root = scml.SelectSingleNode("spriter_data/entity");

            for (int bank = 0; bank < bundleData.AnimData.symbols; bank++)
            {
                XmlElement animation = scml.CreateElement("animation");
                animation.SetAttribute("id", bank.ToString());
                animation.SetAttribute("name", bundleData.AnimData.symbolsList[bank].name);
                animation.SetAttribute("length", bundleData.AnimData.symbolsList[bank].frames.ToString());
                root.AppendChild(animation);

                InitMainlineInfo(animation, bank);
                InitTimelineInfo(animation, bank);
            }

            scml.Save(FilePath + "\\_" + FileName + ".scml");
        }

        public void InitMainlineInfo(XmlNode parent, int symbolIndex)
        {
            XmlElement mainline = scml.CreateElement("mainline");
            parent.AppendChild(mainline);

            for (int frame = 0; frame < bundleData.AnimData.symbolsList[symbolIndex].frames; frame++)
            {
                XmlElement key = scml.CreateElement("key");
                key.SetAttribute("id", frame.ToString());
                key.SetAttribute("time", frame.ToString());
                for (int element = 0; element < bundleData.AnimData.symbolsList[symbolIndex].framesList[frame].elements; element++)
                {
                    XmlElement object_ref = scml.CreateElement("object_ref");
                    object_ref.SetAttribute("id", element.ToString());
                    object_ref.SetAttribute("timeline", element.ToString());
                    object_ref.SetAttribute("key", "0");
                    object_ref.SetAttribute("z_index", (bundleData.AnimData.symbolsList[symbolIndex].framesList[frame].elements - element).ToString());

                    key.AppendChild(object_ref);
                }

                mainline.AppendChild(key);
            }
        }

        public void InitTimelineInfo(XmlNode parent, int symbolIndex)
        {
            var frameTemp = bundleData.AnimData.symbolsList[symbolIndex].framesList[0];
            for (int element = 0; element < frameTemp.elements; element++)
            {
                string file = bundleData.AnimHash[frameTemp.elementsList[element].symbol] + "_" + frameTemp.elementsList[element].frame;

                XmlElement timeline = scml.CreateElement("timeline");
                timeline.SetAttribute("id", element.ToString());
                timeline.SetAttribute("name", element + "_" + file);

                bool isFirst = true;
                double lastangle = 0;

                int current = 0;
                for (int frame = 0; frame < bundleData.AnimData.symbolsList[symbolIndex].frames; frame++)
                {
                    if (!fileNameIndex.ContainsKey(file)) { continue; }
                    if (bundleData.AnimData.symbolsList[symbolIndex].framesList[frame].elements == 0) { continue; }

                    //查找每一帧对比一下 是当前的物体则导出数据
                    var obj = bundleData.AnimData.symbolsList[symbolIndex].framesList[frame].elementsList[element];

                    double scale_x = Math.Sqrt(obj.m1 * obj.m1 + obj.m2 * obj.m2);
                    double scale_y = Math.Sqrt(obj.m3 * obj.m3 + obj.m4 * obj.m4);

                    double det = obj.m1 * obj.m4 - obj.m3 * obj.m2;
                    if (det < 0)
                    {
                        if (isFirst)
                        {
                            scale_x = -scale_x;
                            isFirst = false;
                        }
                        else
                        {
                            scale_y = -scale_y;
                        }
                    }

                    double sin_approx = 0.5 * (obj.m3 / scale_y - obj.m2 / scale_x);
                    double cos_approx = 0.5 * (obj.m1 / scale_x + obj.m4 / scale_y);
                    double angle = Math.Atan2(sin_approx, cos_approx);

                    double spin = (angle - lastangle) <= Math.PI ? 1 : -1;
                    if (angle < lastangle) { spin = -spin; }

                    if (angle < 0) { angle += 2 * Math.PI; }
                    angle *= 180 / Math.PI;
                    lastangle = angle;

                    XmlElement key = scml.CreateElement("key");
                    key.SetAttribute("id", frame.ToString());
                    key.SetAttribute("time", frame.ToString());
                    key.SetAttribute("spin", spin.ToString());

                    XmlElement object_def = scml.CreateElement("object");
                    object_def.SetAttribute("folder", "0");
                    object_def.SetAttribute("file", fileNameIndex[file]);
                    object_def.SetAttribute("x", (+obj.m5 * 0.5).ToString());
                    object_def.SetAttribute("y", (-obj.m6 * 0.5).ToString());
                    object_def.SetAttribute("angle", angle.ToString());
                    object_def.SetAttribute("scale_x", scale_x.ToString());
                    object_def.SetAttribute("scale_y", scale_y.ToString());

                    key.AppendChild(object_def);

                    timeline.AppendChild(key);

                    current++;
                }

                parent.AppendChild(timeline);
            }
        }
    }
}
