using DS.Data.Save;
using DS.Enums;
using DS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Elements {
    public class DSSingleChoiceNode : DSNode {
        public override void Initialize(string name, DSGraphView graphView, Vector2 position) {
            base.Initialize(name, graphView, position);

            dialogueType = DSDialogueType.SingleChoice;
            DSChoiceSaveData choiceData = new DSChoiceSaveData();
            choiceData.text = "Next Dialogue";
            choices.Add(choiceData);
        }

        public override void Draw() {
            base.Draw();

            foreach (DSChoiceSaveData choice in choices) {
                Port choicePort = this.CreatePort(choice.text);
                choicePort.userData = choice;
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}