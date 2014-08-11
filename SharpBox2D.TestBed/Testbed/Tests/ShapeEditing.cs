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
 * Created at 2:04:52 PM Jan 23, 2011
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

    public class ShapeEditing : TestbedTest
    {

        private Body m_body;
        private Fixture m_fixture1;
        private Fixture m_fixture2;


        public override void initTest(bool argDeserialized)
        {
            {
                BodyDef bd = new BodyDef();
                Body ground = getWorld().createBody(bd);

                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
                ground.createFixture(shape, 0.0f);
            }

            BodyDef bd2 = new BodyDef();
            bd2.type = BodyType.DYNAMIC;
            bd2.position.set(0.0f, 10.0f);
            m_body = getWorld().createBody(bd2);

            PolygonShape shape2 = new PolygonShape();
            shape2.setAsBox(4.0f, 4.0f, new Vec2(0.0f, 0.0f), 0.0f);
            m_fixture1 = m_body.createFixture(shape2, 10.0f);

            m_fixture2 = null;
        }


        public override void keyPressed(char key, int argKeyCode)
        {
            switch (key)
            {
                case 'c':
                    if (m_fixture2 == null)
                    {
                        CircleShape shape = new CircleShape();
                        shape.m_radius = 3.0f;
                        shape.m_p.set(0.5f, -4.0f);
                        m_fixture2 = m_body.createFixture(shape, 10.0f);
                        m_body.setAwake(true);
                    }
                    break;

                case 'd':
                    if (m_fixture2 != null)
                    {
                        m_body.destroyFixture(m_fixture2);
                        m_fixture2 = null;
                        m_body.setAwake(true);
                    }
                    break;
            }
        }


        public override void step(TestbedSettings settings)
        {
            base.step(settings);
            addTextLine("Press: (c) create a shape, (d) destroy a shape.");
        }


        public override string getTestName()
        {
            return "Shape Editing";
        }
    }
}
