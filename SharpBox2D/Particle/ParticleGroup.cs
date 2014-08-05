using SharpBox2D.Common;
using SharpBox2D.Particle;

namespace SharpBox2D.Particle
{
    public class ParticleGroup
    {
        internal ParticleSystem m_system;
        internal int m_firstIndex;
        internal int m_lastIndex;
        internal int m_groupFlags;
        internal float m_strength;
        internal ParticleGroup m_prev;
        internal ParticleGroup m_next;

        internal int m_timestamp;
        internal float m_mass;
        internal float m_inertia;
        internal Vec2 m_center = new Vec2();
        internal Vec2 m_linearVelocity = new Vec2();
        internal float m_angularVelocity;
        internal Transform m_transform = new Transform();

        internal bool m_destroyAutomatically;
        internal bool m_toBeDestroyed;
        internal bool m_toBeSplit;

        internal object m_userData;

        public ParticleGroup()
        {
            // m_system = null;
            m_firstIndex = 0;
            m_lastIndex = 0;
            m_groupFlags = 0;
            m_strength = 1.0f;

            m_timestamp = -1;
            m_mass = 0;
            m_inertia = 0;
            m_angularVelocity = 0;
            m_transform.setIdentity();

            m_destroyAutomatically = true;
            m_toBeDestroyed = false;
            m_toBeSplit = false;
        }

        public ParticleGroup getNext()
        {
            return m_next;
        }

        public int getParticleCount()
        {
            return m_lastIndex - m_firstIndex;
        }

        public int getBufferIndex()
        {
            return m_firstIndex;
        }

        public int getGroupFlags()
        {
            return m_groupFlags;
        }

        public void setGroupFlags(int flags)
        {
            m_groupFlags = flags;
        }

        public float getMass()
        {
            updateStatistics();
            return m_mass;
        }

        public float getInertia()
        {
            updateStatistics();
            return m_inertia;
        }

        public Vec2 getCenter()
        {
            updateStatistics();
            return m_center;
        }

        public Vec2 getLinearVelocity()
        {
            updateStatistics();
            return m_linearVelocity;
        }

        public float getAngularVelocity()
        {
            updateStatistics();
            return m_angularVelocity;
        }

        public Transform getTransform()
        {
            return m_transform;
        }

        public Vec2 getPosition()
        {
            return m_transform.p;
        }

        public float getAngle()
        {
            return m_transform.q.getAngle();
        }

        public object getUserData()
        {
            return m_userData;
        }

        public void setUserData(object data)
        {
            m_userData = data;
        }



        public void updateStatistics()
        {
            if (m_timestamp != m_system.m_timestamp)
            {
                float m = m_system.getParticleMass();
                m_mass = 0;
                m_center.setZero();
                m_linearVelocity.setZero();
                for (int i = m_firstIndex; i < m_lastIndex; i++)
                {
                    m_mass += m;
                    Vec2 pos = m_system.m_positionBuffer.data[i];
                    m_center.x += m*pos.x;
                    m_center.y += m*pos.y;
                    Vec2 vel = m_system.m_velocityBuffer.data[i];
                    m_linearVelocity.x += m*vel.x;
                    m_linearVelocity.y += m*vel.y;
                }
                if (m_mass > 0)
                {
                    m_center.x *= 1/m_mass;
                    m_center.y *= 1/m_mass;
                    m_linearVelocity.x *= 1/m_mass;
                    m_linearVelocity.y *= 1/m_mass;
                }
                m_inertia = 0;
                m_angularVelocity = 0;
                for (int i = m_firstIndex; i < m_lastIndex; i++)
                {
                    Vec2 pos = m_system.m_positionBuffer.data[i];
                    Vec2 vel = m_system.m_velocityBuffer.data[i];
                    float px = pos.x - m_center.x;
                    float py = pos.y - m_center.y;
                    float vx = vel.x - m_linearVelocity.x;
                    float vy = vel.y - m_linearVelocity.y;
                    m_inertia += m*(px*px + py*py);
                    m_angularVelocity += m*(px*vy - py*vx);
                }
                if (m_inertia > 0)
                {
                    m_angularVelocity *= 1/m_inertia;
                }
                m_timestamp = m_system.m_timestamp;
            }
        }
    }
}
