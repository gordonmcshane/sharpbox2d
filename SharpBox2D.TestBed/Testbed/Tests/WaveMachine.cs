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


    public class WaveMachine : TestbedTest
    {

        private RevoluteJoint m_joint;
        private float m_time;


        public override void step(TestbedSettings settings)
        {
            base.step(settings);
            float hz = settings.getSetting(TestbedSettings.Hz).value;
            if (hz > 0)
            {
                m_time += 1/hz;
            }
            m_joint.setMotorSpeed(0.05f*MathUtils.cos(m_time)*MathUtils.PI);
        }


        public override void initTest(bool deserialized)
        {
            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.allowSleep = false;
                bd.position.set(0.0f, 10.0f);
                Body body = m_world.createBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(0.5f, 10.0f, new Vec2(20.0f, 0.0f), 0.0f);
                body.createFixture(shape, 5.0f);
                shape.setAsBox(0.5f, 10.0f, new Vec2(-20.0f, 0.0f), 0.0f);
                body.createFixture(shape, 5.0f);
                shape.setAsBox(20.0f, 0.5f, new Vec2(0.0f, 10.0f), 0.0f);
                body.createFixture(shape, 5.0f);
                shape.setAsBox(20.0f, 0.5f, new Vec2(0.0f, -10.0f), 0.0f);
                body.createFixture(shape, 5.0f);

                RevoluteJointDef jd = new RevoluteJointDef();
                jd.bodyA = getGroundBody();
                jd.bodyB = body;
                jd.localAnchorA.set(0.0f, 10.0f);
                jd.localAnchorB.set(0.0f, 0.0f);
                jd.referenceAngle = 0.0f;
                jd.motorSpeed = 0.05f*MathUtils.PI;
                jd.maxMotorTorque = 1e7f;
                jd.enableMotor = true;
                m_joint = (RevoluteJoint) m_world.createJoint(jd);
            }

            m_world.setParticleRadius(0.15f);
            m_world.setParticleDamping(0.2f);

            {
                ParticleGroupDef pd = new ParticleGroupDef();
                pd.flags = 0;

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(9.0f, 9.0f, new Vec2(0.0f, 10.0f), 0.0f);

                pd.shape = shape;
                m_world.createParticleGroup(pd);
            }

            m_time = 0;
        }


        public override string getTestName()
        {
            return "Wave Machine";
        }
    }
}
