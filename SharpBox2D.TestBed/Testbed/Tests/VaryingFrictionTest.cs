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


    public class VaryingFrictionTest : TestbedTest
    {


        public override string getTestName()
        {
            return "Varying Friction";
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
                shape.set(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
                ground.createFixture(shape, 0.0f);
            }

            {
                PolygonShape shape2 = new PolygonShape();
                shape2.setAsBox(13.0f, 0.25f);

                BodyDef bd2 = new BodyDef();
                bd2.position.set(-4.0f, 22.0f);
                bd2.angle = -0.25f;

                Body ground = getWorld().createBody(bd2);
                ground.createFixture(shape2, 0.0f);
            }

            {
                PolygonShape shape3 = new PolygonShape();
                shape3.setAsBox(0.25f, 1.0f);

                BodyDef bd3 = new BodyDef();
                bd3.position.set(10.5f, 19.0f);

                Body ground = getWorld().createBody(bd3);
                ground.createFixture(shape3, 0.0f);
            }

            {
                PolygonShape shape4 = new PolygonShape();
                shape4.setAsBox(13.0f, 0.25f);

                BodyDef bd4 = new BodyDef();
                bd4.position.set(4.0f, 14.0f);
                bd4.angle = 0.25f;

                Body ground = getWorld().createBody(bd4);
                ground.createFixture(shape4, 0.0f);
            }

            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.25f, 1.0f);

                BodyDef bd = new BodyDef();
                bd.position.set(-10.5f, 11.0f);

                Body ground = getWorld().createBody(bd);
                ground.createFixture(shape, 0.0f);
            }

            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(13.0f, 0.25f);

                BodyDef bd = new BodyDef();
                bd.position.set(-4.0f, 6.0f);
                bd.angle = -0.25f;

                Body ground = getWorld().createBody(bd);
                ground.createFixture(shape, 0.0f);
            }

            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.5f, 0.5f);

                FixtureDef fd = new FixtureDef();
                fd.shape = shape;
                fd.density = 25.0f;

                float[] friction = {0.75f, 0.5f, 0.35f, 0.1f, 0.0f};

                for (int i = 0; i < 5; ++i)
                {
                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(-15.0f + 4.0f*i, 28.0f);
                    Body body = getWorld().createBody(bd);

                    fd.friction = friction[i];
                    body.createFixture(fd);
                }
            }
        }

    }
}
