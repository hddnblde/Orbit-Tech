// 👨‍💻 John Carlo Cariño Gamboa
// 📧 hddnblde@gmail.com

using UnityEngine;
using HddnBlde.OrbitTech.Core;

namespace HddnBlde.OrbitTech
{
	public class OrbitCamera : OrbitCameraLite
	{
		#region Serialized Fields
		[SerializeField]
		private float m_minDistance = Satellite.MinDistance;

		[SerializeField]
		private float m_maxDistance = 10f;

		[SerializeField]
		private float m_verticalOffset = 0f;

		[SerializeField]
		private Vector2 m_pan = Vector2.zero;
		#endregion


		#region Properties
		public override float distance
		{
			set { base.distance = Mathf.Clamp(value, m_minDistance, m_maxDistance); }
		}

		public float normalizedDistance
		{
			get { return Mathf.InverseLerp(m_minDistance, m_maxDistance, distance); }
			set
			{
				value = Mathf.Clamp01(value);
				distance = Mathf.Lerp(m_minDistance, m_maxDistance, value);
			}
		}

		public float verticalOffset
		{
			get { return m_verticalOffset; }
			set { m_verticalOffset = value; }
		}

		public Vector2 pan
		{
			get { return m_pan; }
			set { m_pan = value; }
		}
		#endregion


		#region Orbit Function Overrides
		protected override sealed Vector3 OrbitPositionOffset()
		{
			Vector3 panDirection = ((Vector3.right * m_pan.x) + (Vector3.up * m_pan.y));

			return orbitRotation * panDirection;
		}

		protected override sealed Vector3 FocalPositionOffset()
		{
			return (Vector3.up * m_verticalOffset);
		}
		#endregion


		#region Public Methods
		public void SetDistanceLimits(float min, float max)
		{
			m_minDistance = min;
			m_maxDistance = max;
			distance = distance;
		}
        #endregion


        #region Validation Implementation Override
        protected override void OnInspectorReset()
        {
			orbit = new Orbit(transform.forward, transform.up, OrbitConstraint.minimal);
            distance = 1f;
            SetFocalPosition(transform.position + transform.forward);
        }
        #endregion
    }
}