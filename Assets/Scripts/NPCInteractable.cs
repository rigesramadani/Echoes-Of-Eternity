using UnityEngine;

public class NPCInteractable : MonoBehaviour, InteractableInterface {
    [SerializeField] private string interactionText;

    public void Interact() { }

    public string GetInteractionText() {
        return interactionText;
    }

    public Transform GetTransform() {
        return transform;
    }
}