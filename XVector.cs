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

namespace OBJ2MAP
{
  public class XVector
  {
    public double x;
    public double y;
    public double z;

    public XVector()
    {
      this.x = this.y = this.z = 0.0;
    }

    public XVector(double _x, double _y, double _z)
    {
      this.x = _x;
      this.y = _y;
      this.z = _z;
    }

    public XVector(XVector _V)
    {
      this.x = _V.x;
      this.y = _V.y;
      this.z = _V.z;
    }

    public XVector Normalize()
    {
      double sizeSquared = this.GetSizeSquared();
      if (sizeSquared == 0.0)
        return this;
      this.x /= sizeSquared;
      this.y /= sizeSquared;
      this.z /= sizeSquared;
      return this;
    }

    public double GetSizeSquared()
    {
      return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
    }

    public static XVector Cross(XVector _A, XVector _B)
    {
      return new XVector()
      {
        x = _A.y * _B.z - _A.z * _B.y,
        y = _A.z * _B.x - _A.x * _B.z,
        z = _A.x * _B.y - _A.y * _B.x
      };
    }

    public static double Dot(XVector _A, XVector _B)
    {
      return _A.x * _B.x + _A.y * _B.y + _A.z * _B.z;
    }

    public static XVector Subtract(XVector _A, XVector _B)
    {
      return new XVector(_A.x - _B.x, _A.y - _B.y, _A.z - _B.z);
    }

    public static XVector Add(XVector _A, XVector _B)
    {
      return new XVector(_A.x + _B.x, _A.y + _B.y, _A.z + _B.z);
    }

    public static XVector Multiply(XVector _V, double _Scalar)
    {
      return new XVector(_V.x * _Scalar, _V.y * _Scalar, _V.z * _Scalar);
    }

    public static XVector Divide(XVector _V, double _Scalar)
    {
      return new XVector(_V.x / _Scalar, _V.y / _Scalar, _V.z / _Scalar);
    }
  }
}
