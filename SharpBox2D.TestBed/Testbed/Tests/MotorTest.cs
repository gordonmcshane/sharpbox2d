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

    public class MotorTest : TestbedTest
    {
        private MotorJoint m_joint;
        private float m_time;
        private bool m_go;


        public override void initTest(bool deserialized)
        {
            {
                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-20.0f, 0.0f), new Vec2(20.0f, 0.0f));
                FixtureDef fd = new FixtureDef();
                fd.shape = shape;
                getGroundBody().createFixture(fd);
            }

            // Define motorized body
            {
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.position.set(0.0f, 8.0f);
                Body body = getWorld().createBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(2.0f, 0.5f);

                FixtureDef fd = new FixtureDef();
                fd.shape = shape;
                fd.friction = 0.6f;
                fd.density = 2.0f;
                body.createFixture(fd);

                MotorJointDef mjd = new MotorJointDef();
                mjd.initialize(getGroundBody(), body);
                mjd.maxForce = 1000.0f;
                mjd.maxTorque = 1000.0f;
                m_joint = (MotorJoint) m_world.createJoint(mjd);
            }

            m_go = false;
            m_time = 0.0f;
        }


        public override void keyPressed(char keyCar, int keyCode)
        {
            base.keyPressed(keyCar, keyCode);

            switch (keyCar)
            {
                case 's':
                    m_go = !m_go;
                    break;
            }
        }

        // pooling
        private Vec2 linearOffset = new Vec2();
        private Color4f color = new Color4f(0.9f, 0.9f, 0.9f);


        public override void step(TestbedSettings settings)
        {
            float hz = settings.getSetting(TestbedSettings.Hz).value;
            if (m_go && hz > 0.0f)
            {
                m_time += 1.0f/hz;
            }

            linearOffset.x = 6.0f*MathUtils.sin(2.0f*m_time);
            linearOffset.y = 8.0f + 4.0f*MathUtils.sin(1.0f*m_time);

            float angularOffset = 4.0f*m_time;

            m_joint.setLinearOffset(linearOffset);
            m_joint.setAngularOffset(angularOffset);

            getDebugDraw().drawPoint(linearOffset, 4.0f, color);
            base.step(settings);
            addTextLine("Keys: (s) pause");
        }


        public override string getTestName()
        {
            return "Motor Joint";
        }
    }
}
