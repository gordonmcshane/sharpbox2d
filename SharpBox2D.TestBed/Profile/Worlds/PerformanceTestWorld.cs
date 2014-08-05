

using SharpBox2D.Dynamics;

namespace SharpBox2D.TestBed.Profile.Worlds
{
    public interface PerformanceTestWorld {
        void setupWorld(World world);
        void step();
    }
}
