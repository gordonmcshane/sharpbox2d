namespace SharpBox2D.Particle
{
    public class ParticleGroupType
    {
        /** resists penetration */
        public static readonly int b2_solidParticleGroup = 1 << 0;
        /** keeps its shape */
        public static readonly int b2_rigidParticleGroup = 1 << 1;
    }
}
