using UnityEngine;

namespace Cheat
{
    public class CheatConsole : MonoBehaviour
    {
        [SerializeField] bool isHideButton;
        [SerializeField] CheatButton cheatButton;
        [SerializeField] GameObject cheatPanel;

        public bool IsHideButton => isHideButton;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void OpenCheatPanel()
        {
            cheatPanel.SetActive(true);
        }

        public void CloseCheatPanel()
        {
            cheatPanel.SetActive(false);

            if (isHideButton)
            {
                cheatButton.ShowButtonTemporarily();
            }
        }
    }
}
