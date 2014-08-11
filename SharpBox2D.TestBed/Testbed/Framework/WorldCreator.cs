using SharpBox2D.Common;
using SharpBox2D.Dynamics;

namespace SharpBox2D.TestBed.Framework
{
    public interface WorldCreator
    {
        World createWorld(Vec2 gravity);
    }
}
