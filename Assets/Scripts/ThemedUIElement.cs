using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ThemedUIElement : MonoBehaviour {
    public ThemedUIData themeController;
    Color outline;
    Image image;
    GameObject message;

    public enum OutlineStyle { solidThin, solidThick, dottedThin, dottedThick};
    public bool hasImage = false;
    public bool isText = false;

    private void Awake() {
        OnSkinUI();
    }

    private void Update() {
        OnSkinUI();
    }

    private void OnSkinUI() {
        if (hasImage) {
            image = GetComponent<Image>();
            image.color = themeController.currentColor;
        }

        message = gameObject;

        if (isText) {
            message.GetComponent<TextMeshPro>().color = themeController.textColor;
        }
    }
}