// 👨‍💻 John Carlo Cariño Gamboa
// 📧 hddnblde@gmail.com

using UnityEngine;

namespace HddnBlde.OrbitTech
{
    [System.Serializable]
    public struct OrbitConstraint
    {
        #region Shorthand Values Implementation
        private static readonly OrbitConstraint m_full = new OrbitConstraint(AxisConstraint.full, AxisConstraint.full, AxisConstraint.full);
        private static readonly OrbitConstraint m_minimal = new OrbitConstraint(AxisConstraint.full, AxisConstraint.half - Orbit.QuarterRotation, AxisConstraint.full);
        private static readonly OrbitConstraint m_standard = new OrbitConstraint(AxisConstraint.full, AxisConstraint.half - Orbit.QuarterRotation, AxisConstraint.half - Orbit.QuarterRotation);

        public static OrbitConstraint full
        {
            get { return m_full; }
        }

        public static OrbitConstraint minimal
        {
            get { return m_minimal; }
        }

        public static OrbitConstraint standard
        {
            get { return m_standard; }
        }
        #endregion


        #region Fields
        [SerializeField]
        private AxisConstraint m_yaw;

        [SerializeField]
        private AxisConstraint m_pitch;

        [SerializeField]
        private AxisConstraint m_roll;
        #endregion


        #region Constructors
        public OrbitConstraint(AxisConstraint yaw, AxisConstraint pitch)
        {
            m_yaw = yaw;
            m_pitch = pitch;
            m_roll = new AxisConstraint();
        }

        public OrbitConstraint(AxisConstraint yaw, AxisConstraint pitch, AxisConstraint roll)
        {
            m_yaw = yaw;
            m_pitch = pitch;
            m_roll = roll;
        }
        #endregion


        #region Functions
        public void Set(AxisConstraint yaw, AxisConstraint pitch, AxisConstraint roll)
        {
            m_yaw = yaw;
            m_pitch = pitch;
            m_roll = roll;
        }

        public Orbit Constrain(float yaw, float pitch, float roll)
        {
            if (!m_yaw.isUnconstrained)
                yaw = m_yaw.Constrain(yaw);

            if (!m_pitch.isUnconstrained)
                pitch = m_pitch.Constrain(pitch);

            if (!m_pitch.isUnconstrained)
                roll = m_roll.Constrain(roll);

            return new Orbit(yaw, pitch, roll);
        }

        public float Yaw(float value)
        {
            return m_yaw.Constrain(value);
        }

        public float Pitch(float value)
        {
            return m_pitch.Constrain(value);
        }

        public float Roll(float value)
        {
            return m_roll.Constrain(value);
        }
        #endregion


        #region Math Implementation
        public static OrbitConstraint Lerp(OrbitConstraint a, OrbitConstraint b, float t)
        {
            AxisConstraint yaw = AxisConstraint.Lerp(a.m_yaw, b.m_yaw, t);
            AxisConstraint pitch = AxisConstraint.Lerp(a.m_pitch, b.m_pitch, t);
            AxisConstraint roll = AxisConstraint.Lerp(a.m_roll, b.m_roll, t);

            return new OrbitConstraint(yaw, pitch, roll);
        }
        #endregion
    }
}