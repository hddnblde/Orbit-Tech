// 👨‍💻 John Carlo Cariño Gamboa
// 📧 hddnblde@gmail.com

using UnityEngine;

namespace HddnBlde.OrbitTech
{
    [System.Serializable]
    public struct Orbit : ISerializationCallbackReceiver
    {
        #region Constants
        public const float OriginRotation = 0f;
        public const float QuarterRotation = 90f;
        public const float HalfRotation = 180f;
        public const float FullRotation = 360f;
        #endregion


        #region Shorthand Values Implementation
        private static readonly Orbit m_origin = new Orbit(OriginRotation, 30f, OrbitConstraint.full);
        private static readonly Orbit m_originWithMinimalConstraint = new Orbit(OriginRotation, 30f, OrbitConstraint.minimal);
        private static readonly Orbit m_originWithStandardConstraint = new Orbit(OriginRotation, 30f, OrbitConstraint.standard);

        public static Orbit origin
        {
            get { return m_origin; }
        }

        public static Orbit originWithMinimalConstraint
        {
            get { return m_originWithMinimalConstraint; }
        }

        public static Orbit originWithStandardConstraint
        {
            get { return m_originWithStandardConstraint; }
        }
        #endregion


        #region Constructors
        public Orbit(float yaw, float pitch) : this(yaw, pitch, OriginRotation, OrbitConstraint.full) {}

        public Orbit(float yaw, float pitch, float roll) : this(yaw, pitch, roll, OrbitConstraint.full) {}

        public Orbit(Vector3 direction) : this(direction, OrbitConstraint.full) {}

        public Orbit(Vector3 direction, Vector3 up) : this(direction, up, OrbitConstraint.full) {}

        public Orbit(Quaternion rotation) : this(rotation, OrbitConstraint.full) {}

        public Orbit(float yaw, float pitch, OrbitConstraint constraint) : this(yaw, pitch, OriginRotation, constraint) {}

        public Orbit(float yaw, float pitch, float roll, OrbitConstraint constraint)
        {
            m_constraint = constraint;
            m_yaw = constraint.Yaw(yaw);
            m_pitch = constraint.Pitch(pitch);
            m_roll = constraint.Roll(roll);
            m_direction = CalculateDirection(m_yaw, m_pitch);
            m_rotation = CalculateRotation(m_yaw, m_pitch, m_roll);
        }

        public Orbit(Vector3 direction, OrbitConstraint constraint) : this(DirectionToOrbit(direction, constraint)) {}

        public Orbit(Vector3 direction, Vector3 up, OrbitConstraint constraint) : this(DirectionToOrbit(direction, up, constraint)) {}

        public Orbit(Quaternion rotation, OrbitConstraint constraint) : this(QuaternionToOrbit(rotation, constraint)) {}

        public Orbit(Orbit copy)
        {
            m_constraint = copy.m_constraint;
            m_yaw = copy.yaw;
            m_pitch = copy.pitch;
            m_roll = copy.roll;
            m_direction = copy.direction;
            m_rotation = copy.rotation;
        }
        #endregion


        #region Serialized Fields
        [SerializeField]
        private float m_yaw;

        [SerializeField]
        private float m_pitch;

        [SerializeField]
        private float m_roll;

        [SerializeField, HideInInspector]
        private OrbitConstraint m_constraint;
        #endregion


        #region Internal Fields
        private Vector3 m_direction;
        private Quaternion m_rotation;
        #endregion


        #region Properties
        public float yaw
        {
            get { return m_yaw; }
            set { Validate(ref m_yaw, value); }
        }

        public float pitch
        {
            get { return m_pitch; }
            set { Validate(ref m_pitch, value); }
        }

        public float roll
        {
            get { return m_roll; }
            set { Validate(ref m_roll, value); }
        }

        public Vector3 direction
        {
            get { return m_direction; }
        }

        public Quaternion rotation
        {
            get { return m_rotation; }
        }
        #endregion


        #region Public Methods
        public void Clone(Orbit copy)
        {
            m_constraint = copy.m_constraint;
            m_yaw = copy.yaw;
            m_pitch = copy.pitch;
            m_roll = copy.roll;
            m_direction = copy.m_direction;
            m_rotation = copy.m_rotation;
        }

        public void Set(float yaw, float pitch)
        {
            Set(yaw, pitch, m_roll);
        }

        public void Set(float yaw, float pitch, float roll)
        {
            m_yaw = yaw;
            m_pitch = pitch;
            m_roll = roll;
            WrapAxes();
            CalculateTransformations();
        }

        public void OrientTowards(Vector3 direction)
        {
            this = DirectionToOrbit(direction, m_constraint);
        }

        public void OrientTowards(Vector3 direction, Vector3 up)
        {
            this = DirectionToOrbit(direction, up, m_constraint);
        }

        public void OrientTowards(Quaternion rotation)
        {
            this = QuaternionToOrbit(rotation, m_constraint);
        }

        public void Constrain(OrbitConstraint constraint)
        {
            m_constraint = constraint;
            WrapAxes();
        }

        public void Constrain(AxisConstraint yaw, AxisConstraint pitch, AxisConstraint roll)
        {
            OrbitConstraint constraint = new OrbitConstraint(yaw, pitch, roll);
            Constrain(constraint);
        }
        #endregion


        #region Internal Methods
        private void Validate(ref float parameter, float value)
        {
            if (Mathf.Approximately(parameter, value))
                return;

            parameter = value;
            WrapAxes();
            CalculateTransformations();
        }

