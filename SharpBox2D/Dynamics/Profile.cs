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


using System.Collections.Generic;
using SharpBox2D.Common;

namespace SharpBox2D.Dynamics
{
    public class Profile
    {
        private static readonly int LONG_AVG_NUMS = 20;
        private static readonly float LONG_FRACTION = 1f/LONG_AVG_NUMS;
        private static readonly int SHORT_AVG_NUMS = 5;
        private static readonly float SHORT_FRACTION = 1f/SHORT_AVG_NUMS;

        public class ProfileEntry
        {
            private float longAvg;
            private float shortAvg;
            private float min;
            private float max;
            private float _accum;

            public ProfileEntry()
            {
                min = float.MaxValue;
                max = -float.MaxValue;
            }

            public void record(float value)
            {
                longAvg = longAvg*(1 - LONG_FRACTION) + value*LONG_FRACTION;
                shortAvg = shortAvg*(1 - SHORT_FRACTION) + value*SHORT_FRACTION;
                min = MathUtils.min(value, min);
                max = MathUtils.max(value, max);
            }

            public void startAccum()
            {
                _accum = 0;
            }

            public void accum(float value)
            {
                _accum += value;
            }

            public void endAccum()
            {
                record(_accum);
            }

            public override string ToString()
            {
                return string.Format("{0:F2} {1:F2} [{2:F2},{3:F2}]", shortAvg, longAvg, min, max);
            }
        }

        public ProfileEntry step = new ProfileEntry();
        public ProfileEntry stepInit = new ProfileEntry();
        public ProfileEntry collide = new ProfileEntry();
        public ProfileEntry solveParticleSystem = new ProfileEntry();
        public ProfileEntry solve = new ProfileEntry();
        public ProfileEntry solveInit = new ProfileEntry();
        public ProfileEntry solveVelocity = new ProfileEntry();
        public ProfileEntry solvePosition = new ProfileEntry();
        public ProfileEntry broadphase = new ProfileEntry();
        public ProfileEntry solveTOI = new ProfileEntry();

        public void toDebugStrings(List<string> strings)
        {
            strings.Add("Profile:");
            strings.Add(" step: " + step);
            strings.Add("  init: " + stepInit);
            strings.Add("  collide: " + collide);
            strings.Add("  particles: " + solveParticleSystem);
            strings.Add("  solve: " + solve);
            strings.Add("   solveInit: " + solveInit);
            strings.Add("   solveVelocity: " + solveVelocity);
            strings.Add("   solvePosition: " + solvePosition);
            strings.Add("   broadphase: " + broadphase);
            strings.Add("  solveTOI: " + solveTOI);
        }
    }
}