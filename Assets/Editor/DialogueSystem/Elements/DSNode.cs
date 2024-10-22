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
        public DSGroup group;
        private DSGraphView graphView;

        public virtual void Initialize(DSGraphView graphView, Vector2 position) {
            dialogueName = "DialogueName";
            choices = new List<string>();
            dialogueText = "Dialogue Text";
            this.graphView = graphView;
            
            SetPosition(new Rect(position, Vector2.zero));
            
            mainContainer.AddClasses("ds-node-main-container");
            extensionContainer.AddClasses("ds-node-extension-container");
        }

        public virtual void Draw() {
            TextField dialogueTextField = DSElementUtility.CreateTextField(dialogueName, null, callBack => {
                TextField target = (TextField) callBack.target;
                target.value = callBack.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                
                if (group == null) {
                    graphView.RemoveUngroupedNode(this);
                    dialogueName = target.value;
                    graphView.AddUngroupedNode(this);
                    return;
                }
                
                DSGroup currentGroup = group;
                
                graphView.RemoveGroupedNode(this, group);
                dialogueName = target.value;
                graphView.AddGroupedNode(this, currentGroup);
            });
            
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
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
            evt.menu.AppendAction("Disconnect input ports", action => DisconnectPorts(inputContainer));
            evt.menu.AppendAction("Disconnect output ports", action => DisconnectPorts(outputContainer));
            
            base.BuildContextualMenu(evt);
        }

        public void DisconnectAllPorts() {
            DisconnectPorts(inputContainer);
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container) {
            foreach (Port port in container.Children()) {
                if (!port.connected) {
                    continue;
                }
                
                graphView.DeleteElements(port.connections);
            }
        }

        public void SetErrorColor(Color color) {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetColor() {
            mainContainer.style.backgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f); //Need to divide by 255 because the backgroundColor can't be Color32
        }
    }
}