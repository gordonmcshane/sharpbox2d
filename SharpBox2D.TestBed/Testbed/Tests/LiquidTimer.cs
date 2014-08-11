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


    public class LiquidTimer : TestbedTest
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
            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(20, 4, new Vec2(0, 36), 0);
                ParticleGroupDef pd = new ParticleGroupDef();
                pd.flags = ParticleType.b2_tensileParticle | ParticleType.b2_viscousParticle;
                pd.shape = shape;
                m_world.createParticleGroup(pd);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-20, 32), new Vec2(-12, 32));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-11, 32), new Vec2(20, 32));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-12, 32), new Vec2(-12, 28));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-11, 32), new Vec2(-11, 28));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-16, 24), new Vec2(8, 20));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(16, 16), new Vec2(-8, 12));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-12, 8), new Vec2(-12, 0));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-4, 8), new Vec2(-4, 0));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(4, 8), new Vec2(4, 0));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(12, 8), new Vec2(12, 0));
                body.createFixture(shape, 0.1f);
            }
        }


        public override string getTestName()
        {
            return "Liquid Timer";
        }
    }
}
