using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace JumpingBeast
{
    public class TrackController : MonoBehaviour
    {
        [SerializeField] private TrackSectionsHolder sectionsHolder;
        [SerializeField] private ParticleSystem warpEffect;
        [SerializeField] private TrackEnd trackEnd;
                         private List<TrackSection> sections;
                         private float trackSpeed;

        private Vector3 LastSectionPosition
        {
            get
            {
                Vector3 position = Vector3.zero;
                float lastSectionLength = 0.0f;

                foreach (var section in sections)
                    if (section.transform.position.z < position.z)
                    {
                        position = section.transform.position;
                        lastSectionLength = section.Size.z;
                    }
                        
                position.z -= lastSectionLength;

                return position;
            }
        }

        private Vector3 SectionSpawnPoint
        {
            get
            {
                Vector3 position = Vector3.zero;
                float heightOffset = 15.0f;

                foreach (var section in sections)
                    if (section.transform.position.z > position.z)
                        position = section.transform.position;

                position.y -= heightOffset;

                return position;
            }
        }

        private void Awake()
        {
            InitSections();
            trackSpeed = 0.0f;
            trackEnd.OnViewOutEvent += UpdateTrack;
        }

        private void Update()
        {
            if (trackSpeed > 0)
            {
                // Track movement
                foreach (var section in sections)
                    section.transform.position = Vector3.MoveTowards(section.transform.position, trackEnd.transform.position, trackSpeed * Time.deltaTime);
            }

            // apply warp effect
            if (trackSpeed > 0 && warpEffect.isStopped)
                warpEffect.Play();
            if (trackSpeed == 0 && warpEffect.isPlaying)
            {
                warpEffect.Stop();
                warpEffect.Clear();
            }
        }

        private void OnDisable() => trackEnd.OnViewOutEvent -= UpdateTrack;

        private void InitSections()
        {
            sections = new List<TrackSection>();

            foreach (Transform section in transform)
                sections.Add(section.GetComponent<TrackSection>());
        }

        private void UpdateTrack(TrackSection viewOutSection)
        {
            sections.Remove(viewOutSection);
            Destroy(viewOutSection.gameObject);
            
            
            StartCoroutine(AddNewSection());
        }
        
        private IEnumerator AddNewSection()
        {
            float animationSpeedBoost = 6.0f;
            float offsetX = 5.0f;

            TrackSection newSection = sectionsHolder.GetRandom();
            //newSection.transform.position = SectionSpawnPoint;
            Vector3 sectionSpawnPoint = SectionSpawnPoint;
            sectionSpawnPoint.x -= offsetX;
            newSection.transform.position = sectionSpawnPoint;

            // moving new section parallel to the track
            while (true)
            {
                newSection.transform.position = Vector3.MoveTowards(newSection.transform.position, 
                                                new Vector3(newSection.transform.position.x, newSection.transform.position.y, LastSectionPosition.z),
                                                trackSpeed * animationSpeedBoost * Time.deltaTime);

                if (Vector3.Distance(newSection.transform.position, new Vector3(newSection.transform.position.x, newSection.transform.position.y, LastSectionPosition.z)) > 0.1f)
                    yield return null;
                else
                    break;
            }

            // moving new section upward
            while (true)
            {
                newSection.transform.position = Vector3.MoveTowards(newSection.transform.position, LastSectionPosition, trackSpeed * animationSpeedBoost * Time.deltaTime);

                if (Vector3.Distance(newSection.transform.position, LastSectionPosition) > 0.1f)
                    yield return null;
                else
                    break;
            }

            newSection.transform.SetParent(transform, true);
            sections.Add(newSection);
        }

        public void SetSpeed(float value) => trackSpeed = Mathf.Clamp(value, 0.0f, 10.0f);
        public float GetSpeed() => trackSpeed; 
    }
}