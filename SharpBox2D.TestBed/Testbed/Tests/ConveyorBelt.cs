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


    public class ConveyorBelt : TestbedTest
    {
        private static long platformTag = 98752L;
        private Fixture m_platform;


        public override long getTag(Fixture argFixture)
        {
            if (argFixture == m_platform)
            {
                return platformTag;
            }
            return base.getTag(argFixture);
        }


        public override void processFixture(Fixture argFixture, long argTag)
        {
            if (argTag == platformTag)
            {
                m_platform = argFixture;
                return;
            }
            base.processFixture(argFixture, argTag);
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
            // Ground
            {

                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-20.0f, 0.0f), new Vec2(20.0f, 0.0f));
                getGroundBody().createFixture(shape, 0.0f);
            }

            // Platform
            {
                BodyDef bd = new BodyDef();
                bd.position.set(-5.0f, 5.0f);
                Body body = getWorld().createBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(10.0f, 0.5f);

                FixtureDef fd = new FixtureDef();
                fd.shape = shape;
                fd.friction = 0.8f;
                m_platform = body.createFixture(fd);
            }

            // Boxes
            for (int i = 0; i < 5; ++i)
            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.position.set(-10.0f + 2.0f*i, 7.0f);
                Body body = m_world.createBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.5f, 0.5f);
                body.createFixture(shape, 20.0f);
            }
        }


        public override void preSolve(Contact contact, Manifold oldManifold)
        {
            base.preSolve(contact, oldManifold);

            Fixture fixtureA = contact.getFixtureA();
            Fixture fixtureB = contact.getFixtureB();

            if (fixtureA == m_platform || fixtureB == m_platform)
            {
                contact.setTangentSpeed(5.0f);
            }
        }


        public override string getTestName()
        {
            return "Conveyor Belt";
        }
    }
}
