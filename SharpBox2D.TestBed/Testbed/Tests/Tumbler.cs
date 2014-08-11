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


    public class Tumbler : TestbedTest
    {
        private static int MAX_NUM = 800;
        private RevoluteJoint m_joint;
        private int m_count;
        private object _stepLock = new object();


        public override void initTest(bool deserialized)
        {
            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.allowSleep = false;
                bd.position.set(0.0f, 10.0f);
                Body body = m_world.createBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.5f, 10.0f, new Vec2(10.0f, 0.0f), 0.0f);
                body.createFixture(shape, 5.0f);
                shape.setAsBox(0.5f, 10.0f, new Vec2(-10.0f, 0.0f), 0.0f);
                body.createFixture(shape, 5.0f);
                shape.setAsBox(10.0f, 0.5f, new Vec2(0.0f, 10.0f), 0.0f);
                body.createFixture(shape, 5.0f);
                shape.setAsBox(10.0f, 0.5f, new Vec2(0.0f, -10.0f), 0.0f);
                body.createFixture(shape, 5.0f);

                RevoluteJointDef jd = new RevoluteJointDef();
                jd.bodyA = getGroundBody();
                jd.bodyB = body;
                jd.localAnchorA.set(0.0f, 10.0f);
                jd.localAnchorB.set(0.0f, 0.0f);
                jd.referenceAngle = 0.0f;
                jd.motorSpeed = 0.05f*MathUtils.PI;
                jd.maxMotorTorque = 1e8f;
                jd.enableMotor = true;
                m_joint = (RevoluteJoint) m_world.createJoint(jd);
            }
            m_count = 0;
        }


        public override void step(TestbedSettings settings)
        {
            lock (_stepLock)
            {
                base.step(settings);

                if (m_count < MAX_NUM)
                {
                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(0.0f, 10.0f);
                    Body body = m_world.createBody(bd);

                    PolygonShape shape = new PolygonShape();
                    shape.setAsBox(0.125f, 0.125f);
                    body.createFixture(shape, 1.0f);

                    ++m_count;
                }
            }
        }


        public override string getTestName()
        {
            return "Tumbler";
        }
    }
}
