using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace JumpingBeast
{
    public class ButtonRestart : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(ResetLevel);
        }

        private void ResetLevel() => SceneManager.LoadScene(0);
    }
}