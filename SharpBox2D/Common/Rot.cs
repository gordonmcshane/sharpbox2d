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

/**
 * Represents a rotation
 * 
 * @author Daniel
 */

using System;
using System.Diagnostics;

namespace SharpBox2D.Common
{
    public class Rot : IEquatable<Rot>
    {
        public float s;
        public float c; // sin and cos

        public Rot(float angle) : this()
        {
            set(angle);
        }
        
        public Rot()
        {
            set(0);
        }

        public float getSin()
        {
            return s;
        }

        public override string ToString()
        {
            return "Rot(s:" + s + ", c:" + c + ")";
        }

        public float getCos()
        {
            return c;
        }

        public Rot set(float angle)
        {
            s = MathUtils.sin(angle);
            c = MathUtils.cos(angle);
            return this;
        }

        public Rot set(Rot other)
        {
            s = other.s;
            c = other.c;
            return this;
        }

        public Rot setIdentity()
        {
            s = 0;
            c = 1;
            return this;
        }

        public float getAngle()
        {
            return MathUtils.atan2(s, c);
        }

        public void getXAxis(Vec2 xAxis)
        {
            xAxis.set(c, s);
        }

        public void getYAxis(Vec2 yAxis)
        {
            yAxis.set(-s, c);
        }

        // @Override // annotation omitted for GWT-compatibility
        public Rot clone()
        {
            Rot copy = new Rot();
            copy.s = s;
            copy.c = c;
            return copy;
        }

        public static void mul(Rot q, Rot r, ref Rot rOut)
        {
            float tempc = q.c*r.c - q.s*r.s;
            rOut.s = q.s*r.c + q.c*r.s;
            rOut.c = tempc;
        }

        public static void mulUnsafe(Rot q, Rot r, ref Rot rOut)
        {
            Debug.Assert(r != rOut);
            Debug.Assert(q != rOut);
            // [qc -qs] * [rc -rs] = [qc*rc-qs*rs -qc*rs-qs*rc]
            // [qs qc] [rs rc] [qs*rc+qc*rs -qs*rs+qc*rc]
            // s = qs * rc + qc * rs
            // c = qc * rc - qs * rs
            rOut.s = q.s*r.c + q.c*r.s;
            rOut.c = q.c*r.c - q.s*r.s;
        }

        public static void mulTrans(Rot q, Rot r, ref Rot rOut)
        {
            float tempc = q.c*r.c + q.s*r.s;
            rOut.s = q.c*r.s - q.s*r.c;
            rOut.c = tempc;
        }

        public static void mulTransUnsafe(Rot q, Rot r, ref Rot rOut)
        {
            // [ qc qs] * [rc -rs] = [qc*rc+qs*rs -qc*rs+qs*rc]
            // [-qs qc] [rs rc] [-qs*rc+qc*rs qs*rs+qc*rc]
            // s = qc * rs - qs * rc
            // c = qc * rc + qs * rs
            rOut.s = q.c*r.s - q.s*r.c;
            rOut.c = q.c*r.c + q.s*r.s;
        }

        public static void mulToOut(Rot q, Vec2 v, ref Vec2 v2)
        {
            float tempy = q.s*v.x + q.c*v.y;
            v2.x = q.c*v.x - q.s*v.y;
            v2.y = tempy;
        }

        public static void mulToOutUnsafe(Rot q, Vec2 v, ref Vec2 v2)
        {
            v2.x = q.c*v.x - q.s*v.y;
            v2.y = q.s*v.x + q.c*v.y;
        }

        public static void mulTrans(Rot q, Vec2 v, ref Vec2 v2)
        {
            float tempy = -q.s*v.x + q.c*v.y;
            v2.x = q.c*v.x + q.s*v.y;
            v2.y = tempy;
        }

        public static void mulTransUnsafe(Rot q, Vec2 v, ref Vec2 v2)
        {
            v2.x = q.c*v.x + q.s*v.y;
            v2.y = -q.s*v.x + q.c*v.y;
        }

        public bool Equals(Rot other)
        {
            return s.Equals(other.s) && c.Equals(other.c);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Rot && Equals((Rot) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (s.GetHashCode()*397) ^ c.GetHashCode();
            }
        }

        public static bool operator ==(Rot left, Rot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rot left, Rot right)
        {
            return !left.Equals(right);
        }
    }
}
