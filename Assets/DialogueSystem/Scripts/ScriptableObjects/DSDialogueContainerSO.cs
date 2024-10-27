using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects {
    public class DSDialogueContainerSO : ScriptableObject {
        public string fileName;
        public SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>> dialogueGroups;
        public List<DSDialogueSO> ungroupedDialogues;

        public void Initialize(string fileName) {
            this.fileName = fileName;
            dialogueGroups = new SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>>();
            ungroupedDialogues = new List<DSDialogueSO>();
        }
    }
}