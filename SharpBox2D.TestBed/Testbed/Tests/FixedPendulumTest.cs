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


    public class FixedPendulumTest : TestbedTest
    {
        private bool switchBodiesInJoint;

        public FixedPendulumTest(bool switchBodiesInJoint)
        {
            this.switchBodiesInJoint = switchBodiesInJoint;
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
            Body pendulum;
            Body ground;

            {
                CircleShape circleShape = new CircleShape();
                circleShape.m_radius = 1;
                Shape shape = circleShape;

                BodyDef bodyDef = new BodyDef();
                bodyDef.type = BodyType.DYNAMIC;
                bodyDef.position.set(-5, 0);
                bodyDef.allowSleep = false;
                pendulum = getWorld().createBody(bodyDef);
                pendulum.createFixture(shape, 1);
            }

            {
                BodyDef bodyDef = new BodyDef();
                bodyDef.type = BodyType.STATIC;
                ground = getWorld().createBody(bodyDef);
            }

            RevoluteJointDef jointDef = new RevoluteJointDef();

            if (switchBodiesInJoint)
                jointDef.initialize(pendulum, ground, new Vec2(0, 0));
            else
                jointDef.initialize(ground, pendulum, new Vec2(0, 0));

            pendulum.applyAngularImpulse(10000);

            getWorld().createJoint(jointDef);
        }


        public override string getTestName()
        {
            return "Fixed Pendulum " + (switchBodiesInJoint ? "1" : "0");
        }
    }
}
