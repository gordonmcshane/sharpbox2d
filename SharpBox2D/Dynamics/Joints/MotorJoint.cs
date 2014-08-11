using System.Diagnostics;
using SharpBox2D.Common;
using SharpBox2D.Pooling;

namespace SharpBox2D.Dynamics.Joints
{


//Point-to-point constraint
//Cdot = v2 - v1
//   = v2 + cross(w2, r2) - v1 - cross(w1, r1)
//J = [-I -r1_skew I r2_skew ]
//Identity used:
//w k % (rx i + ry j) = w * (-ry i + rx j)

//Angle constraint
//Cdot = w2 - w1
//J = [0 0 -1 0 0 1]
//K = invI1 + invI2

/**
 * A motor joint is used to control the relative motion between two bodies. A typical usage is to
 * control the movement of a dynamic body with respect to the ground.
 * 
 * @author dmurph
 */

    public class MotorJoint : Joint
    {

        // Solver shared
        private Vec2 m_linearOffset = new Vec2();
        private float m_angularOffset;
        private Vec2 m_linearImpulse = new Vec2();
        private float m_angularImpulse;
        private float m_maxForce;
        private float m_maxTorque;
        private float m_correctionFactor;

        // Solver temp
        private int m_indexA;
        private int m_indexB;
        private Vec2 m_rA = new Vec2();
        private Vec2 m_rB = new Vec2();
        private Vec2 m_localCenterA = new Vec2();
        private Vec2 m_localCenterB = new Vec2();
        private Vec2 m_linearError = new Vec2();
        private float m_angularError;
        private float m_invMassA;
        private float m_invMassB;
        private float m_invIA;
        private float m_invIB;
        private Mat22 m_linearMass = new Mat22();
        private float m_angularMass;

        public MotorJoint(IWorldPool pool, MotorJointDef def) :
            base(pool, def)
        {
            m_linearOffset.set(def.linearOffset);
            m_angularOffset = def.angularOffset;

            m_angularImpulse = 0.0f;

            m_maxForce = def.maxForce;
            m_maxTorque = def.maxTorque;
            m_correctionFactor = def.correctionFactor;
        }

        public override void getAnchorA(ref Vec2 v)
        {
            v.set(m_bodyA.getPosition());
        }

        public override void getAnchorB(ref Vec2 v)
        {
            v.set(m_bodyB.getPosition());
        }

        public override void getReactionForce(float inv_dt, ref Vec2 v)
        {
            v.set(m_linearImpulse);
            v.mulLocal(inv_dt);
        }

        public override float getReactionTorque(float inv_dt)
        {
            return m_angularImpulse*inv_dt;
        }

        public float getCorrectionFactor()
        {
            return m_correctionFactor;
        }

        public void setCorrectionFactor(float correctionFactor)
        {
            this.m_correctionFactor = correctionFactor;
        }

        /**
   * Set the target linear offset, in frame A, in meters.
   */

        public void setLinearOffset(Vec2 linearOffset)
        {
            if (linearOffset.x != m_linearOffset.x || linearOffset.y != m_linearOffset.y)
            {
                m_bodyA.setAwake(true);
                m_bodyB.setAwake(true);
                m_linearOffset.set(linearOffset);
            }
        }

        /**
   * Get the target linear offset, in frame A, in meters.
   */

        public void getLinearOffset(ref Vec2 v)
        {
            v.set(m_linearOffset);
        }

        /**
   * Get the target linear offset, in frame A, in meters. Do not modify.
   */

        public Vec2 getLinearOffset()
        {
            return m_linearOffset;
        }

        /**
   * Set the target angular offset, in radians.
   * 
   * @param angularOffset
   */

        public void setAngularOffset(float angularOffset)
        {
            if (angularOffset != m_angularOffset)
            {
                m_bodyA.setAwake(true);
                m_bodyB.setAwake(true);
                m_angularOffset = angularOffset;
            }
        }

        public float getAngularOffset()
        {
            return m_angularOffset;
        }

        /**
   * Set the maximum friction force in N.
   * 
   * @param force
   */

