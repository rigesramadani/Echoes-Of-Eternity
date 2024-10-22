using DS.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DS.Windows {
    public class DSEditorWindow : EditorWindow {
        private TextField fileNameTextField;
        private Button saveButton;
        
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
            DSGraphView graphView = new DSGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void AddToolBar() {
            Toolbar toolbar = new Toolbar();
            fileNameTextField = DSElementUtility.CreateTextField("DialogueFileName", "File name:",
                callback => {
                    fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                });
            saveButton = DSElementUtility.CreateButton("Save");
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            
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
    }
}