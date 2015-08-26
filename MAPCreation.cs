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
using System.Text;
using System.IO;
using System.Globalization;

namespace OBJ2MAP
{
	class MAPCreation
	{
		public static void LoadOBJ(MainForm _MainForm,
									string[] fileLines,
								   MainForm.EGRP egrp,
								   StreamWriter streamWriter,
								   ref List<XVector> _Vertices,
								   ref List<XFace> _Faces,
								   ref List<XBrush> _Brushes,
								   float scale,
								   char[] separator1,
								   char[] separator2)
		{
			_MainForm.UpdateProgress("Loading OBJ File...");

			foreach (string Line in fileLines)
			{
				_MainForm.UpdateProgress();

				string TrimmedLine = Line.Trim();

				streamWriter.WriteLine(string.Format("# OBJ Line: {0}", (object)TrimmedLine));
				if (!TrimmedLine.StartsWith("# ") && TrimmedLine.Length != 0)
				{
					if (egrp == MainForm.EGRP.Undefined && (TrimmedLine.StartsWith("o ") || TrimmedLine.StartsWith("g ")))
					{
						egrp = !TrimmedLine.StartsWith("g ") ? MainForm.EGRP.Ungrouped : MainForm.EGRP.Grouped;
					}

					if (TrimmedLine.StartsWith("g ") && egrp == MainForm.EGRP.Grouped || TrimmedLine.StartsWith("o ") && egrp == MainForm.EGRP.Ungrouped)
					{
						if (_Faces.Count > 0)
						{
							XBrush B = new XBrush();
							_Brushes.Add( B );
							B.Faces = _Faces;
						}
						_Faces = new List<XFace>();
					}
					if (TrimmedLine.StartsWith("v "))
					{
						string[] strArray = TrimmedLine.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
						if (strArray.Length == 4)
						{
							XVector xvector = new XVector(double.Parse(strArray[1], new CultureInfo("en-US")), double.Parse(strArray[2], new CultureInfo("en-US")), double.Parse(strArray[3], new CultureInfo("en-US")));
							double num6 = xvector.y;
							xvector.y = -xvector.z;
							xvector.z = num6;
							xvector.x *= (double)scale / 100.0;
							xvector.y *= (double)scale / 100.0;
							xvector.z *= (double)scale / 100.0;
							_Vertices.Add(xvector);
						}
					}
					if (TrimmedLine.StartsWith("f "))
					{
						XFace xface = new XFace();
						string[] strArray1 = TrimmedLine.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
						for (int index = 1; index < strArray1.Length; ++index)
						{
							string[] strArray2 = strArray1[index].Split(separator2, StringSplitOptions.RemoveEmptyEntries);
							xface.VertIdx.Add(int.Parse(strArray2[0]) - 1);
						}
						_Faces.Add(xface);
					}
				}
			}
		}

