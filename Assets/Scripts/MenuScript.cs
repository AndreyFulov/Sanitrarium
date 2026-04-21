using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string optionsSceneName;

    public void NewGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void Continue()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene(optionsSceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}