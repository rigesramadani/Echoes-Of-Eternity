using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {
    public void Update() {
        if (Input.GetKeyDown(KeyCode.E) && !PauseMenuController.isGamePaused) {
            InteractableInterface interactable = getInteractable();
            if (interactable != null) {
                interactable.Interact();
            }
        }
    }

    public InteractableInterface getInteractable() {
        List<InteractableInterface> interactableList = new List<InteractableInterface>();
        float interactRange = 2f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliders) {
            if (collider.TryGetComponent(out InteractableInterface interactable)) {
                interactableList.Add(interactable);
            }
        }
        
        InteractableInterface closestInteractable = null;
        foreach ( InteractableInterface interactable in interactableList) {
            if (closestInteractable == null) {
                closestInteractable = interactable;
            }
            else {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) < 
                    Vector3.Distance(transform.position, closestInteractable.GetTransform().position)) {
                    closestInteractable = interactable;
                }
            }
        }
        
        return closestInteractable;
    }
}