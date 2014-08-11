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


    public class ParticleTypes : TestbedTest
    {

        private Body circle;
        private int flags = ParticleType.b2_tensileParticle;


        public override void initTest(bool deserialized)
        {
            {
                BodyDef bd = new BodyDef();
                Body ground = m_world.createBody(bd);

                {
                    PolygonShape shape = new PolygonShape();
                    Vec2[] vertices =
                        new Vec2[] {new Vec2(-40, -10), new Vec2(40, -10), new Vec2(40, 0), new Vec2(-40, 0)};
                    shape.set(vertices, 4);
                    ground.createFixture(shape, 0.0f);
                }

                {
                    PolygonShape shape = new PolygonShape();
                    Vec2[] vertices =
                        new Vec2[] {new Vec2(-40, -1), new Vec2(-20, -1), new Vec2(-20, 20), new Vec2(-40, 30)};
                    shape.set(vertices, 4);
                    ground.createFixture(shape, 0.0f);
                }

                {
                    PolygonShape shape = new PolygonShape();
                    Vec2[] vertices =
                        new Vec2[] {new Vec2(20, -1), new Vec2(40, -1), new Vec2(40, 30), new Vec2(20, 20)};
                    shape.set(vertices, 4);
                    ground.createFixture(shape, 0.0f);
                }
            }

            m_world.setParticleRadius(0.2f);
            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(20, 10, new Vec2(0, 10), 0);
                ParticleGroupDef pd = new ParticleGroupDef();
                pd.flags = flags;
                pd.shape = shape;
                m_world.createParticleGroup(pd);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.KINEMATIC;
                Body body = m_world.createBody(bd);
                circle = body;
                CircleShape shape = new CircleShape();
                shape.m_p.set(0, 5);
                shape.m_radius = 1;
                body.createFixture(shape, 0.1f);
                body.setLinearVelocity(new Vec2(-6, 0.0f));
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                Body body = m_world.createBody(bd);
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(1, 1, new Vec2(-10, 5), 0);
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                Body body = m_world.createBody(bd);
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(1, 1, new Vec2(10, 5), 0.5f);
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(0, 20), new Vec2(1, 21));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(3, 20), new Vec2(4, 21));
                body.createFixture(shape, 0.1f);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                Body body = m_world.createBody(bd);
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-3, 21), new Vec2(-2, 20));
                body.createFixture(shape, 0.1f);
            }
        }


        public override void step(TestbedSettings settings)
        {
            base.step(settings);

            Vec2 p = circle.getTransform().p;
            Vec2 v = circle.getLinearVelocity();

            if ((p.x < -10.0f && v.x < 0.0f) || (p.x > 10.0f && v.x > 0.0f))
            {
                v.x = -v.x;
                circle.setLinearVelocity(v);
            }
            int[] flagsBuffer = m_world.getParticleFlagsBuffer();
            for (int i = 0; i < m_world.getParticleCount(); i++)
            {
                flagsBuffer[i] = flags;
            }

            addTextLine("'a' Clear");
            addTextLine("'e' Elastic " + ((flags & ParticleType.b2_elasticParticle) != 0));
            addTextLine("'q' Powder  " + ((flags & ParticleType.b2_powderParticle) != 0));
            addTextLine("'t' Tensile " + ((flags & ParticleType.b2_tensileParticle) != 0));
            addTextLine("'v' Viscous " + ((flags & ParticleType.b2_viscousParticle) != 0));
        }


        public override void keyPressed(char keyCar, int keyCode)
        {
            base.keyPressed(keyCar, keyCode);
            int toggle = 0;
            switch (keyCar)
            {
                case 'a':
                    flags = 0;
                    break;
                case 'e':
                    toggle = ParticleType.b2_elasticParticle;
                    break;
                case 'q':
                    toggle = ParticleType.b2_powderParticle;
                    break;
                case 't':
                    toggle = ParticleType.b2_tensileParticle;
                    break;
                case 'v':
                    toggle = ParticleType.b2_viscousParticle;
                    break;
            }
            if (toggle != 0)
            {
                if ((flags & toggle) != 0)
                {
                    flags = flags & ~toggle;
                }
                else
                {
                    flags = flags | toggle;
                }
            }
        }


        public override string getTestName()
        {
            return "ParticleTypes";
        }
    }
}
