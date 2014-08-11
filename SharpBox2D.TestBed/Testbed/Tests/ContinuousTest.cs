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
 * Created at 12:32:26 AM Aug 15, 2010
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

    public class ContinuousTest : TestbedTest
    {

        private Body m_body;
        private Fixture currFixture;
        private PolygonShape m_poly;
        private CircleShape m_circle;
        private Shape nextShape = null;
        private bool polygon = false;
        private float m_angularVelocity;


        public override string getTestName()
        {
            return "Continuous";
        }

        public void switchObjects()
        {
            if (polygon)
            {
                nextShape = m_circle;
            }
            else
            {
                nextShape = m_poly;
            }
            polygon = !polygon;
        }

        private Random _random = new Random();

        public override void initTest(bool argDeserialized)
        {
            {
                BodyDef bd = new BodyDef();
                bd.position.set(0.0f, 0.0f);
                Body body = getWorld().createBody(bd);

                EdgeShape shape = new EdgeShape();

                shape.set(new Vec2(-10.0f, 0.0f), new Vec2(10.0f, 0.0f));
                body.createFixture(shape, 0.0f);

                PolygonShape pshape = new PolygonShape();
                pshape.setAsBox(0.2f, 1.0f, new Vec2(0.5f, 1.0f), 0.0f);
                body.createFixture(pshape, 0.0f);
            }
            m_poly = new PolygonShape();
            m_poly.setAsBox(2.0f, 0.1f);
            m_circle = new CircleShape();
            m_circle.m_p.setZero();
            m_circle.m_radius = 0.5f;

            BodyDef bd2 = new BodyDef();
            bd2.type = BodyType.DYNAMIC;
            bd2.position.set(0.0f, 20.0f);

            m_body = getWorld().createBody(bd2);
            currFixture = m_body.createFixture(m_poly, 1.0f);

            m_angularVelocity = (float) _random.NextDouble()*100 - 50;
            m_angularVelocity = 33.468121f;
            m_body.setLinearVelocity(new Vec2(0.0f, -100.0f));
            m_body.setAngularVelocity(m_angularVelocity);

            TimeOfImpact.toiCalls = 0;
            TimeOfImpact.toiIters = 0;
            TimeOfImpact.toiMaxIters = 0;
            TimeOfImpact.toiRootIters = 0;
            TimeOfImpact.toiMaxRootIters = 0;
        }

        public void launch()
        {
            m_body.setTransform(new Vec2(0.0f, 20.0f), 0.0f);
            m_angularVelocity = (float) _random.NextDouble()*100 - 50;
            m_body.setLinearVelocity(new Vec2(0.0f, -100.0f));
            m_body.setAngularVelocity(m_angularVelocity);
        }


        public override void step(TestbedSettings settings)
        {
            if (nextShape != null)
            {
                m_body.destroyFixture(currFixture);
                currFixture = m_body.createFixture(nextShape, 1f);
                nextShape = null;
            }
            // if (stepCount == 12){
            // stepCount += 0;
            // } what is this?

            base.step(settings);

            if (Distance.GJK_CALLS > 0)
            {
                addTextLine(string.Format("gjk calls = {0}, ave gjk iters = {1:F1}, max gjk iters = {2}",
                    Distance.GJK_CALLS, Distance.GJK_ITERS*(1f/Distance.GJK_CALLS), Distance.GJK_MAX_ITERS));
            }

            if (TimeOfImpact.toiCalls > 0)
            {
                int toiCalls = TimeOfImpact.toiCalls;
                int toiIters = TimeOfImpact.toiIters;
                int toiMaxIters = TimeOfImpact.toiMaxIters;
                int toiRootIters = TimeOfImpact.toiRootIters;
                int toiMaxRootIters = TimeOfImpact.toiMaxRootIters;
                addTextLine(string.Format("toi calls = {0}, ave toi iters = %3.1f, max toi iters = {2}",
                    toiCalls, toiIters*1f/toiCalls, toiMaxIters));

                addTextLine(string.Format("ave toi root iters = {0:F1}, max toi root iters = {1}", toiRootIters
                                                                                                   *(1f/toiCalls),
                    toiMaxRootIters));
            }

            addTextLine("Press 'c' to change launch shape");

            if (getStepCount()%60 == 0)
            {
                launch();
            }
        }


        public override void keyPressed(char argKeyChar, int argKeyCode)
        {
            switch (argKeyChar)
            {
                case 'c':
                    switchObjects();
                    break;
            }
        }
    }
}
