using SharpBox2D.Common;
using SharpBox2D.Dynamics;

namespace SharpBox2D.Particle
{
    public class ParticleBodyContact
    {
        /** Index of the particle making contact. */
        public int index;
        /** The body making contact. */
        public Body body;
        /** Weight of the contact. A value between 0.0f and 1.0f. */
        internal float weight;
        /** The normalized direction from the particle to the body. */
        public Vec2 normal = new Vec2();
        /** The effective mass used in calculating force. */
        internal float mass;
    }
}
