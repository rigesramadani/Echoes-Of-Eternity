using System.Collections.Generic;
using DS.ScriptableObjects;
using DS.Utilities;
using DS.Utility;
using UnityEditor;

namespace DS.Inspectors {
    [CustomEditor(typeof(DSDialogue))]
    public class DSInspector : Editor {
        private SerializedProperty dialogueContainerProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;
        private SerializedProperty groupedDialoguesProperty;
        private SerializedProperty startingDialoguesOnlyProperty;
        private SerializedProperty selectedDialogueGroupIndexProperty;
        private SerializedProperty selectedDialogueIndexProperty;

        private void OnEnable() {
            dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");
            groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
            startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");
            selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
            selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawDialogueContainerArea();
            
            DSDialogueContainerSO dialogueContainer = (DSDialogueContainerSO) dialogueContainerProperty.objectReferenceValue;
            
            if (dialogueContainer == null) {
                StopDrawing("No dialogue container selected.");
                return;
                
            }
            
            DrawFiltersArea();
            
            bool currentStartingDialoguesOnlyFilter = startingDialoguesOnlyProperty.boolValue;

            List<string> dialogueNames;
            string dialogueFolderPath = "Assets/DialogueSystem/Dialogues/" + dialogueContainer.fileName;
            string dialogueInfoMessage;

            if (groupedDialoguesProperty.boolValue) {
                List<string> dialogueGroupNames = dialogueContainer.GetGroupNames();

                if (dialogueGroupNames.Count == 0) {
                    StopDrawing("The selected container has no dialogue groups.");
                    return;
                }
                
                DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);
                DSDialogueGroupSO dialogueGroup = (DSDialogueGroupSO) dialogueGroupProperty.objectReferenceValue;
                dialogueNames = dialogueContainer.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += "/Groups/" + dialogueGroup.groupName + "/Dialogues";
                dialogueInfoMessage = "There are no" + (currentStartingDialoguesOnlyFilter ? " starting" : "") +" dialogues in this dialogue group.";
            } else {
                dialogueNames = dialogueContainer.GetUngroupedNames(currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += "/Global/Dialogues/";
                dialogueInfoMessage = "There are no" + (currentStartingDialoguesOnlyFilter ? " starting" : "") +" ungrouped dialogues in this dialogue container.";
            }

            if (dialogueNames.Count == 0) {
                StopDrawing(dialogueInfoMessage);
                return;
            }
            
            DrawDialogueArea(dialogueNames, dialogueFolderPath);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDialogueContainerArea() {
            DSInspectorUtilities.DrawHeader("Dialogue Container");
            dialogueContainerProperty.DrawPropertyField();
            DSInspectorUtilities.DrawSpace();
        }
        
        private void DrawFiltersArea() {
            DSInspectorUtilities.DrawHeader("Filters");
            groupedDialoguesProperty.DrawPropertyField();
            startingDialoguesOnlyProperty.DrawPropertyField();
            DSInspectorUtilities.DrawSpace();
        }
        
        private void DrawDialogueGroupArea(DSDialogueContainerSO dialogueContainer, List<string> dialogueGroupNames) {
            DSInspectorUtilities.DrawHeader("Dialogue Group");
            int oldSelectedDialogueGroupIndex = selectedDialogueGroupIndexProperty.intValue;
            DSDialogueGroupSO oldDialogueGroup = (DSDialogueGroupSO) dialogueGroupProperty.objectReferenceValue;
            bool isOldDialogueGroupNull = oldDialogueGroup == null;
            string oldDialogueGroupName = !oldDialogueGroup ? "" : oldDialogueGroup.name;
            UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueGroupIndexProperty, oldSelectedDialogueGroupIndex, oldDialogueGroupName, isOldDialogueGroupNull);
            selectedDialogueGroupIndexProperty.intValue = DSInspectorUtilities.DrawPopup("Dialogue Group", selectedDialogueGroupIndexProperty.intValue, dialogueGroupNames.ToArray());
            string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];
            DSDialogueGroupSO selectedDialogueGroup = DSSaveUtility.LoadAsset<DSDialogueGroupSO>("Assets/DialogueSystem/Dialogues/" + dialogueContainer.fileName + "/Groups/" + selectedDialogueGroupName, selectedDialogueGroupName);
            dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;
            DSInspectorUtilities.DrawDisabledFields(() => dialogueGroupProperty.DrawPropertyField());
            DSInspectorUtilities.DrawSpace();
        }
        
        private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath) {
            DSInspectorUtilities.DrawHeader("Dialogue");
            int oldSelectedDialogueIndex = selectedDialogueIndexProperty.intValue;
            DSDialogueSO oldDialogue = (DSDialogueSO) dialogueProperty.objectReferenceValue;
            bool isOldDialogueNull = oldDialogue == null;
            string oldDialogueName = !oldDialogue ? "" : oldDialogue.dialogueName;
            UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectedDialogueIndex, oldDialogueName, isOldDialogueNull);
            selectedDialogueIndexProperty.intValue = DSInspectorUtilities.DrawPopup("Dialogue", selectedDialogueIndexProperty.intValue, dialogueNames.ToArray());
            string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];
            DSDialogueSO selectedDialogue = DSSaveUtility.LoadAsset<DSDialogueSO>(dialogueFolderPath, selectedDialogueName);
            dialogueProperty.objectReferenceValue = selectedDialogue;
            DSInspectorUtilities.DrawDisabledFields(() => dialogueProperty.DrawPropertyField());
        }
        
        private void StopDrawing(string message, MessageType messageType = MessageType.Info) {
            DSInspectorUtilities.DrawHelpBox(message, messageType);
            serializedObject.ApplyModifiedProperties();
        }
        
        private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull) {
            if (isOldPropertyNull) {
                indexProperty.intValue = 0;
                return;
            }
            
            if (oldSelectedPropertyIndex > optionNames.Count - 1 || oldPropertyName != optionNames[oldSelectedPropertyIndex]) {
                if (optionNames.Contains(oldPropertyName)) {
                    indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                } else {
                    indexProperty.intValue = 0;
                }
            }
        }
    }
}