        public void setMaxForce(float force)
        {
            Debug.Assert(force >= 0.0f);
            m_maxForce = force;
        }

        /**
   * Get the maximum friction force in N.
   */

        public float getMaxForce()
        {
            return m_maxForce;
        }

        /**
   * Set the maximum friction torque in N*m.
   */

        public void setMaxTorque(float torque)
        {
            Debug.Assert(torque >= 0.0f);
            m_maxTorque = torque;
        }

        /**
   * Get the maximum friction torque in N*m.
   */

        public float getMaxTorque()
        {
            return m_maxTorque;
        }

        public override void initVelocityConstraints(SolverData data)
        {
            m_indexA = m_bodyA.m_islandIndex;
            m_indexB = m_bodyB.m_islandIndex;
            m_localCenterA.set(m_bodyA.m_sweep.localCenter);
            m_localCenterB.set(m_bodyB.m_sweep.localCenter);
            m_invMassA = m_bodyA.m_invMass;
            m_invMassB = m_bodyB.m_invMass;
            m_invIA = m_bodyA.m_invI;
            m_invIB = m_bodyB.m_invI;

            Vec2 cA = data.positions[m_indexA].c;
            float aA = data.positions[m_indexA].a;
            Vec2 vA = data.velocities[m_indexA].v;
            float wA = data.velocities[m_indexA].w;

            Vec2 cB = data.positions[m_indexB].c;
            float aB = data.positions[m_indexB].a;
            Vec2 vB = data.velocities[m_indexB].v;
            float wB = data.velocities[m_indexB].w;

            Rot qA = pool.popRot();
            Rot qB = pool.popRot();
            Vec2 temp = pool.popVec2();
            Mat22 K = pool.popMat22();

            qA.set(aA);
            qB.set(aB);

            // Compute the effective mass matrix.
            // m_rA = b2Mul(qA, -m_localCenterA);
            // m_rB = b2Mul(qB, -m_localCenterB);
            m_rA.x = qA.c*-m_localCenterA.x - qA.s*-m_localCenterA.y;
            m_rA.y = qA.s*-m_localCenterA.x + qA.c*-m_localCenterA.y;
            m_rB.x = qB.c*-m_localCenterB.x - qB.s*-m_localCenterB.y;
            m_rB.y = qB.s*-m_localCenterB.x + qB.c*-m_localCenterB.y;

            // J = [-I -r1_skew I r2_skew]
            // [ 0 -1 0 1]
            // r_skew = [-ry; rx]

            // Matlab
            // K = [ mA+r1y^2*iA+mB+r2y^2*iB, -r1y*iA*r1x-r2y*iB*r2x, -r1y*iA-r2y*iB]
            // [ -r1y*iA*r1x-r2y*iB*r2x, mA+r1x^2*iA+mB+r2x^2*iB, r1x*iA+r2x*iB]
            // [ -r1y*iA-r2y*iB, r1x*iA+r2x*iB, iA+iB]
            float mA = m_invMassA, mB = m_invMassB;
            float iA = m_invIA, iB = m_invIB;

            K.ex.x = mA + mB + iA*m_rA.y*m_rA.y + iB*m_rB.y*m_rB.y;
            K.ex.y = -iA*m_rA.x*m_rA.y - iB*m_rB.x*m_rB.y;
            K.ey.x = K.ex.y;
            K.ey.y = mA + mB + iA*m_rA.x*m_rA.x + iB*m_rB.x*m_rB.x;

            K.invertToOut(ref m_linearMass);

            m_angularMass = iA + iB;
            if (m_angularMass > 0.0f)
            {
                m_angularMass = 1.0f/m_angularMass;
            }

            // m_linearError = cB + m_rB - cA - m_rA - b2Mul(qA, m_linearOffset);
            Rot.mulToOutUnsafe(qA, m_linearOffset, ref temp);
            m_linearError.x = cB.x + m_rB.x - cA.x - m_rA.x - temp.x;
            m_linearError.y = cB.y + m_rB.y - cA.y - m_rA.y - temp.y;
            m_angularError = aB - aA - m_angularOffset;

            if (data.step.warmStarting)
            {
                // Scale impulses to support a variable time step.
                m_linearImpulse.x *= data.step.dtRatio;
                m_linearImpulse.y *= data.step.dtRatio;
                m_angularImpulse *= data.step.dtRatio;

                Vec2 P = m_linearImpulse;
                vA.x -= mA*P.x;
                vA.y -= mA*P.y;
                wA -= iA*(m_rA.x*P.y - m_rA.y*P.x + m_angularImpulse);
                vB.x += mB*P.x;
                vB.y += mB*P.y;
                wB += iB*(m_rB.x*P.y - m_rB.y*P.x + m_angularImpulse);
            }
            else
            {
                m_linearImpulse.setZero();
                m_angularImpulse = 0.0f;
            }

            pool.pushVec2(1);
            pool.pushMat22(1);
            pool.pushRot(2);

            // data.velocities[m_indexA].v = vA;
            data.velocities[m_indexA].w = wA;
            // data.velocities[m_indexB].v = vB;
            data.velocities[m_indexB].w = wB;
        }

