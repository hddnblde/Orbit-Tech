// 👨‍💻 John Carlo Cariño Gamboa
// 📧 hddnblde@gmail.com

using UnityEngine;

namespace HddnBlde.OrbitTech.Core
{
    [DisallowMultipleComponent]
    public abstract class Satellite : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private UpdateMode m_updateMode = UpdateMode.LateUpdate;

        [SerializeField]
        private Orbit m_orbit = Orbit.origin;

        [SerializeField]
        private float m_distance = 1f;

        [SerializeField]
        private Vector3 m_focalPosition = Vector3.zero;

        public const float MinDistance = 0f;
        public const float MaxDistance = 300f;
        private enum UpdateMode
        {
            LateUpdate,
            Manual
        }
        #endregion


        #region Properties
        protected Orbit orbit
        {
            get { return m_orbit; }
            set { m_orbit = value; }
        }

        public virtual float yaw
        {
            get { return m_orbit.yaw; }
            set { m_orbit.yaw = value; }
        }

        public virtual float pitch
        {
            get { return m_orbit.pitch; }
            set { m_orbit.pitch = value; }
        }

        public virtual float roll
        {
            get { return m_orbit.roll; }
            set { m_orbit.roll = value; }
        }

        public virtual float distance
        {
            get { return m_distance; }
            set { m_distance = Mathf.Clamp(value, MinDistance, MaxDistance); }
        }

        public virtual Vector3 focalPosition
        {
            get { return m_focalPosition + FocalPositionOffset(); }
        }

        public Vector3 orbitDirection
        {
            get { return m_orbit.direction; }
        }

        public Vector3 orbitPosition
        {
            get { return focalPosition + (orbitDirection * distance); }
        }

        public Quaternion orbitRotation
        {
            get { return m_orbit.rotation; }
        }

        private bool updateOrbitManually
        {
            get { return m_updateMode == UpdateMode.Manual; }
        }
        #endregion


        #region Orbit Functions
        protected virtual Vector3 FocalPositionOffset()
        {
            return Vector3.zero;
        }

        protected virtual Vector3 OrbitPositionOffset()
        {
            return Vector3.zero;
        }

        protected virtual Quaternion OrbitRotationOffset()
        {
            return Quaternion.identity;
        }
        #endregion


        #region Public Methods
        public virtual void SetOrbit(float yaw, float pitch, float roll)
        {
            m_orbit.Set(yaw, pitch, roll);
        }

        public void SetOrbit(Orbit orbit)
        {
            m_orbit.Clone(orbit);
        }

        public void SetOrbit(float yaw, float pitch)
        {
            SetOrbit(yaw, pitch, m_orbit.roll);
        }

        public void SetFocalPosition(Vector3 position)
        {
            m_focalPosition = position;
        }

        public void Rotate(float yaw, float pitch)
        {
            Rotate(yaw, pitch, 0f);
        }

        public void Rotate(float yaw, float pitch, float roll)
        {
            float yawValue = m_orbit.yaw + yaw;
            float pitchValue = m_orbit.pitch + pitch;
            float rollValue = m_orbit.roll + roll;

            SetOrbit(yawValue, pitchValue, rollValue);
        }

        public void Dolly(float forwardDelta)
        {
            distance += forwardDelta;
        }

        public void Lerp(Satellite from, Satellite to, float t)
        {
            Lerp(from, to, t, AnimationCurve.Linear(0f, 0f, 1f, 1f));
        }

        public virtual void Lerp(Satellite from, Satellite to, float t, AnimationCurve curve)
        {
            t = Mathf.Clamp01(t);
            float curvedT = curve.Evaluate(t);
            m_orbit = Orbit.Lerp(from.orbit, to.orbit, curvedT);
            m_distance = Mathf.LerpUnclamped(from.distance, to.distance, curvedT);
            m_focalPosition = Vector3.LerpUnclamped(from.focalPosition, to.focalPosition, curvedT);
        }

        public void UpdateOrbit()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Manual Update will only work during runtime.");
                return;
            }

            if (!updateOrbitManually)
            {
                Debug.LogWarning("Cannot update orbit manually! Make sure that 'Manual' is selected as update mode in the inspector.");
                return;
            }

            UpdateTransformation();
        }
        #endregion


        #region Validation Implementation
        protected virtual void OnInspectorReset()
        {
            m_orbit = new Orbit(transform.forward, transform.up);
            m_distance = 1f;
            m_focalPosition = (transform.position + transform.forward);
        }

        protected virtual void OnInspectorValidate(){}
        #endregion


        #region MonoBehaviour Implementation
        private void Reset()
        {
            OnInspectorReset();
            UpdateTransformation();
        }

        private void OnValidate()
        {
            ValidateDistance();
            UpdateTransformation();
            OnInspectorValidate();
        }

        private void LateUpdate()
        {
            if (!updateOrbitManually)
                UpdateTransformation();
        }
        #endregion


        #region Internal Methods
        private void ValidateDistance()
        {
            distance = m_distance;
        }

        private void UpdateTransformation()
        {
            Vector3 finalPosition = orbitPosition + OrbitPositionOffset();
            Quaternion finalRotation = orbitRotation * OrbitRotationOffset();
            transform.SetPositionAndRotation(finalPosition, finalRotation);
        }
        #endregion
    }
}