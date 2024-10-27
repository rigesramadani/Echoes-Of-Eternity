using System;
using DS.ScriptableObjects;

namespace DS.Data {
    
    [Serializable]
    public class DSDialogueChoiceData {
        public string text;
        public DSDialogue nextDialogue;
    }
}