        public override void solveVelocityConstraints(SolverData data)
        {
            Vec2 vA = data.velocities[m_indexA].v;
            float wA = data.velocities[m_indexA].w;
            Vec2 vB = data.velocities[m_indexB].v;
            float wB = data.velocities[m_indexB].w;

            float mA = m_invMassA, mB = m_invMassB;
            float iA = m_invIA, iB = m_invIB;

            float h = data.step.dt;
            float inv_h = data.step.inv_dt;

            Vec2 temp = pool.popVec2();

            // Solve angular friction
            {
                float Cdot1 = wB - wA + inv_h*m_correctionFactor*m_angularError;
                float impulse = -m_angularMass*Cdot1;

                float oldImpulse = m_angularImpulse;
                float maxImpulse = h*m_maxTorque;
                m_angularImpulse = MathUtils.clamp(m_angularImpulse + impulse, -maxImpulse, maxImpulse);
                impulse = m_angularImpulse - oldImpulse;

                wA -= iA*impulse;
                wB += iB*impulse;
            }

            Vec2 Cdot2 = pool.popVec2();

            // Solve linear friction
            {
                // Cdot = vB + b2Cross(wB, m_rB) - vA - b2Cross(wA, m_rA) + inv_h * m_correctionFactor *
                // m_linearError;
                Cdot2.x =
                    vB.x + -wB*m_rB.y - vA.x - -wA*m_rA.y + inv_h*m_correctionFactor*m_linearError.x;
                Cdot2.y =
                    vB.y + wB*m_rB.x - vA.y - wA*m_rA.x + inv_h*m_correctionFactor*m_linearError.y;

                Vec2 impulse = temp;
                Mat22.mulToOutUnsafe(m_linearMass, Cdot2, ref impulse);
                impulse.negateLocal();
                Vec2 oldImpulse = pool.popVec2();
                oldImpulse.set(m_linearImpulse);
                m_linearImpulse.addLocal(impulse);

                float maxImpulse = h*m_maxForce;

                if (m_linearImpulse.lengthSquared() > maxImpulse*maxImpulse)
                {
                    m_linearImpulse.normalize();
                    m_linearImpulse.mulLocal(maxImpulse);
                }

                impulse.x = m_linearImpulse.x - oldImpulse.x;
                impulse.y = m_linearImpulse.y - oldImpulse.y;

                vA.x -= mA*impulse.x;
                vA.y -= mA*impulse.y;
                wA -= iA*(m_rA.x*impulse.y - m_rA.y*impulse.x);

                vB.x += mB*impulse.x;
                vB.y += mB*impulse.y;
                wB += iB*(m_rB.x*impulse.y - m_rB.y*impulse.x);
            }

            pool.pushVec2(3);

            // data.velocities[m_indexA].v.set(vA);
            data.velocities[m_indexA].w = wA;
            // data.velocities[m_indexB].v.set(vB);
            data.velocities[m_indexB].w = wB;
        }

        public override bool solvePositionConstraints(SolverData data)
        {
            return true;
        }
    }
}
