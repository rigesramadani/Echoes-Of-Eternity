using DS.Data.Save;
using DS.Enums;
using DS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements {
    public class DSMultipleChoiceNode : DSNode {
        public override void Initialize(string name, DSGraphView graphView, Vector2 position) {
            base.Initialize(name, graphView, position);
            
            dialogueType = DSDialogueType.MultipleChoice;
            DSChoiceSaveData choiceData = new DSChoiceSaveData();
            choiceData.text = "New Choice";
            
            choices.Add(choiceData);
        }

        public override void Draw() {
            base.Draw();

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () => {
                DSChoiceSaveData choiceData = new DSChoiceSaveData();
                choiceData.text = "New Choice";
                choices.Add(choiceData);
                Port choicePort = CreateChoicePort(choiceData);
                outputContainer.Add(choicePort);
            });
            addChoiceButton.AddClasses("ds-node-button");
            mainContainer.Insert(1, addChoiceButton);
            
            foreach (DSChoiceSaveData choice in choices) {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData) {
            Port choicePort = this.CreatePort();
            choicePort.portName = "";
            choicePort.userData = userData;
            
            DSChoiceSaveData choiceData = (DSChoiceSaveData) userData;

            Button deleteChoiceButton = DSElementUtility.CreateButton("Delete Choice", () => {
                if (choices.Count == 1) {
                    return;
                }

                if (choicePort.connected) {
                    graphView.DeleteElements(choicePort.connections);
                }
                
                choices.Remove(choiceData);
                graphView.RemoveElement(choicePort);
            });
            deleteChoiceButton.AddClasses("ds-node-button");
                
            TextField choiceTextField = DSElementUtility.CreateTextField(choiceData.text, null, callback => {
                choiceData.text = callback.newValue;
            });
            choiceTextField.AddClasses("ds-node-textfield", "ds-node-choice-textfield", "ds-node-textfield-hidden");
                
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            
            return choicePort;
        }
    }
}