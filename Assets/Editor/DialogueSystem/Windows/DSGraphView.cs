using System;
using System.Collections.Generic;
using DS.Data.Error;
using DS.Elements;
using DS.Enums;
using DS.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DSGraphView : GraphView {
    private SerializableDictionary<string, DSNodeErrorData> ungroupedNodes;
    private SerializableDictionary<string, DSGroupErrorData> groups;
    private SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>> groupedNodes;
    
    public DSGraphView() {
        ungroupedNodes = new SerializableDictionary<string, DSNodeErrorData>();
        groups = new SerializableDictionary<string, DSGroupErrorData>();
        groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>>();
        
        AddManipulators();
        AddGridBackground();
        
        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
        
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
                actionEvent => CreateGroup("Dialogue Group", actionEvent.eventInfo.localMousePosition))
        );

        return contextualMenuManipulator;
    }

    private void CreateGroup(string title, Vector2 eventInfoLocalMousePosition) {
        DSGroup group = new DSGroup(title, eventInfoLocalMousePosition);
        AddGroup(group);
        AddElement(group);

        foreach (GraphElement selectedElement in selection) {
            if (!(selectedElement is DSNode)) {
                continue;
            }
            
            DSNode node = (DSNode) selectedElement;
            group.AddElement(node);
        }
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
            Type groupType = typeof(DSGroup);
            Type edgeType = typeof(Edge);
            
            List<DSGroup> groupsToDelete = new List<DSGroup>();
            List<Edge> edgesToDelete = new List<Edge>();
            List<DSNode> nodesToDelete = new List<DSNode>();
            
            foreach (GraphElement element in selection) {
                if (element is DSNode node) {
                    nodesToDelete.Add(node);
                }

                if (element.GetType() == edgeType) {
                    edgesToDelete.Add((Edge) element);
                    continue;
                }

                if (element.GetType() != groupType) {
                    continue;
                }
                
                DSGroup group = (DSGroup) element;
                groupsToDelete.Add(group);
            }

            foreach (DSGroup group in groupsToDelete) {
                List<DSNode> groupNodes = new List<DSNode>();
                
                foreach (GraphElement groupElement in group.containedElements) {
                    if (!(groupElement is DSNode)) {
                        continue;
                    }
                    
                    groupNodes.Add((DSNode) groupElement);
                }
                
                group.RemoveElements(groupNodes);
                RemoveGroup(group);
                RemoveElement(group);
            }
            
            DeleteElements(edgesToDelete);

            foreach (DSNode node in nodesToDelete) {
                if (node.group != null) {
                    node.group.RemoveElement(node);
                }
                RemoveUngroupedNode(node);
                node.DisconnectAllPorts();
                RemoveElement(node);
            }
        };
    }
    
    private void AddGroup(DSGroup group) {
        string groupName = group.title;

        if (!groups.ContainsKey(groupName)) {
            DSGroupErrorData groupErrorData = new DSGroupErrorData();
            groupErrorData.Groups.Add(group);
            groups.Add(groupName, groupErrorData);
            return;
        }
        
        groups[groupName].Groups.Add(group);
        Color errorColor = groups[groupName].ErrorData.color;
        group.SetErrorStyle(errorColor);

        if (groups[groupName].Groups.Count == 2) {
            groups[groupName].Groups[0].SetErrorStyle(errorColor);
        }
    }

    private void RemoveGroup(DSGroup group) {
        string groupName = group.oldTitle;
        groups[groupName].Groups.Remove(group);
        group.ResetStyle();

        if (groups[groupName].Groups.Count == 1) {
            groups[groupName].Groups[0].ResetStyle();
        }

        if (groups[groupName].Groups.Count == 0) {
            groups.Remove(groupName);
        }
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
                
                DSGroup nodeGroup = (DSGroup) group;
                DSNode node = (DSNode) element;
                
                RemoveUngroupedNode(node);
                AddGroupedNode(node, nodeGroup);
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

    private void OnGroupRenamed() {
        groupTitleChanged = (group, newTitle) => {
            DSGroup dsGroup = (DSGroup) group;
            RemoveGroup(dsGroup);
            
            dsGroup.oldTitle = newTitle;
            AddGroup(dsGroup);
        };
    }

    public void AddGroupedNode(DSNode node, DSGroup group) {
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