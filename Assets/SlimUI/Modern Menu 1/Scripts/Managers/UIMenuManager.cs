using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu {
    public class UIMenuManager : MonoBehaviour {
        private static readonly int Animate = Animator.StringToHash("Animate");
        private Animator CameraObject;

        [Header("MENUS")] [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;

        [Tooltip("THe first list of buttons")] public GameObject firstMenu;

        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject playMenu;

        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;

        [Tooltip("Optional 4th Menu")] public GameObject extrasMenu;
        
        [Header("PAGES")] [Tooltip("The settings page of the menu")]
        public GameObject settingsPage;
        
        [Tooltip("The controls page of the menu")] public GameObject controlsPage;

        public enum Theme {
            custom1,
            custom2,
            custom3
        };

        [Header("THEME SETTINGS")] public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")] [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;

        [Tooltip("The UI Panel that holds the CONTROLS window tab")]
        public GameObject PanelControls;

        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        public GameObject PanelVideo;

        [Tooltip("The UI Panel that holds the GAME window tab")]
        public GameObject PanelGame;

        [Header("SETTINGS SCREEN")] [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineGame;

        [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
        public GameObject lineVideo;

        [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
        public GameObject lineControls;

        [Header("LOADING SCREEN")] [Tooltip("If this is true, the loaded scene won't load until receiving user input")]
        public bool waitForInput = true;

        public GameObject loadingMenu;

        [Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;

        public TMP_Text loadPromptText;
        public KeyCode userPromptKey;

        [Header("SFX")] [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;

        void Start() {
            CameraObject = transform.GetComponent<Animator>();

            playMenu.SetActive(false);
            exitMenu.SetActive(false);
            extrasMenu.SetActive(false);
            firstMenu.SetActive(true);
            mainMenu.SetActive(true);

            SetThemeColors();
        }

        void SetThemeColors() {
            switch (theme) {
                case Theme.custom1:
                    themeController.currentColor = themeController.custom1.graphic1;
                    themeController.textColor = themeController.custom1.text1;
                    themeIndex = 0;
                    break;
                case Theme.custom2:
                    themeController.currentColor = themeController.custom2.graphic2;
                    themeController.textColor = themeController.custom2.text2;
                    themeIndex = 1;
                    break;
                case Theme.custom3:
                    themeController.currentColor = themeController.custom3.graphic3;
                    themeController.textColor = themeController.custom3.text3;
                    themeIndex = 2;
                    break;
                default:
                    Debug.Log("Invalid theme selected.");
                    break;
            }
        }

        public void PlayCampaign() {
            exitMenu.SetActive(false);
            extrasMenu.SetActive(false);
            playMenu.SetActive(true);
        }

        public void ReturnMenu() {
            playMenu.SetActive(false);
            extrasMenu.SetActive(false);
            exitMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void LoadScene(string scene) {
            if (scene != "") {
                StartCoroutine(LoadAsynchronously(scene));
            }
        }

        public void GoToSettings() {
            playMenu.SetActive(false);
            settingsPage.SetActive(true);
            controlsPage.SetActive(false);
            CameraObject.SetFloat(Animate, 1);
        }
        
        public void GoToControls() {
            playMenu.SetActive(false);
            settingsPage.SetActive(false);
            controlsPage.SetActive(true);
            CameraObject.SetFloat(Animate, 1);
        }

        public void GoToMainPage() {
            CameraObject.SetFloat(Animate, 0);
        }

        void DisablePanels() {
            PanelControls.SetActive(false);
            PanelVideo.SetActive(false);
            PanelGame.SetActive(false);

            lineGame.SetActive(false);
            lineControls.SetActive(false);
            lineVideo.SetActive(false);
        }

        public void GamePanel() {
            DisablePanels();
            PanelGame.SetActive(true);
            lineGame.SetActive(true);
        }

        public void VideoPanel() {
            DisablePanels();
            PanelVideo.SetActive(true);
            lineVideo.SetActive(true);
        }

        public void ControlsPanel() {
            DisablePanels();
            PanelControls.SetActive(true);
            lineControls.SetActive(true);
        }

        public void PlayHover() {
            hoverSound.Play();
        }


        public void AreYouSure() {
            exitMenu.SetActive(true);
            extrasMenu.SetActive(false);
            playMenu.SetActive(false);
        }

        public void QuitGame() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
				Application.Quit();
            #endif
        }

        IEnumerator LoadAsynchronously(string sceneName) {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            mainCanvas.SetActive(false);
            loadingMenu.SetActive(true);

            while (!operation.isDone) {
                float progress = Mathf.Clamp01(operation.progress / .95f);
                loadingBar.value = progress;

                if (operation.progress >= 0.9f && waitForInput) {
                    loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
                    loadingBar.value = 1;

                    if (Input.GetKeyDown(userPromptKey)) {
                        operation.allowSceneActivation = true;
                    }
                } else if (operation.progress >= 0.9f && !waitForInput) {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}