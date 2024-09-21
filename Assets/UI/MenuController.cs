using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    private UIDocument document;
    private VisualElement container;
    private Button startButton;
    private Button controlsButton;
    private Button aboutButton;
    private Button quitButton;
    private VisualElement controlsElements;
    private VisualElement aboutElements;
    public VisualTreeAsset controlsPage;
    public VisualTreeAsset aboutPage;
    
    void Start() {
        document = GetComponent<UIDocument>();
        container = document.rootVisualElement.Q<VisualElement>("Container");
        startButton = document.rootVisualElement.Q<Button>("StartButton");
        controlsButton = document.rootVisualElement.Q<Button>("ControlsButton");
        aboutButton = document.rootVisualElement.Q<Button>("AboutButton");
        quitButton = document.rootVisualElement.Q<Button>("QuitButton");
        
        startButton.clicked += StartButtonClicked;
        controlsButton.clicked += ControlsButtonClicked;
        aboutButton.clicked += AboutButtonClicked;
        quitButton.clicked += QuitButtonClicked;
        
        aboutElements = aboutPage.CloneTree();
        controlsElements = controlsPage.CloneTree();
        
        var backButtonControls = controlsElements.Q<Button>("BackButton");
        backButtonControls.clicked += BackButtonClicked;
        
        var backButtonAbout = aboutElements.Q<Button>("BackButton");
        backButtonAbout.clicked += BackButtonClicked;
    }

    private void StartButtonClicked() {
        SceneManager.LoadScene("Game");
    }
    
    private void AboutButtonClicked() {
        container.Clear();
        container.Add(aboutElements);
    }
    
    private void ControlsButtonClicked() {
        container.Clear();
        container.Add(controlsElements);
    }
    
    private void QuitButtonClicked() {
        Application.Quit();
    }
    
    private void BackButtonClicked() {
        container.Clear();
        container.Add(startButton);
        container.Add(controlsButton);
        container.Add(aboutButton);
        container.Add(quitButton);
    }
}
