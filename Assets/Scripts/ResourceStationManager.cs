
using UnityEngine;

public class ResourceStationManager : MonoBehaviour
{
    public GameObject resourcePanel;

    void Start()
    {
        resourcePanel.SetActive(false);
    }

    void Update()
    {
        if (resourcePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            resourcePanel.SetActive(false);
        }
    }

    public void OpenResources()
    {
        resourcePanel.SetActive(true);
    }
}
