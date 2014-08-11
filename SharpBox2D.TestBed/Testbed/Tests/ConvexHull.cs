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
using SharpBox2D.Callbacks;
using SharpBox2D.Collision;
using SharpBox2D.Collision.Shapes;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;
using SharpBox2D.Dynamics.Contacts;
using SharpBox2D.Dynamics.Joints;
using SharpBox2D.TestBed.Framework;

namespace SharpBox2D.TestBed.Tests
{


    public class ConvexHull : TestbedTest
    {

        private int e_count = Settings.maxPolygonVertices;

        private bool m_auto = false;
        private Vec2[] m_points = new Vec2[Settings.maxPolygonVertices];
        private int m_count;


        public override void initTest(bool deserialized)
        {
            if (deserialized)
            {
                return;
            }
            generate();
        }

        private void generate()
        {
            Vec2 lowerBound = new Vec2(-8f, -8f);
            Vec2 upperBound = new Vec2(8f, 8f);

            for (int i = 0; i < e_count; i++)
            {
                float x = MathUtils.randomFloat(-8, 8);
                float y = MathUtils.randomFloat(-8, 8);

                Vec2 v = new Vec2(x, y);
                MathUtils.clampToOut(v, lowerBound, upperBound, v);
                m_points[i] = v;
            }
            m_count = e_count;
        }

        public override void keyPressed(char argKeyChar, int argKeyCode)
        {
            if (argKeyChar == 'g')
            {
                generate();
            }
            else if (argKeyChar == 'a')
            {
                m_auto = !m_auto;
            }
        }

        private PolygonShape shape = new PolygonShape();
        private Color4f color = new Color4f(.9f, .9f, .9f);
        private Color4f color2 = new Color4f(.9f, .5f, .5f);
        private object _stepLock = new object();


        public override void step(TestbedSettings settings)
        {

            lock (_stepLock)
            {
                base.step(settings);

                shape.set(m_points, m_count);

                addTextLine("Press g to generate a new random convex hull");

                getDebugDraw().drawPolygon(shape.m_vertices, shape.m_count, color);

                for (int i = 0; i < m_count; ++i)
                {
                    getDebugDraw().drawPoint(m_points[i], 2.0f, color2);
                    getDebugDraw().drawString(m_points[i].add(new Vec2(0.05f, 0.05f)), i + "", Color4f.WHITE);
                }

                Debug.Assert(shape.validate());


                if (m_auto)
                {
                    generate();
                }
            }
        }


        public override string getTestName()
        {
            return "Convex Hull";
        }

    }
}