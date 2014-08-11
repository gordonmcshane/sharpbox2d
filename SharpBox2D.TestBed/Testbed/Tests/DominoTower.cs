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
 * Created at 8:41:50 PM Jan 23, 2011
 */
using System;
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

    public class DominoTower : TestbedTest
    {
        private float dwidth = .20f;
        private float dheight = 1.0f;
        private float ddensity; // = 10f;
        private float dfriction = 0.1f;
        private int baseCount = 25;

        public void makeDomino(float x, float y, bool horizontal, World world)
        {

            PolygonShape sd = new PolygonShape();
            sd.setAsBox(.5f*dwidth, .5f*dheight);
            FixtureDef fd = new FixtureDef();
            fd.shape = sd;
            fd.density = ddensity;
            BodyDef bd = new BodyDef();
            bd.type = BodyType.DYNAMIC;
            fd.friction = dfriction;
            fd.restitution = 0.65f;
            bd.position = new Vec2(x, y);
            bd.angle = horizontal ? (float) (System.Math.PI/2.0) : 0f;
            Body myBody = getWorld().createBody(bd);
            myBody.createFixture(fd);
        }


        public override Vec2 getDefaultCameraPos()
        {
            return new Vec2(0, 12);
        }


        public override bool isSaveLoadEnabled()
        {
            return true;
        }


        public override void initTest(bool argDeserialized)
        {
            if (argDeserialized)
            {
                return;
            }

            {
                // Floor
                PolygonShape sd = new PolygonShape();
                sd.setAsBox(50.0f, 10.0f);

                BodyDef bd = new BodyDef();
                bd.position = new Vec2(0.0f, -10.0f);
                getWorld().createBody(bd).createFixture(sd, 0f);
            }

            {
                ddensity = 10f;
                // Make bullet
                PolygonShape sd = new PolygonShape();
                sd.setAsBox(.7f, .7f);
                FixtureDef fd = new FixtureDef();
                fd.density = 35f;
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                fd.shape = sd;
                fd.friction = 0f;
                fd.restitution = 0.85f;
                bd.bullet = true;
                // bd.addShape(sd);
                bd.position = new Vec2(30f, 50f);
                Body b = getWorld().createBody(bd);
                b.createFixture(fd);
                b.setLinearVelocity(new Vec2(-25f, -25f));
                b.setAngularVelocity(6.7f);

                fd.density = 25f;
                bd.position = new Vec2(-30, 25f);
                b = getWorld().createBody(bd);
                b.createFixture(fd);
                b.setLinearVelocity(new Vec2(35f, -10f));
                b.setAngularVelocity(-8.3f);
            }

            {
                float currX;
                // Make base
                for (int i = 0; i < baseCount; ++i)
                {
                    currX = i*1.5f*dheight - (1.5f*dheight*baseCount/2f);
                    makeDomino(currX, dheight/2.0f, false, m_world);
                    makeDomino(currX, dheight + dwidth/2.0f, true, m_world);
                }
                currX = baseCount*1.5f*dheight - (1.5f*dheight*baseCount/2f);
                // Make 'I's
                for (int j = 1; j < baseCount; ++j)
                {
                    if (j > 3)
                        ddensity *= .8f;
                    float currY = dheight*.5f + (dheight + 2f*dwidth)*.99f*j; // y at center of 'I'
                    // structure

                    for (int i = 0; i < baseCount - j; ++i)
                    {
                        currX = i*1.5f*dheight - (1.5f*dheight*(baseCount - j)/2f); // +
                        // parent.random(-.05f,
                        // .05f);
                        ddensity *= 2.5f;
                        if (i == 0)
                        {
                            makeDomino(currX - (1.25f*dheight) + .5f*dwidth, currY - dwidth, false, m_world);
                        }
                        if (i == baseCount - j - 1)
                        {
                            // if (j != 1) //djm: why is this here? it makes it off balance
                            makeDomino(currX + (1.25f*dheight) - .5f*dwidth, currY - dwidth, false, m_world);
                        }
                        ddensity /= 2.5f;
                        makeDomino(currX, currY, false, m_world);
                        makeDomino(currX, currY + .5f*(dwidth + dheight), true, m_world);
                        makeDomino(currX, currY - .5f*(dwidth + dheight), true, m_world);
                    }
                }
            }
        }


        public override string getTestName()
        {
            return "Domino Tower";
        }

    }
}
