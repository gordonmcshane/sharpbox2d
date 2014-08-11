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
 * Created at 8:02:54 PM Jan 23, 2011
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

    public class CircleStress : TestbedTest
    {

        private static long JOINT_TAG = 1;

        private RevoluteJoint joint;


        public override long getTag(Joint argJoint)
        {
            if (argJoint == joint)
            {
                return JOINT_TAG;
            }
            return default(long);
        }


        public override void processJoint(Joint argJoint, long argTag)
        {
            if (argTag == JOINT_TAG)
            {
                joint = (RevoluteJoint) argJoint;
            }
        }


        public override bool isSaveLoadEnabled()
        {
            return true;
        }


        public override Vec2 getDefaultCameraPos()
        {
            return new Vec2(0, 20);
        }


        public override float getDefaultCameraScale()
        {
            return 5;
        }


        public override void initTest(bool argDeserialized)
        {
            if (argDeserialized)
            {
                return;
            }

            Body leftWall = null;
            Body rightWall = null;
            {
                // Ground
                PolygonShape sd = new PolygonShape();
                sd.setAsBox(50.0f, 10.0f);
                BodyDef bd = new BodyDef();
                bd.type = BodyType.STATIC;
                bd.position = new Vec2(0.0f, -10.0f);
                Body b = getWorld().createBody(bd);
                FixtureDef fd = new FixtureDef();
                fd.shape = sd;
                fd.friction = 1.0f;
                b.createFixture(fd);

                // Walls
                sd.setAsBox(3.0f, 50.0f);
                bd = new BodyDef();
                bd.position = new Vec2(45.0f, 25.0f);
                rightWall = getWorld().createBody(bd);
                rightWall.createFixture(sd, 0);
                bd.position = new Vec2(-45.0f, 25.0f);
                leftWall = getWorld().createBody(bd);
                leftWall.createFixture(sd, 0);

                // Corners
                bd = new BodyDef();
                sd.setAsBox(20.0f, 3.0f);
                bd.angle = (float) (-System.Math.PI/4.0);
                bd.position = new Vec2(-35f, 8.0f);
                Body myBod = getWorld().createBody(bd);
                myBod.createFixture(sd, 0);
                bd.angle = (float) (System.Math.PI/4.0);
                bd.position = new Vec2(35f, 8.0f);
                myBod = getWorld().createBody(bd);
                myBod.createFixture(sd, 0);

                // top
                sd.setAsBox(50.0f, 10.0f);
                bd.type = BodyType.STATIC;
                bd.angle = 0;
                bd.position = new Vec2(0.0f, 75.0f);
                b = getWorld().createBody(bd);
                fd.shape = sd;
                fd.friction = 1.0f;
                b.createFixture(fd);

            }

            CircleShape cd;
            FixtureDef fd2 = new FixtureDef();

            BodyDef bd2 = new BodyDef();
            bd2.type = BodyType.DYNAMIC;
            int numPieces = 5;
            float radius = 6f;
            bd2.position = new Vec2(0.0f, 10.0f);
            Body body = getWorld().createBody(bd2);
            for (int i = 0; i < numPieces; i++)
            {
                cd = new CircleShape();
                cd.m_radius = 1.2f;
                fd2.shape = cd;
                fd2.density = 25;
                fd2.friction = .1f;
                fd2.restitution = .9f;
                float xPos = radius*(float) System.Math.Cos(2f*System.Math.PI*(i/(float) (numPieces)));
                float yPos = radius*(float) System.Math.Sin(2f*System.Math.PI*(i/(float) (numPieces)));
                cd.m_p.set(xPos, yPos);

                body.createFixture(fd2);
            }

            body.setBullet(false);

            RevoluteJointDef rjd = new RevoluteJointDef();
            rjd.initialize(body, getGroundBody(), body.getPosition());
            rjd.motorSpeed = MathUtils.PI;
            rjd.maxMotorTorque = 1000000.0f;
            rjd.enableMotor = true;
            joint = (RevoluteJoint) getWorld().createJoint(rjd);

            {
                int loadSize = 41;

                for (int j = 0; j < 15; j++)
                {
                    for (int i = 0; i < loadSize; i++)
                    {
                        CircleShape circ = new CircleShape();
                        BodyDef bod = new BodyDef();
                        bod.type = BodyType.DYNAMIC;
                        circ.m_radius = 1.0f + (i%2 == 0 ? 1.0f : -1.0f)*.5f*MathUtils.randomFloat(.5f, 1f);
                        FixtureDef fd3 = new FixtureDef();
                        fd3.shape = circ;
                        fd3.density = circ.m_radius*1.5f;
                        fd3.friction = 0.5f;
                        fd3.restitution = 0.7f;
                        float xPos = -39f + 2*i;
                        float yPos = 50f + j;
                        bod.position = new Vec2(xPos, yPos);
                        Body myBody = getWorld().createBody(bod);
                        myBody.createFixture(fd3);

                    }
                }

            }

            getWorld().setGravity(new Vec2(0, -50));
        }


        public override void keyPressed(char argKeyChar, int argKeyCode)
        {
            switch (argKeyChar)
            {
                case 's':
                    joint.setMotorSpeed(0);
                    break;
                case '1':
                    joint.setMotorSpeed(MathUtils.PI);
                    break;
                case '2':
                    joint.setMotorSpeed(MathUtils.PI*2);
                    break;
                case '3':
                    joint.setMotorSpeed(MathUtils.PI*3);
                    break;
                case '4':
                    joint.setMotorSpeed(MathUtils.PI*6);
                    break;
                case '5':
                    joint.setMotorSpeed(MathUtils.PI*10);
                    break;
            }
        }


        public override void step(TestbedSettings settings)
        {
            // TODO Auto-generated method stub
            base.step(settings);

            addTextLine("Press 's' to stop, and '1' - '5' to change speeds");
        }


        public override string getTestName()
        {
            return "Circle Stress Test";
        }

    }
}