		public static void AddBrushesToMAP(MainForm _MainForm,
								MainForm.EConvOption econvOption,
								  List<XVector> _Vertices,
								  List<XFace> _Faces,
								  List<XBrush> _Brushes,
								  string format,
								  ref string _MAPText,
								  string _VisibleTextureName,
								  string _HiddenTextureName,
								  double _Scalar
								  )
		{
			_MainForm.UpdateProgress( "Adding Brushes to MAP...");
			int BrushCount = 0;

			switch (econvOption)
			{
				case MainForm.EConvOption.Extrusion:
					using (List<XBrush>.Enumerator enumerator = _Brushes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							_MainForm.UpdateProgress(string.Format("Adding Brush {0:n0} to MAP...", BrushCount++));
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
								_MAPText += "{\n";
								XVector xvector1 = list3[0];
								XVector xvector2 = list3[1];
								XVector xvector3 = list3[2];
								_MAPText += string.Format("\t( {0} {1} {2} ) ", (object)xvector1.x.ToString(format, new CultureInfo("en-US")), (object)xvector1.y.ToString(format, new CultureInfo("en-US")), (object)xvector1.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) ", (object)xvector3.x.ToString(format, new CultureInfo("en-US")), (object)xvector3.y.ToString(format, new CultureInfo("en-US")), (object)xvector3.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector2.x.ToString(format, new CultureInfo("en-US")), (object)xvector2.y.ToString(format, new CultureInfo("en-US")), (object)xvector2.z.ToString(format, new CultureInfo("en-US")), (object)_VisibleTextureName);
								XVector xvector4 = list4[2];
								XVector xvector5 = list4[1];
								XVector xvector6 = list4[0];
								_MAPText += string.Format("\t( {0} {1} {2} ) ", (object)xvector4.x.ToString(format, new CultureInfo("en-US")), (object)xvector4.y.ToString(format, new CultureInfo("en-US")), (object)xvector4.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) ", (object)xvector6.x.ToString(format, new CultureInfo("en-US")), (object)xvector6.y.ToString(format, new CultureInfo("en-US")), (object)xvector6.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector5.x.ToString(format, new CultureInfo("en-US")), (object)xvector5.y.ToString(format, new CultureInfo("en-US")), (object)xvector5.z.ToString(format, new CultureInfo("en-US")), (object)_HiddenTextureName);
								for (int index = 0; index < list3.Count; ++index)
								{
									XVector xvector7 = list3[index];
									XVector xvector8 = list3[(index + 1) % list3.Count];
									XVector xvector9 = list4[index];
									_MAPText += string.Format("\t( {0} {1} {2} ) ", (object)xvector7.x.ToString(format, new CultureInfo("en-US")), (object)xvector7.y.ToString(format, new CultureInfo("en-US")), (object)xvector7.z.ToString(format, new CultureInfo("en-US")));
									_MAPText += string.Format("( {0} {1} {2} ) ", (object)xvector8.x.ToString(format, new CultureInfo("en-US")), (object)xvector8.y.ToString(format, new CultureInfo("en-US")), (object)xvector8.z.ToString(format, new CultureInfo("en-US")));
									_MAPText += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector9.x.ToString(format, new CultureInfo("en-US")), (object)xvector9.y.ToString(format, new CultureInfo("en-US")), (object)xvector9.z.ToString(format, new CultureInfo("en-US")), (object)_HiddenTextureName);
								}
								_MAPText += "}\n";
							}
						}
						break;
					}
				case MainForm.EConvOption.Spikes:
					using (List<XBrush>.Enumerator enumerator = _Brushes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							_MainForm.UpdateProgress(string.Format("Adding Brush {0:n0} to MAP...", BrushCount++));
							foreach (XFace xface in enumerator.Current.Faces)
							{
								XVector _B1 = XVector.Multiply(xface.Normal, _Scalar);
								List<XVector> list3 = new List<XVector>();
								foreach (int index in xface.VertIdx)
								{
									XVector xvector = new XVector(_Vertices[index]);
									list3.Add(xvector);
								}
								_MAPText += "{\n";
								XVector xvector1 = list3[0];
								XVector xvector2 = list3[1];
								XVector xvector3 = list3[2];
								_MAPText += string.Format("\t( {0} {1} {2} ) ", (object)xvector1.x.ToString(format, new CultureInfo("en-US")), (object)xvector1.y.ToString(format, new CultureInfo("en-US")), (object)xvector1.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) ", (object)xvector3.x.ToString(format, new CultureInfo("en-US")), (object)xvector3.y.ToString(format, new CultureInfo("en-US")), (object)xvector3.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector2.x.ToString(format, new CultureInfo("en-US")), (object)xvector2.y.ToString(format, new CultureInfo("en-US")), (object)xvector2.z.ToString(format, new CultureInfo("en-US")), (object)_VisibleTextureName);
								XVector xvector4 = new XVector();
								foreach (int index in xface.VertIdx)
								{
									XVector _B2 = new XVector(_Vertices[index]);
									xvector4 = XVector.Add(xvector4, _B2);
								}
								XVector xvector5 = XVector.Add(XVector.Divide(xvector4, (double)xface.VertIdx.Count), _B1);
								for (int index = 0; index < list3.Count; ++index)
								{
									XVector xvector6 = list3[index];
									XVector xvector7 = list3[(index + 1) % list3.Count];
									_MAPText += string.Format("\t( {0} {1} {2} ) ", (object)xvector5.x.ToString(format, new CultureInfo("en-US")), (object)xvector5.y.ToString(format, new CultureInfo("en-US")), (object)xvector5.z.ToString(format, new CultureInfo("en-US")));
									_MAPText += string.Format("( {0} {1} {2} ) ", (object)xvector6.x.ToString(format, new CultureInfo("en-US")), (object)xvector6.y.ToString(format, new CultureInfo("en-US")), (object)xvector6.z.ToString(format, new CultureInfo("en-US")));
									_MAPText += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector7.x.ToString(format, new CultureInfo("en-US")), (object)xvector7.y.ToString(format, new CultureInfo("en-US")), (object)xvector7.z.ToString(format, new CultureInfo("en-US")), (object)_HiddenTextureName);
								}
								_MAPText += "}\n";
							}
						}
						break;
					}
				default:
					using (List<XBrush>.Enumerator enumerator = _Brushes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							XBrush current = enumerator.Current;
							_MAPText += "{\n";
							_MainForm.UpdateProgress(string.Format("Adding Brush {0:n0} to MAP...", BrushCount++));
							foreach (XFace xface in current.Faces)
							{
								XVector xvector1 = _Vertices[xface.VertIdx[0]];
								XVector xvector2 = _Vertices[xface.VertIdx[2]];
								XVector xvector3 = _Vertices[xface.VertIdx[1]];
								_MAPText += string.Format("\t( {0} {1} {2} ) ", (object)xvector1.x.ToString(format, new CultureInfo("en-US")), (object)xvector1.y.ToString(format, new CultureInfo("en-US")), (object)xvector1.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) ", (object)xvector2.x.ToString(format, new CultureInfo("en-US")), (object)xvector2.y.ToString(format, new CultureInfo("en-US")), (object)xvector2.z.ToString(format, new CultureInfo("en-US")));
								_MAPText += string.Format("( {0} {1} {2} ) {3} 0 0 0 1 1\n", (object)xvector3.x.ToString(format, new CultureInfo("en-US")), (object)xvector3.y.ToString(format, new CultureInfo("en-US")), (object)xvector3.z.ToString(format, new CultureInfo("en-US")), (object)_VisibleTextureName);
							}
							_MAPText += "}\n";
						}
						break;
					}
			}
		}
	}
}
