using UnityEngine;

namespace JumpingBeast
{
    public class CubeStackText : MonoBehaviour
    {
        public void Destroy() => Destroy(transform.parent.gameObject);
    }
}