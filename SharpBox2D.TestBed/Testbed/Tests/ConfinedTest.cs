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

    public class ConfinedTest : TestbedTest
    {

        private int e_columnCount = 0;
        private int e_rowCount = 0;


        public override bool isSaveLoadEnabled()
        {
            return true;
        }


        public override string getTestName()
        {
            return "Confined";
        }


        public override void initTest(bool argDeserialized)
        {
            if (argDeserialized)
            {
                return;
            }
            {
                BodyDef bd = new BodyDef();
                Body ground = getWorld().createBody(bd);

                EdgeShape shape = new EdgeShape();

                // Floor
                shape.set(new Vec2(-10.0f, 0.0f), new Vec2(10.0f, 0.0f));
                ground.createFixture(shape, 0.0f);

                // Left wall
                shape.set(new Vec2(-10.0f, 0.0f), new Vec2(-10.0f, 20.0f));
                ground.createFixture(shape, 0.0f);

                // Right wall
                shape.set(new Vec2(10.0f, 0.0f), new Vec2(10.0f, 20.0f));
                ground.createFixture(shape, 0.0f);

                // Roof
                shape.set(new Vec2(-10.0f, 20.0f), new Vec2(10.0f, 20.0f));
                ground.createFixture(shape, 0.0f);
            }

            float radius = 0.5f;
            CircleShape shape2 = new CircleShape();
            shape2.m_p.setZero();
            shape2.m_radius = radius;

            FixtureDef fd = new FixtureDef();
            fd.shape = shape2;
            fd.density = 1.0f;
            fd.friction = 0.1f;

            for (int j = 0; j < e_columnCount; ++j)
            {
                for (int i = 0; i < e_rowCount; ++i)
                {
                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(-10.0f + (2.1f*j + 1.0f + 0.01f*i)*radius, (2.0f*i + 1.0f)*radius);
                    Body body = getWorld().createBody(bd);

                    body.createFixture(fd);
                }
            }

            getWorld().setGravity(new Vec2(0.0f, 0.0f));
        }

        private Random _random = new Random();

        public void createCircle()
        {
            float radius = 2.0f;
            CircleShape shape = new CircleShape();
            shape.m_p.setZero();
            shape.m_radius = radius;

            FixtureDef fd = new FixtureDef();
            fd.shape = shape;
            fd.density = 1.0f;
            fd.friction = 0.0f;

            Vec2 p = new Vec2((float) _random.NextDouble(), 3.0f + (float) _random.NextDouble());
            BodyDef bd = new BodyDef();
            bd.type = BodyType.DYNAMIC;
            bd.position = p;
            //bd.allowSleep = false;
            Body body = getWorld().createBody(bd);

            body.createFixture(fd);
        }


        public override void step(TestbedSettings settings)
        {

            base.step(settings);

            for (Body b = getWorld().getBodyList(); b != null; b = b.getNext())
            {
                if (b.getType() != BodyType.DYNAMIC)
                {
                    continue;
                }

                Vec2 p = b.getPosition();
                if (p.x <= -10.0f || 10.0f <= p.x || p.y <= 0.0f || 20.0f <= p.y)
                {
                    p.x += 0.0f;
                }
            }

            addTextLine("Press 'c' to create a circle");
        }


        public override void keyPressed(char argKeyChar, int argKeyCode)
        {
            switch (argKeyChar)
            {
                case 'c':
                    createCircle();
                    break;
            }
        }
    }
}
