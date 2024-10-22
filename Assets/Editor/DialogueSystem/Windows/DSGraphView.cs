using System;
using System.Collections.Generic;
using System.Linq;
using DS.Data.Error;
using DS.Elements;
using DS.Enums;
using DS.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DSGraphView : GraphView {
    private SerializableDictionary<string, DSNodeErrorData> ungroupedNodes;
    private SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>> groupedNodes;
    
    public DSGraphView() {
        ungroupedNodes = new SerializableDictionary<string, DSNodeErrorData>();
        groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>>();
        
        AddManipulators();
        AddGridBackground();
        
        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        
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
        node.Initialize(this, position);
        node.Draw();

        AddUngroupedNode(node);
        return node;
    }
    
    private void OnElementsDeleted() {
        deleteSelection = (operationName, askUser) => {
            List<DSNode> nodesToDelete = new List<DSNode>();
            
            foreach (GraphElement element in selection) {
                if (element is DSNode node) {
                    nodesToDelete.Add(node);
                }
            }

            foreach (DSNode node in nodesToDelete) {
                if (node.group != null) {
                    node.group.RemoveElement(node);
                }
                RemoveUngroupedNode(node);
                RemoveElement(node);
            }
        };
    }

    public void AddUngroupedNode(DSNode node) {
        string nodeName = node.dialogueName;
        
        if (!ungroupedNodes.ContainsKey(nodeName)) {
            DSNodeErrorData nodeErrorData = new DSNodeErrorData();
            
            nodeErrorData.Nodes.Add(node);
            ungroupedNodes.Add(nodeName, nodeErrorData);
            return;
        }
        
        ungroupedNodes[nodeName].Nodes.Add(node);

        
        Color errorColor = ungroupedNodes[nodeName].ErrorData.color;
        node.SetErrorColor(errorColor);

        if (ungroupedNodes[nodeName].Nodes.Count == 2) {
            ungroupedNodes[nodeName].Nodes[0].SetErrorColor(errorColor);
        }
    }

    public void RemoveUngroupedNode(DSNode node) {
        string nodeName = node.dialogueName;
        ungroupedNodes[nodeName].Nodes.Remove(node);
        node.ResetColor();

        if (ungroupedNodes[nodeName].Nodes.Count == 1) {
            ungroupedNodes[nodeName].Nodes[0].ResetColor();
            return;
        }

        if (ungroupedNodes[nodeName].Nodes.Count == 0) {
            ungroupedNodes.Remove(nodeName);
        }
    }

    private void OnGroupElementsAdded() {
        elementsAddedToGroup = (group, elements) => {
            foreach (GraphElement element in elements) {
                if (!(element is DSNode)) {
                    continue;
                }

                DSNode node = (DSNode) element;
                RemoveUngroupedNode(node);
                AddGroupedNode(node, group);
            }
        };
    }

    private void OnGroupElementsRemoved() {
        elementsRemovedFromGroup = (group, elements) => {
            foreach (GraphElement element in elements) {
                if (!(element is DSNode)) {
                    continue;
                }

                DSNode node = (DSNode) element;
                
                RemoveGroupedNode(node, group);
                AddUngroupedNode(node);
            }
        };
    }

    public void AddGroupedNode(DSNode node, Group group) {
        string nodeName = node.dialogueName;
        node.group = group;
        
        if (!groupedNodes.ContainsKey(group)) {
            groupedNodes.Add(group, new SerializableDictionary<string, DSNodeErrorData>());
        }

        if (!groupedNodes[group].ContainsKey(nodeName)) {
            DSNodeErrorData nodeErrorData = new DSNodeErrorData();
            nodeErrorData.Nodes.Add(node);
            groupedNodes[group].Add(nodeName, nodeErrorData);
            return;
        }

        groupedNodes[group][nodeName].Nodes.Add(node);
        Color errorColor = groupedNodes[group][nodeName].ErrorData.color;
        node.SetErrorColor(errorColor);


        if (groupedNodes[group][nodeName].Nodes.Count == 2) {
            groupedNodes[group][nodeName].Nodes[0].SetErrorColor(errorColor);
        }
    }
    
    public void RemoveGroupedNode(DSNode node, Group group) {
        string nodeName = node.dialogueName;
        node.group = null;
        
        groupedNodes[group][nodeName].Nodes.Remove(node);
        node.ResetColor();

        if (groupedNodes[group][nodeName].Nodes.Count == 1) {
            groupedNodes[group][nodeName].Nodes[0].ResetColor();
            return;
        }

        if (groupedNodes[group][nodeName].Nodes.Count == 0) {
            groupedNodes[group].Remove(nodeName);

            if (groupedNodes[group].Count == 0) {
                groupedNodes.Remove(group);
            }
        }
    }
}