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
 * Created at 4:56:29 AM Jan 14, 2011
 */
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


/**
 * @author Daniel Murphy
 */

    public class VerticalStack : TestbedTest
    {
        private static long BULLET_TAG = 1;

        public static int e_columnCount = 5;
        public static int e_rowCount = 15;

        private Body m_bullet;


        public override long getTag(Body argBody)
        {
            if (argBody == m_bullet)
            {
                return BULLET_TAG;
            }
            return base.getTag(argBody);
        }


        public override void processBody(Body argBody, long argTag)
        {
            if (argTag == BULLET_TAG)
            {
                m_bullet = argBody;
                return;
            }
            base.processBody(argBody, argTag);
        }


        public override bool isSaveLoadEnabled()
        {
            return true;
        }


        public override void initTest(bool deserialized)
        {
            if (deserialized)
            {
                return;
            }

            {
                BodyDef bd = new BodyDef();
                Body ground = getWorld().createBody(bd);

                EdgeShape shape = new EdgeShape();
                // shape.setAsBox(40, 10, new Vec2(0,-10), 0);
                shape.set(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
                ground.createFixture(shape, 0.0f);

                shape.set(new Vec2(20.0f, 0.0f), new Vec2(20.0f, 20.0f));
                ground.createFixture(shape, 0.0f);
            }

            float[] xs = new float[] {0.0f, -10.0f, -5.0f, 5.0f, 10.0f};

            for (int j = 0; j < e_columnCount; ++j)
            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.5f, 0.5f);

                FixtureDef fd = new FixtureDef();
                fd.shape = shape;
                fd.density = 1.0f;
                fd.friction = 0.3f;

                for (int i = 0; i < e_rowCount; ++i)
                {
                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;

                    int n = j*e_rowCount + i;
                    Debug.Assert(n < e_rowCount*e_columnCount);

                    float x = 0.0f;
                    // float x = RandomFloat(-0.02f, 0.02f);
                    // float x = i % 2 == 0 ? -0.025f : 0.025f;
                    bd.position.set(xs[j] + x, 0.752f + 1.54f*i);
                    Body body = getWorld().createBody(bd);

                    body.createFixture(fd);
                }
            }

            m_bullet = null;
        }


        public override void keyPressed(char argKeyChar, int argKeyCode)
        {
            switch (argKeyChar)
            {
                case ',':
                    if (m_bullet != null)
                    {
                        getWorld().destroyBody(m_bullet);
                        m_bullet = null;
                    }

                {
                    CircleShape shape = new CircleShape();
                    shape.m_radius = 0.25f;

                    FixtureDef fd = new FixtureDef();
                    fd.shape = shape;
                    fd.density = 20.0f;
                    fd.restitution = 0.05f;

                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.bullet = true;
                    bd.position.set(-31.0f, 5.0f);

                    m_bullet = getWorld().createBody(bd);
                    m_bullet.createFixture(fd);

                    m_bullet.setLinearVelocity(new Vec2(400.0f, 0.0f));
                }
                    break;
            }
        }


        public override void step(TestbedSettings settings)
        {
            base.step(settings);
            addTextLine("Press ',' to launch bullet.");
        }


        public override string getTestName()
        {
            return "Vertical Stack";
        }
    }
}
