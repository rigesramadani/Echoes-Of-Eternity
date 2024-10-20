using System.Collections.Generic;
using DS.Enums;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements {
    public class DSNode : Node {
        public string dialogueName;
        public List<string> choices;
        public string dialogueText;
        public DSDialogueType dialogueType;

        public void Initialize(Vector2 position) {
            dialogueName = "Dialogue Name";
            choices = new List<string>();
            dialogueText = "Dialogue Text";

            SetPosition(new Rect(position, Vector2.zero));
        }

        public void Draw() {
            TextField dialogueTextField = new TextField();
            dialogueTextField.value = dialogueName;
            titleContainer.Insert(0, dialogueTextField);

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            Foldout textFoldout = new Foldout();
            textFoldout.text = "Dialogue Text";
            TextField textTextField = new TextField();
            textTextField.value = dialogueText;
            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}