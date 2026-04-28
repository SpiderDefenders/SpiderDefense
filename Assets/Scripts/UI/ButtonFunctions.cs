using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Optional References")]
    [SerializeField] private GameObject settingsPanel;
    
    private PauseUI pauseUI;
    
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    
    public void OpenSettings()
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public void ContinueGame()
    {
        if (pauseUI == null) pauseUI = FindAnyObjectByType<PauseUI>();
        if (pauseUI == null) return;
        pauseUI.TogglePause(); 
    }
}