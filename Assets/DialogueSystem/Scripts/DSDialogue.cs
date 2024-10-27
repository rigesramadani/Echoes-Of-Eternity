using DS.ScriptableObjects;
using UnityEngine;

namespace DS {
    public class DSDialogue : MonoBehaviour {
        [SerializeField] private DSDialogueContainerSO dialogueContainer;
        [SerializeField] private DSDialogueGroupSO dialogueGroup;
        [SerializeField] private DSDialogueSO dialogue;
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;
    }
}
