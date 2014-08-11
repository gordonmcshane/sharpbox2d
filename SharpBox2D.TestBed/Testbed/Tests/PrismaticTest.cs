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
 * Created at 6:00:03 AM Jan 12, 2011
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

    public class PrismaticTest : TestbedTest
    {
        private static long JOINT_TAG = 1;
        private PrismaticJoint m_joint;


        public override long getTag(Joint joint)
        {
            if (joint == m_joint)
                return JOINT_TAG;
            return base.getTag(joint);
        }


        public override void processJoint(Joint joint, long tag)
        {
            if (tag == JOINT_TAG)
            {
                m_joint = (PrismaticJoint) joint;
            }
            else
            {
                base.processJoint(joint, tag);
            }
        }


        public override bool isSaveLoadEnabled()
        {
            return true;
        }


        public override void initTest(bool deserialized)
        {
            if (deserialized)
            {
                return;
            }
            Body ground = null;
            {
                BodyDef bd = new BodyDef();
                ground = getWorld().createBody(bd);

                EdgeShape shape = new EdgeShape();
                shape.set(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
                ground.createFixture(shape, 0.0f);
            }

            {
                PolygonShape shape = new PolygonShape();
                shape.setAsBox(2.0f, 0.5f);

                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                bd.position.set(-10.0f, 10.0f);
                bd.angle = 0.5f*MathUtils.PI;
                bd.allowSleep = false;
                Body body = getWorld().createBody(bd);
                body.createFixture(shape, 5.0f);

                PrismaticJointDef pjd = new PrismaticJointDef();

                // Bouncy limit
                Vec2 axis = new Vec2(2.0f, 1.0f);
                axis.normalize();
                pjd.initialize(ground, body, new Vec2(0.0f, 0.0f), axis);

                // Non-bouncy limit
                // pjd.Initialize(ground, body, Vec2(-10.0f, 10.0f), Vec2(1.0f, 0.0f));

                pjd.motorSpeed = 10.0f;
                pjd.maxMotorForce = 10000.0f;
                pjd.enableMotor = true;
                pjd.lowerTranslation = 0.0f;
                pjd.upperTranslation = 20.0f;
                pjd.enableLimit = true;

                m_joint = (PrismaticJoint) getWorld().createJoint(pjd);
            }
        }


        public override void step(TestbedSettings settings)
        {
            base.step(settings);
            addTextLine("Keys: (l) limits, (m) motors, (s) speed");
            float force = m_joint.getMotorForce(1);
            addTextLine("Motor Force = " + force);
        }


        public override void keyPressed(char argKeyChar, int argKeyCode)
        {

            switch (argKeyChar)
            {
                case 'l':
                    m_joint.enableLimit(!m_joint.isLimitEnabled());
                    getModel().getKeys()['l'] = false;
                    break;
                case 'm':
                    m_joint.enableMotor(!m_joint.isMotorEnabled());
                    getModel().getKeys()['m'] = false;
                    break;
                case 's':
                    m_joint.setMotorSpeed(-m_joint.getMotorSpeed());
                    getModel().getKeys()['s'] = false;
                    break;
            }
        }


        public override string getTestName()
        {
            return "Prismatic";
        }
    }
}
