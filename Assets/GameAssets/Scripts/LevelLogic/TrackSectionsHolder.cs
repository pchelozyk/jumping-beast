using UnityEngine;

namespace JumpingBeast
{
    [CreateAssetMenu(menuName = "Track Sections Holder")]
    public class TrackSectionsHolder : ScriptableObject
    {
        [SerializeField]
        private TrackSection[] sections;

        public TrackSection GetRandom()
        {
            int i = Random.Range(0, sections.Length);
            return Instantiate(sections[i]);
        }
    }
}
