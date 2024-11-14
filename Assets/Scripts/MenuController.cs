using UnityEngine;

public class MenuController : MonoBehaviour {
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject aboutPanel;
    private GameObject currentPanel;

    public void GoToControlsPage() {
        if (currentPanel != null) {
            currentPanel.SetActive(false);
        } else {
            mainPanel.SetActive(false);
        }
        
        controlsPanel.SetActive(true);
        currentPanel = controlsPanel;
    }
    
    public void GoToAboutPage() {
        if (currentPanel != null) {
            currentPanel.SetActive(false);
        } else {
            mainPanel.SetActive(false);
        }
        
        aboutPanel.SetActive(true);
        currentPanel = aboutPanel;
    }

    public void GoBackToMainPage() {
        currentPanel.SetActive(false);
        mainPanel.SetActive(true);
        currentPanel = mainPanel;
    }

    public void QuitGame() {
        Application.Quit();
    }
}