        private void CalculateTransformations()
        {
            m_direction = CalculateDirection(m_yaw, m_pitch);
            m_rotation = CalculateRotation(m_yaw, m_pitch, m_roll);
        }

        private void WrapAxes()
        {
            m_yaw = m_constraint.Yaw(m_yaw);
            m_pitch = m_constraint.Pitch(m_pitch);
            m_roll = m_constraint.Roll(m_roll);
        }
        #endregion


        #region Math Implementation
        public static Orbit Lerp(Orbit a, Orbit b, float t)
        {
            float yaw = Mathf.LerpAngle(a.yaw, b.yaw, t);
            float pitch = Mathf.LerpAngle(a.pitch, b.pitch, t);
            float roll = Mathf.LerpAngle(a.roll, b.roll, t);
            OrbitConstraint constraint = OrbitConstraint.Lerp(a.m_constraint, b.m_constraint, t);

            return new Orbit(yaw, pitch, roll, constraint);
        }

        public static Orbit Delta(Orbit current, Orbit target)
        {
            float yaw = Mathf.DeltaAngle(current.yaw, target.yaw);
            float pitch = Mathf.DeltaAngle(current.pitch, target.pitch);
            float roll = Mathf.DeltaAngle(current.roll, target.roll);

            return new Orbit(yaw, pitch, roll, current.m_constraint);
        }

        public static Orbit RotateTowards(Orbit current, Orbit target, float delta)
        {
            float yaw = Mathf.MoveTowardsAngle(current.yaw, target.yaw, delta);
            float pitch = Mathf.MoveTowardsAngle(current.pitch, target.pitch, delta);
            float roll = Mathf.MoveTowardsAngle(current.roll, target.roll, delta);

            return new Orbit(yaw, pitch, roll, current.m_constraint);
        }

        private static Vector3 CalculateDirection(float yaw, float pitch)
        {
            float radianYaw = yaw * Mathf.Deg2Rad;
            float radianPitch = pitch * Mathf.Deg2Rad;
            float diameterScalar = Mathf.Cos(radianPitch);

            float y = Mathf.Sin(radianPitch);
            float x = Mathf.Cos(radianYaw) * diameterScalar;
            float z = Mathf.Sin(radianYaw) * diameterScalar;

            return new Vector3(x, y, z);
        }

        private static Quaternion CalculateRotation(float yaw, float pitch, float roll)
        {
            return Quaternion.Euler(pitch, -yaw - 90f, -roll);
        }

        private static Orbit DirectionToOrbit(Vector3 direction)
        {
            return DirectionToOrbit(direction, OrbitConstraint.full);
        }

        private static Orbit DirectionToOrbit(Vector3 direction, Vector3 upwards)
        {
            return DirectionToOrbit(direction, upwards, OrbitConstraint.full);
        }

        private static Orbit QuaternionToOrbit(Quaternion rotation)
        {
            return QuaternionToOrbit(rotation, OrbitConstraint.full);
        }

        private static Orbit DirectionToOrbit(Vector3 direction, OrbitConstraint constraint)
        {
            return DirectionToOrbit(direction, Vector3.up, constraint);
        }

        private static Orbit DirectionToOrbit(Vector3 direction, Vector3 up, OrbitConstraint constraint)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, up);

            return QuaternionToOrbit(rotation, constraint);
        }

        private static Orbit QuaternionToOrbit(Quaternion rotation, OrbitConstraint constraint)
        {
            Vector3 angles = rotation.eulerAngles;

            float yaw = WrapAxis((-angles.y - 90f));
            float pitch = WrapAxis(angles.x);
            float roll = WrapAxis(-angles.z);

            return new Orbit(yaw, pitch, roll, constraint);
        }

        public static float WrapAxis(float value)
        {
            return Mathf.Repeat(value, FullRotation);
        }
        #endregion


        #region Operator Overloads
        public static Orbit operator +(Orbit firstAddend, Orbit secondAddend)
        {
            float yaw = (firstAddend.yaw + secondAddend.yaw);
            float pitch = (firstAddend.pitch + secondAddend.pitch);
            float roll = (firstAddend.roll + secondAddend.roll);

            return new Orbit(yaw, pitch, roll, firstAddend.m_constraint);
        }

        public static Orbit operator -(Orbit minuend, Orbit subtrahend)
        {
            float yaw = (minuend.yaw - subtrahend.yaw);
            float pitch = (minuend.pitch - subtrahend.pitch);
            float roll = (minuend.roll - subtrahend.roll);

            return new Orbit(yaw, pitch, roll, minuend.m_constraint);
        }

        public static Orbit operator *(Orbit multiplicand, Orbit multiplier)
        {
            float yaw = (multiplicand.yaw * multiplier.yaw);
            float pitch = (multiplicand.pitch * multiplier.pitch);
            float roll = (multiplicand.roll * multiplier.roll);

            return new Orbit(yaw, pitch, roll, multiplicand.m_constraint);
        }

        public static Orbit operator *(Orbit multiplicand, float multiplier)
        {
            float yaw = (multiplicand.yaw * multiplier);
            float pitch = (multiplicand.pitch * multiplier);
            float roll = (multiplicand.roll * multiplier);

            return new Orbit(yaw, pitch, roll, multiplicand.m_constraint);
        }
        #endregion


        #region Serialization Callback Receiver Implementation
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            WrapAxes();
            CalculateTransformations();
        }
        #endregion
    }
}