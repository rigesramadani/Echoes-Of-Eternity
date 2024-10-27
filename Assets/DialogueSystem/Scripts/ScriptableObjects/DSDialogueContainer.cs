using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects {
    public class DSDialogueContainer : ScriptableObject {
        public string fileName;
        public SerializableDictionary<DSDialogueGroup, List<DSDialogue>> dialogueGroups;
        public List<DSDialogue> ungroupedDialogues;

        public void Initialize(string fileName) {
            this.fileName = fileName;
            dialogueGroups = new SerializableDictionary<DSDialogueGroup, List<DSDialogue>>();
            ungroupedDialogues = new List<DSDialogue>();
        }
    }
}