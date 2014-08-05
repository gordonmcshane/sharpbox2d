namespace SharpBox2D.Particle
{
/**
 * The particle type. Can be combined with | operator. Zero means liquid.
 * 
 * @author dmurph
 */

    public class ParticleType
    {
        public static readonly int b2_waterParticle = 0;
        /** removed after next step */
        public static readonly int b2_zombieParticle = 1 << 1;
        /** zero velocity */
        public static readonly int b2_wallParticle = 1 << 2;
        /** with restitution from stretching */
        public static readonly int b2_springParticle = 1 << 3;
        /** with restitution from deformation */
        public static readonly int b2_elasticParticle = 1 << 4;
        /** with viscosity */
        public static readonly int b2_viscousParticle = 1 << 5;
        /** without isotropic pressure */
        public static readonly int b2_powderParticle = 1 << 6;
        /** with surface tension */
        public static readonly int b2_tensileParticle = 1 << 7;
        /** mixing color between contacting particles */
        public static readonly int b2_colorMixingParticle = 1 << 8;
        /** call b2DestructionListener on destruction */
        public static readonly int b2_destructionListener = 1 << 9;
    }
}