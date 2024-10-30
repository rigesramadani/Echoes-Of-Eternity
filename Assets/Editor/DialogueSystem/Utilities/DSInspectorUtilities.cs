using System;
using UnityEditor;

namespace DS.Utilities {
    public static class DSInspectorUtilities {
        public static void DrawDisabledFields(Action action) {
            EditorGUI.BeginDisabledGroup(true);
            action.Invoke();
            EditorGUI.EndDisabledGroup();
        }
        
        public static void DrawHeader(string lable) {
            EditorGUILayout.LabelField(lable, EditorStyles.boldLabel);
        }
        
        public static void DrawPropertyField(this SerializedProperty property) {
            EditorGUILayout.PropertyField(property);
        }
        
        public static int DrawPopup(string lable, SerializedProperty selectedIndexProperty, string[] options) {
            return EditorGUILayout.Popup(lable, selectedIndexProperty.intValue, options);
        }
        
        public static int DrawPopup(string lable, int selectedIndexProperty, string[] options) {
            return EditorGUILayout.Popup(lable, selectedIndexProperty, options);
        }
        
        public static void DrawSpace(int space = 7) {
            EditorGUILayout.Space(space);
        }

        public static void DrawHelpBox(string message, MessageType messageType = MessageType.Info, bool wide = true) {
            EditorGUILayout.HelpBox(message, messageType, wide);
        }
    }
}
