
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject resourcesPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("RehabilitationRoom");
    }

    public void ShowResources()
    {
        resourcesPanel.SetActive(true);
    }

    public void CloseResources()
    {
        resourcesPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
