using SharpBox2D.Common;

namespace SharpBox2D.Particle
{

    public class ParticleDef
    {
        /**
   * Specifies the type of particle. A particle may be more than one type. Multiple types are
   * chained by logical sums, for example: pd.flags = ParticleType.b2_elasticParticle |
   * ParticleType.b2_viscousParticle.
   */
        public int flags;

        /** The world position of the particle. */
        public Vec2 position = new Vec2();

        /** The linear velocity of the particle in world co-ordinates. */
        public Vec2 velocity = new Vec2();

        /** The color of the particle. */
        public ParticleColor color;

        /** Use this to store application-specific body data. */
        public object userData;
    }
}
