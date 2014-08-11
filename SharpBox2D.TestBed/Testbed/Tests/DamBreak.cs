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


    public class DamBreak : TestbedTest
    {


        public override void initTest(bool deserialized)
        {
            {
                BodyDef bd = new BodyDef();
                Body ground = m_world.createBody(bd);

                ChainShape shape = new ChainShape();
                Vec2[] vertices =
                    new Vec2[] {new Vec2(-20, 0), new Vec2(20, 0), new Vec2(20, 40), new Vec2(-20, 40)};
                shape.createLoop(vertices, 4);
                ground.createFixture(shape, 0.0f);

            }

            m_world.setParticleRadius(0.15f);
            m_world.setParticleDamping(0.2f);
            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(8, 10, new Vec2(-12, 10.1f), 0);
                ParticleGroupDef pd = new ParticleGroupDef();
                pd.shape = shape;
                m_world.createParticleGroup(pd);
            }
        }


        public override string getTestName()
        {
            return "Dam Break";
        }
    }
}