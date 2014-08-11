/*******************************************************************************
 * Copyright (c) 2013, Daniel Murphy
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
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
using System.Collections.Generic;
using SharpBox2D.Callbacks;
using SharpBox2D.Collision;
using SharpBox2D.Collision.Shapes;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;
using SharpBox2D.Dynamics.Contacts;
using SharpBox2D.Dynamics.Joints;
using SharpBox2D.Particle;

namespace SharpBox2D.TestBed.Framework
{

/**
 * @author Daniel Murphy
 */

    public abstract class TestbedTest :
        ContactListener, DestructionListener, ParticleDestructionListener
    {
        public static readonly int MAX_CONTACT_POINTS = 4048;
        public static readonly float ZOOM_SCALE_DIFF = .05f;
        public static readonly int TEXT_LINE_SPACE = 13;
        public static readonly int TEXT_SECTION_SPACE = 3;
        public static readonly int MOUSE_JOINT_BUTTON = 1;
        public static readonly int BOMB_SPAWN_BUTTON = 10;

        protected static readonly long GROUND_BODY_TAG = 1897450239847L;
        protected static readonly long BOMB_TAG = 98989788987L;
        protected static readonly long MOUSE_JOINT_TAG = 4567893364789L;

        public ContactPoint[] points = new ContactPoint[MAX_CONTACT_POINTS];

        /**
   * Only visible for compatibility. Should use {@link #getWorld()} instead.
   */
        protected World m_world;
        protected Body groundBody;
        private MouseJoint mouseJoint;

        private Body bomb;
        private Vec2 bombMousePoint = new Vec2();
        private Vec2 bombSpawnPoint = new Vec2();
        private bool bombSpawning = false;

        protected bool mouseTracing;
        private Vec2 mouseTracerPosition = new Vec2();
        private Vec2 mouseTracerVelocity = new Vec2();

        private Vec2 mouseWorld = new Vec2();
        private int pointCount;
        private int stepCount;

        private TestbedModel model;
        protected DestructionListener destructionListener;
        protected ParticleDestructionListener particleDestructionListener;


        private string title = null;
        protected int m_textLine;
        private LinkedList<string> textList = new LinkedList<string>();

        private TestbedCamera camera;

        //private JbSerializer serializer;
        //private JbDeserializer deserializer;

        private Transform identity = new Transform();

        public TestbedTest()
        {
            identity.setIdentity();
            for (int i = 0; i < MAX_CONTACT_POINTS; i++)
            {
                points[i] = new ContactPoint();
            }
            //serializer = new PbSerializer(this, new SignerAdapter(this) {
            //  
            //  public long getTag(Body argBody) {
            //    if (isSaveLoadEnabled()) {
            //      if (argBody == groundBody) {
            //        return GROUND_BODY_TAG;
            //      } else if (argBody == bomb) {
            //        return BOMB_TAG;
            //      }
            //    }
            //    return base.getTag(argBody);
            //  }

            //  
            //  public long getTag(Joint argJoint) {
            //    if (isSaveLoadEnabled()) {
            //      if (argJoint == mouseJoint) {
            //        return MOUSE_JOINT_TAG;
            //      }
            //    }
            //    return base.getTag(argJoint);
            //  }
            //});
            //deserializer = new PbDeserializer(this, new ListenerAdapter(this) {
            //  
            //  public void processBody(Body argBody, long argTag) {
            //    if (isSaveLoadEnabled()) {
            //      if (argTag == GROUND_BODY_TAG) {
            //        groundBody = argBody;
            //        return;
            //      } else if (argTag == BOMB_TAG) {
            //        bomb = argBody;
            //        return;
            //      }
            //    }
            //    base.processBody(argBody, argTag);
            //  }

            //  
            //  public void processJoint(Joint argJoint, long argTag) {
            //    if (isSaveLoadEnabled()) {
            //      if (argTag == MOUSE_JOINT_TAG) {
            //        mouseJoint = (MouseJoint) argJoint;
            //        return;
            //      }
            //    }
            //    base.processJoint(argJoint, argTag);
            //  }
            //});

            camera = new TestbedCamera(getDefaultCameraPos(), getDefaultCameraScale(), ZOOM_SCALE_DIFF);
        }

        public void sayGoodbye(int index)
        {
            particleDestroyed(index);
        }

        public void sayGoodbye(ParticleGroup group)
        {
            particleGroupDestroyed(group);
        }

        public void sayGoodbye(Fixture fixture)
        {
            fixtureDestroyed(fixture);
        }

        public void sayGoodbye(Joint joint)
        {
            if (mouseJoint == joint)
            {
                mouseJoint = null;
            }
            else
            {
                jointDestroyed(joint);
            }
        }

        public void init(TestbedModel model)
        {
            this.model = model;

            Vec2 gravity = new Vec2(0, -10f);
            m_world = model.getWorldCreator().createWorld(gravity);
            m_world.setParticleGravityScale(0.4f);
            m_world.setParticleDensity(1.2f);
            bomb = null;
            mouseJoint = null;

            mouseTracing = false;
            mouseTracerPosition.setZero();
            mouseTracerVelocity.setZero();

            BodyDef bodyDef = new BodyDef();
            groundBody = m_world.createBody(bodyDef);

            init(m_world, false);
        }

        public void init(World world, bool deserialized)
        {
            m_world = world;
            pointCount = 0;
            stepCount = 0;
            bombSpawning = false;
            model.getDebugDraw().setViewportTransform(camera.getTransform());

            world.setDestructionListener(destructionListener);
            world.setParticleDestructionListener(particleDestructionListener);
            world.setContactListener(this);
            world.setDebugDraw(model.getDebugDraw());
            title = getTestName();
            initTest(deserialized);
        }

        //protected JbSerializer getSerializer() {
        //  return serializer;
        //}

        //protected JbDeserializer getDeserializer() {
        //  return deserializer;
        //}

        /**
   * Gets the current world
   */

        public World getWorld()
        {
            return m_world;
        }

        /**
   * Gets the testbed model
   */

        public TestbedModel getModel()
        {
            return model;
        }

        /**
   * Gets the contact points for the current test
   */

        public ContactPoint[] getContactPoints()
        {
            return points;
        }

        /**
   * Gets the ground body of the world, used for some joints
   */

        public Body getGroundBody()
        {
            return groundBody;
        }

        /**
   * Gets the debug draw for the testbed
   */

        public DebugDraw getDebugDraw()
        {
            return model.getDebugDraw();
        }

        /**
   * Gets the world position of the mouse
   */

        public Vec2 getWorldMouse()
        {
            return mouseWorld;
        }

        public int getStepCount()
        {
            return stepCount;
        }

        /**
   * The number of contact points we're storing
   */

        public int getPointCount()
        {
            return pointCount;
        }

        public TestbedCamera getCamera()
        {
            return camera;
        }

        /**
   * Gets the 'bomb' body if it's present
   */

        public Body getBomb()
        {
            return bomb;
        }

        /**
   * Override for a different default camera position
   */

        public virtual Vec2 getDefaultCameraPos()
        {
            return new Vec2(0, 20);
        }

        /**
   * Override for a different default camera scale
   */

        public virtual float getDefaultCameraScale()
        {
            return 10;
        }

        public bool isMouseTracing()
        {
            return mouseTracing;
        }

        public Vec2 getMouseTracerPosition()
        {
            return mouseTracerPosition;
        }

        public Vec2 getMouseTracerVelocity()
        {
            return mouseTracerVelocity;
        }

        /**
   * Gets the filename of the current test. Default implementation uses the test name with no
   * spaces".
   */

        public string getFilename()
        {
            return getTestName().ToLower().Replace(" ", "_") + ".box2d";
        }

        /** @deprecated use {@link #getCamera()} */

        public void setCamera(Vec2 argPos)
        {
            camera.setCamera(argPos);
        }

        /** @deprecated use {@link #getCamera()} */

        public void setCamera(Vec2 argPos, float scale)
        {
            camera.setCamera(argPos, scale);
        }

        /**
   * Initializes the current test.
   * 
   * @param deserialized if the test was deserialized from a file. If so, all physics objects are
   *        already added.
   */
        public abstract void initTest(bool deserialized);

        /**
   * The name of the test
   */
        public abstract string getTestName();

        /**
   * Adds a text line to the reporting area
   */

        public virtual void addTextLine(string line)
        {
            textList.AddLast(line);
        }

        /**
   * called when the tests exits
   */

        public virtual void exit()
        {
        }

        private Color4f color1 = new Color4f(.3f, .95f, .3f);
        private Color4f color2 = new Color4f(.3f, .3f, .95f);
        private Color4f color3 = new Color4f(.9f, .9f, .9f);
        private Color4f color4 = new Color4f(.6f, .61f, 1);
        private Color4f color5 = new Color4f(.9f, .9f, .3f);
        private Color4f mouseColor = new Color4f(0f, 1f, 0f);
        private Vec2 p1 = new Vec2();
        private Vec2 p2 = new Vec2();
        private Vec2 tangent = new Vec2();
        private List<string> statsList = new List<string>();

        private Vec2 acceleration = new Vec2();
        private CircleShape pshape = new CircleShape();
        private ParticleVelocityQueryCallback pcallback = new ParticleVelocityQueryCallback();
        private AABB paabb = new AABB();

        public virtual void step(TestbedSettings settings)
        {
            float hz = settings.getSetting(TestbedSettings.Hz).value;
            float timeStep = hz > 0f ? 1f/hz : 0;
            if (settings.singleStep && !settings.pause)
            {
                settings.pause = true;
            }

            DebugDraw debugDraw = model.getDebugDraw();
            m_textLine = 20;

            if (title != null)
            {
                debugDraw.drawString(camera.getTransform().getExtents().x, 15, title, Color4f.WHITE);
                m_textLine += TEXT_LINE_SPACE;
            }

            if (settings.pause)
            {
                if (settings.singleStep)
                {
                    settings.singleStep = false;
                }
                else
                {
                    timeStep = 0;
                }

                debugDraw.drawString(5, m_textLine, "****PAUSED****", Color4f.WHITE);
                m_textLine += TEXT_LINE_SPACE;
            }

            DebugDrawFlags flags = 0;
            flags |= settings.getSetting(TestbedSettings.DrawShapes).enabled ? DebugDrawFlags.Shapes : 0;
            flags |= settings.getSetting(TestbedSettings.DrawJoints).enabled ? DebugDrawFlags.Joints : 0;
            flags |= settings.getSetting(TestbedSettings.DrawAABBs).enabled ? DebugDrawFlags.AABB : 0;
            flags |=
                settings.getSetting(TestbedSettings.DrawCOMs).enabled ? DebugDrawFlags.CenterOfMass : 0;
            flags |= settings.getSetting(TestbedSettings.DrawTree).enabled ? DebugDrawFlags.DynamicTree : 0;
            flags |=
                settings.getSetting(TestbedSettings.DrawWireframe).enabled
                    ? DebugDrawFlags.Wireframe
                    : 0;
            debugDraw.setFlags(flags);

            m_world.setAllowSleep(settings.getSetting(TestbedSettings.AllowSleep).enabled);
            m_world.setWarmStarting(settings.getSetting(TestbedSettings.WarmStarting).enabled);
            m_world.setSubStepping(settings.getSetting(TestbedSettings.SubStepping).enabled);
            m_world.setContinuousPhysics(settings.getSetting(TestbedSettings.ContinuousCollision).enabled);

            pointCount = 0;

            m_world.step(timeStep, settings.getSetting(TestbedSettings.VelocityIterations).value,
                settings.getSetting(TestbedSettings.PositionIterations).value);

            m_world.drawDebugData();

            if (timeStep > 0f)
            {
                ++stepCount;
            }

            debugDraw.drawString(5, m_textLine, "Engine Info", color4);
            m_textLine += TEXT_LINE_SPACE;
            debugDraw.drawString(5, m_textLine, "Framerate: " + (int) model.getCalculatedFps(),
                Color4f.WHITE);
            m_textLine += TEXT_LINE_SPACE;

            if (settings.getSetting(TestbedSettings.DrawStats).enabled)
            {
                int particleCount = m_world.getParticleCount();
                int groupCount = m_world.getParticleGroupCount();
                debugDraw.drawString(
                    5,
                    m_textLine,
                    "bodies/contacts/joints/proxies/particles/groups = " + m_world.getBodyCount() + "/"
                    + m_world.getContactCount() + "/" + m_world.getJointCount() + "/"
                    + m_world.getProxyCount() + "/" + particleCount + "/" + groupCount, Color4f.WHITE);
                m_textLine += TEXT_LINE_SPACE;

                debugDraw.drawString(5, m_textLine, "World mouse position: " + mouseWorld.ToString(),
                    Color4f.WHITE);
                m_textLine += TEXT_LINE_SPACE;


                statsList.Clear();
                Dynamics.Profile profile = getWorld().getProfile();
                profile.toDebugStrings(statsList);

                foreach (string s in statsList)
                {
                    debugDraw.drawString(5, m_textLine, s, Color4f.WHITE);
                    m_textLine += TEXT_LINE_SPACE;
                }
                m_textLine += TEXT_SECTION_SPACE;
            }

            if (settings.getSetting(TestbedSettings.DrawHelp).enabled)
            {
                debugDraw.drawString(5, m_textLine, "Help", color4);
                m_textLine += TEXT_LINE_SPACE;
                List<string> help = model.getImplSpecificHelp();
                foreach (string s in help)
                {
                    debugDraw.drawString(5, m_textLine, s, Color4f.WHITE);
                    m_textLine += TEXT_LINE_SPACE;
                }
                m_textLine += TEXT_SECTION_SPACE;
            }

            if (textList.Count != 0)
            {
                debugDraw.drawString(5, m_textLine, "Test Info", color4);
                m_textLine += TEXT_LINE_SPACE;
                foreach (string s in textList)
                {
                    debugDraw.drawString(5, m_textLine, s, Color4f.WHITE);
                    m_textLine += TEXT_LINE_SPACE;
                }
                textList.Clear();
            }

            if (mouseTracing && mouseJoint == null)
            {
                float delay = 0.1f;
                acceleration.x =
                    2/delay*(1/delay*(mouseWorld.x - mouseTracerPosition.x) - mouseTracerVelocity.x);
                acceleration.y =
                    2/delay*(1/delay*(mouseWorld.y - mouseTracerPosition.y) - mouseTracerVelocity.y);
                mouseTracerVelocity.x += timeStep*acceleration.x;
                mouseTracerVelocity.y += timeStep*acceleration.y;
                mouseTracerPosition.x += timeStep*mouseTracerVelocity.x;
                mouseTracerPosition.y += timeStep*mouseTracerVelocity.y;
                pshape.m_p.set(mouseTracerPosition);
                pshape.m_radius = 2;
                pcallback.init(m_world, pshape, mouseTracerVelocity);
                pshape.computeAABB(paabb, identity, 0);
                m_world.queryAABB(pcallback, paabb);
            }

            if (mouseJoint != null)
            {
                mouseJoint.getAnchorB(ref p1);
                Vec2 p2 = mouseJoint.getTarget();

                debugDraw.drawSegment(p1, p2, mouseColor);
            }

            if (bombSpawning)
            {
                debugDraw.drawSegment(bombSpawnPoint, bombMousePoint, Color4f.WHITE);
            }

            if (settings.getSetting(TestbedSettings.DrawContactPoints).enabled)
            {
                float k_impulseScale = 0.1f;
                float axisScale = 0.3f;

                for (int i = 0; i < pointCount; i++)
                {

                    ContactPoint point = points[i];

                    if (point.state == Collision.Collision.PointState.ADD_STATE)
                    {
                        debugDraw.drawPoint(point.position, 10f, color1);
                    }
                    else if (point.state == Collision.Collision.PointState.PERSIST_STATE)
                    {
                        debugDraw.drawPoint(point.position, 5f, color2);
                    }

                    if (settings.getSetting(TestbedSettings.DrawContactNormals).enabled)
                    {
                        p1.set(point.position);
                        p2.set(point.normal);
                        p2.mulLocal(axisScale);
                        p2.addLocal(p1);
                        debugDraw.drawSegment(p1, p2, color3);

                    }
                    else if (settings.getSetting(TestbedSettings.DrawContactImpulses).enabled)
                    {
                        p1.set(point.position);
                        p2.set(point.normal);
                        p2.mulLocal(k_impulseScale);
                        p2.mulLocal(point.normalImpulse);
                        p2.addLocal(p1);
                        debugDraw.drawSegment(p1, p2, color5);
                    }

                    if (settings.getSetting(TestbedSettings.DrawFrictionImpulses).enabled)
                    {
                        Vec2.crossToOutUnsafe(point.normal, 1, ref tangent);
                        p1.set(point.position);
                        p2.set(tangent);
                        p2.mulLocal(k_impulseScale);
                        p2.mulLocal(point.tangentImpulse);
                        p2.addLocal(p1);
                        debugDraw.drawSegment(p1, p2, color5);
                    }
                }
            }
        }

        /************ INPUT ************/

        /**
   * Called for mouse-up
   */

        public virtual void mouseUp(Vec2 p, int button)
        {
            mouseTracing = false;
            if (button == MOUSE_JOINT_BUTTON)
            {
                destroyMouseJoint();
            }
            completeBombSpawn(p);
        }

        public virtual void keyPressed(char keyChar, int keyCode)
        {
        }

        public virtual void keyReleased(char keyChar, int keyCode)
        {
        }

        public virtual void mouseDown(Vec2 p, int button)
        {
            mouseWorld.set(p);
            mouseTracing = true;
            mouseTracerVelocity.setZero();
            mouseTracerPosition.set(p);

            if (button == BOMB_SPAWN_BUTTON)
            {
                beginBombSpawn(p);
            }

            if (button == MOUSE_JOINT_BUTTON)
            {
                spawnMouseJoint(p);
            }
        }

        public virtual void mouseMove(Vec2 p)
        {
            mouseWorld.set(p);
        }

        public virtual void mouseDrag(Vec2 p, int button)
        {
            mouseWorld.set(p);
            if (button == MOUSE_JOINT_BUTTON)
            {
                updateMouseJoint(p);
            }
            if (button == BOMB_SPAWN_BUTTON)
            {
                bombMousePoint.set(p);
            }
        }

        /************ MOUSE JOINT ************/

        private AABB queryAABB = new AABB();
        private TestQueryCallback callback = new TestQueryCallback();

        private void spawnMouseJoint(Vec2 p)
        {
            if (mouseJoint != null)
            {
                return;
            }
            queryAABB.lowerBound.set(p.x - .001f, p.y - .001f);
            queryAABB.upperBound.set(p.x + .001f, p.y + .001f);
            callback.point.set(p);
            callback.fixture = null;
            m_world.queryAABB(callback, queryAABB);

            if (callback.fixture != null)
            {
                Body body = callback.fixture.getBody();
                MouseJointDef def = new MouseJointDef();
                def.bodyA = groundBody;
                def.bodyB = body;
                def.collideConnected = true;
                def.target.set(p);
                def.maxForce = 1000f*body.getMass();
                mouseJoint = (MouseJoint) m_world.createJoint(def);
                body.setAwake(true);
            }
        }

        private void updateMouseJoint(Vec2 target)
        {
            if (mouseJoint != null)
            {
                mouseJoint.setTarget(target);
            }
        }

        private void destroyMouseJoint()
        {
            if (mouseJoint != null)
            {
                m_world.destroyJoint(mouseJoint);
                mouseJoint = null;
            }
        }

        /********** BOMB ************/

        private Vec2 p = new Vec2();
        private Vec2 v = new Vec2();

        public void lanchBomb()
        {
            p.set((float) _random.NextDouble()*30 - 15, 30f);
            v.set(p);
            v.mulLocal(-5f);
            launchBomb(p, v);
        }

        private AABB aabb = new AABB();

        private void launchBomb(Vec2 position, Vec2 velocity)
        {
            if (bomb != null)
            {
                m_world.destroyBody(bomb);
                bomb = null;
            }
            // todo optimize this
            BodyDef bd = new BodyDef();
            bd.type = BodyType.DYNAMIC;
            bd.position.set(position);
            bd.bullet = true;
            bomb = m_world.createBody(bd);
            bomb.setLinearVelocity(velocity);

            CircleShape circle = new CircleShape();
            circle.m_radius = 0.3f;

            FixtureDef fd = new FixtureDef();
            fd.shape = circle;
            fd.density = 20f;
            fd.restitution = 0;

            Vec2 minV = new Vec2(position);
            Vec2 maxV = new Vec2(position);

            minV.subLocal(new Vec2(.3f, .3f));
            maxV.addLocal(new Vec2(.3f, .3f));

            aabb.lowerBound.set(minV);
            aabb.upperBound.set(maxV);

            bomb.createFixture(fd);
        }

        private void beginBombSpawn(Vec2 worldPt)
        {
            bombSpawnPoint.set(worldPt);
            bombMousePoint.set(worldPt);
            bombSpawning = true;
        }

        private Vec2 vel = new Vec2();

        private void completeBombSpawn(Vec2 p)
        {
            if (bombSpawning == false)
            {
                return;
            }

            float multiplier = 30f;
            vel.set(bombSpawnPoint);
            vel.subLocal(p);
            vel.mulLocal(multiplier);
            launchBomb(bombSpawnPoint, vel);
            bombSpawning = false;
        }

        /************ SERIALIZATION *************/

        /**
       * Override to enable saving and loading. Remember to also override the {@link ObjectListener} and
       * {@link ObjectSigner} methods if you need to
       * 
       * @return
       */

        public virtual bool isSaveLoadEnabled()
        {
            return false;
        }

        public virtual long getTag(Body body)
        {
            return default(long);
        }

        public virtual long getTag(Fixture fixture)
        {
            return default(long);
        }

        public virtual long getTag(Joint joint)
        {
            return default(long);
        }

        public long getTag(Shape shape)
        {
            return default(long);
        }

        public long getTag(World world)
        {
            return default(long);
        }

        public virtual void processBody(Body body, long tag)
        {
        }

        public virtual void processFixture(Fixture fixture, long tag)
        {
        }

        public virtual void processJoint(Joint joint, long tag)
        {
        }

        public virtual void processShape(Shape shape, long tag)
        {
        }

        public virtual void processWorld(World world, long tag)
        {
        }

        public virtual bool isUnsupported(InvalidOperationException exception)
        {
            return true;
        }

        public virtual void fixtureDestroyed(Fixture fixture)
        {
        }

        public virtual void jointDestroyed(Joint joint)
        {
        }

        public virtual void beginContact(Contact contact)
        {
        }

        public virtual void endContact(Contact contact)
        {
        }

        public virtual void particleDestroyed(int particle)
        {
        }

        public virtual void particleGroupDestroyed(ParticleGroup group)
        {
        }

        public virtual void postSolve(Contact contact, ContactImpulse impulse)
        {
        }

        private Collision.Collision.PointState[] state1 = new Collision.Collision.PointState[Settings.maxManifoldPoints];
        private Collision.Collision.PointState[] state2 = new Collision.Collision.PointState[Settings.maxManifoldPoints];
        private WorldManifold worldManifold = new WorldManifold();
        private Random _random = new Random();

        public virtual void preSolve(Contact contact, Manifold oldManifold)
        {
            Manifold manifold = contact.getManifold();

            if (manifold.pointCount == 0)
            {
                return;
            }

            Fixture fixtureA = contact.getFixtureA();
            Fixture fixtureB = contact.getFixtureB();

            Collision.Collision.getPointStates(state1, state2, oldManifold, manifold);

            contact.getWorldManifold(worldManifold);

            for (int i = 0; i < manifold.pointCount && pointCount < MAX_CONTACT_POINTS; i++)
            {
                ContactPoint cp = points[pointCount];
                cp.fixtureA = fixtureA;
                cp.fixtureB = fixtureB;
                cp.position.set(worldManifold.points[i]);
                cp.normal.set(worldManifold.normal);
                cp.state = state2[i];
                cp.normalImpulse = manifold.points[i].normalImpulse;
                cp.tangentImpulse = manifold.points[i].tangentImpulse;
                cp.separation = worldManifold.separations[i];
                ++pointCount;
            }
        }
    }


    internal class TestQueryCallback : QueryCallback
    {

        public Vec2 point;
        public Fixture fixture;

        public TestQueryCallback()
        {
            point = new Vec2();
            fixture = null;
        }

        public bool reportFixture(Fixture argFixture)
        {
            Body body = argFixture.getBody();
            if (body.getType() == BodyType.DYNAMIC)
            {
                bool inside = argFixture.testPoint(point);
                if (inside)
                {
                    fixture = argFixture;

                    return false;
                }
            }

            return true;
        }
    }


    internal class ParticleVelocityQueryCallback : ParticleQueryCallback
    {
        private World world;
        private Shape shape;
        private Vec2 velocity;
        private Transform xf = new Transform();

        public ParticleVelocityQueryCallback()
        {
            xf.setIdentity();
        }

        public void init(World world, Shape shape, Vec2 velocity)
        {
            this.world = world;
            this.shape = shape;
            this.velocity = velocity;
        }

        public bool reportParticle(int index)
        {
            Vec2 p = world.getParticlePositionBuffer()[index];
            if (shape.testPoint(xf, p))
            {
                Vec2 v = world.getParticleVelocityBuffer()[index];
                v.set(velocity);
            }
            return true;
        }
    }


