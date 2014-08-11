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


    public class EdgeTest : TestbedTest
    {


        public override void initTest(bool deserialized)
        {
            {
                Body ground = getGroundBody();

                Vec2 v1 = new Vec2(-10.0f, 0.0f), v2 = new Vec2(-7.0f, -2.0f), v3 = new Vec2(-4.0f, 0.0f);
                Vec2 v4 = new Vec2(0.0f, 0.0f),
                    v5 = new Vec2(4.0f, 0.0f),
                    v6 = new Vec2(7.0f, 2.0f),
                    v7 =
                        new Vec2(10.0f, 0.0f);

                EdgeShape shape = new EdgeShape();

                shape.set(v1, v2);
                shape.m_hasVertex3 = true;
                shape.m_vertex3.set(v3);
                ground.createFixture(shape, 0.0f);

                shape.set(v2, v3);
                shape.m_hasVertex0 = true;
                shape.m_hasVertex3 = true;
                shape.m_vertex0.set(v1);
                shape.m_vertex3.set(v4);
                ground.createFixture(shape, 0.0f);

                shape.set(v3, v4);
                shape.m_hasVertex0 = true;
                shape.m_hasVertex3 = true;
                shape.m_vertex0.set(v2);
                shape.m_vertex3.set(v5);
                ground.createFixture(shape, 0.0f);

                shape.set(v4, v5);
                shape.m_hasVertex0 = true;
                shape.m_hasVertex3 = true;
                shape.m_vertex0.set(v3);
                shape.m_vertex3.set(v6);
                ground.createFixture(shape, 0.0f);

                shape.set(v5, v6);
                shape.m_hasVertex0 = true;
                shape.m_hasVertex3 = true;
                shape.m_vertex0.set(v4);
                shape.m_vertex3.set(v7);
                ground.createFixture(shape, 0.0f);

                shape.set(v6, v7);
                shape.m_hasVertex0 = true;
                shape.m_vertex0.set(v5);
                ground.createFixture(shape, 0.0f);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.position.set(-0.5f, 0.6f);
                bd.allowSleep = false;
                Body body = m_world.createBody(bd);

                CircleShape shape = new CircleShape();
                shape.m_radius = 0.5f;

                body.createFixture(shape, 1.0f);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.position.set(1.0f, 0.6f);
                bd.allowSleep = false;
                Body body = m_world.createBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.5f, 0.5f);

                body.createFixture(shape, 1.0f);
            }
        }


        public override string getTestName()
        {
            return "Edge Test";
        }

    }
}
