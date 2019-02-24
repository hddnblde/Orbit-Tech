// 👨‍💻 John Carlo Cariño Gamboa
// 📧 hddnblde@gmail.com

using UnityEngine;

namespace HddnBlde.OrbitTech
{
    [System.Serializable]
    public struct AxisConstraint
    {
        #region Shorthand Values Implementation
        private static readonly AxisConstraint m_full = new AxisConstraint(Orbit.FullRotation);
        private static readonly AxisConstraint m_half = new AxisConstraint(Orbit.HalfRotation);
        private static readonly AxisConstraint m_quarter = new AxisConstraint(Orbit.QuarterRotation);

        public static AxisConstraint full
        {
            get { return m_full; }
        }

        public static AxisConstraint half
        {
            get { return m_half; }
        }

        public static AxisConstraint quarter
        {
            get { return m_quarter; }
        }
        #endregion


        #region Fields
        [SerializeField]
        private float m_origin;

        [SerializeField]
        private float m_range;

        private const float MinRangeLimit = 15f;
        #endregion


        #region Properties
        public float origin
        {
            get { return m_origin; }
            set { m_origin = Orbit.WrapAxis(value); }
        }

        public float range
        {
            get { return m_range; }
            set { m_range = Mathf.Clamp(value, MinRangeLimit, Orbit.FullRotation); }
        }

        public float remainder
        {
            get { return Orbit.FullRotation - m_range; }
        }

        public bool isUnconstrained
        {
            get { return m_range >= Orbit.FullRotation; }
        }
        #endregion


        #region Constructors
        public AxisConstraint(float range)
        {
            m_range = range;
            m_origin = Orbit.OriginRotation;
        }

        public AxisConstraint(float range, float origin)
        {
            m_range = range;
            m_origin = origin;
        }
        #endregion


        #region Functions
        public float Constrain(float value)
        {
            if (isUnconstrained)
                return Orbit.WrapAxis(value);
            else
                return Clamp(value, m_range, m_origin);
        }

        private static float Clamp(float value, float range, float origin = Orbit.OriginRotation)
        {
            value = Orbit.WrapAxis(value);
            range = Mathf.Clamp(range, MinRangeLimit, Orbit.FullRotation);

            if (range >= Orbit.FullRotation)
                return value;

            if (Mathf.Abs(origin) > Orbit.OriginRotation)
                return Orbit.WrapAxis(Clamp(value - origin, range) + origin);

            float remainder = Orbit.FullRotation - range;
            float angleBoundary = range + (remainder / 2f);
            bool valueBeyondBoundary = (value > angleBoundary);

            if (value > Orbit.HalfRotation && valueBeyondBoundary)
                value -= Orbit.FullRotation;

            return Orbit.WrapAxis(Mathf.Clamp(value, origin, range));
        }
        #endregion


        #region Operator Overloads
        public static AxisConstraint operator +(AxisConstraint firstAddend, float secondAddend)
        {
            firstAddend.origin += secondAddend;

            return firstAddend;
        }

        public static AxisConstraint operator -(AxisConstraint minuend, float subtrahend)
        {
            minuend.origin -= subtrahend;

            return minuend;
        }
        #endregion


        #region Math Implementation
        public static AxisConstraint Lerp(AxisConstraint a, AxisConstraint b, float t)
        {
            float origin = Mathf.LerpAngle(a.origin, b.origin, t);
            float range = Mathf.Lerp(a.range, b.range, t);

            return new AxisConstraint(origin, range);
        }
        #endregion
    }
}