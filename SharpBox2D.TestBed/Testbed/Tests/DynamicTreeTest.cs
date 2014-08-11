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
 * Created at 5:43:20 AM Jan 14, 2011
 */
using System;
using System.Diagnostics;
using SharpBox2D.Callbacks;
using SharpBox2D.Collision;
using SharpBox2D.Collision.Broadphase;
using SharpBox2D.Collision.Shapes;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;
using SharpBox2D.Dynamics.Contacts;
using SharpBox2D.Dynamics.Joints;
using SharpBox2D.Pooling.Arrays;
using SharpBox2D.TestBed.Framework;

namespace SharpBox2D.TestBed.Tests
{


/**
 * @author Daniel Murphy
 */

    public class DynamicTreeTest : TestbedTest, TreeCallback,
        TreeRayCastCallback
    {

        private const int _e_actorCount = 128;
        private float worldExtent;
        private float m_proxyExtent;

        private BroadPhaseStrategy m_tree;
        private AABB m_queryAABB;
        private RayCastInput m_rayCastInput;
        private RayCastOutput m_rayCastOutput;
        private Actor m_rayActor;
        private Actor[] m_actors = new Actor[_e_actorCount];
        private int m_stepCount;
        private bool m_automated;
        private Random rand = new Random();


        public override void initTest(bool argDeserialized)
        {
            worldExtent = 15.0f;
            m_proxyExtent = 0.5f;

            m_tree = new DynamicTree();

            for (int i = 0; i < _e_actorCount; ++i)
            {
                Actor actor = m_actors[i] = new Actor();
                GetRandomAABB(actor.aabb);
                actor.proxyId = m_tree.createProxy(actor.aabb, actor);
            }

            m_stepCount = 0;

            float h = worldExtent;
            m_queryAABB = new AABB();
            m_queryAABB.lowerBound.set(-3.0f, -4.0f + h);
            m_queryAABB.upperBound.set(5.0f, 6.0f + h);

            m_rayCastInput = new RayCastInput();
            m_rayCastInput.p1.set(-5.0f, 5.0f + h);
            m_rayCastInput.p2.set(7.0f, -4.0f + h);
            // m_rayCastInput.p1.set(0.0f, 2.0f + h);
            // m_rayCastInput.p2.set(0.0f, -2.0f + h);
            m_rayCastInput.maxFraction = 1.0f;

            m_rayCastOutput = new RayCastOutput();

            m_automated = false;
        }


        public override void keyPressed(char argKeyChar, int argKeyCode)
        {
            switch (argKeyChar)
            {
                case 'a':
                    m_automated = !m_automated;
                    break;

                case 'c':
                    CreateProxy();
                    break;

                case 'd':
                    DestroyProxy();
                    break;

                case 'm':
                    MoveProxy();
                    break;
            }
        }

        private Vec2Array vecPool = new Vec2Array();


        public override void step(TestbedSettings settings)
        {
            m_rayActor = null;
            for (int i = 0; i < _e_actorCount; ++i)
            {
                m_actors[i].fraction = 1.0f;
                m_actors[i].overlap = false;
            }

            if (m_automated == true)
            {
                int actionCount = MathUtils.max(1, _e_actorCount >> 2);

                for (int i = 0; i < actionCount; ++i)
                {
                    Action();
                }
            }

            Query();
            RayCast();
            Vec2[] vecs = vecPool.get(4);

            for (int i = 0; i < _e_actorCount; ++i)
            {
                Actor actor = m_actors[i];
                if (actor.proxyId == -1)
                    continue;

                Color4f c = new Color4f(0.9f, 0.9f, 0.9f);
                if (actor == m_rayActor && actor.overlap)
                {
                    c.set(0.9f, 0.6f, 0.6f);
                }
                else if (actor == m_rayActor)
                {
                    c.set(0.6f, 0.9f, 0.6f);
                }
                else if (actor.overlap)
                {
                    c.set(0.6f, 0.6f, 0.9f);
                }
                actor.aabb.getVertices(vecs);
                getDebugDraw().drawPolygon(vecs, 4, c);
            }

            Color4f c2 = new Color4f(0.7f, 0.7f, 0.7f);
            m_queryAABB.getVertices(vecs);
            getDebugDraw().drawPolygon(vecs, 4, c2);

            getDebugDraw().drawSegment(m_rayCastInput.p1, m_rayCastInput.p2, c2);

            Color4f c1 = new Color4f(0.2f, 0.9f, 0.2f);
            Color4f c3 = new Color4f(0.9f, 0.2f, 0.2f);
            getDebugDraw().drawPoint(m_rayCastInput.p1, 6.0f, c1);
            getDebugDraw().drawPoint(m_rayCastInput.p2, 6.0f, c3);

            if (m_rayActor != null)
            {
                Color4f cr = new Color4f(0.2f, 0.2f, 0.9f);
                m_rayCastInput.p2.sub(m_rayCastInput.p1);
                m_rayCastInput.p2.mulLocal(m_rayActor.fraction);
                m_rayCastInput.p2.addLocal(m_rayCastInput.p1);
                Vec2 p = m_rayCastInput.p2;
                getDebugDraw().drawPoint(p, 6.0f, cr);
            }

            ++m_stepCount;

            if (settings.getSetting(TestbedSettings.DrawTree).enabled)
            {
                m_tree.drawTree(getDebugDraw());
            }

            getDebugDraw().drawString(5, 30,
                "(c)reate proxy, (d)estroy proxy, (a)utomate", Color4f.WHITE);
        }

        public bool treeCallback(int proxyId)
        {
            Actor actor = (Actor) m_tree.getUserData(proxyId);
            actor.overlap = AABB.testOverlap(m_queryAABB, actor.aabb);
            return true;
        }

        public float raycastCallback(RayCastInput input,
            int proxyId)
        {
            Actor actor = (Actor) m_tree.getUserData(proxyId);

            RayCastOutput output = new RayCastOutput();
            bool hit = actor.aabb.raycast(output, input, getWorld().getPool());

            if (hit)
            {
                m_rayCastOutput = output;
                m_rayActor = actor;
                m_rayActor.fraction = output.fraction;
                return output.fraction;
            }

            return input.maxFraction;
        }

        public class Actor
        {
            internal AABB aabb = new AABB();
            internal float fraction;
            internal bool overlap;
            internal int proxyId;
        }

        public void GetRandomAABB(AABB aabb)
        {
            Vec2 w = new Vec2();
            w.set(2.0f*m_proxyExtent, 2.0f*m_proxyExtent);
            // aabb.lowerBound.x = -m_proxyExtent;
            // aabb.lowerBound.y = -m_proxyExtent + worldExtent;
            aabb.lowerBound.x = MathUtils.randomFloat(rand, -worldExtent,
                worldExtent);
            aabb.lowerBound.y = MathUtils.randomFloat(rand, 0.0f,
                2.0f*worldExtent);
            aabb.upperBound.set(aabb.lowerBound);
            aabb.upperBound.addLocal(w);
        }

        public void MoveAABB(AABB aabb)
        {
            Vec2 d = new Vec2();
            d.x = MathUtils.randomFloat(rand, -0.5f, 0.5f);
            d.y = MathUtils.randomFloat(rand, -0.5f, 0.5f);
            // d.x = 2.0f;
            // d.y = 0.0f;
            aabb.lowerBound.addLocal(d);
            aabb.upperBound.addLocal(d);
            aabb.lowerBound.add(aabb.upperBound);
            aabb.lowerBound.mulLocal(.5f);
            Vec2 c0 = aabb.lowerBound;
            Vec2 min = new Vec2();
            min.set(-worldExtent, 0.0f);
            Vec2 max = new Vec2();
            max.set(worldExtent, 2.0f*worldExtent);
            Vec2 c = MathUtils.clamp(c0, min, max);

            aabb.lowerBound.addLocal(c.sub(c0));
            aabb.upperBound.addLocal(c.sub(c0));
        }

        public void CreateProxy()
        {
            for (int i = 0; i < _e_actorCount; ++i)
            {
                int j = MathUtils.abs(_random.Next()%_e_actorCount);
                Actor actor = m_actors[j];
                if (actor.proxyId == -1)
                {
                    GetRandomAABB(actor.aabb);
                    actor.proxyId = m_tree.createProxy(actor.aabb, actor);
                    return;
                }
            }
        }

        public void DestroyProxy()
        {
            for (int i = 0; i < _e_actorCount; ++i)
            {
                int j = MathUtils.abs(_random.Next()%_e_actorCount);
                Actor actor = m_actors[j];
                if (actor.proxyId != -1)
                {
                    m_tree.destroyProxy(actor.proxyId);
                    actor.proxyId = -1;
                    return;
                }
            }
        }

        public void MoveProxy()
        {
            for (int i = 0; i < _e_actorCount; ++i)
            {
                int j = MathUtils.abs(_random.Next()%_e_actorCount);
                Actor actor = m_actors[j];
                if (actor.proxyId == -1)
                {
                    continue;
                }

                AABB aabb0 = new AABB(actor.aabb);
                MoveAABB(actor.aabb);
                Vec2 displacement = actor.aabb.getCenter().sub(aabb0.getCenter());
                m_tree.moveProxy(actor.proxyId, new AABB(actor.aabb), displacement);
                return;
            }
        }

        private Random _random = new Random();

        public void Action()
        {
            int choice = MathUtils.abs(_random.Next()%20);

            switch (choice)
            {
                case 0:
                    CreateProxy();
                    break;

                case 1:
                    DestroyProxy();
                    break;

                default:
                    MoveProxy();
                    break;
            }
        }

        public void Query()
        {
            m_tree.query(this, m_queryAABB);

            for (int i = 0; i < _e_actorCount; ++i)
            {
                if (m_actors[i].proxyId == -1)
                {
                    continue;
                }

                bool overlap = AABB.testOverlap(m_queryAABB, m_actors[i].aabb);
                Debug.Assert(overlap == m_actors[i].overlap);
            }
        }

        public void RayCast()
        {
            m_rayActor = null;

            RayCastInput input = new RayCastInput();
            input.set(m_rayCastInput);

            // Ray cast against the dynamic tree.
            m_tree.raycast(this, input);

            // Brute force ray cast.
            Actor bruteActor = null;
            RayCastOutput bruteOutput = new RayCastOutput();
            for (int i = 0; i < _e_actorCount; ++i)
            {
                if (m_actors[i].proxyId == -1)
                {
                    continue;
                }

                RayCastOutput output = new RayCastOutput();
                bool hit = m_actors[i].aabb.raycast(output, input,
                    getWorld().getPool());
                if (hit)
                {
                    bruteActor = m_actors[i];
                    bruteOutput = output;
                    input.maxFraction = output.fraction;
                }
            }

            if (bruteActor != null)
            {
                if (MathUtils.abs(bruteOutput.fraction
                                  - m_rayCastOutput.fraction) > Settings.EPSILON)
                {
                    Debug.WriteLine("wrong!");
                    Debug.Assert(MathUtils.abs(bruteOutput.fraction
                                               - m_rayCastOutput.fraction) <= 20*Settings.EPSILON);
                }

            }
        }


        public override string getTestName()
        {
            return "Dynamic Tree";
        }

    }
}
