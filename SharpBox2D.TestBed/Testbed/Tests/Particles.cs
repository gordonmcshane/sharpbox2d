using System;
using SharpBox2D.Callbacks;
using SharpBox2D.Collision;
using SharpBox2D.Collision.Shapes;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;
using SharpBox2D.Dynamics.Contacts;
using SharpBox2D.Dynamics.Joints;
using SharpBox2D.Particle;
using SharpBox2D.TestBed.Framework;

namespace SharpBox2D.TestBed.Tests
{


    public class Particles : TestbedTest
    {

        public override string getTestName()
        {
            return "Particles";
        }


        public override void initTest(bool deserialized)
        {
            {
                {
                    PolygonShape shape = new PolygonShape();
                    Vec2[] vertices =
                        new Vec2[] {new Vec2(-40, -10), new Vec2(40, -10), new Vec2(40, 0), new Vec2(-40, 0)};
                    shape.set(vertices, 4);
                    getGroundBody().createFixture(shape, 0.0f);
                }

                {
                    PolygonShape shape = new PolygonShape();
                    Vec2[] vertices =
                    {new Vec2(-40, -1), new Vec2(-20, -1), new Vec2(-20, 20), new Vec2(-40, 30)};
                    shape.set(vertices, 4);
                    getGroundBody().createFixture(shape, 0.0f);
                }

                {
                    PolygonShape shape = new PolygonShape();
                    Vec2[] vertices = {new Vec2(20, -1), new Vec2(40, -1), new Vec2(40, 30), new Vec2(20, 20)};
                    shape.set(vertices, 4);
                    getGroundBody().createFixture(shape, 0.0f);
                }
            }

            m_world.setParticleRadius(0.35f);
            m_world.setParticleDamping(0.2f);

            {
                CircleShape shape = new CircleShape();
                shape.m_p.set(0, 30);
                shape.m_radius = 20;
                ParticleGroupDef pd = new ParticleGroupDef();
                pd.flags = ParticleType.b2_waterParticle;
                pd.shape = shape;
                m_world.createParticleGroup(pd);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                Body body = m_world.createBody(bd);
                CircleShape shape = new CircleShape();
                shape.m_p.set(0, 80);
                shape.m_radius = 5;
                body.createFixture(shape, 0.5f);
            }

        }
    }
}