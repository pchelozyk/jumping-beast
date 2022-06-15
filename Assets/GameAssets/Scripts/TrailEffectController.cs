using UnityEngine;

namespace JumpingBeast
{
    public class TrailEffectController : MonoBehaviour
    {
        [SerializeField] private TrackController trackController;
                         private ParticleSystem trailEffect;

        private void Awake()
        {
            trailEffect = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            float trackSpeed = trackController.GetSpeed();

            if (trackSpeed == 0 && trailEffect.isPlaying)
                trailEffect.Pause();
            if (trackSpeed > 0 && trailEffect.isStopped)
                trailEffect.Play();
        }
    }
}