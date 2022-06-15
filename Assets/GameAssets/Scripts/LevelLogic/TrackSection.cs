using UnityEngine;

namespace JumpingBeast
{
    public class TrackSection : MonoBehaviour
    {
        [SerializeField] private Renderer sectionRenderer;

        public Vector3 Size => sectionRenderer.bounds.size;
    }
}