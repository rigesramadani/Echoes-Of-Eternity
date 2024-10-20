using DS.Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DSGraphView : GraphView {
    public DSGraphView() {
        AddManipulators();
        AddGridBackground();
        AddStyles();
    }

    private void AddManipulators() {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(CreateNodeContextualMenu());
        this.AddManipulator(new SelectionDragger()); // Needs to be added before the RectangleSelector to work
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ContentDragger());
    }

    private IManipulator CreateNodeContextualMenu() {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Node",
                actionEvent => AddElement(CreateNode(actionEvent.eventInfo.localMousePosition)))
        );

        return contextualMenuManipulator;
    }

    private void AddGridBackground() {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles() {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSGraphViewStyles.uss");
        styleSheets.Add(styleSheet);
    }

    private DSNode CreateNode(Vector2 position) {
        DSNode node = new DSNode();
        node.Initialize(position);
        node.Draw();

        return node;
    }
}