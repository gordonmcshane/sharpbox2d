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
 * @author Daniel Murphy
 */

    public struct Vec3 : IEquatable<Vec3>
    {
        public float x, y, z;

        public Vec3(float argX, float argY, float argZ)
        {
            x = argX;
            y = argY;
            z = argZ;
        }

        public Vec3(Vec3 copy)
        {
            x = copy.x;
            y = copy.y;
            z = copy.z;
        }

        public Vec3 set(Vec3 vec)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
            return this;
        }

        public Vec3 set(float argX, float argY, float argZ)
        {
            x = argX;
            y = argY;
            z = argZ;
            return this;
        }

        public Vec3 addLocal(Vec3 argVec)
        {
            x += argVec.x;
            y += argVec.y;
            z += argVec.z;
            return this;
        }

        public Vec3 add(Vec3 argVec)
        {
            return new Vec3(x + argVec.x, y + argVec.y, z + argVec.z);
        }

        public Vec3 subLocal(Vec3 argVec)
        {
            x -= argVec.x;
            y -= argVec.y;
            z -= argVec.z;
            return this;
        }

        public Vec3 sub(Vec3 argVec)
        {
            return new Vec3(x - argVec.x, y - argVec.y, z - argVec.z);
        }

        public Vec3 mulLocal(float argScalar)
        {
            x *= argScalar;
            y *= argScalar;
            z *= argScalar;
            return this;
        }

        public Vec3 mul(float argScalar)
        {
            return new Vec3(x*argScalar, y*argScalar, z*argScalar);
        }

        public Vec3 negate()
        {
            return new Vec3(-x, -y, -z);
        }

        public Vec3 negateLocal()
        {
            x = -x;
            y = -y;
            z = -z;
            return this;
        }

        public void setZero()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Vec3 clone()
        {
            return new Vec3(this);
        }

        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }


        public static float dot(Vec3 a, Vec3 b)
        {
            return a.x*b.x + a.y*b.y + a.z*b.z;
        }

        public static Vec3 cross(Vec3 a, Vec3 b)
        {
            return new Vec3(a.y*b.z - a.z*b.y, a.z*b.x - a.x*b.z, a.x*b.y - a.y*b.x);
        }

        public static void crossToOut(Vec3 a, Vec3 b, ref Vec3 c)
        {
            float tempy = a.z*b.x - a.x*b.z;
            float tempz = a.x*b.y - a.y*b.x;
            c.x = a.y*b.z - a.z*b.y;
            c.y = tempy;
            c.z = tempz;
        }

        public static void crossToOutUnsafe(Vec3 a, Vec3 b, ref Vec3 c)
        {
            Debug.Assert(c != b);
            Debug.Assert(c != a);
            c.x = a.y*b.z - a.z*b.y;
            c.y = a.z*b.x - a.x*b.z;
            c.z = a.x*b.y - a.y*b.x;
        }

        public bool Equals(Vec3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Vec3 && Equals((Vec3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x.GetHashCode();
                hashCode = (hashCode*397) ^ y.GetHashCode();
                hashCode = (hashCode*397) ^ z.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Vec3 left, Vec3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !left.Equals(right);
        }
    }
}
