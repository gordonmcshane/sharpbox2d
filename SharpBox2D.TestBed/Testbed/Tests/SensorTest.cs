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
 * Created at 1:25:51 PM Jan 23, 2011
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

    public class SensorTest : TestbedTest
    {

        private class BoolWrapper
        {
            public bool tf;
        }

        private const int _e_count = 7;
        private Fixture m_sensor;
        private Body[] m_bodies = new Body[_e_count];
        private BoolWrapper[] m_touching = new BoolWrapper[_e_count];


        public override void initTest(bool deserialized)
        {

            for (int i = 0; i < m_touching.Length; i++)
            {
                m_touching[i] = new BoolWrapper();
            }

            {
                BodyDef bd = new BodyDef();
                Body ground = getWorld().createBody(bd);

                {
                    EdgeShape shape = new EdgeShape();
                    shape.set(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
                    ground.createFixture(shape, 0.0f);
                }

                {
                    CircleShape shape = new CircleShape();
                    shape.m_radius = 5.0f;
                    shape.m_p.set(0.0f, 10.0f);

                    FixtureDef fd = new FixtureDef();
                    fd.shape = shape;
                    fd._isSensor = true;
                    m_sensor = ground.createFixture(fd);
                }
            }

            {
                CircleShape shape = new CircleShape();
                shape.m_radius = 1.0f;

                for (int i = 0; i < _e_count; ++i)
                {
                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(-10.0f + 3.0f*i, 20.0f);
                    bd.userData = m_touching[i];

                    m_touching[i].tf = false;
                    m_bodies[i] = getWorld().createBody(bd);

                    m_bodies[i].createFixture(shape, 1.0f);
                }
            }
        }

        // Implement contact listener.
        public override void beginContact(Contact contact)
        {
            Fixture fixtureA = contact.getFixtureA();
            Fixture fixtureB = contact.getFixtureB();

            if (fixtureA == m_sensor)
            {
                Object userData = fixtureB.getBody().getUserData();
                if (userData != null)
                {
                    ((BoolWrapper) userData).tf = true;
                }
            }

            if (fixtureB == m_sensor)
            {
                Object userData = fixtureA.getBody().getUserData();
                if (userData != null)
                {
                    ((BoolWrapper) userData).tf = true;
                }
            }
        }

        // Implement contact listener.
        public override void endContact(Contact contact)
        {
            Fixture fixtureA = contact.getFixtureA();
            Fixture fixtureB = contact.getFixtureB();

            if (fixtureA == m_sensor)
            {
                Object userData = fixtureB.getBody().getUserData();
                if (userData != null)
                {
                    ((BoolWrapper) userData).tf = false;
                }
            }

            if (fixtureB == m_sensor)
            {
                Object userData = fixtureA.getBody().getUserData();
                if (userData != null)
                {
                    ((BoolWrapper) userData).tf = false;
                }
            }
        }


        public override void step(TestbedSettings settings)
        {
            // TODO Auto-generated method stub
            base.step(settings);

            // Traverse the contact results. Apply a force on shapes
            // that overlap the sensor.
            for (int i = 0; i < _e_count; ++i)
            {
                if (m_touching[i].tf == false)
                {
                    continue;
                }

                Body body = m_bodies[i];
                Body ground = m_sensor.getBody();

                CircleShape circle = (CircleShape) m_sensor.getShape();
                Vec2 center = ground.getWorldPoint(circle.m_p);

                Vec2 position = body.getPosition();

                Vec2 d = center.sub(position);
                if (d.lengthSquared() < Settings.EPSILON*Settings.EPSILON)
                {
                    continue;
                }

                d.normalize();
                d.mulLocal(100f);
                Vec2 F = d;
                body.applyForce(F, position);
            }
        }


        public override string getTestName()
        {
            return "Sensor Test";
        }
    }
}