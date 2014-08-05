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

using System.Diagnostics;

namespace SharpBox2D.Common
{

/**
 * A 3-by-3 matrix. Stored in column-major order.
 * 
 * @author Daniel Murphy
 */

    public struct Mat33
    {
        public static readonly Mat33 IDENTITY = new Mat33(new Vec3(1, 0, 0), new Vec3(0, 1, 0), new Vec3(0,
            0, 1));

        public Vec3 ex, ey, ez;

        public Mat33(float exx, float exy, float exz, float eyx, float eyy, float eyz, float ezx,
            float ezy, float ezz)
        {
            ex = new Vec3(exx, exy, exz);
            ey = new Vec3(eyx, eyy, eyz);
            ez = new Vec3(ezx, ezy, ezz);
        }

        public Mat33(Vec3 argCol1, Vec3 argCol2, Vec3 argCol3)
        {
            ex = argCol1.clone();
            ey = argCol2.clone();
            ez = argCol3.clone();
        }

        public void setZero()
        {
            ex.setZero();
            ey.setZero();
            ez.setZero();
        }

        public void set(float exx, float exy, float exz, float eyx, float eyy, float eyz, float ezx,
            float ezy, float ezz)
        {
            ex.x = exx;
            ex.y = exy;
            ex.z = exz;
            ey.x = eyx;
            ey.y = eyy;
            ey.z = eyz;
            ez.x = eyx;
            ez.y = eyy;
            ez.z = eyz;
        }

        public void set(Mat33 mat)
        {
            Vec3 vec = mat.ex;
            ex.x = vec.x;
            ex.y = vec.y;
            ex.z = vec.z;
            Vec3 vec1 = mat.ey;
            ey.x = vec1.x;
            ey.y = vec1.y;
            ey.z = vec1.z;
            Vec3 vec2 = mat.ez;
            ez.x = vec2.x;
            ez.y = vec2.y;
            ez.z = vec2.z;
        }

        public void setIdentity()
        {
            ex.x = (float) 1;
            ex.y = (float) 0;
            ex.z = (float) 0;
            ey.x = (float) 0;
            ey.y = (float) 1;
            ey.z = (float) 0;
            ez.x = (float) 0;
            ez.y = (float) 0;
            ez.z = (float) 1;
        }

        // / Multiply a matrix times a vector.
        public static Vec3 mul(Mat33 A, Vec3 v)
        {
            return new Vec3(v.x*A.ex.x + v.y*A.ey.x + v.z + A.ez.x, v.x*A.ex.y + v.y*A.ey.y + v.z
                                                                    *A.ez.y, v.x*A.ex.z + v.y*A.ey.z + v.z*A.ez.z);
        }

        public static Vec2 mul22(Mat33 A, Vec2 v)
        {
            return new Vec2(A.ex.x*v.x + A.ey.x*v.y, A.ex.y*v.x + A.ey.y*v.y);
        }

        public static void mul22ToOut(Mat33 A, Vec2 v, ref Vec2 v2)
        {
            float tempx = A.ex.x*v.x + A.ey.x*v.y;
            v2.y = A.ex.y*v.x + A.ey.y*v.y;
            v2.x = tempx;
        }

        public static void mul22ToOutUnsafe(Mat33 A, Vec2 v, ref Vec2 v2)
        {
            Debug.Assert(v != v2);
            v2.y = A.ex.y*v.x + A.ey.y*v.y;
            v2.x = A.ex.x*v.x + A.ey.x*v.y;
        }

        public static void mulToOut(Mat33 A, Vec3 v, ref Vec3 v2)
        {
            float tempy = v.x*A.ex.y + v.y*A.ey.y + v.z*A.ez.y;
            float tempz = v.x*A.ex.z + v.y*A.ey.z + v.z*A.ez.z;
            v2.x = v.x*A.ex.x + v.y*A.ey.x + v.z*A.ez.x;
            v2.y = tempy;
            v2.z = tempz;
        }

        public static void mulToOutUnsafe(Mat33 A, Vec3 v, ref Vec3 v2)
        {
            Debug.Assert(v2 != v);
            v2.x = v.x*A.ex.x + v.y*A.ey.x + v.z*A.ez.x;
            v2.y = v.x*A.ex.y + v.y*A.ey.y + v.z*A.ez.y;
            v2.z = v.x*A.ex.z + v.y*A.ey.z + v.z*A.ez.z;
        }

