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

        public List<string> GetGroupNames() {
            List<string> groupNames = new List<string>();
            
            foreach (DSDialogueGroupSO dialogueGroup in dialogueGroups.Keys) {
                groupNames.Add(dialogueGroup.groupName);
            }
            
            return groupNames;
        }

        public List<string> GetGroupedDialogueNames(DSDialogueGroupSO dialogueGroup, bool startingDialoguesOnly) {
            List<DSDialogueSO> groupedDialogues = dialogueGroups[dialogueGroup];
            List<string> groupedDialogueNames = new List<string>();
            
            foreach (DSDialogueSO groupedDialogue in groupedDialogues) {
                if (startingDialoguesOnly && !groupedDialogue.isStartingDialogue) {
                    continue;
                }
                
                groupedDialogueNames.Add(groupedDialogue.dialogueName);
            }
            
            return groupedDialogueNames;
        }
        
        public List<string> GetUngroupedNames(bool startingDialoguesOnly) {
            List<string> ungroupedDialogueNames = new List<string>();
            
            foreach (DSDialogueSO ungroupedDialogue in ungroupedDialogues) {
                if (startingDialoguesOnly && !ungroupedDialogue.isStartingDialogue) {
                    continue;
                }
                
                ungroupedDialogueNames.Add(ungroupedDialogue.dialogueName);
            }
            
            return ungroupedDialogueNames;
        }

        public List<DSDialogueGroupSO> GetGroups() {
            List<DSDialogueGroupSO> groups = new List<DSDialogueGroupSO>();

            foreach (DSDialogueGroupSO dialogueGroup in dialogueGroups.Keys) {
                groups.Add(dialogueGroup);
            }
            
            return groups;
        }

        public List<DSDialogueSO> GetGroupDialogues(DSDialogueGroupSO group) {
            return dialogueGroups[group];
        }
    }
}