using SharpBox2D.Collision.Broadphase;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;
using SharpBox2D.Pooling;
using SharpBox2D.TestBed.Profile.Worlds;

namespace SharpBox2D.TestBed.Profile
{
    public class BroadphasePerformanceTest : BasicPerformanceTest {

        private static int NUM_TESTS = 2;
        private PerformanceTestWorld world;

        public BroadphasePerformanceTest(int iters, PerformanceTestWorld world) :
            base(NUM_TESTS, iters, 1000) {
            this.world = world;
            setFormat(ResultFormat.MILLISECONDS);
            }

        public static void main(string[] args) {
            BroadphasePerformanceTest benchmark = new BroadphasePerformanceTest(10, new PistonWorld());
            benchmark.go();
        }

        public void setupTest(int testNum) {
            World w;
            IWorldPool pool = new DefaultWorldPool(50, 50);
            if (testNum == 0) {
                w = new World(new Vec2(0.0f, -10.0f), pool);
            } else {
                w = new World(new Vec2(0, -10), pool, new DynamicTreeFlatNodes());
            }
            world.setupWorld(w);
        }

  
        public override void step(int testNum) {
            world.step();
        }

  
        public override string getTestName(int testNum) {
            switch (testNum) {
                case 0:
                    return "Normal";
                case 1:
                    return "Flat";
                default:
                    return "";
            }
        }
    }
}
