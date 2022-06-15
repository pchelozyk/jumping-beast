using UnityEngine;
using UnityEngine.Events;

namespace JumpingBeast
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnPlayerDead;

        private Rigidbody m_rigidbody;
        private Collider m_collider;
        private Animator animator;
        private int jumpAnimID;

        public bool IsDead { get; private set; }

        private void Awake()
        {
            IsDead = false;

            animator = GetComponent<Animator>();
            m_collider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            jumpAnimID = Animator.StringToHash("Jump");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsDead) return;

            int layerMask = 1 << 8;
            if (collision.gameObject.layer == 8 && Physics.Raycast(m_collider.bounds.center, Vector3.back, m_collider.bounds.size.z, layerMask))
                Kill();
        }

        public void Kill()
        {
            IsDead = true;
            EnableRagdoll();
            OnPlayerDead?.Invoke();
        }

        private void EnableRagdoll()
        {
            animator.enabled = false;
            m_collider.enabled = false;
            m_rigidbody.constraints = RigidbodyConstraints.None;
        }

        public void Jump(float height)
        {
            if (!animator.enabled)
                return;

            animator.SetTrigger(jumpAnimID);
            transform.position = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        }
    }
}