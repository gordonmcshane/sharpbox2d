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
    public class BulletTest : TestbedTest
    {

        private Body m_body;
        private Body m_bullet;
        private float m_x;


        public override Vec2 getDefaultCameraPos()
        {
            return new Vec2(0, 6);
        }


        public override float getDefaultCameraScale()
        {
            return 40;
        }


        public override void initTest(bool deserialized)
        {
            {
                BodyDef bd = new BodyDef();
                bd.position.set(0.0f, 0.0f);
                Body body = m_world.createBody(bd);

                EdgeShape edge = new EdgeShape();

                edge.set(new Vec2(-10.0f, 0.0f), new Vec2(10.0f, 0.0f));
                body.createFixture(edge, 0.0f);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.2f, 1.0f, new Vec2(0.5f, 1.0f), 0.0f);
                body.createFixture(shape, 0.0f);
            }

            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.position.set(0.0f, 4.0f);

                PolygonShape box = new PolygonShape();
                box.setAsBox(2.0f, 0.1f);

                m_body = m_world.createBody(bd);
                m_body.createFixture(box, 1.0f);

                box.setAsBox(0.25f, 0.25f);

                // m_x = RandomFloat(-1.0f, 1.0f);
                m_x = -0.06530577f;
                bd.position.set(m_x, 10.0f);
                bd.bullet = true;

                m_bullet = m_world.createBody(bd);
                m_bullet.createFixture(box, 100.0f);

                m_bullet.setLinearVelocity(new Vec2(0.0f, -50.0f));
            }
        }

        public void launch()
        {
            m_body.setTransform(new Vec2(0.0f, 4.0f), 0.0f);
            m_body.setLinearVelocity(new Vec2());
            m_body.setAngularVelocity(0.0f);

            m_x = MathUtils.randomFloat(-1.0f, 1.0f);
            m_bullet.setTransform(new Vec2(m_x, 10.0f), 0.0f);
            m_bullet.setLinearVelocity(new Vec2(0.0f, -50.0f));
            m_bullet.setAngularVelocity(0.0f);

            Distance.GJK_CALLS = 0;
            Distance.GJK_ITERS = 0;
            Distance.GJK_MAX_ITERS = 0;

            TimeOfImpact.toiCalls = 0;
            TimeOfImpact.toiIters = 0;
            TimeOfImpact.toiMaxIters = 0;
            TimeOfImpact.toiRootIters = 0;
            TimeOfImpact.toiMaxRootIters = 0;
        }


        public override void step(TestbedSettings settings)
        {
            base.step(settings);


            if (Distance.GJK_CALLS > 0)
            {
                addTextLine(String.Format("gjk calls = {0}, ave gjk iters = {1:F1}, max gjk iters = {2}",
                    Distance.GJK_CALLS, Distance.GJK_ITERS*1.0/(Distance.GJK_CALLS),
                    Distance.GJK_MAX_ITERS));
            }

            if (TimeOfImpact.toiCalls > 0)
            {
                addTextLine(String.Format("toi calls = {0}, ave toi iters = {1:F1}, max toi iters = {2}",
                    TimeOfImpact.toiCalls, TimeOfImpact.toiIters*1f/(TimeOfImpact.toiCalls),
                    TimeOfImpact.toiMaxRootIters));

                addTextLine(String.Format("ave toi root iters = {0:F1}, max toi root iters = {1}",
                    TimeOfImpact.toiRootIters*1f/(TimeOfImpact.toiCalls), TimeOfImpact.toiMaxRootIters.ToString()));
            }

            if (getStepCount()%60 == 0)
            {
                launch();
            }
        }


        public override string getTestName()
        {
            return "Bullet Test";
        }

    }
}
