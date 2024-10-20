using System.Collections.Generic;
using DS.Enums;
using DS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements {
    public class DSNode : Node {
        public string dialogueName;
        public List<string> choices;
        public string dialogueText;
        public DSDialogueType dialogueType;

        public virtual void Initialize(Vector2 position) {
            dialogueName = "Dialogue Name";
            choices = new List<string>();
            dialogueText = "Dialogue Text";

            SetPosition(new Rect(position, Vector2.zero));
            
            mainContainer.AddClasses("ds-node-main-container");
            extensionContainer.AddClasses("ds-node-extension-container");
        }

        public virtual void Draw() {
            TextField dialogueTextField = DSElementUtility.CreateTextField(dialogueName);
            dialogueTextField.AddClasses("ds-node-textfield", "ds-node-filename-textfield", "ds-node-textfield-hidden");
            titleContainer.Insert(0, dialogueTextField);

            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddClasses("ds-node-custom-data-container");
            
            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");
            
            TextField textTextField = DSElementUtility.CreateTextArea(dialogueText);
            textTextField.AddClasses("ds-node-textfield", "ds-node-quote-textfield");
            
            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
        }
    }
}