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
		private IContainer components = (IContainer)null;
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
		private Label ProgressLabel;
		private ProgressBar ProgressBar;
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
			openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Select OBJ File To Convert";
			openFileDialog.Filter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.FileOk += openFileDialog_FileOk;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.OBJFilename.Text = openFileDialog.FileName;
			}
		}

		public void UpdateProgress( string _ProgressText = "" )
		{
			// Only update the progress label text if there's something there.  [performance]
			if (_ProgressText.Length > 0)
			{
				ProgressLabel.Text = _ProgressText;
			}

			// Allow the app to process events so the controls will get updated.  Otherwise, they won't paint until everything is done.
			Application.DoEvents();
		}

		private void openFileDialog_FileOk(Object sender, CancelEventArgs e)
		{
			if (File.Exists(Path.Combine(Path.GetDirectoryName(openFileDialog.FileName), Path.GetFileNameWithoutExtension(openFileDialog.FileName) + ".xml")))
			{
				SceneSettings.SetPathForLoading(openFileDialog.FileName);
				this.MAPFilename.Text = SceneSettings.SettingsLoad(SceneSettings.EFieldNames.MapOutput);

				switch (SceneSettings.SettingsLoad(SceneSettings.EFieldNames.BrushMethod))
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

				if (SceneSettings.SettingsLoad(SceneSettings.EFieldNames.CopyToClipboard) == "True")
					this.CopyToClipboardCheck.Checked = true;
				else
					this.CopyToClipboardCheck.Checked = false;

				this.DepthTextBox.Text = SceneSettings.SettingsLoad(SceneSettings.EFieldNames.Depth);
				this.ScaleTextBox.Text = SceneSettings.SettingsLoad(SceneSettings.EFieldNames.Scale);
				this.DecimalsTextBox.Text = SceneSettings.SettingsLoad(SceneSettings.EFieldNames.DecimalPlaces);
				this.ClassTextBox.Text = SceneSettings.SettingsLoad(SceneSettings.EFieldNames.Class);
				this.VisibleTextureTextBox.Text = SceneSettings.SettingsLoad(SceneSettings.EFieldNames.VisibleTexture);
				this.HiddenTextureTextBox.Text = SceneSettings.SettingsLoad(SceneSettings.EFieldNames.HiddenTexture);

				if (SceneSettings.SettingsLoad(SceneSettings.EFieldNames.AxisAligned) == "True")
					this.AxisAlignedCheckBox.Checked = true;
				else
					this.AxisAlignedCheckBox.Checked = false;
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
			{
				return;
			}
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
			List<bool> SavedCtrlStates = new List<bool>();

			foreach( Control C in Controls )
			{
				SavedCtrlStates.Add(C.Enabled);
				C.Enabled = false;
			}

			ProgressBar.Enabled = true;
			ProgressLabel.Enabled = true;

			ProgressLabel.Show();
			ProgressBar.Show();
			GOButton.Hide();

			UpdateProgress("Initializing...");

			string OBJFilename = this.OBJFilename.Text;
			string MAPFilename = this.MAPFilename.Text;
			MainForm.EConvOption econvOption = MainForm.EConvOption.Standard;
			if (this.RB_Extrusion.Checked)
			{
				econvOption = MainForm.EConvOption.Extrusion;
			}
			else if (this.RB_Spikes.Checked)
			{
				econvOption = MainForm.EConvOption.Spikes;
			}
			double Depth = double.Parse(this.DepthTextBox.Text);
			bool bAxisAligned = this.AxisAlignedCheckBox.Checked;
			int NumDecimals = Math.Max(int.Parse(this.DecimalsTextBox.Text), 0);
			float ScaleFactor = float.Parse(this.ScaleTextBox.Text);
			string VisibleTextureName = VisibleTextureTextBox.Text.Length > 0 ? VisibleTextureTextBox.Text : "DEFAULT";
			string HiddenTextureName = HiddenTextureTextBox.Text.Length > 0 ? HiddenTextureTextBox.Text : "SKIP";
			bool bCopyToClipboard = this.CopyToClipboardCheck.Checked;
			string Classname = this.ClassTextBox.Text;

			StreamWriter LogFile = new StreamWriter("OBJ2MAP.log");

			// Error checking

			if (!File.Exists(OBJFilename))
			{
				int num3 = (int)MessageBox.Show("OBJ file doesn't exist.");
			}
			else if ((double)ScaleFactor <= 0.0)
			{
				int num4 = (int)MessageBox.Show("Scale needs to be above 0%.");
			}
			else if (econvOption != MainForm.EConvOption.Standard && Depth < 1.0)
			{
				int num5 = (int)MessageBox.Show("Depth must be greater than 0.");
			}
			else
			{
				// Input looks good, let's go...

				LogFile.AutoFlush = true;
				LogFile.WriteLine(">>> OBJ-2-MAP v1.1 starting up. <<<");
				LogFile.WriteLine(string.Format("{0}", (object)DateTime.Now));
				LogFile.WriteLine(string.Format("OBJ Filename : {0}", (object)OBJFilename));
				StreamWriter streamWriter2 = (StreamWriter)null;
				if (MAPFilename.Length > 0)
				{
					streamWriter2 = File.CreateText(MAPFilename);
					LogFile.WriteLine(string.Format("MAP Filename : {0}", (object)MAPFilename));
				}
				MainForm.EGRP egrp = MainForm.EGRP.Undefined;
				List<XVector> _Vertices = new List<XVector>();
				List<XFace> list1 = new List<XFace>();
				List<XBrush> list2 = new List<XBrush>();
				char[] separator1 = new char[1] { ' ' };
				char[] separator2 = new char[1] { '/' };

				string format = string.Format("F{0}", (object)NumDecimals);
				LogFile.WriteLine("");
				LogFile.WriteLine(string.Format("Method : {0}", (object)econvOption.ToString()));
				LogFile.WriteLine(string.Format("Copy To Clipboard : {0}", (object)bCopyToClipboard.ToString()));
				LogFile.WriteLine(string.Format("Depth: {0}", (object)Depth));
				LogFile.WriteLine(string.Format("Scale: {0}", (object)ScaleFactor));
				LogFile.WriteLine(string.Format("Decimal Places: {0}", (object)NumDecimals));
				LogFile.WriteLine(string.Format("Class: {0}", Classname.Length > 0 ? (object)Classname : (object)"worldspawn"));
				LogFile.WriteLine(string.Format("Visible Texture: {0}", VisibleTextureName.Length > 0 ? (object)VisibleTextureName : (object)"DEFAULT"));
				LogFile.WriteLine(string.Format("Hidden Texture: {0}", HiddenTextureName.Length > 0 ? (object)HiddenTextureName : (object)"SKIP"));
				LogFile.WriteLine("");
				LogFile.WriteLine("! Reading OBJ file into memory");

				string[] fileLines = File.ReadAllLines(OBJFilename);
				MAPCreation.LoadOBJ(this, fileLines, egrp, LogFile, ref _Vertices, ref list1, ref list2, ScaleFactor, separator1, separator2);

				if (list1.Count > 0)
					list2.Add(new XBrush()
					{
						Faces = list1
					});
				LogFile.WriteLine("");
				LogFile.WriteLine("Summary:");
				LogFile.WriteLine(string.Format("Vertices: {0}", (object)_Vertices.Count));
				LogFile.WriteLine(string.Format("Faces: {0}", (object)list1.Count));
				LogFile.WriteLine(string.Format("Brushes: {0}", (object)list2.Count));
				LogFile.WriteLine("");
				LogFile.WriteLine("! Computing face normals.");
				foreach (XFace xface in list1)
				{
					UpdateProgress("Processing Faces...");
					xface.ComputeNormal(ref _Vertices);
					if (bAxisAligned)
					{
						XVector[] AxisArray = new XVector[6]
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
							double num7 = XVector.Dot(AxisArray[index2], xface.Normal);
							if (num7 > num6)
							{
								num6 = num7;
								index1 = index2;
							}
						}
						xface.Normal = AxisArray[index1];
					}
				}
				LogFile.WriteLine("! Beginning MAP construction.");
				string str5 = "" + "{\n" + string.Format("\"classname\" \"{0}\"\n", Classname.Length > 0 ? (object)Classname : (object)"worldspawn");

				// HACK: Fix for crash.
				foreach (var brush in list2)
				{
					foreach (var face in brush.Faces)
					{
						if (face.Normal == null)
						{
							face.ComputeNormal(ref _Vertices);
						}
					}
				}

				MAPCreation.AddBrushesToMAP(this,econvOption, _Vertices, list1, list2, format, ref str5, VisibleTextureName, HiddenTextureName, Depth);

				string text4 = str5 + "}\n";
				if (streamWriter2 != null)
				{
					streamWriter2.Write(text4);
					streamWriter2.Close();
				}
				if ( bCopyToClipboard )
				{
					Clipboard.Clear();
					Clipboard.SetText(text4);
				}

				SceneSettings.SettingsSave(this.MAPFilename.Text, econvOption, bCopyToClipboard, Depth, ScaleFactor, NumDecimals, Classname, VisibleTextureName, HiddenTextureName, OBJFilename, this.AxisAlignedCheckBox.Checked);
			}

			int SCSIdx = 0;

			foreach (Control C in Controls)
			{
				C.Enabled = SavedCtrlStates[ SCSIdx++ ];
			}

			UpdateProgress(" ");
			ProgressLabel.Hide();
			ProgressBar.Hide();
			GOButton.Show();

			string str6 = "Done!" + "\n\n" + string.Format("\"{0}\" converted successfully.\n", (object)OBJFilename) + "\n";
			if (MAPFilename.Length > 0)
				str6 += string.Format("Written to \"{0}\"", (object)MAPFilename);
			string FinishMsg = !bCopyToClipboard ? str6 + "." : (MAPFilename.Length <= 0 ? str6 + "MAP text copied to the clipboard." : str6 + "and MAP text copied to the clipboard.");
			LogFile.WriteLine(FinishMsg);
			MessageBox.Show(FinishMsg);
			LogFile.Close();
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
			this.ProgressLabel = new System.Windows.Forms.Label();
			this.ProgressBar = new System.Windows.Forms.ProgressBar();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.OBJFilename);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(436, 58);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "INPUT";
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
			this.groupBox2.Location = new System.Drawing.Point(12, 75);
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
			this.GOButton.Location = new System.Drawing.Point(12, 329);
			this.GOButton.Name = "GOButton";
			this.GOButton.Size = new System.Drawing.Size(436, 52);
			this.GOButton.TabIndex = 2;
			this.GOButton.Text = "GO!";
			this.GOButton.UseVisualStyleBackColor = false;
			this.GOButton.Click += new System.EventHandler(this.GoButton_Click);
			// 
			// ProgressLabel
			// 
			this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ProgressLabel.Location = new System.Drawing.Point(12, 330);
			this.ProgressLabel.Name = "ProgressLabel";
			this.ProgressLabel.Size = new System.Drawing.Size(436, 27);
			this.ProgressLabel.TabIndex = 3;
			this.ProgressLabel.Text = "Working...";
			this.ProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.ProgressLabel.Visible = false;
			// 
			// ProgressBar
			// 
			this.ProgressBar.Location = new System.Drawing.Point(15, 358);
			this.ProgressBar.Name = "ProgressBar";
			this.ProgressBar.Size = new System.Drawing.Size(433, 23);
			this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.ProgressBar.TabIndex = 4;
			this.ProgressBar.Visible = false;
			// 
			// MainForm
			// 
			this.ClientSize = new System.Drawing.Size(460, 392);
			this.Controls.Add(this.ProgressBar);
			this.Controls.Add(this.ProgressLabel);
			this.Controls.Add(this.GOButton);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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