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
 *.created at 3:31:07 PM Jan 14, 2011
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

    public class EdgeShapes : TestbedTest
    {

        private const int e_maxBodies = 256;
        private int m_bodyIndex;
        private Body[] m_bodies = new Body[e_maxBodies];
        private PolygonShape[] m_polygons = new PolygonShape[4];
        private CircleShape m_circle;

        private float m_angle;


        public override void initTest(bool argDeserialized)
        {
            for (int i = 0; i < m_bodies.Length; i++)
            {
                m_bodies[i] = null;
            }
            // Ground body
            {
                BodyDef bd = new BodyDef();
                Body ground = getWorld().createBody(bd);

                float x1 = -20.0f;
                float y1 = 2.0f*MathUtils.cos(x1/10.0f*MathUtils.PI);
                for (int i = 0; i < 80; ++i)
                {
                    float x2 = x1 + 0.5f;
                    float y2 = 2.0f*MathUtils.cos(x2/10.0f*MathUtils.PI);

                    EdgeShape shape = new EdgeShape();
                    shape.set(new Vec2(x1, y1), new Vec2(x2, y2));
                    ground.createFixture(shape, 0.0f);

                    x1 = x2;
                    y1 = y2;
                }
            }

            {
                Vec2[] vertices = new Vec2[3];
                vertices[0] = new Vec2(-0.5f, 0.0f);
                vertices[1] = new Vec2(0.5f, 0.0f);
                vertices[2] = new Vec2(0.0f, 1.5f);
                m_polygons[0] = new PolygonShape();
                m_polygons[0].set(vertices, 3);
            }

            {
                Vec2[] vertices = new Vec2[3];
                vertices[0] = new Vec2(-0.1f, 0.0f);
                vertices[1] = new Vec2(0.1f, 0.0f);
                vertices[2] = new Vec2(0.0f, 1.5f);
                m_polygons[1] = new PolygonShape();
                m_polygons[1].set(vertices, 3);
            }

            {
                float w = 1.0f;
                float b = w/(2.0f + MathUtils.sqrt(2.0f));
                float s = MathUtils.sqrt(2.0f)*b;

                Vec2[] vertices = new Vec2[8];
                vertices[0] = new Vec2(0.5f*s, 0.0f);
                vertices[1] = new Vec2(0.5f*w, b);
                vertices[2] = new Vec2(0.5f*w, b + s);
                vertices[3] = new Vec2(0.5f*s, w);
                vertices[4] = new Vec2(-0.5f*s, w);
                vertices[5] = new Vec2(-0.5f*w, b + s);
                vertices[6] = new Vec2(-0.5f*w, b);
                vertices[7] = new Vec2(-0.5f*s, 0.0f);

                m_polygons[2] = new PolygonShape();
                m_polygons[2].set(vertices, 8);
            }

            {
                m_polygons[3] = new PolygonShape();
                m_polygons[3].setAsBox(0.5f, 0.5f);
            }

            {
                m_circle = new CircleShape();
                m_circle.m_radius = 0.5f;
            }

            m_bodyIndex = 0;
            m_angle = 0.0f;
        }

        private void Create(int index)
        {
            if (m_bodies[m_bodyIndex] != null)
            {
                getWorld().destroyBody(m_bodies[m_bodyIndex]);
                m_bodies[m_bodyIndex] = null;
            }

            BodyDef bd = new BodyDef();

            float x = MathUtils.randomFloat(-10.0f, 10.0f);
            float y = MathUtils.randomFloat(10.0f, 20.0f);
            bd.position.set(x, y);
            bd.angle = MathUtils.randomFloat(-MathUtils.PI, MathUtils.PI);
            bd.type = BodyType.DYNAMIC;

            if (index == 4)
            {
                bd.angularDamping = 0.02f;
            }

            m_bodies[m_bodyIndex] = getWorld().createBody(bd);

            if (index < 4)
            {
                FixtureDef fd = new FixtureDef();
                fd.shape = m_polygons[index];
                fd.friction = 0.3f;
                fd.density = 20.0f;
                m_bodies[m_bodyIndex].createFixture(fd);
            }
            else
            {
                FixtureDef fd = new FixtureDef();
                fd.shape = m_circle;
                fd.friction = 0.3f;
                fd.density = 20.0f;
                m_bodies[m_bodyIndex].createFixture(fd);
            }

            m_bodyIndex = (m_bodyIndex + 1)%e_maxBodies;
        }

        private void DestroyBody()
        {
            for (int i = 0; i < e_maxBodies; ++i)
            {
                if (m_bodies[i] != null)
                {
                    getWorld().destroyBody(m_bodies[i]);
                    m_bodies[i] = null;
                    return;
                }
            }
        }


        public override void keyPressed(char argKeyChar, int argKeyCode)
        {
            switch (argKeyChar)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                    Create(argKeyChar - '1');
                    break;

                case 'd':
                    DestroyBody();
                    break;
            }
        }

        private EdgeShapesCallback callback = new EdgeShapesCallback();


        public override void step(TestbedSettings settings)
        {

            bool advanceRay = settings.pause == false || settings.singleStep;

            base.step(settings);
            addTextLine("Press 1-5 to drop stuff");

            float L = 25.0f;
            Vec2 point1 = new Vec2(0.0f, 10.0f);
            Vec2 d = new Vec2(L*MathUtils.cos(m_angle), -L*MathUtils.abs(MathUtils.sin(m_angle)));
            Vec2 point2 = point1.add(d);


            callback.m_fixture = null;
            getWorld().raycast(callback, point1, point2);

            if (callback.m_fixture != null)
            {
                getDebugDraw().drawPoint(callback.m_point, 5.0f, new Color4f(0.4f, 0.9f, 0.4f));

                getDebugDraw().drawSegment(point1, callback.m_point, new Color4f(0.8f, 0.8f, 0.8f));
                callback.m_normal.mul(.5f);
                callback.m_normal.addLocal(callback.m_point);
                Vec2 head = callback.m_normal;
                getDebugDraw().drawSegment(callback.m_point, head, new Color4f(0.9f, 0.9f, 0.4f));
            }
            else
            {
                getDebugDraw().drawSegment(point1, point2, new Color4f(0.8f, 0.8f, 0.8f));
            }

            if (advanceRay)
            {
                m_angle += 0.25f*MathUtils.PI/180.0f;
            }
        }


        public override string getTestName()
        {
            return "Edge Shapes";
        }

    }


    internal class EdgeShapesCallback : RayCastCallback
    {
        internal EdgeShapesCallback()
        {
            m_fixture = null;
        }

        public float reportFixture(Fixture fixture, Vec2 point, Vec2 normal, float fraction)
        {
            m_fixture = fixture;
            m_point = point;
            m_normal = normal;

            return fraction;
        }

        internal Fixture m_fixture;
        internal Vec2 m_point;
        internal Vec2 m_normal;
    }
}