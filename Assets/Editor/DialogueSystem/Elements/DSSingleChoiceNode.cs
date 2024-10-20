using DS.Enums;
using DS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Elements {
    public class DSSingleChoiceNode : DSNode {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);

            dialogueType = DSDialogueType.SingleChoice;
            choices.Add("Next Dialogue");
        }

        public override void Draw() {
            base.Draw();

            foreach (string choice in choices) {
                Port choicePort = this.CreatePort(choice);
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}