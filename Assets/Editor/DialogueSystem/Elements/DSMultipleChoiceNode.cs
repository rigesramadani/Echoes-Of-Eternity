using DS.Enums;
using DS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements {
    public class DSMultipleChoiceNode : DSNode {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);

            dialogueType = DSDialogueType.MultipleChoice;
            choices.Add("New Choice");
        }

        public override void Draw() {
            base.Draw();

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () => {
                Port choicePort = CreateChoicePort("New Choice");
                choices.Add("New Choice");
                outputContainer.Add(choicePort);
            });
            addChoiceButton.AddClasses("ds-node-button");
            mainContainer.Insert(1, addChoiceButton);
            
            foreach (string choice in choices) {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(string choice) {
            Port choicePort = this.CreatePort();
            choicePort.portName = "";

            Button deleteChoiceButton = DSElementUtility.CreateButton("Delete Choice");
            deleteChoiceButton.AddClasses("ds-node-button");
                
            TextField choiceTextField = DSElementUtility.CreateTextField(choice);
            choiceTextField.AddClasses("ds-node-textfield", "ds-node-choice-textfield", "ds-node-textfield-hidden");
                
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            
            return choicePort;
        }
    }
}