using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    private static readonly int Animate = Animator.StringToHash("Animate");
    private Animator cameraAnimator;

    [Header("MENUS")] [Tooltip("THe first list of buttons")]
    public GameObject firstMenu;

    [Tooltip("The Menu for when the PLAY button is clicked")]
    public GameObject playMenu;

    [Tooltip("The Menu for when the EXIT button is clicked")]
    public GameObject exitMenu;

    public enum Theme {
        custom1,
        custom2,
        custom3
    };

    [Header("THEME SETTINGS")] public Theme theme;
    public ThemedUIData themeController;

    [Header("PANELS")] [Tooltip("The UI Panel that holds the CONTROLS window tab")]
    public GameObject controlsPanel;

    [Tooltip("The UI Panel that holds the GAME window tab")]
    public GameObject audioPanel;

    [Header("SETTINGS SCREEN")] [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
    public GameObject audioLine;

    [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
    public GameObject controlsLine;

    [Header("CONTROLS SETTINGS")] public GameObject musicSlider;

    [Header("SFX")] [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    public AudioSource hoverSound;

    void Start() {
        cameraAnimator = transform.GetComponent<Animator>();
        musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");

        playMenu.SetActive(false);
        exitMenu.SetActive(false);
        firstMenu.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetThemeColors();
    }

    void SetThemeColors() {
        switch (theme) {
            case Theme.custom1:
                themeController.currentColor = themeController.custom1.graphic1;
                themeController.textColor = themeController.custom1.text1;
                break;
            case Theme.custom2:
                themeController.currentColor = themeController.custom2.graphic2;
                themeController.textColor = themeController.custom2.text2;
                break;
            case Theme.custom3:
                themeController.currentColor = themeController.custom3.graphic3;
                themeController.textColor = themeController.custom3.text3;
                break;
            default:
                Debug.Log("Invalid theme selected.");
                break;
        }
    }

    public void PlayCampaign() {
        exitMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void ReturnMenu() {
        playMenu.SetActive(false);
        exitMenu.SetActive(false);
    }

    public void GoToSettings() {
        playMenu.SetActive(false);
        cameraAnimator.SetFloat(Animate, 1);
    }

    public void GoToMainPage() {
        cameraAnimator.SetFloat(Animate, 0);
    }

    void DisablePanels() {
        controlsPanel.SetActive(false);
        audioPanel.SetActive(false);

        audioLine.SetActive(false);
        controlsLine.SetActive(false);
    }

    public void AudioPanel() {
        DisablePanels();
        audioPanel.SetActive(true);
        audioLine.SetActive(true);
    }

    public void ControlsPanel() {
        DisablePanels();
        controlsPanel.SetActive(true);
        controlsLine.SetActive(true);
    }

    public void PlayHover() {
        hoverSound.Play();
    }

    public void AreYouSure() {
        exitMenu.SetActive(true);
        playMenu.SetActive(false);
    }

    public void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
				Application.Quit();
        #endif
    }

    public void MusicSlider() {
        PlayerPrefs.SetFloat("Volume", musicSlider.GetComponent<Slider>().value);
    }
}