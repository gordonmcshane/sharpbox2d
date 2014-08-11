using System;
using SharpBox2D.Collision.Shapes;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;
using SharpBox2D.Dynamics.Joints;

namespace SharpBox2D.TestBed.Profile.Worlds
{
    public class PistonWorld : PerformanceTestWorld {
        public float timeStep = 1f / 60;
        public int velIters = 8;
        public int posIters = 3;

        public RevoluteJoint m_joint1;
        public PrismaticJoint m_joint2;
        public World world;

        public PistonWorld() {}

  
        public void setupWorld(World world) {
            this.world = world;
            Body ground = null;
            {
                BodyDef bd = new BodyDef();
                ground = world.createBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.setAsBox(5.0f, 100.0f);
                bd = new BodyDef();
                bd.type = BodyType.STATIC;
                FixtureDef sides = new FixtureDef();
                sides.shape = shape;
                sides.density = 0;
                sides.friction = 0;
                sides.restitution = .8f;
                sides.filter.categoryBits = 4;
                sides.filter.maskBits = 2;

                bd.position.set(-10.01f, 50.0f);
                Body bod = world.createBody(bd);
                bod.createFixture(sides);
                bd.position.set(10.01f, 50.0f);
                bod = world.createBody(bd);
                bod.createFixture(sides);
            }

            // turney
            {
                CircleShape cd;
                FixtureDef fd = new FixtureDef();
                BodyDef bd = new BodyDef();
                bd.type = BodyType.DYNAMIC;
                int numPieces = 5;
                float radius = 4f;
                bd.position = new Vec2(0.0f, 25.0f);
                Body body = world.createBody(bd);
                for (int i = 0; i < numPieces; i++) {
                    cd = new CircleShape();
                    cd.m_radius = .5f;
                    fd.shape = cd;
                    fd.density = 25;
                    fd.friction = .1f;
                    fd.restitution = .9f;
                    float xPos = radius * (float) System.Math.Cos(2f * System.Math.PI * (i / (float) (numPieces)));
                    float yPos = radius * (float) System.Math.Sin(2f * System.Math.PI * (i / (float) (numPieces)));
                    cd.m_p.set(xPos, yPos);

                    body.createFixture(fd);
                }

                RevoluteJointDef rjd = new RevoluteJointDef();
                rjd.initialize(body, ground, body.getPosition());
                rjd.motorSpeed = MathUtils.PI;
                rjd.maxMotorTorque = 1000000.0f;
                rjd.enableMotor = true;
                world.createJoint(rjd);
            }

            {
                Body prevBody = ground;

                // Define crank.
                {
                    PolygonShape shape = new PolygonShape();
                    shape.setAsBox(0.5f, 2.0f);

                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(0.0f, 7.0f);
                    Body body = world.createBody(bd);
                    body.createFixture(shape, 2.0f);

                    RevoluteJointDef rjd = new RevoluteJointDef();
                    rjd.initialize(prevBody, body, new Vec2(0.0f, 5.0f));
                    rjd.motorSpeed = 1.0f * MathUtils.PI;
                    rjd.maxMotorTorque = 20000;
                    rjd.enableMotor = true;
                    m_joint1 = (RevoluteJoint) world.createJoint(rjd);

                    prevBody = body;
                }

                // Define follower.
                {
                    PolygonShape shape = new PolygonShape();
                    shape.setAsBox(0.5f, 4.0f);

                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(0.0f, 13.0f);
                    Body body = world.createBody(bd);
                    body.createFixture(shape, 2.0f);

                    RevoluteJointDef rjd = new RevoluteJointDef();
                    rjd.initialize(prevBody, body, new Vec2(0.0f, 9.0f));
                    rjd.enableMotor = false;
                    world.createJoint(rjd);

                    prevBody = body;
                }

                // Define piston
                {
                    PolygonShape shape = new PolygonShape();
                    shape.setAsBox(7f, 2f);

                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    bd.position.set(0.0f, 17.0f);
                    Body body = world.createBody(bd);
                    FixtureDef piston = new FixtureDef();
                    piston.shape = shape;
                    piston.density = 2;
                    piston.filter.categoryBits = 1;
                    piston.filter.maskBits = 2;
                    body.createFixture(piston);

                    RevoluteJointDef rjd = new RevoluteJointDef();
                    rjd.initialize(prevBody, body, new Vec2(0.0f, 17.0f));
                    world.createJoint(rjd);

                    PrismaticJointDef pjd = new PrismaticJointDef();
                    pjd.initialize(ground, body, new Vec2(0.0f, 17.0f), new Vec2(0.0f, 1.0f));

                    pjd.maxMotorForce = 1000.0f;
                    pjd.enableMotor = true;

                    m_joint2 = (PrismaticJoint) world.createJoint(pjd);
                }

                // Create a payload
                {
                    PolygonShape sd = new PolygonShape();
                    BodyDef bd = new BodyDef();
                    bd.type = BodyType.DYNAMIC;
                    FixtureDef fixture = new FixtureDef();
                    Body body;
                    for (int i = 0; i < 100; ++i) {
                        sd.setAsBox(0.4f, 0.3f);
                        bd.position.set(-1.0f, 23.0f + i);

                        bd.bullet = false;
                        body = world.createBody(bd);
                        fixture.shape = sd;
                        fixture.density = .1f;
                        fixture.filter.categoryBits = 2;
                        fixture.filter.maskBits = 1 | 4 | 2;
                        body.createFixture(fixture);
                    }

                    CircleShape cd = new CircleShape();
                    cd.m_radius = 0.36f;
                    for (int i = 0; i < 100; ++i) {
                        bd.position.set(1.0f, 23.0f + i);
                        bd.bullet = false;
                        fixture.shape = cd;
                        fixture.density = 2f;
                        fixture.filter.categoryBits = 2;
                        fixture.filter.maskBits = 1 | 4 | 2;
                        body = world.createBody(bd);
                        body.createFixture(fixture);
                    }

                    float angle = 0.0f;
                    float delta = MathUtils.PI / 3.0f;
                    Vec2[] vertices = new Vec2[6];
                    for (int i = 0; i < 6; ++i) {
                        vertices[i] = new Vec2(0.3f * MathUtils.cos(angle), 0.3f * MathUtils.sin(angle));
                        angle += delta;
                    }

                    PolygonShape shape = new PolygonShape();
                    shape.set(vertices, 6);

                    for (int i = 0; i < 100; ++i) {
                        bd.position.set(0f, 23.0f + i);
                        bd.type = BodyType.DYNAMIC;
                        bd.fixedRotation = true;
                        bd.bullet = false;
                        fixture.shape = shape;
                        fixture.density = 1f;
                        fixture.filter.categoryBits = 2;
                        fixture.filter.maskBits = 1 | 4 | 2;
                        body = world.createBody(bd);
                        body.createFixture(fixture);
                    }
                }
            }
        }

  
        public void step() {
            world.step(timeStep, posIters, velIters);
        }
    }
}
