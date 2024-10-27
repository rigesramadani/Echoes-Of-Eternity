using System.IO;
using DS.Utilities;
using DS.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DS.Windows {
    public class DSEditorWindow : EditorWindow {
        private static TextField fileNameTextField;
        private Button saveButton;
        private DSGraphView graphView;
        
        [MenuItem("Window/DS/Dialogue Graph")]
        public static void ShowExample() {
            GetWindow<DSEditorWindow>("Dialogue Graph");
        }

        private void OnEnable() {
            AddGraphView();
            AddToolBar();
            
            AddStyles();
        }

        private void AddGraphView() {
            graphView = new DSGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void AddToolBar() {
            Toolbar toolbar = new Toolbar();
            fileNameTextField = DSElementUtility.CreateTextField("DialogueFileName", "File name:",
                callback => {
                    fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                });
            saveButton = DSElementUtility.CreateButton("Save", () => Save());

            Button clearButton = DSElementUtility.CreateButton("Clear", () => graphView.ClearGraph());
            Button loadButton = DSElementUtility.CreateButton("Load", () => Load());
            Button resetButton = DSElementUtility.CreateButton("Reset Name", () => UpdateFileName("DialogueFileName"));
            
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(clearButton);
            toolbar.Add(loadButton);
            toolbar.Add(resetButton);
            
            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");
            
            rootVisualElement.Add(toolbar);
        }

        private void AddStyles() {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }

        public void EnableSaveButton() {
            saveButton.SetEnabled(true);
        }
        
        public void DisableSaveButton() {
            saveButton.SetEnabled(false);
        }

        private void Save() {
            if (string.IsNullOrEmpty(fileNameTextField.value)) {
                EditorUtility.DisplayDialog("Error", "Please enter a file name", "OK");
                return;
            }
            
            DSSaveUtility.Initialize(graphView, fileNameTextField.value);
            DSSaveUtility.Save();
        }

        public static void UpdateFileName(string newFileName) {
            fileNameTextField.value = newFileName;
        }
        
        private void Load() {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");
            
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }
            
            graphView.ClearGraph();
            DSSaveUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            DSSaveUtility.Load();
        }
    }
}