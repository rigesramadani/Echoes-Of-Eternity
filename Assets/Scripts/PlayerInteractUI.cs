using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour {
    [SerializeField] private GameObject container;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactionText;

    private void Update() {
        if (playerInteract.getInteractable() != null) {
            Show(playerInteract.getInteractable());
        } else {
            Hide();
        }
    }
    
    private void Show(InteractableInterface interactable) {
        container.SetActive(true);
        interactionText.text = interactable.GetInteractionText();
    }
    
    private void Hide() {
        container.SetActive(false);
    }
    
    
}