//class SignerAdapter : ObjectSigner {
//  private  ObjectSigner signer;

//  public SignerAdapter(ObjectSigner argSigner) {
//    signer = argSigner;
//  }

//  public long getTag(World argWorld) {
//    return signer.getTag(argWorld);
//  }

//  public long getTag(Body argBody) {
//    return signer.getTag(argBody);
//  }

//  public long getTag(Shape argShape) {
//    return signer.getTag(argShape);
//  }

//  public long getTag(Fixture argFixture) {
//    return signer.getTag(argFixture);
//  }

//  public long getTag(Joint argJoint) {
//    return signer.getTag(argJoint);
//  }
//}


//class ListenerAdapter : ObjectListener {
//  private  ObjectListener listener;

//  public ListenerAdapter(ObjectListener argListener) {
//    listener = argListener;
//  }

//  public void processWorld(World argWorld, long argTag) {
//    listener.processWorld(argWorld, argTag);
//  }

//  public void processBody(Body argBody, long argTag) {
//    listener.processBody(argBody, argTag);
//  }

//  public void processFixture(Fixture argFixture, long argTag) {
//    listener.processFixture(argFixture, argTag);
//  }

//  public void processShape(Shape argShape, long argTag) {
//    listener.processShape(argShape, argTag);
//  }

//  public void processJoint(Joint argJoint, long argTag) {
//    listener.processJoint(argJoint, argTag);
//  }
}

