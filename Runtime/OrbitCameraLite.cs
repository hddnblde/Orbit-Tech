// 👨‍💻 John Carlo Cariño Gamboa
// 📧 hddnblde@gmail.com

using UnityEngine;
using HddnBlde.OrbitTech.Core;

namespace HddnBlde.OrbitTech
{
    public class OrbitCameraLite : Satellite
    {
        #region Serialized Fields
        [SerializeField]
        private Transform m_target = null;

        [SerializeField, Range(MinimumDampTime, MaximumDampTime)]
        private float m_focusDampTime = 0.05f;
        #endregion


        #region Internal Fields
        private const float CollisionOffset = 0.15f;
        private const float MinimumDampTime = 0f;
        private const float MaximumDampTime = 1f;
        private Vector3 m_smoothFocalPosition = Vector3.zero;
        private Vector3 m_focalPositionVelocity = Vector3.zero;
        #endregion


        #region MonoBehaviour Implementation
        protected virtual void Awake()
        {
            InitializeSmoothPosition();
        }
        #endregion


        #region Property Override
        public override Vector3 focalPosition
        {
            get
            {
                TrackTarget();

                Vector3 focalPositionOffset = FocalPositionOffset();
                Vector3 rawFocalPosition = base.focalPosition - focalPositionOffset;
                bool smoothInterpolation = Application.isPlaying && (m_focusDampTime > 0f);
                InterpolateFocalPosition(rawFocalPosition, smoothInterpolation);

                return m_smoothFocalPosition + focalPositionOffset;
            }
        }
        #endregion


        #region Public Methods
        public void SetDampTime(float value)
        {
            m_focusDampTime = Mathf.Clamp(value, MinimumDampTime, MaximumDampTime);
        }

        public void SetFocalPosition(Vector3 focalPosition, float dampTime)
        {
            SetDampTime(dampTime);
            SetFocalPosition(focalPosition);
        }

        public void SetFocalTarget(Transform target, float dampTime, bool snap = false)
        {
            SetDampTime(dampTime);
            SetFocalTarget(target, snap);
        }

        public void SetFocalTarget(Transform target, bool snap = false)
        {
            if (target == transform)
            {
                Debug.LogWarning("Cannot set self as focal target!");
                return;
            }

            m_target = target;

            if (snap && target != null)
                m_smoothFocalPosition = m_target.position;
        }
        #endregion


        #region Validation Implementation Override
        protected override void OnInspectorReset()
        {
            orbit = new Orbit(transform.forward, transform.up, OrbitConstraint.full);
            distance = 1f;
            SetFocalPosition(transform.position + transform.forward);
        }
        #endregion


        #region Internal Methods
        private void InitializeSmoothPosition()
        {
            m_smoothFocalPosition = base.focalPosition - FocalPositionOffset();
        }

        private void TrackTarget()
        {
            bool hasTarget = (m_target != null);

            if (hasTarget)
                SetFocalPosition(m_target.position);
        }

        private void InterpolateFocalPosition(Vector3 rawFocalPosition, bool smoothInterpolation)
        {
            float dampTime = (smoothInterpolation ? m_focusDampTime : 0f);
            m_smoothFocalPosition = GetSmoothDampPosition(m_smoothFocalPosition, rawFocalPosition, dampTime);
        }

        private Vector3 GetSmoothDampPosition(Vector3 current, Vector3 target, float dampTime)
        {
            if (dampTime > 0f)
                return Vector3.SmoothDamp(current, target, ref m_focalPositionVelocity, dampTime);
            else
                return target;
        }
        #endregion
    }
}