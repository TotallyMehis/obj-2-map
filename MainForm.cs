//	Copyright (c) 2015, Warren Marshall <warren@warrenmarshall.biz>
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

// Decompiled source. Needs refactoring

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace OBJ2MAP
{
  public class MainForm : Form
  {
    private IContainer components = (IContainer) null;
    private GroupBox groupBox1;
    private Button button1;
    private TextBox OBJFilename;
    private Label label1;
    private GroupBox groupBox2;
    private Button button2;
    private TextBox MAPFilename;
    private Label label2;
    private TextBox DepthTextBox;
    private GroupBox groupBox3;
    private RadioButton RB_Spikes;
    private RadioButton RB_Extrusion;
    private RadioButton RB_Standard;
    private Label DepthLabel;
    private Button GOButton;
    private CheckBox CopyToClipboardCheck;
    private Label label4;
    private TextBox ScaleTextBox;
    private Label label3;
    private Label label5;
    private Label label7;
    private TextBox ClassTextBox;
    private Label label6;
    private TextBox DecimalsTextBox;
    private Label label8;
    private TextBox VisibleTextureTextBox;
    private Label label9;
    private TextBox HiddenTextureTextBox;
    private Label label10;
    private CheckBox loadSettingFileCheckBox;
    private CheckBox AxisAlignedCheckBox;

    public MainForm()
    {
      this.InitializeComponent();
    }

    private void MenuFileOpen_Click(object sender, EventArgs e)
    {
    }

    static OpenFileDialog openFileDialog;

    private void OBJBrowse_Click(object sender, EventArgs e)
    {
      //OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Select OBJ File To Convert";
      openFileDialog.Filter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;
      openFileDialog.FileOk += openFileDialog_FileOk;
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      this.OBJFilename.Text = openFileDialog.FileName;
    }

    private void openFileDialog_FileOk(Object sender, CancelEventArgs e)
    {
        if(loadSettingFileCheckBox.Checked){
            Console.Write("\n======> Path: " + openFileDialog.FileName);
            if (File.Exists(Path.Combine(Path.GetDirectoryName(openFileDialog.FileName), Path.GetFileNameWithoutExtension(openFileDialog.FileName) + ".xml")))
            {
                SceneSettings sS = new SceneSettings(this);
                sS.SetPathForLoading(openFileDialog.FileName);

                this.MAPFilename.Text = sS.SettingsLoad(0);
                Console.Write("\n======> MAPPath: " + sS.SettingsLoad(0));
                Console.Write("\n======> Checkbox: " + sS.SettingsLoad(1));

                switch (sS.SettingsLoad(1))
                {
                    case "Spikes":
                        this.RB_Spikes.Checked = true;
                        break;
                    case "Standard":
                        this.RB_Standard.Checked = true;
                        break;
                    case "Extrusion":
                        this.RB_Extrusion.Checked = true;
                        break;
                }

                

                if (sS.SettingsLoad(2) == "True")
                    this.CopyToClipboardCheck.Checked = true;
                else
                    this.CopyToClipboardCheck.Checked = false;

                this.DepthTextBox.Text = sS.SettingsLoad(3);
                this.ScaleTextBox.Text = sS.SettingsLoad(4);
                this.DecimalsTextBox.Text = sS.SettingsLoad(5);
                this.ClassTextBox.Text = sS.SettingsLoad(6);
                this.VisibleTextureTextBox.Text = sS.SettingsLoad(7);
                this.HiddenTextureTextBox.Text = sS.SettingsLoad(8);

                if (sS.SettingsLoad(9) == "True")
                    this.AxisAlignedCheckBox.Checked = true;
                else
                    this.AxisAlignedCheckBox.Checked = false;
            }
        }
    }

    private void MAPBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Select MAP File To Write Into";
      openFileDialog.Filter = "MAP files (*.map)|*.map|All files (*.*)|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;
      openFileDialog.CheckFileExists = false;
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      this.MAPFilename.Text = openFileDialog.FileName;
    }

    private void RB_Standard_CheckedChanged(object sender, EventArgs e)
    {
      this.DepthLabel.Enabled = false;
      this.DepthTextBox.Enabled = false;
      this.AxisAlignedCheckBox.Enabled = false;
    }

    private void RB_Extrusion_CheckedChanged(object sender, EventArgs e)
    {
      this.DepthLabel.Enabled = true;
      this.DepthTextBox.Enabled = true;
      this.AxisAlignedCheckBox.Enabled = true;
    }

    private void RB_Spikes_CheckedChanged(object sender, EventArgs e)
    {
      this.DepthLabel.Enabled = true;
      this.DepthTextBox.Enabled = true;
      this.AxisAlignedCheckBox.Enabled = true;
    }

    private void GoButton_Click(object sender, EventArgs e)
    {
      string text1 = this.OBJFilename.Text;
      string text2 = this.MAPFilename.Text;
      bool checked1 = this.RB_Standard.Checked;
      MainForm.EConvOption econvOption = MainForm.EConvOption.Standard;
      if (this.RB_Extrusion.Checked)
        econvOption = MainForm.EConvOption.Extrusion;
      if (this.RB_Spikes.Checked)
        econvOption = MainForm.EConvOption.Spikes;
      double _Scalar = double.Parse(this.DepthTextBox.Text);
      bool checked2 = this.AxisAlignedCheckBox.Checked;
      int num1 = Math.Max(int.Parse(this.DecimalsTextBox.Text), 0);
      float num2 = float.Parse(this.ScaleTextBox.Text);
      string str1 = this.VisibleTextureTextBox.Text;
      if (str1.Length == 0)
        str1 = "DEFAULT";
      string str2 = this.HiddenTextureTextBox.Text;
      if (str2.Length == 0)
        str2 = "SKIP";
      bool checked3 = this.CopyToClipboardCheck.Checked;
      string text3 = this.ClassTextBox.Text;
      if (!File.Exists(text1))
      {
        int num3 = (int) MessageBox.Show("OBJ file doesn't exist.");
      }
      else if ((double) num2 <= 0.0)
      {
        int num4 = (int) MessageBox.Show("Scale needs to be above 0%.");
      }
      else if (econvOption != MainForm.EConvOption.Standard && _Scalar < 1.0)
      {
        int num5 = (int) MessageBox.Show("Depth must be greater than 0.");
      }
      else
      {
        StreamWriter streamWriter1 = new StreamWriter("OBJ2MAP.log");
        streamWriter1.AutoFlush = true;
        streamWriter1.WriteLine(">>> OBJ-2-MAP v1.1 starting up. <<<");
        streamWriter1.WriteLine(string.Format("{0}", (object) DateTime.Now));
        streamWriter1.WriteLine(string.Format("OBJ Filename : {0}", (object) text1));
        StreamWriter streamWriter2 = (StreamWriter) null;
        if (text2.Length > 0)
        {
          streamWriter2 = File.CreateText(text2);
          streamWriter1.WriteLine(string.Format("MAP Filename : {0}", (object) text2));
        }
        MainForm.EGRP egrp = MainForm.EGRP.Undefined;
        List<XVector> _Vertices = new List<XVector>();
        List<XFace> list1 = new List<XFace>();
        List<XBrush> list2 = new List<XBrush>();
        char[] separator1 = new char[1]
        {
          ' '
        };
        char[] separator2 = new char[1]
        {
          '/'
        };

        string format = string.Format("F{0}", (object) num1);
        streamWriter1.WriteLine("");
        streamWriter1.WriteLine(string.Format("Method : {0}", (object) econvOption.ToString()));
        streamWriter1.WriteLine(string.Format("Copy To Clipboard : {0}", (object) checked3.ToString()));
        streamWriter1.WriteLine(string.Format("Depth: {0}", (object) _Scalar));
        streamWriter1.WriteLine(string.Format("Scale: {0}", (object) num2));
        streamWriter1.WriteLine(string.Format("Decimal Places: {0}", (object) num1));
        streamWriter1.WriteLine(string.Format("Class: {0}", text3.Length > 0 ? (object) text3 : (object) "worldspawn"));
        streamWriter1.WriteLine(string.Format("Visible Texture: {0}", str1.Length > 0 ? (object) str1 : (object) "DEFAULT"));
        streamWriter1.WriteLine(string.Format("Hidden Texture: {0}", str2.Length > 0 ? (object) str2 : (object) "SKIP"));
        streamWriter1.WriteLine("");
        streamWriter1.WriteLine("! Reading OBJ file into memory");
        foreach (string str3 in File.ReadAllLines(text1))
        {
          string str4 = str3.Trim();
          streamWriter1.WriteLine(string.Format("# OBJ Line: {0}", (object) str4));
          if (!str4.StartsWith("# ") && str4.Length != 0)
          {
            if (egrp == MainForm.EGRP.Undefined && (str4.StartsWith("o ") || str4.StartsWith("g ")))
              egrp = !str4.StartsWith("g ") ? MainForm.EGRP.Ungrouped : MainForm.EGRP.Grouped;
            if (str4.StartsWith("g ") && egrp == MainForm.EGRP.Grouped || str4.StartsWith("o ") && egrp == MainForm.EGRP.Ungrouped)
            {
              if (list1.Count > 0)
                list2.Add(new XBrush()
                {
                  Faces = list1
                });
              list1 = new List<XFace>();
            }
            if (str4.StartsWith("v "))
            {
              string[] strArray = str4.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
              if (strArray.Length == 4)
              {
                XVector xvector = new XVector(double.Parse(strArray[1],new CultureInfo("en-US")), double.Parse(strArray[2],new CultureInfo("en-US")), double.Parse(strArray[3],new CultureInfo("en-US")));
                double num6 = xvector.y;
                xvector.y = -xvector.z;
                xvector.z = num6;
                xvector.x *= (double) num2 / 100.0;
                xvector.y *= (double) num2 / 100.0;
                xvector.z *= (double) num2 / 100.0;
                _Vertices.Add(xvector);
              }
            }
            if (str4.StartsWith("f "))
            {
              XFace xface = new XFace();
              string[] strArray1 = str4.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
              for (int index = 1; index < strArray1.Length; ++index)
              {
                string[] strArray2 = strArray1[index].Split(separator2, StringSplitOptions.RemoveEmptyEntries);
                xface.VertIdx.Add(int.Parse(strArray2[0]) - 1);
              }
              list1.Add(xface);
            }
          }
        }
        if (list1.Count > 0)
          list2.Add(new XBrush()
          {
            Faces = list1
          });
        streamWriter1.WriteLine("");
        streamWriter1.WriteLine("Summary:");
        streamWriter1.WriteLine(string.Format("Vertices: {0}", (object) _Vertices.Count));
        streamWriter1.WriteLine(string.Format("Faces: {0}", (object) list1.Count));
        streamWriter1.WriteLine(string.Format("Brushes: {0}", (object) list2.Count));
        streamWriter1.WriteLine("");
        streamWriter1.WriteLine("! Computing face normals.");
        foreach (XFace xface in list1)
        {
          xface.ComputeNormal(ref _Vertices);
          if (checked2)
          {
            XVector[] xvectorArray = new XVector[6]
            {
              new XVector(1.0, 0.0, 0.0),
              new XVector(-1.0, 0.0, 0.0),
              new XVector(0.0, 1.0, 0.0),
              new XVector(0.0, -1.0, 0.0),
              new XVector(0.0, 0.0, 1.0),
              new XVector(0.0, 0.0, -1.0)
            };
            int index1 = -1;
            double num6 = -999.0;
            for (int index2 = 0; index2 < 6; ++index2)
            {
              double num7 = XVector.Dot(xvectorArray[index2], xface.Normal);
              if (num7 > num6)
              {
                num6 = num7;
                index1 = index2;
              }
            }
            xface.Normal = xvectorArray[index1];
          }
        }
        streamWriter1.WriteLine("! Beginning MAP construction.");
        string str5 = "" + "{\n" + string.Format("\"classname\" \"{0}\"\n", text3.Length > 0 ? (object) text3 : (object) "worldspawn");
        switch (econvOption)
        {
          case MainForm.EConvOption.Extrusion:
            using (List<XBrush>.Enumerator enumerator = list2.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                foreach (XFace xface in enumerator.Current.Faces)
                {
                  XVector _B = XVector.Multiply(xface.Normal, _Scalar);
                  List<XVector> list3 = new List<XVector>();
                  List<XVector> list4 = new List<XVector>();
                  foreach (int index in xface.VertIdx)
                  {
                    XVector _A = new XVector(_Vertices[index]);
                    list3.Add(_A);
                    list4.Add(XVector.Add(_A, _B));
                  }
                  str5 += "{\n";
                  XVector xvector1 = list3[0];
                  XVector xvector2 = list3[1];
                  XVector xvector3 = list3[2];
                  str5 += string.Format("\t( {0} {1} {2} ) ", (object)xvector1.x.ToString(format, new CultureInfo("en-US")), (object)xvector1.y.ToString(format, new CultureInfo("en-US")), (object)xvector1.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) ", (object)xvector3.x.ToString(format, new CultureInfo("en-US")), (object)xvector3.y.ToString(format, new CultureInfo("en-US")), (object)xvector3.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector2.x.ToString(format, new CultureInfo("en-US")), (object)xvector2.y.ToString(format, new CultureInfo("en-US")), (object)xvector2.z.ToString(format, new CultureInfo("en-US")), (object)str1);
                  XVector xvector4 = list4[2];
                  XVector xvector5 = list4[1];
                  XVector xvector6 = list4[0];
                  str5 += string.Format("\t( {0} {1} {2} ) ", (object)xvector4.x.ToString(format, new CultureInfo("en-US")), (object)xvector4.y.ToString(format, new CultureInfo("en-US")), (object)xvector4.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) ", (object)xvector6.x.ToString(format, new CultureInfo("en-US")), (object)xvector6.y.ToString(format, new CultureInfo("en-US")), (object)xvector6.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector5.x.ToString(format, new CultureInfo("en-US")), (object)xvector5.y.ToString(format, new CultureInfo("en-US")), (object)xvector5.z.ToString(format, new CultureInfo("en-US")), (object)str2);
                  for (int index = 0; index < list3.Count; ++index)
                  {
                    XVector xvector7 = list3[index];
                    XVector xvector8 = list3[(index + 1) % list3.Count];
                    XVector xvector9 = list4[index];
                    str5 += string.Format("\t( {0} {1} {2} ) ", (object)xvector7.x.ToString(format, new CultureInfo("en-US")), (object)xvector7.y.ToString(format, new CultureInfo("en-US")), (object)xvector7.z.ToString(format, new CultureInfo("en-US")));
                    str5 += string.Format("( {0} {1} {2} ) ", (object)xvector8.x.ToString(format, new CultureInfo("en-US")), (object)xvector8.y.ToString(format, new CultureInfo("en-US")), (object)xvector8.z.ToString(format, new CultureInfo("en-US")));
                    str5 += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector9.x.ToString(format, new CultureInfo("en-US")), (object)xvector9.y.ToString(format, new CultureInfo("en-US")), (object)xvector9.z.ToString(format, new CultureInfo("en-US")), (object)str2);
                  }
                  str5 += "}\n";
                }
              }
              break;
            }
          case MainForm.EConvOption.Spikes:
            using (List<XBrush>.Enumerator enumerator = list2.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                foreach (XFace xface in enumerator.Current.Faces)
                {
                  XVector _B1 = XVector.Multiply(xface.Normal, _Scalar);
                  List<XVector> list3 = new List<XVector>();
                  foreach (int index in xface.VertIdx)
                  {
                    XVector xvector = new XVector(_Vertices[index]);
                    list3.Add(xvector);
                  }
                  str5 += "{\n";
                  XVector xvector1 = list3[0];
                  XVector xvector2 = list3[1];
                  XVector xvector3 = list3[2];
                  str5 += string.Format("\t( {0} {1} {2} ) ", (object)xvector1.x.ToString(format, new CultureInfo("en-US")), (object)xvector1.y.ToString(format, new CultureInfo("en-US")), (object)xvector1.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) ", (object)xvector3.x.ToString(format, new CultureInfo("en-US")), (object)xvector3.y.ToString(format, new CultureInfo("en-US")), (object)xvector3.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector2.x.ToString(format, new CultureInfo("en-US")), (object)xvector2.y.ToString(format, new CultureInfo("en-US")), (object)xvector2.z.ToString(format, new CultureInfo("en-US")), (object)str1);
                  XVector xvector4 = new XVector();
                  foreach (int index in xface.VertIdx)
                  {
                    XVector _B2 = new XVector(_Vertices[index]);
                    xvector4 = XVector.Add(xvector4, _B2);
                  }
                  XVector xvector5 = XVector.Add(XVector.Divide(xvector4, (double) xface.VertIdx.Count), _B1);
                  for (int index = 0; index < list3.Count; ++index)
                  {
                    XVector xvector6 = list3[index];
                    XVector xvector7 = list3[(index + 1) % list3.Count];
                    str5 += string.Format("\t( {0} {1} {2} ) ", (object)xvector5.x.ToString(format, new CultureInfo("en-US")), (object)xvector5.y.ToString(format, new CultureInfo("en-US")), (object)xvector5.z.ToString(format, new CultureInfo("en-US")));
                    str5 += string.Format("( {0} {1} {2} ) ", (object)xvector6.x.ToString(format, new CultureInfo("en-US")), (object)xvector6.y.ToString(format, new CultureInfo("en-US")), (object)xvector6.z.ToString(format, new CultureInfo("en-US")));
                    str5 += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector7.x.ToString(format, new CultureInfo("en-US")), (object)xvector7.y.ToString(format, new CultureInfo("en-US")), (object)xvector7.z.ToString(format, new CultureInfo("en-US")), (object)str2);
                  }
                  str5 += "}\n";
                }
              }
              break;
            }
          default:
            using (List<XBrush>.Enumerator enumerator = list2.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                XBrush current = enumerator.Current;
                str5 += "{\n";
                foreach (XFace xface in current.Faces)
                {
                  XVector xvector1 = _Vertices[xface.VertIdx[0]];
                  XVector xvector2 = _Vertices[xface.VertIdx[2]];
                  XVector xvector3 = _Vertices[xface.VertIdx[1]];
                  str5 += string.Format("\t( {0} {1} {2} ) ", (object)xvector1.x.ToString(format, new CultureInfo("en-US")), (object)xvector1.y.ToString(format, new CultureInfo("en-US")), (object)xvector1.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) ", (object)xvector2.x.ToString(format, new CultureInfo("en-US")), (object)xvector2.y.ToString(format, new CultureInfo("en-US")), (object)xvector2.z.ToString(format, new CultureInfo("en-US")));
                  str5 += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector3.x.ToString(format, new CultureInfo("en-US")), (object)xvector3.y.ToString(format, new CultureInfo("en-US")), (object)xvector3.z.ToString(format, new CultureInfo("en-US")), (object)str1);
                }
                str5 += "}\n";
              }
              break;
            }
        }
        string text4 = str5 + "}\n";
        if (streamWriter2 != null)
        {
          streamWriter2.Write(text4);
          streamWriter2.Close();
        }
        if (checked3)
        {
          Clipboard.Clear();
          Clipboard.SetText(text4);
        }

        SceneSettings sS = new SceneSettings(this);

        bool saveDone = sS.SettingsSave(this.MAPFilename.Text,econvOption,checked3,_Scalar,num2,num1,text3,str1,str2,text1,this.AxisAlignedCheckBox.Checked);

        string str6 = "Done!" + "\n\n" + string.Format("\"{0}\" converted successfully.\n", (object)text1) + "\n";
        if (text2.Length > 0)
          str6 += string.Format("Written to \"{0}\"", (object) text2);
        string text5 = !checked3 ? str6 + "." : (text2.Length <= 0 ? str6 + "MAP text copied to the clipboard." : str6 + "and MAP text copied to the clipboard.");
        streamWriter1.WriteLine(text5);
        int num8 = (int) MessageBox.Show(text5);
        streamWriter1.Close();
      }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.OBJFilename.Focus();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.loadSettingFileCheckBox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.OBJFilename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.HiddenTextureTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.VisibleTextureTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DecimalsTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ClassTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ScaleTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CopyToClipboardCheck = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.AxisAlignedCheckBox = new System.Windows.Forms.CheckBox();
            this.RB_Spikes = new System.Windows.Forms.RadioButton();
            this.RB_Extrusion = new System.Windows.Forms.RadioButton();
            this.RB_Standard = new System.Windows.Forms.RadioButton();
            this.DepthLabel = new System.Windows.Forms.Label();
            this.DepthTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.MAPFilename = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.GOButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.loadSettingFileCheckBox);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.OBJFilename);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(436, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "INPUT";
            // 
            // loadSettingFileCheckBox
            // 
            this.loadSettingFileCheckBox.Checked = true;
            this.loadSettingFileCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loadSettingFileCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadSettingFileCheckBox.Location = new System.Drawing.Point(9, 48);
            this.loadSettingFileCheckBox.Name = "loadSettingFileCheckBox";
            this.loadSettingFileCheckBox.Size = new System.Drawing.Size(181, 24);
            this.loadSettingFileCheckBox.TabIndex = 6;
            this.loadSettingFileCheckBox.Text = "Load setting file";
            this.loadSettingFileCheckBox.UseCompatibleTextRendering = true;
            this.loadSettingFileCheckBox.CheckedChanged += new System.EventHandler(this.loadSettingFileCheckBox_CheckedChanged);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(399, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OBJBrowse_Click);
            // 
            // OBJFilename
            // 
            this.OBJFilename.Location = new System.Drawing.Point(59, 22);
            this.OBJFilename.Name = "OBJFilename";
            this.OBJFilename.Size = new System.Drawing.Size(334, 20);
            this.OBJFilename.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "OBJ File:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.HiddenTextureTextBox);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.VisibleTextureTextBox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.DecimalsTextBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.ClassTextBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.ScaleTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.CopyToClipboardCheck);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.MAPFilename);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.Location = new System.Drawing.Point(12, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(436, 248);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OUTPUT";
            // 
            // HiddenTextureTextBox
            // 
            this.HiddenTextureTextBox.Location = new System.Drawing.Point(312, 215);
            this.HiddenTextureTextBox.Name = "HiddenTextureTextBox";
            this.HiddenTextureTextBox.Size = new System.Drawing.Size(104, 20);
            this.HiddenTextureTextBox.TabIndex = 22;
            this.HiddenTextureTextBox.Text = "SKIP";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(196, 218);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(110, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Hidden Face Texture:";
            // 
            // VisibleTextureTextBox
            // 
            this.VisibleTextureTextBox.Location = new System.Drawing.Point(312, 189);
            this.VisibleTextureTextBox.Name = "VisibleTextureTextBox";
            this.VisibleTextureTextBox.Size = new System.Drawing.Size(104, 20);
            this.VisibleTextureTextBox.TabIndex = 20;
            this.VisibleTextureTextBox.Text = "DEFAULT";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(196, 192);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Visible Face Texture:";
            // 
            // DecimalsTextBox
            // 
            this.DecimalsTextBox.Location = new System.Drawing.Point(282, 163);
            this.DecimalsTextBox.Name = "DecimalsTextBox";
            this.DecimalsTextBox.Size = new System.Drawing.Size(40, 20);
            this.DecimalsTextBox.TabIndex = 17;
            this.DecimalsTextBox.Text = "3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(196, 166);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Decimal Places:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(236, 147);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(191, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "(optional  - leave blank for worldspawn)";
            // 
            // ClassTextBox
            // 
            this.ClassTextBox.Location = new System.Drawing.Point(237, 124);
            this.ClassTextBox.Name = "ClassTextBox";
            this.ClassTextBox.Size = new System.Drawing.Size(179, 20);
            this.ClassTextBox.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(196, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Class:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(56, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(200, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "(optional  - leave blank for clipboard only)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(285, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "%";
            // 
            // ScaleTextBox
            // 
            this.ScaleTextBox.Location = new System.Drawing.Point(239, 98);
            this.ScaleTextBox.Name = "ScaleTextBox";
            this.ScaleTextBox.Size = new System.Drawing.Size(40, 20);
            this.ScaleTextBox.TabIndex = 6;
            this.ScaleTextBox.Text = "100";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(196, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Scale:";
            // 
            // CopyToClipboardCheck
            // 
            this.CopyToClipboardCheck.Checked = true;
            this.CopyToClipboardCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CopyToClipboardCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CopyToClipboardCheck.Location = new System.Drawing.Point(199, 74);
            this.CopyToClipboardCheck.Name = "CopyToClipboardCheck";
            this.CopyToClipboardCheck.Size = new System.Drawing.Size(126, 24);
            this.CopyToClipboardCheck.TabIndex = 5;
            this.CopyToClipboardCheck.Text = "Copy To Clipboard?";
            this.CopyToClipboardCheck.CheckedChanged += new System.EventHandler(this.CopyToClipboardCheck_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.AxisAlignedCheckBox);
            this.groupBox3.Controls.Add(this.RB_Spikes);
            this.groupBox3.Controls.Add(this.RB_Extrusion);
            this.groupBox3.Controls.Add(this.RB_Standard);
            this.groupBox3.Controls.Add(this.DepthLabel);
            this.groupBox3.Controls.Add(this.DepthTextBox);
            this.groupBox3.Location = new System.Drawing.Point(9, 62);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(181, 157);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Method";
            // 
            // AxisAlignedCheckBox
            // 
            this.AxisAlignedCheckBox.Checked = true;
            this.AxisAlignedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AxisAlignedCheckBox.Enabled = false;
            this.AxisAlignedCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AxisAlignedCheckBox.Location = new System.Drawing.Point(6, 124);
            this.AxisAlignedCheckBox.Name = "AxisAlignedCheckBox";
            this.AxisAlignedCheckBox.Size = new System.Drawing.Size(96, 24);
            this.AxisAlignedCheckBox.TabIndex = 23;
            this.AxisAlignedCheckBox.Text = "Axis Aligned?";
            // 
            // RB_Spikes
            // 
            this.RB_Spikes.AutoSize = true;
            this.RB_Spikes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RB_Spikes.Location = new System.Drawing.Point(6, 65);
            this.RB_Spikes.Name = "RB_Spikes";
            this.RB_Spikes.Size = new System.Drawing.Size(56, 17);
            this.RB_Spikes.TabIndex = 2;
            this.RB_Spikes.TabStop = true;
            this.RB_Spikes.Text = "Spikes";
            this.RB_Spikes.UseVisualStyleBackColor = true;
            this.RB_Spikes.CheckedChanged += new System.EventHandler(this.RB_Spikes_CheckedChanged);
            // 
            // RB_Extrusion
            // 
            this.RB_Extrusion.AutoSize = true;
            this.RB_Extrusion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RB_Extrusion.Location = new System.Drawing.Point(6, 42);
            this.RB_Extrusion.Name = "RB_Extrusion";
            this.RB_Extrusion.Size = new System.Drawing.Size(67, 17);
            this.RB_Extrusion.TabIndex = 1;
            this.RB_Extrusion.TabStop = true;
            this.RB_Extrusion.Text = "Extrusion";
            this.RB_Extrusion.UseVisualStyleBackColor = true;
            this.RB_Extrusion.CheckedChanged += new System.EventHandler(this.RB_Extrusion_CheckedChanged);
            // 
            // RB_Standard
            // 
            this.RB_Standard.AutoSize = true;
            this.RB_Standard.Checked = true;
            this.RB_Standard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RB_Standard.Location = new System.Drawing.Point(6, 19);
            this.RB_Standard.Name = "RB_Standard";
            this.RB_Standard.Size = new System.Drawing.Size(67, 17);
            this.RB_Standard.TabIndex = 0;
            this.RB_Standard.TabStop = true;
            this.RB_Standard.Text = "Standard";
            this.RB_Standard.UseVisualStyleBackColor = true;
            this.RB_Standard.CheckedChanged += new System.EventHandler(this.RB_Standard_CheckedChanged);
            // 
            // DepthLabel
            // 
            this.DepthLabel.AutoSize = true;
            this.DepthLabel.Enabled = false;
            this.DepthLabel.Location = new System.Drawing.Point(3, 101);
            this.DepthLabel.Name = "DepthLabel";
            this.DepthLabel.Size = new System.Drawing.Size(39, 13);
            this.DepthLabel.TabIndex = 7;
            this.DepthLabel.Text = "Depth:";
            // 
            // DepthTextBox
            // 
            this.DepthTextBox.Enabled = false;
            this.DepthTextBox.Location = new System.Drawing.Point(48, 98);
            this.DepthTextBox.Name = "DepthTextBox";
            this.DepthTextBox.Size = new System.Drawing.Size(42, 20);
            this.DepthTextBox.TabIndex = 4;
            this.DepthTextBox.Text = "8";
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(399, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.MAPBrowse_Click);
            // 
            // MAPFilename
            // 
            this.MAPFilename.Location = new System.Drawing.Point(59, 19);
            this.MAPFilename.Name = "MAPFilename";
            this.MAPFilename.Size = new System.Drawing.Size(334, 20);
            this.MAPFilename.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "MAP File:";
            // 
            // GOButton
            // 
            this.GOButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.GOButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GOButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GOButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.GOButton.Location = new System.Drawing.Point(12, 350);
            this.GOButton.Name = "GOButton";
            this.GOButton.Size = new System.Drawing.Size(436, 42);
            this.GOButton.TabIndex = 2;
            this.GOButton.Text = "GO!";
            this.GOButton.UseVisualStyleBackColor = false;
            this.GOButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(460, 405);
            this.Controls.Add(this.GOButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OBJ-2-MAP v1.1.1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

    }

    public enum EGRP
    {
      Undefined,
      Grouped,
      Ungrouped,
    }

    public enum EConvOption
    {
      Standard,
      Extrusion,
      Spikes,
    }

    private void loadSettingFileCheckBox_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void CopyToClipboardCheck_CheckedChanged(object sender, EventArgs e)
    {

    }
  }
}