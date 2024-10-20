using DS.Enums;
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

            Button addChoiceButton = new Button();
            addChoiceButton.text = "Add Choice";
            addChoiceButton.AddToClassList("ds-node-button");
            mainContainer.Insert(1, addChoiceButton);
            
            foreach (string choice in choices) {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                choicePort.portName = "";

                Button deleteChoiceButton = new Button();
                deleteChoiceButton.text = "Delete";
                deleteChoiceButton.AddToClassList("ds-node-button");
                
                TextField choiceTextField = new TextField();
                choiceTextField.value = choice;
                choiceTextField.AddToClassList("ds-node-textfield");
                choiceTextField.AddToClassList("ds-node-choice-textfield");
                choiceTextField.AddToClassList("ds-node-textfield-hidden");
                
                choicePort.Add(choiceTextField);
                choicePort.Add(deleteChoiceButton);
                
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}