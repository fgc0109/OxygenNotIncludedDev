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
        private static XmlDocument scml = null;
        DataSet dataSet = null;
        private static DataTable bildTable = null;
        private static DataTable animTable = null;
        private static AnimationBundleReader bundleData = null;
        private static Dictionary<string, string> fileNameIndex = null;

        public static string FilePath { get; private set; }
        public static string FileName { get; private set; }

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

        private static void InitEntityInfo()
        {
            XmlNode root = scml.SelectSingleNode("spriter_data");

            XmlElement entity = scml.CreateElement("entity");
            entity.SetAttribute("id", "0");
            entity.SetAttribute("name", ((Bild)bildTable.Rows[0]["bild"]).name);
            root.AppendChild(entity);
        }

        private static void InitAnimationInfo()
        {
            XmlNode root = scml.SelectSingleNode("spriter_data/entity");

            for (int animIndex = 0; animIndex < bundleData.AnimData.anims; animIndex++)
            {
                var anim = bundleData.AnimData.animList[animIndex];

                XmlElement animation = scml.CreateElement("animation");
                animation.SetAttribute("id", animIndex.ToString());
                animation.SetAttribute("name", anim.name);
                animation.SetAttribute("length", (anim.rate * anim.frames).ToString());
                root.AppendChild(animation);

                InitMainlineInfo(animation, animIndex);
                InitTimelineInfo(animation, animIndex);
            }

            scml.Save(FilePath + "\\_" + FileName + ".scml");
        }

        private static void InitMainlineInfo(XmlNode parent, int animIndex)
        {
            XmlElement mainline = scml.CreateElement("mainline");
            parent.AppendChild(mainline);

            var anim = bundleData.AnimData.animList[animIndex];
            float rate = anim.rate;
            for (int frame = 0; frame < anim.frames; frame++)
            {
                XmlElement key = scml.CreateElement("key");
                key.SetAttribute("id", frame.ToString());
                key.SetAttribute("time", (frame * rate).ToString());
                for (int element = 0; element < anim.framesList[frame].elements; element++)
                {
                    var dataline = animTable.Select(
                        "idanim = '" + animIndex + "' and " +
                        "idframe = '" + frame + "' and " +
                        "idelement = '" + element + "'")[0];
                    int timeline = (int)dataline["timeline"];
                    int line_key = (int)dataline["line_key"];


                    XmlElement object_ref = scml.CreateElement("object_ref");
                    object_ref.SetAttribute("id", element.ToString());
                    object_ref.SetAttribute("timeline", timeline.ToString());
                    object_ref.SetAttribute("key", line_key.ToString());
                    object_ref.SetAttribute("z_index", (anim.framesList[frame].elements - element).ToString());

                    key.AppendChild(object_ref);
                }

                mainline.AppendChild(key);
            }
        }

        private static void InitTimelineInfo(XmlNode parent, int animIndex)
        {
            var anim = bundleData.AnimData.animList[animIndex];
            var frameTemp = anim.framesList[0];

            Dictionary<string, AnimElement> timelines = new Dictionary<string, AnimElement>();
            foreach (var frame in anim.framesList)
            {
                foreach (var element in frame.elementsList)
                {
                    string temp = element.image + "_" + element.index + "_" + element.layer + "_" + element.repeat;
                    if (!timelines.ContainsKey(temp))
                    {
                        timelines.Add(temp, element);
                    }
                }
            }

            List<string> timelinesKeys = new List<string>();
            foreach (var item in timelines.Keys)
            {
                timelinesKeys.Add(item);
            }

            //查找动画每个时间线的数据(一个物体一条时间线)
            for (int line = 0; line < timelines.Count; line++)
            {

                string keyd = timelinesKeys[line];
                string file = bundleData.AnimHash[timelines[keyd].image] + "_" + timelines[keyd].index;

                XmlElement timeline = scml.CreateElement("timeline");
                timeline.SetAttribute("id", line.ToString());
                timeline.SetAttribute("name", line + "_" + file);

                bool isFirst = false;
                double lastangle = 0;

                float rate = anim.rate;
                int current = 0;
                bool insertline = false;

                //从每一帧查找看有没有这个物体,有的话在时间线里面加入这个物体的数据
                for (int frame = 0; frame < anim.frames; frame++)
                {

                    if (animIndex == 4 && line == 9)
                    {
                        int aaaa = 0;
                    }

                    if (!fileNameIndex.ContainsKey(file)) { continue; }
                    if (anim.framesList[frame].elements == 0) { continue; }

                    var obj = anim.framesList[frame].elementsList.FirstOrDefault(ele =>
                        ele.layer == timelines[timelinesKeys[line]].layer &&
                        ele.index == timelines[timelinesKeys[line]].index &&
                        ele.image == timelines[timelinesKeys[line]].image &&
                        ele.repeat == timelines[timelinesKeys[line]].repeat
                        );
                    if (obj.image == 0) { continue; }

                    insertline = true;

                    var dataline = animTable.Select(
                       "idanim = '" + animIndex + "' and " +
                       "idframe = '" + frame + "' and " +
                       "image = '" + obj.image + "' and " +
                       "index = '" + obj.index + "' and " +
                       "layer = '" + obj.layer + "' and " +
                       "repeat = '" + obj.repeat + "'")[0];
                    int line_key = (int)dataline["line_key"];
                    int time = (int)dataline["idframe"] * (int)rate;

                    if (line_key == 0)
                    {
                        isFirst = true;
                    }

                    double scale_x = Math.Sqrt(obj.m1 * obj.m1 + obj.m2 * obj.m2);
                    double scale_y = Math.Sqrt(obj.m3 * obj.m3 + obj.m4 * obj.m4);

                    double det = obj.m1 * obj.m4 - obj.m3 * obj.m2;
                    if (det < 0)
                    {
                        if (isFirst) { scale_x = -scale_x; isFirst = false; }
                        else { scale_y = -scale_y; }
                    }

                    double sin_approx = 0.5 * (obj.m3 / scale_y - obj.m2 / scale_x);
                    double cos_approx = 0.5 * (obj.m1 / scale_x + obj.m4 / scale_y);
                    double angle = Math.Atan2(sin_approx, cos_approx);

                    double spin = Math.Abs(angle - lastangle) <= Math.PI ? 1 : -1;
                    if (angle < lastangle) { spin = -spin; }

                    if (angle < 0) { angle += 2 * Math.PI; }
                    angle *= 180 / Math.PI;
                    lastangle = angle;

                    XmlElement key = scml.CreateElement("key");
                    key.SetAttribute("id", line_key.ToString());
                    key.SetAttribute("time", time.ToString());
                    key.SetAttribute("spin", spin.ToString());

                    XmlElement object_def = scml.CreateElement("object");
                    object_def.SetAttribute("folder", "0");
                    //object_def.SetAttribute("file", file);
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

                if (insertline) { parent.AppendChild(timeline); }
            }
        }
    }
}
