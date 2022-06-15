using System.Collections.Generic;
using System.Collections;
using Cinemachine;
using UnityEngine;

namespace JumpingBeast
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Transform platformsParent;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private ParticleSystem stackEffect;
        [SerializeField] private GameObject cubeStackText;

        private List<Platform> platforms;

        private readonly float movementSpeed = 15.0f;
        private readonly float wallHitForce = 10.0f;
        private readonly float movementBoundsSize = 2.0f;
        
        private float screenWidth;
        private float height;

        private void Awake() => screenWidth = (float)Screen.width / 2.0f;

        private void Start()
        {
            InitBasePlatforms();
        }

        private void Update()
        {
            if (player.IsDead)
                return;

            //platforms & player movement
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 pos = touch.position;
                pos.x = -((pos.x - screenWidth) / screenWidth);
                pos.y = 0.0f;
                pos.z = 0.0f;

                if ((pos.x > 0.0f && transform.position.x < movementBoundsSize) ||
                    (pos.x < 0.0f && transform.position.x > -movementBoundsSize))
                    transform.Translate(pos * movementSpeed * Time.deltaTime);
            }
        }
               

        private void InitBasePlatforms()
        {
            platforms = new List<Platform>();
            height = 0.0f;

            foreach (Transform wrapper in platformsParent)
            {
                Platform platform = wrapper.GetChild(0).GetComponent<Platform>();
                platforms.Add(platform);

                height += platform.Size.y;

                platform.OnCollisionWall += ProcessWallCollision;
                platform.OnCollisionPlatform += ProcessPlatformCollision;
            }
        }

        private void ProcessWallCollision(Platform platform)
        {
            height -= platform.Size.y;

            platform.OnCollisionWall -= ProcessWallCollision;
            platform.OnCollisionPlatform -= ProcessPlatformCollision;

            platforms.Remove(platform);

            // find nearest track section & set it as new parent
            int layerMask = 1 << 6;
            if (Physics.Raycast(platform.transform.position, platform.transform.TransformDirection(Vector3.down), out var hit, Mathf.Infinity, layerMask))
            {
                Transform trackSection = hit.transform.parent.parent;
                Transform platformWrapper = platform.transform.parent;
                platformWrapper.SetParent(trackSection, true);
            }
            
            platform.AddForce(Vector3.forward * wallHitForce); // additional force after collision for better visual
            VibrationTool.Vibrate();
            StartCoroutine(ShakeCamera(0.5f, 1.0f));

            if (platforms.Count == 0)
                player.Kill();
        }

        private void ProcessPlatformCollision(GameObject platformAsGameObject)
        {
            if (PlatformsContains(platformAsGameObject))
                return;

            Platform platform = platformAsGameObject.GetComponent<Platform>();
            Transform platformWrapper = platform.transform.parent;
            Vector3 platformCenter = new Vector3(transform.position.x, height + platform.Extents.y, transform.position.z);

            platformWrapper.position = platformCenter;
            platformWrapper.SetParent(platformsParent, true);

            // additional effect on platforms collision
            stackEffect.transform.position = platformCenter;
            stackEffect.Clear();
            stackEffect.Play();

            // additional text on platform collision
            Instantiate(cubeStackText, player.transform.position, Quaternion.identity);

            height += platform.Size.y;

            platforms.Add(platform);
            platform.OnCollisionWall += ProcessWallCollision;
            platform.OnCollisionPlatform += ProcessPlatformCollision;

            player.Jump(platform.Size.y);
        }

        private bool PlatformsContains(GameObject platform)
        {
            foreach (var item in platforms)
            {
                if (ReferenceEquals(item.gameObject, platform))
                    return true;
            }

            return false;
        }

        private IEnumerator ShakeCamera(float duration, float amplitude)
        {
            CinemachineBasicMultiChannelPerlin temp = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            temp.m_AmplitudeGain = amplitude;

            yield return new WaitForSeconds(duration);

            temp.m_AmplitudeGain = 0.0f;
        }
    }
}