        /**
   * Solve A * x = b, where b is a column vector. This is more efficient than computing the inverse
   * in one-shot cases.
   * 
   * @param b
   * @return
   */

        public Vec2 solve22(Vec2 b)
        {
            Vec2 x = new Vec2();
            solve22ToOut(b, ref x);
            return x;
        }

        /**
   * Solve A * x = b, where b is a column vector. This is more efficient than computing the inverse
   * in one-shot cases.
   * 
   * @param b
   * @return
   */

        public void solve22ToOut(Vec2 b, ref Vec2 vOut)
        {
            float a11 = ex.x, a12 = ey.x, a21 = ex.y, a22 = ey.y;
            float det = a11*a22 - a12*a21;
            if (det != 0.0f)
            {
                det = 1.0f/det;
            }
            vOut.x = det*(a22*b.x - a12*b.y);
            vOut.y = det*(a11*b.y - a21*b.x);
        }

        // djm pooling from below
        /**
   * Solve A * x = b, where b is a column vector. This is more efficient than computing the inverse
   * in one-shot cases.
   * 
   * @param b
   * @return
   */

        public Vec3 solve33(Vec3 b)
        {
            Vec3 x = new Vec3();
            solve33ToOut(b, ref x);
            return x;
        }

        /**
   * Solve A * x = b, where b is a column vector. This is more efficient than computing the inverse
   * in one-shot cases.
   * 
   * @param b
   * @param ref the result
   */

        public void solve33ToOut(Vec3 b, ref Vec3 vOut)
        {
            Debug.Assert(b != vOut);
            Vec3.crossToOutUnsafe(ey, ez, ref vOut);
            float det = Vec3.dot(ex, vOut);
            if (det != 0.0f)
            {
                det = 1.0f/det;
            }
            Vec3.crossToOutUnsafe(ey, ez, ref vOut);
            float x = det*Vec3.dot(b, vOut);
            Vec3.crossToOutUnsafe(b, ez, ref vOut);
            float y = det*Vec3.dot(ex, vOut);
            Vec3.crossToOutUnsafe(ey, b, ref vOut);
            float z = det*Vec3.dot(ex, vOut);
            vOut.x = x;
            vOut.y = y;
            vOut.z = z;
        }

        public void getInverse22(Mat33 M)
        {
            float a = ex.x, b = ey.x, c = ex.y, d = ey.y;
            float det = a*d - b*c;
            if (det != 0.0f)
            {
                det = 1.0f/det;
            }

            M.ex.x = det*d;
            M.ey.x = -det*b;
            M.ex.z = 0.0f;
            M.ex.y = -det*c;
            M.ey.y = det*a;
            M.ey.z = 0.0f;
            M.ez.x = 0.0f;
            M.ez.y = 0.0f;
            M.ez.z = 0.0f;
        }

        // / Returns the zero matrix if singular.
        public void getSymInverse33(Mat33 M)
        {
            float bx = ey.y*ez.z - ey.z*ez.y;
            float by = ey.z*ez.x - ey.x*ez.z;
            float bz = ey.x*ez.y - ey.y*ez.x;
            float det = ex.x*bx + ex.y*by + ex.z*bz;
            if (det != 0.0f)
            {
                det = 1.0f/det;
            }

            float a11 = ex.x, a12 = ey.x, a13 = ez.x;
            float a22 = ey.y, a23 = ez.y;
            float a33 = ez.z;

            M.ex.x = det*(a22*a33 - a23*a23);
            M.ex.y = det*(a13*a23 - a12*a33);
            M.ex.z = det*(a12*a23 - a13*a22);

            M.ey.x = M.ex.y;
            M.ey.y = det*(a11*a33 - a13*a13);
            M.ey.z = det*(a13*a12 - a11*a23);

            M.ez.x = M.ex.z;
            M.ez.y = M.ey.z;
            M.ez.z = det*(a11*a22 - a12*a12);
        }


        public static void setScaleTransform(float scale, ref Mat33 m)
        {
            m.ex.x = scale;
            m.ey.y = scale;
        }

    }
}
