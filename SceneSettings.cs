//	Copyright (c) 2015, Warren Marshall <warren@warrenmarshall.biz>
//	Copyright (c) 2015, Aleksander Marhall
//	
//	Permission to use, copy, modify, and/or distribute this software for any
//	purpose with or without fee is hereby granted, provided that the above
//	copyright notice and this permission notice appear in all copies.
//
//	THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
//	WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
//	MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
//	ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
//	WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
//	ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
//	OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace OBJ2MAP
{
    class SceneSettings
    {
        MainForm progForm;

        public SceneSettings(MainForm form)
        {
            progForm = form;
        }
        public bool SettingsSave(
            string output,
            MainForm.EConvOption econvOption,
            bool checked3,
            double _Scalar,
            float num2,
            int num1,
            string text3,
            string str1,
            string str2,
            string text1,
            bool axis)
        {
            XElement settings = new XElement("OBJ2MAPSettings",
                new XElement("MAPOutput", @output),
                new XElement("BrushMethod", @econvOption.ToString()),
                new XElement("CopyToClipboard", @checked3.ToString()),
                new XElement("Depth", @_Scalar),
                new XElement("Scale", @num2),
                new XElement("DecimalPlaces", @num1),
                new XElement("Class", @text3),
                new XElement("VisibleTexture", @str1),
                new XElement("HiddenTexture", @str2),
                new XElement("AxisAligned", @axis.ToString())
            );

            settings.Save(Path.Combine(Path.GetDirectoryName(text1), Path.GetFileNameWithoutExtension(text1) + ".xml"));

            string test = text3.ToString();
            return true;
        }

        private XElement loadFile;

        public void SetPathForLoading(string text1)
        {
            loadFile = XElement.Load(Path.Combine(Path.GetDirectoryName(text1), Path.GetFileNameWithoutExtension(text1) + ".xml"));
        }

        public string SettingsLoad(int mode)
        {
            if (loadFile != null)
            {
                switch (mode)
                {
                    case 0:
                        return loadFile.Element("MAPOutput").Value;
                    case 1:
                        return loadFile.Element("BrushMethod").Value;
                    case 2:
                        return loadFile.Element("CopyToClipboard").Value;
                    case 3:
                        return loadFile.Element("Depth").Value;
                    case 4:
                        return loadFile.Element("Scale").Value;
                    case 5:
                        return loadFile.Element("DecimalPlaces").Value;
                    case 6:
                        return loadFile.Element("Class").Value;
                    case 7:
                        return loadFile.Element("VisibleTexture").Value;
                    case 8:
                        return loadFile.Element("HiddenTexture").Value;
                    case 9:
                        return loadFile.Element("AxisAligned").Value;
                    default:
                        return "";
                }
            }
            else
                return "";
        }
    }
}
