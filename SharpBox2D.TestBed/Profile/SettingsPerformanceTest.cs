
using SharpBox2D.Common;
using SharpBox2D.Dynamics;
using SharpBox2D.TestBed.Profile.Worlds;

namespace SharpBox2D.TestBed.Profile
{
    public class SettingsPerformanceTest : BasicPerformanceTest
    {

        private static int NUM_TESTS = 14;
        private PerformanceTestWorld world;

        public SettingsPerformanceTest(int iters, PerformanceTestWorld world) :
            base(NUM_TESTS, iters, 300)
        {
            this.world = world;
        }

        public static void main(string[] args)
        {
            SettingsPerformanceTest benchmark = new SettingsPerformanceTest(10, new PistonWorld());
            benchmark.go();
        }


        public void setupTest(int testNum)
        {
            World w = new World(new Vec2(0, -10));
            world.setupWorld(w);
        }


        public void preStep(int testNum)
        {
            Settings.FAST_ABS = testNum == 1;
            Settings.FAST_ATAN2 = testNum == 2;
            Settings.FAST_CEIL = testNum == 3;
            Settings.FAST_FLOOR = testNum == 4;
            Settings.FAST_ROUND = testNum == 5;
            Settings.SINCOS_LUT_ENABLED = testNum == 6;

            if (testNum == 7)
            {
                Settings.FAST_ABS = true;
                Settings.FAST_ATAN2 = true;
                Settings.FAST_CEIL = true;
                Settings.FAST_FLOOR = true;
                Settings.FAST_ROUND = true;
                Settings.SINCOS_LUT_ENABLED = true;
            }

            if (testNum > 7)
            {
                Settings.FAST_ABS = testNum != 8;
                Settings.FAST_ATAN2 = testNum != 9;
                Settings.FAST_CEIL = testNum != 10;
                Settings.FAST_FLOOR = testNum != 11;
                Settings.FAST_ROUND = testNum != 12;
                Settings.SINCOS_LUT_ENABLED = testNum != 13;
            }
        }


        public override void step(int testNum)
        {
            world.step();
        }


        public override string getTestName(int testNum)
        {
            switch (testNum)
            {
                case 0:
                    return "No optimizations";
                case 1:
                    return "Fast abs";
                case 2:
                    return "Fast atan2";
                case 3:
                    return "Fast ceil";
                case 4:
                    return "Fast floor";
                case 5:
                    return "Fast round";
                case 6:
                    return "Sincos lookup table";
                case 7:
                    return "All optimizations on";
                case 8:
                    return "no Fast abs";
                case 9:
                    return "no Fast atan2";
                case 10:
                    return "no Fast ceil";
                case 11:
                    return "no Fast floor";
                case 12:
                    return "no Fast round";
                case 13:
                    return "no Sincos lookup";
                default:
                    return "";
            }
        }
    }
}
