using UnityEngine;
using TMPro;
using SharedModules.ED;

namespace Cheat
{
    public class CheatPanel : MonoBehaviour
    {
        [SerializeField] TMP_InputField levelInputField;

        public void OnClickLoadLevel()
        {
            if (int.TryParse(levelInputField.text, out int level))
            {
                DataManager.Ins.Data.LevelIndex = level - 1;
                LoadSceneManager.Ins.LoadScene(SceneId.Game);
            }
        }

        public void OnClickNextLevel()
        {
            DataManager.Ins.Data.LevelIndex++;
            LoadSceneManager.Ins.LoadScene(SceneId.Game);
        }

        public void OnClickPreviousLevel()
        {
            DataManager.Ins.Data.LevelIndex--;
            if (DataManager.Ins.Data.LevelIndex < 0)
            {
                DataManager.Ins.Data.LevelIndex = 0;
            }
            LoadSceneManager.Ins.LoadScene(SceneId.Game);
        }
    }
}
