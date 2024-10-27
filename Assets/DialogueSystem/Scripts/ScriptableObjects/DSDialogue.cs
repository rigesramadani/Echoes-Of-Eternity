using System.Collections.Generic;
using DS.Data;
using DS.Enums;
using UnityEngine;

namespace DS.ScriptableObjects {
    public class DSDialogue : ScriptableObject {
        public string dialogueName;
        [TextArea] public string dialogueText;
        public List<DSDialogueChoiceData> choices;
        public DSDialogueType dialogueType;
        public bool isStartingDialogue;

        public void Initialize(string dialogueName, string dialogueText, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue) {
            this.dialogueName = dialogueName;
            this.dialogueText = dialogueText;
            this.choices = choices;
            this.dialogueType = dialogueType;
            this.isStartingDialogue = isStartingDialogue;
        }
    }
}