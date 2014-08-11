using SharpBox2D.Common;
using SharpBox2D.Dynamics;

namespace SharpBox2D.TestBed.Framework
{

    public class DefaultWorldCreator : WorldCreator
    {

        public World createWorld(Vec2 gravity)
        {
            return new World(gravity);
        }
    }
}
