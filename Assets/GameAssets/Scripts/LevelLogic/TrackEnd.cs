using System;
using UnityEngine;

namespace JumpingBeast
{
    public class TrackEnd : MonoBehaviour
    {
        public event Action<TrackSection> OnViewOutEvent;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 6)
            {
                TrackSection viewOutSection = other.transform.parent.parent.GetComponent<TrackSection>();
                OnViewOutEvent?.Invoke(viewOutSection);
            }
        }
    }
}