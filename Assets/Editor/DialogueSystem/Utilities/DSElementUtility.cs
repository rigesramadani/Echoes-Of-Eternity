using System;
using DS.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DS.Utilities {
    public static class DSElementUtility {
        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null) {
            TextField textField = new TextField();
            textField.value = value;
            textField.label = label;
            
            if (onValueChanged != null) {
                textField.RegisterValueChangedCallback(onValueChanged);
            }
            
            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null) {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;
            
            return textArea;
        }

        public static Foldout CreateFoldout(string title, bool collapse = false) {
            Foldout foldout = new Foldout();
            foldout.text = title;
            foldout.value = !collapse;
            
            return foldout;
        }

        public static Button CreateButton(string text, Action onClick = null) {
            Button button = new Button(onClick);
            button.text = text;
            
            return button;
        }

        public static Port CreatePort(this DSNode node, string portName = "", Orientation orientation = Orientation.Horizontal, 
            Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single) {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            port.portName = portName;
            
            return port;
        }

    }
}