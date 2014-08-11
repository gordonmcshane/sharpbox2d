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
 * Created at 5:30:15 AM Jan 14, 2011
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

    public class Chain : TestbedTest
    {


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

            Body ground = null;
            {
                BodyDef bd = new BodyDef();
                ground = getWorld().createBody(bd);

                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
                ground.createFixture(shape, 0.0f);
            }

            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.6f, 0.125f);

                FixtureDef fd = new FixtureDef();
                fd.shape = shape;
                fd.density = 20.0f;
                fd.friction = 0.2f;

                RevoluteJointDef jd = new RevoluteJointDef();
                jd.collideConnected = false;

                float y = 25.0f;
                Body prevBody = ground;
                for (int i = 0; i < 30; ++i)
                {
                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(0.5f + i, y);
                    Body body = getWorld().createBody(bd);
                    body.createFixture(fd);

                    Vec2 anchor = new Vec2(i, y);
                    jd.initialize(prevBody, body, anchor);
                    getWorld().createJoint(jd);

                    prevBody = body;
                }
            }
        }


        public override string getTestName()
        {
            return "Chain";
        }
    }
}
