using System;
using System.Collections.Generic;
using DS.Elements;
using DS.Enums;
using DS.Utilities;
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

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        List<Port> compatiblePorts = new List<Port>();
        ports.ForEach(port => {
            if (startPort.node == port.node) {
                return;
            }

            if (startPort.direction == port.direction) {
                return;
            }
            
            compatiblePorts.Add(port);
        });
        
        return compatiblePorts;
    }

    private void AddManipulators() {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new SelectionDragger()); // Needs to be added before the RectangleSelector to work
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ContentDragger());

        this.AddManipulator(CreateNodeContextualMenu("Add single choice node", DSDialogueType.SingleChoice));
        this.AddManipulator(CreateNodeContextualMenu("Add multiple choice node", DSDialogueType.MultipleChoice));
        this.AddManipulator(CreateGroupContextualMenu());
    }

    private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType) {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle,
                actionEvent => AddElement(CreateNode(dialogueType, actionEvent.eventInfo.localMousePosition)))
        );

        return contextualMenuManipulator;
    }
    
    private IManipulator CreateGroupContextualMenu() {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Group",
                actionEvent => AddElement(CreateGroup("Dialogue Group", actionEvent.eventInfo.localMousePosition)))
        );

        return contextualMenuManipulator;
    }

    private Group CreateGroup(string title, Vector2 eventInfoLocalMousePosition) {
        Group group = new Group();
        group.title = title;
        group.SetPosition(new Rect(eventInfoLocalMousePosition, Vector2.zero));

        return group;
    }

    private void AddGridBackground() {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles() {
        this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss", "DialogueSystem/DSNodeStyles.uss");
    }

    private DSNode CreateNode(DSDialogueType dialogueType, Vector2 position) {
        Type nodeType = Type.GetType("DS.Elements.DS" + dialogueType + "Node");

        DSNode node = (DSNode)Activator.CreateInstance(nodeType);
        node.Initialize(position);
        node.Draw();

        return node;
    }
}