using UnityEngine;

public interface InteractableInterface {
    void Interact();
    string GetInteractionText();
    Transform GetTransform();
}