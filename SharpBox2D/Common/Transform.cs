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

// updated to rev 100

/**
 * A transform contains translation and rotation. It is used to represent the position and
 * orientation of rigid frames.
 */

    public struct Transform : IEquatable<Transform>
    {
        /** The translation caused by the transform */
        public Vec2 p;

        /** A matrix representing a rotation */
        public Rot q;

        /** Initialize as a copy of another transform. */

        public Transform(Transform xf)
        {
            p = xf.p.clone();
            q = xf.q.clone();
        }

        /** Initialize using a position vector and a rotation matrix. */

        public Transform(Vec2 _position, Rot _R)
        {
            p = _position.clone();
            q = _R.clone();
        }

        /** Set this to equal another transform. */

        public Transform set(Transform xf)
        {
            p.set(xf.p);
            q.set(xf.q);
            return this;
        }

        /**
   * Set this based on the position and angle.
   * 
   * @param p
   * @param angle
   */

        public void set(Vec2 p, float angle)
        {
            this.p.set(p);
            q.set(angle);
        }

        /** Set this to the identity transform. */

        public void setIdentity()
        {
            p.setZero();
            q.setIdentity();
        }

        public static Vec2 mul(Transform T, Vec2 v)
        {
            return new Vec2((T.q.c*v.x - T.q.s*v.y) + T.p.x, (T.q.s*v.x + T.q.c*v.y) + T.p.y);
        }

        public static void mulToOut(Transform T, Vec2 v, ref Vec2 v2)
        {
            float tempy = (T.q.s*v.x + T.q.c*v.y) + T.p.y;
            v2.x = (T.q.c*v.x - T.q.s*v.y) + T.p.x;
            v2.y = tempy;
        }

        public static void mulToOutUnsafe(Transform T, Vec2 v, ref Vec2 v2)
        {
            Debug.Assert(v != v2);
            v2.x = (T.q.c*v.x - T.q.s*v.y) + T.p.x;
            v2.y = (T.q.s*v.x + T.q.c*v.y) + T.p.y;
        }

        public static Vec2 mulTrans(Transform T, Vec2 v)
        {
            float px = v.x - T.p.x;
            float py = v.y - T.p.y;
            return new Vec2((T.q.c*px + T.q.s*py), (-T.q.s*px + T.q.c*py));
        }

        public static void mulTransToOut(Transform T, Vec2 v, ref Vec2 v2)
        {
            float px = v.x - T.p.x;
            float py = v.y - T.p.y;
            float tempy = (-T.q.s*px + T.q.c*py);
            v2.x = (T.q.c*px + T.q.s*py);
            v2.y = tempy;
        }

        public static void mulTransToOutUnsafe(Transform T, Vec2 v, ref Vec2 v2)
        {
            Debug.Assert(v != v2);
            float px = v.x - T.p.x;
            float py = v.y - T.p.y;
            v2.x = (T.q.c*px + T.q.s*py);
            v2.y = (-T.q.s*px + T.q.c*py);
        }

        public static Transform mul(Transform A, Transform B)
        {
            Transform C = new Transform();
            Rot.mulUnsafe(A.q, B.q, ref C.q);
            Rot.mulToOutUnsafe(A.q, B.p, ref C.p);
            C.p.addLocal(A.p);
            return C;
        }

        public static void mulToOut(Transform A, Transform B, ref Transform tOut)
        {
            Debug.Assert(tOut != A);
            Rot.mul(A.q, B.q, ref tOut.q);
            Rot.mulToOut(A.q, B.p, ref tOut.p);
            tOut.p.addLocal(A.p);
        }

        public static void mulToOutUnsafe(Transform A, Transform B, ref Transform tOut)
        {
            Debug.Assert(tOut != B);
            Debug.Assert(tOut != A);
            Rot.mulUnsafe(A.q, B.q, ref tOut.q);
            Rot.mulToOutUnsafe(A.q, B.p, ref tOut.p);
            tOut.p.addLocal(A.p);
        }

        private static Vec2 pool = new Vec2();

        public static Transform mulTrans(Transform A, Transform B)
        {
            Transform C = new Transform();
            Rot.mulTransUnsafe(A.q, B.q, ref C.q);
            pool.set(B.p).subLocal(A.p);
            Rot.mulTransUnsafe(A.q, pool, ref C.p);
            return C;
        }

        public static void mulTransToOut(Transform A, Transform B, ref Transform tOut)
        {
            Debug.Assert(tOut != A);
            Rot.mulTrans(A.q, B.q, ref tOut.q);
            pool.set(B.p).subLocal(A.p);
            Rot.mulTrans(A.q, pool, ref tOut.p);
        }

        public static void mulTransToOutUnsafe(Transform A, Transform B, ref Transform tOut)
        {
            Debug.Assert(tOut != A);
            Debug.Assert(tOut != B);
            Rot.mulTransUnsafe(A.q, B.q, ref tOut.q);
            pool.set(B.p).subLocal(A.p);
            Rot.mulTransUnsafe(A.q, pool, ref tOut.p);
        }

        public override string ToString()
        {
            string s = "XForm:\n";
            s += "Position: " + p + "\n";
            s += "R: \n" + q + "\n";
            return s;
        }

        public bool Equals(Transform other)
        {
            return p.Equals(other.p) && q.Equals(other.q);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Transform && Equals((Transform) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (p.GetHashCode()*397) ^ q.GetHashCode();
            }
        }

        public static bool operator ==(Transform left, Transform right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Transform left, Transform right)
        {
            return !left.Equals(right);
        }
    }
}