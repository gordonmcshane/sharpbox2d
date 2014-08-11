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
 * Created at 3:29:28 AM Jul 17, 2010
 */
using System;
using System.Collections.Generic;
using SharpBox2D.Common;
using SharpBox2D.TestBed.Profile;

namespace SharpBox2D.TestBed.Pooling
{

    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
/**
 * Sun just HAD to make {@link Color} immutable, so now I have to make another
 * stupid pool and now I'm all hot and bothered.  Also, this pool isn't thread safe!
 * @author Daniel Murphy
 */

    public class ColorPool 
    {

        private Dictionary<ColorKey, Color> colorMap = new Dictionary<ColorKey, Color>();

        private ColorKey queryKey = new ColorKey();

        public Color getColor(float r, float g, float b, float alpha)
        {

            queryKey.set(r, g, b, alpha);
            if (colorMap.ContainsKey(queryKey))
            {
                return colorMap[queryKey];
            }
            else
            {
                Color c = new Color(r, g, b, alpha);
                ColorKey ck = new ColorKey();
                ck.set(r, g, b, alpha);
                colorMap.Add(ck, c);
                return c;
            }
        }

        public Color getColor(float r, float g, float b)
        {
            return getColor(r, g, b, 1);
        }
    }

    internal class ColorKey : IEquatable<ColorKey>
    {
        public bool Equals(ColorKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b) && a.Equals(other.a);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ColorKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = r.GetHashCode();
                hashCode = (hashCode*397) ^ g.GetHashCode();
                hashCode = (hashCode*397) ^ b.GetHashCode();
                hashCode = (hashCode*397) ^ a.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ColorKey left, ColorKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ColorKey left, ColorKey right)
        {
            return !Equals(left, right);
        }

        private float r, g, b, a;

        public void set(float argR, float argG, float argB)
        {
            set(argR, argG, argB, 1);
        }

        public void set(float argR, float argG, float argB, float argA)
        {
            r = argR;
            g = argG;
            b = argB;
            a = argA;
        }


    }
}
