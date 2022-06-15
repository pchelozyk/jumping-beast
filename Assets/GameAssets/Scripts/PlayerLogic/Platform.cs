using System;
using UnityEngine;

namespace JumpingBeast
{
    public class Platform : MonoBehaviour
    {
        public event Action<GameObject> OnCollisionPlatform;
        public event Action<Platform> OnCollisionWall;

        private Rigidbody m_rigidbody;
        private Renderer m_renderer;

        public Vector3 Size => m_renderer.bounds.size;
        public Vector3 Extents => m_renderer.bounds.extents;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_renderer = GetComponent<Renderer>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 7)
                OnCollisionPlatform?.Invoke(collision.gameObject);

            // collision processed only when platform center point intersects with wall
            if (collision.gameObject.layer == 8)
            {
                int layerMask = 1 << 8;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), Size.z, layerMask))
                    OnCollisionWall?.Invoke(this);
            }
        }

        public void AddForce(Vector3 force)
        {
            m_rigidbody.constraints = RigidbodyConstraints.None;
            m_rigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
}