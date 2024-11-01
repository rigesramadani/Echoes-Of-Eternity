using DS;
using UnityEngine;

public class NPCInteractable : MonoBehaviour, InteractableInterface {
    [SerializeField] private string interactionText;
    [SerializeField] private string characterName;
    [SerializeField] private GameObject dialogueUI;

    public void Interact() {
        dialogueUI.GetComponent<DialogueUI>().StartDialogue(characterName, gameObject.GetComponent<DSDialogue>());
    }

    public string GetInteractionText() {
        return interactionText;
    }

    public Transform GetTransform() {
        return transform;
    }

    public string GetCharacterName() {
        return characterName;
    }
}