/*******************************************************************************
 * Copyright (c) 2013, Daniel Murphy
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 	* Redistributions of source code must retain the above copyright notice,
 * 	  this list of conditions and the following disclaimer.
 * 	* Redistributions in binary form must reproduce the above copyright notice,
 * 	  this list of conditions and the following disclaimer in the documentation
 * 	  and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/

using System;
using System.Diagnostics;

namespace SharpBox2D.Common
{

/**
 * A 2-by-2 matrix. Stored in column-major order.
 */
    public class Mat22 : IEquatable<Mat22>
    {
        public Vec2 ex, ey;

        /** Convert the matrix to printable format. */

        public override string ToString()
        {
            string s = "";
            s += "[" + ex.x + "," + ey.x + "]\n";
            s += "[" + ex.y + "," + ey.y + "]";
            return s;
        }

        /**
   * Create a matrix with given vectors as columns.
   * 
   * @param c1 Column 1 of matrix
   * @param c2 Column 2 of matrix
   */
        
        public Mat22()
        {
            ex = new Vec2(1, 0);
            ey = new Vec2(0, 1);
        }

        public Mat22(Vec2 c1, Vec2 c2)
        {
            ex = c1.clone();
            ey = c2.clone();
        }

        /**
   * Create a matrix from four floats.
   * 
   * @param exx
   * @param col2x
   * @param exy
   * @param col2y
   */

        public Mat22(float exx, float col2x, float exy, float col2y)
        {
            ex = new Vec2(exx, exy);
            ey = new Vec2(col2x, col2y);
        }

        /**
   * Set as a copy of another matrix.
   * 
   * @param m Matrix to copy
   */

        public Mat22 set(Mat22 m)
        {
            ex.x = m.ex.x;
            ex.y = m.ex.y;
            ey.x = m.ey.x;
            ey.y = m.ey.y;
            return this;
        }

        public Mat22 set(float exx, float col2x, float exy, float col2y)
        {
            ex.x = exx;
            ex.y = exy;
            ey.x = col2x;
            ey.y = col2y;
            return this;
        }

        /**
   * Return a clone of this matrix. djm fixed double allocation
   */
        // @Override // annotation omitted for GWT-compatibility
        public Mat22 clone()
        {
            return new Mat22(ex, ey);
        }

        /**
   * Set as a matrix representing a rotation.
   * 
   * @param angle Rotation (in radians) that matrix represents.
   */

        public void set(float angle)
        {
            float c = MathUtils.cos(angle), s = MathUtils.sin(angle);
            ex.x = c;
            ey.x = -s;
            ex.y = s;
            ey.y = c;
        }

        /**
   * Set as the identity matrix.
   */

        public void setIdentity()
        {
            ex.x = 1.0f;
            ey.x = 0.0f;
            ex.y = 0.0f;
            ey.y = 1.0f;
        }

        /**
   * Set as the zero matrix.
   */

        public void setZero()
        {
            ex.x = 0.0f;
            ey.x = 0.0f;
            ex.y = 0.0f;
            ey.y = 0.0f;
        }

        /**
   * Extract the angle from this matrix (assumed to be a rotation matrix).
   * 
   * @return
   */

        public float getAngle()
        {
            return MathUtils.atan2(ex.y, ex.x);
        }

        /**
   * Set by column vectors.
   * 
   * @param c1 Column 1
   * @param c2 Column 2
   */

        public void set(Vec2 c1, Vec2 c2)
        {
            ex.x = c1.x;
            ey.x = c2.x;
            ex.y = c1.y;
            ey.y = c2.y;
        }

        /** Returns the inverted Mat22 - does NOT invert the matrix locally! */

        public Mat22 invert()
        {
            float a = ex.x, b = ey.x, c = ex.y, d = ey.y;
            Mat22 B = new Mat22();
            float det = a*d - b*c;
            if (det != 0)
            {
                det = 1.0f/det;
            }
            B.ex.x = det*d;
            B.ey.x = -det*b;
            B.ex.y = -det*c;
            B.ey.y = det*a;
            return B;
        }

        public void invertLocal()
        {
            float a = ex.x, b = ey.x, c = ex.y, d = ey.y;
            float det = a*d - b*c;
            if (det != 0)
            {
                det = 1.0f/det;
            }
            ex.x = det*d;
            ey.x = -det*b;
            ex.y = -det*c;
            ey.y = det*a;
        }

        public void invertToOut(ref Mat22 m)
        {
            float a = ex.x, b = ey.x, c = ex.y, d = ey.y;
            float det = a*d - b*c;
            // b2Debug.Assert(det != 0.0f);
            det = 1.0f/det;
            m.ex.x = det*d;
            m.ey.x = -det*b;
            m.ex.y = -det*c;
            m.ey.y = det*a;
        }



        /**
   * Return the matrix composed of the absolute values of all elements. djm: fixed double allocation
   * 
   * @return Absolute value matrix
   */

        public Mat22 abs()
        {
            return new Mat22(MathUtils.abs(ex.x), MathUtils.abs(ey.x), MathUtils.abs(ex.y),
                MathUtils.abs(ey.y));
        }

        /* djm: added */

        public void absLocal()
        {
            ex.absLocal();
            ey.absLocal();
        }

        /**
   * Return the matrix composed of the absolute values of all elements.
   * 
   * @return Absolute value matrix
   */

        public static Mat22 abs(Mat22 R)
        {
            return R.abs();
        }

        /* djm created */

        public static void absToOut(Mat22 R, Mat22 m)
        {
            m.ex.x = MathUtils.abs(R.ex.x);
            m.ex.y = MathUtils.abs(R.ex.y);
            m.ey.x = MathUtils.abs(R.ey.x);
            m.ey.y = MathUtils.abs(R.ey.y);
        }

        /**
   * Multiply a vector by this matrix.
   * 
   * @param v Vector to multiply by matrix.
   * @return Resulting vector
   */

        public Vec2 mul(Vec2 v)
        {
            return new Vec2(ex.x*v.x + ey.x*v.y, ex.y*v.x + ey.y*v.y);
        }

        public void mulToOut(Vec2 v, ref Vec2 v2)
        {
            float tempy = ex.y*v.x + ey.y*v.y;
            v2.x = ex.x*v.x + ey.x*v.y;
            v2.y = tempy;
        }

        public void mulToOutUnsafe(Vec2 v, ref Vec2 v2)
        {
            Debug.Assert(v != v2);
            v2.x = ex.x*v.x + ey.x*v.y;
            v2.y = ex.y*v.x + ey.y*v.y;
        }


        /**
   * Multiply another matrix by this one (this one on left). djm optimized
   * 
   * @param R
   * @return
   */

        public Mat22 mul(Mat22 R)
        {
            /*
     * Mat22 C = new Mat22();C.set(this.mul(R.ex), this.mul(R.ey));return C;
     */
            Mat22 C = new Mat22();
            C.ex.x = ex.x*R.ex.x + ey.x*R.ex.y;
            C.ex.y = ex.y*R.ex.x + ey.y*R.ex.y;
            C.ey.x = ex.x*R.ey.x + ey.x*R.ey.y;
            C.ey.y = ex.y*R.ey.x + ey.y*R.ey.y;
            // C.set(ex,col2);
            return C;
        }

        public void mulLocal(Mat22 R)
        {
            mulToOut(R, this);
        }

        public void mulToOut(Mat22 R, ref Mat22 m)
        {
            float tempy1 = this.ex.y*R.ex.x + this.ey.y*R.ex.y;
            float tempx1 = this.ex.x*R.ex.x + this.ey.x*R.ex.y;
            m.ex.x = tempx1;
            m.ex.y = tempy1;
            float tempy2 = this.ex.y*R.ey.x + this.ey.y*R.ey.y;
            float tempx2 = this.ex.x*R.ey.x + this.ey.x*R.ey.y;
            m.ey.x = tempx2;
            m.ey.y = tempy2;
        }

        public void mulToOut(Mat22 R, Mat22 m)
        {
            float tempy1 = this.ex.y*R.ex.x + this.ey.y*R.ex.y;
            float tempx1 = this.ex.x*R.ex.x + this.ey.x*R.ex.y;
            m.ex.x = tempx1;
            m.ex.y = tempy1;
            float tempy2 = this.ex.y*R.ey.x + this.ey.y*R.ey.y;
            float tempx2 = this.ex.x*R.ey.x + this.ey.x*R.ey.y;
            m.ey.x = tempx2;
            m.ey.y = tempy2;
        }

        public void mulToOutUnsafe(Mat22 R, ref Mat22 m)
        {
            Debug.Assert(m != R);
            Debug.Assert(m != this);
            m.ex.x = this.ex.x*R.ex.x + this.ey.x*R.ex.y;
            m.ex.y = this.ex.y*R.ex.x + this.ey.y*R.ex.y;
            m.ey.x = this.ex.x*R.ey.x + this.ey.x*R.ey.y;
            m.ey.y = this.ex.y*R.ey.x + this.ey.y*R.ey.y;
        }

        /**
   * Multiply another matrix by the transpose of this one (transpose of this one on left). djm:
   * optimized
   * 
   * @param B
   * @return
   */

        public Mat22 mulTrans(Mat22 B)
        {
            /*
     * Vec2 c1 = new Vec2(Vec2.dot(this.ex, B.ex), Vec2.dot(this.ey, B.ex)); Vec2 c2 = new
     * Vec2(Vec2.dot(this.ex, B.ey), Vec2.dot(this.ey, B.ey)); Mat22 C = new Mat22(); C.set(c1, c2);
     * return C;
     */
            Mat22 C = new Mat22();

            C.ex.x = Vec2.dot(this.ex, B.ex);
            C.ex.y = Vec2.dot(this.ey, B.ex);

            C.ey.x = Vec2.dot(this.ex, B.ey);
            C.ey.y = Vec2.dot(this.ey, B.ey);
            return C;
        }

        public void mulTransLocal(Mat22 B)
        {
            mulTransToOut(B, this);
        }

        public void mulTransToOut(Mat22 B, ref Mat22 m)
        {
            /*
     * ref.ex.x = Vec2.dot(this.ex, B.ex); ref.ex.y = Vec2.dot(this.ey, B.ex); ref.ey.x =
     * Vec2.dot(this.ex, B.ey); ref.ey.y = Vec2.dot(this.ey, B.ey);
     */
            float x1 = this.ex.x*B.ex.x + this.ex.y*B.ex.y;
            float y1 = this.ey.x*B.ex.x + this.ey.y*B.ex.y;
            float x2 = this.ex.x*B.ey.x + this.ex.y*B.ey.y;
            float y2 = this.ey.x*B.ey.x + this.ey.y*B.ey.y;
            m.ex.x = x1;
            m.ey.x = x2;
            m.ex.y = y1;
            m.ey.y = y2;
        }

        public void mulTransToOut(Mat22 B, Mat22 m)
        {
            /*
     * ref.ex.x = Vec2.dot(this.ex, B.ex); ref.ex.y = Vec2.dot(this.ey, B.ex); ref.ey.x =
     * Vec2.dot(this.ex, B.ey); ref.ey.y = Vec2.dot(this.ey, B.ey);
     */
            float x1 = this.ex.x*B.ex.x + this.ex.y*B.ex.y;
            float y1 = this.ey.x*B.ex.x + this.ey.y*B.ex.y;
            float x2 = this.ex.x*B.ey.x + this.ex.y*B.ey.y;
            float y2 = this.ey.x*B.ey.x + this.ey.y*B.ey.y;
            m.ex.x = x1;
            m.ey.x = x2;
            m.ex.y = y1;
            m.ey.y = y2;
        }
         
        public void mulTransToOutUnsafe(Mat22 B, ref Mat22 m)
        {
            Debug.Assert(B != m);
            Debug.Assert(this != m);
            m.ex.x = this.ex.x*B.ex.x + this.ex.y*B.ex.y;
            m.ey.x = this.ex.x*B.ey.x + this.ex.y*B.ey.y;
            m.ex.y = this.ey.x*B.ex.x + this.ey.y*B.ex.y;
            m.ey.y = this.ey.x*B.ey.x + this.ey.y*B.ey.y;
        }

        /**
   * Multiply a vector by the transpose of this matrix.
   * 
   * @param v
   * @return
   */

        public Vec2 mulTrans(Vec2 v)
        {
            // return new Vec2(Vec2.dot(v, ex), Vec2.dot(v, col2));
            return new Vec2((v.x*ex.x + v.y*ex.y), (v.x*ey.x + v.y*ey.y));
        }

        /* djm added */

        public void mulTransToOut(Vec2 v, ref Vec2 v2)
        {
            /*
     * ref.x = Vec2.dot(v, ex); ref.y = Vec2.dot(v, col2);
     */
            float tempx = v.x*ex.x + v.y*ex.y;
            v2.y = v.x*ey.x + v.y*ey.y;
            v2.x = tempx;
        }

        /**
   * Add this matrix to B, return the result.
   * 
   * @param B
   * @return
   */

        public Mat22 add(Mat22 B)
        {
            // return new Mat22(ex.add(B.ex), col2.add(B.ey));
            Mat22 m = new Mat22();
            m.ex.x = ex.x + B.ex.x;
            m.ex.y = ex.y + B.ex.y;
            m.ey.x = ey.x + B.ey.x;
            m.ey.y = ey.y + B.ey.y;
            return m;
        }

        /**
   * Add B to this matrix locally.
   * 
   * @param B
   * @return
   */

        public void addLocal(Mat22 B)
        {
            // ex.addLocal(B.ex);
            // col2.addLocal(B.ey);
            ex.x += B.ex.x;
            ex.y += B.ex.y;
            ey.x += B.ey.x;
            ey.y += B.ey.y;
        }

        /**
   * Solve A * x = b where A = this matrix.
   * 
   * @return The vector x that solves the above equation.
   */

        public Vec2 solve(Vec2 b)
        {
            float a11 = ex.x, a12 = ey.x, a21 = ex.y, a22 = ey.y;
            float det = a11*a22 - a12*a21;
            if (det != 0.0f)
            {
                det = 1.0f/det;
            }
            Vec2 x = new Vec2(det*(a22*b.x - a12*b.y), det*(a11*b.y - a21*b.x));
            return x;
        }

        public void solveToOut(Vec2 b, ref Vec2 v2)
        {
            float a11 = ex.x, a12 = ey.x, a21 = ex.y, a22 = ey.y;
            float det = a11*a22 - a12*a21;
            if (det != 0.0f)
            {
                det = 1.0f/det;
            }
            float tempy = det*(a11*b.y - a21*b.x);
            v2.x = det*(a22*b.x - a12*b.y);
            v2.y = tempy;
        }

        public static Vec2 mul(Mat22 R, Vec2 v)
        {
            // return R.mul(v);
            return new Vec2(R.ex.x*v.x + R.ey.x*v.y, R.ex.y*v.x + R.ey.y*v.y);
        }

        public static void mulToOut(Mat22 R, Vec2 v, ref Vec2 v2)
        {
            float tempy = R.ex.y*v.x + R.ey.y*v.y;
            v2.x = R.ex.x*v.x + R.ey.x*v.y;
            v2.y = tempy;
        }

        public static void mulToOutUnsafe(Mat22 R, Vec2 v, ref Vec2 v2)
        {
            Debug.Assert(v != v2);
            v2.x = R.ex.x*v.x + R.ey.x*v.y;
            v2.y = R.ex.y*v.x + R.ey.y*v.y;
        }

        public static Mat22 mul(Mat22 A, Mat22 B)
        {
            // return A.mul(B);
            Mat22 C = new Mat22();
            C.ex.x = A.ex.x*B.ex.x + A.ey.x*B.ex.y;
            C.ex.y = A.ex.y*B.ex.x + A.ey.y*B.ex.y;
            C.ey.x = A.ex.x*B.ey.x + A.ey.x*B.ey.y;
            C.ey.y = A.ex.y*B.ey.x + A.ey.y*B.ey.y;
            return C;
        }

        public static void mulToOut(Mat22 A, Mat22 B, ref Mat22 m)
        {
            float tempy1 = A.ex.y*B.ex.x + A.ey.y*B.ex.y;
            float tempx1 = A.ex.x*B.ex.x + A.ey.x*B.ex.y;
            float tempy2 = A.ex.y*B.ey.x + A.ey.y*B.ey.y;
            float tempx2 = A.ex.x*B.ey.x + A.ey.x*B.ey.y;
            m.ex.x = tempx1;
            m.ex.y = tempy1;
            m.ey.x = tempx2;
            m.ey.y = tempy2;
        }

        public static void mulToOutUnsafe(Mat22 A, Mat22 B, ref Mat22 m)
        {
            Debug.Assert(m != A);
            Debug.Assert(m != B);
            m.ex.x = A.ex.x*B.ex.x + A.ey.x*B.ex.y;
            m.ex.y = A.ex.y*B.ex.x + A.ey.y*B.ex.y;
            m.ey.x = A.ex.x*B.ey.x + A.ey.x*B.ey.y;
            m.ey.y = A.ex.y*B.ey.x + A.ey.y*B.ey.y;
        }

        public static Vec2 mulTrans(Mat22 R, Vec2 v)
        {
            return new Vec2((v.x*R.ex.x + v.y*R.ex.y), (v.x*R.ey.x + v.y*R.ey.y));
        }

        public static void mulTransToOut(Mat22 R, Vec2 v, ref Vec2 v2)
        {
            float outx = v.x*R.ex.x + v.y*R.ex.y;
            v2.y = v.x*R.ey.x + v.y*R.ey.y;
            v2.x = outx;
        }

        public static void mulTransToOutUnsafe(Mat22 R, Vec2 v, ref Vec2 v2)
        {
            Debug.Assert(v2 != v);
            v2.y = v.x*R.ey.x + v.y*R.ey.y;
            v2.x = v.x*R.ex.x + v.y*R.ex.y;
        }

        public static Mat22 mulTrans(Mat22 A, Mat22 B)
        {
            Mat22 C = new Mat22();
            C.ex.x = A.ex.x*B.ex.x + A.ex.y*B.ex.y;
            C.ex.y = A.ey.x*B.ex.x + A.ey.y*B.ex.y;
            C.ey.x = A.ex.x*B.ey.x + A.ex.y*B.ey.y;
            C.ey.y = A.ey.x*B.ey.x + A.ey.y*B.ey.y;
            return C;
        }

        public static void mulTransToOut(Mat22 A, Mat22 B, ref Mat22 m)
        {
            float x1 = A.ex.x*B.ex.x + A.ex.y*B.ex.y;
            float y1 = A.ey.x*B.ex.x + A.ey.y*B.ex.y;
            float x2 = A.ex.x*B.ey.x + A.ex.y*B.ey.y;
            float y2 = A.ey.x*B.ey.x + A.ey.y*B.ey.y;

            m.ex.x = x1;
            m.ex.y = y1;
            m.ey.x = x2;
            m.ey.y = y2;
        }

        public static void mulTransToOutUnsafe(Mat22 A, Mat22 B, ref Mat22 m)
        {
            Debug.Assert(A != m);
            Debug.Assert(B != m);
            m.ex.x = A.ex.x*B.ex.x + A.ex.y*B.ex.y;
            m.ex.y = A.ey.x*B.ex.x + A.ey.y*B.ex.y;
            m.ey.x = A.ex.x*B.ey.x + A.ex.y*B.ey.y;
            m.ey.y = A.ey.x*B.ey.x + A.ey.y*B.ey.y;
        }

        public static Mat22 createRotationalTransform(float angle)
        {
            Mat22 mat = new Mat22();
            float c = MathUtils.cos(angle);
            float s = MathUtils.sin(angle);
            mat.ex.x = c;
            mat.ey.x = -s;
            mat.ex.y = s;
            mat.ey.y = c;
            return mat;
        }

        public static void createRotationalTransform(float angle, ref Mat22 m)
        {
            float c = MathUtils.cos(angle);
            float s = MathUtils.sin(angle);
            m.ex.x = c;
            m.ey.x = -s;
            m.ex.y = s;
            m.ey.y = c;
        }

        public static Mat22 createScaleTransform(float scale)
        {
            Mat22 mat = new Mat22();
            mat.ex.x = scale;
            mat.ey.y = scale;
            return mat;
        }

        public static void createScaleTransform(float scale, ref Mat22 m)
        {
            m.ex.x = scale;
            m.ey.y = scale;
        }

        public bool Equals(Mat22 other)
        {
            return ex.Equals(other.ex) && ey.Equals(other.ey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Mat22 && Equals((Mat22) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ex.GetHashCode()*397) ^ ey.GetHashCode();
            }
        }

        public static bool operator ==(Mat22 left, Mat22 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Mat22 left, Mat22 right)
        {
            return !left.Equals(right);
        }
    }
}