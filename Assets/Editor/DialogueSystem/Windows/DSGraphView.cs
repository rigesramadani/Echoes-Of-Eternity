using System;
using System.Collections.Generic;
using DS.Data.Error;
using DS.Data.Save;
using DS.Elements;
using DS.Enums;
using DS.Utilities;
using DS.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DSGraphView : GraphView {
    private DSEditorWindow editorWindow;
    private SerializableDictionary<string, DSNodeErrorData> ungroupedNodes;
    private SerializableDictionary<string, DSGroupErrorData> groups;
    private SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>> groupedNodes;
    private int nameErrorsAmount;

    public int RepeatedNamesAmount {
        get {
            return nameErrorsAmount;
        }
        set {
            nameErrorsAmount = value;

            if (nameErrorsAmount == 0) {
                editorWindow.EnableSaveButton();
            } else {
                editorWindow.DisableSaveButton();
            }
        }
    }
    
    public DSGraphView(DSEditorWindow editorWindow) {
        this.editorWindow = editorWindow;
        ungroupedNodes = new SerializableDictionary<string, DSNodeErrorData>();
        groups = new SerializableDictionary<string, DSGroupErrorData>();
        groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>>();
        
        AddManipulators();
        AddGridBackground();
        
        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
        OnGraphViewChanged();
        
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
                actionEvent => AddElement(CreateNode("DialogueName", dialogueType, actionEvent.eventInfo.localMousePosition)))
        );

        return contextualMenuManipulator;
    }
    
    private IManipulator CreateGroupContextualMenu() {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Group",
                actionEvent => CreateGroup("DialogueGroup", actionEvent.eventInfo.localMousePosition))
        );

        return contextualMenuManipulator;
    }

    public DSGroup CreateGroup(string title, Vector2 eventInfoLocalMousePosition) {
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

    public DSNode CreateNode(string nodeName, DSDialogueType dialogueType, Vector2 position, bool shouldDraw = true) {
        Type nodeType = Type.GetType("DS.Elements.DS" + dialogueType + "Node");

        DSNode node = (DSNode) Activator.CreateInstance(nodeType);
        node.Initialize(nodeName, this, position);

        if (shouldDraw) {
            node.Draw();
        }

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
        string groupName = group.title.ToLower();

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
            ++RepeatedNamesAmount;
            groups[groupName].Groups[0].SetErrorStyle(errorColor);
        }
    }

    private void RemoveGroup(DSGroup group) {
        string groupName = group.oldTitle.ToLower();
        groups[groupName].Groups.Remove(group);
        group.ResetStyle();

        if (groups[groupName].Groups.Count == 1) {
            --RepeatedNamesAmount;
            groups[groupName].Groups[0].ResetStyle();
        }

        if (groups[groupName].Groups.Count == 0) {
            groups.Remove(groupName);
        }
    }

    public void AddUngroupedNode(DSNode node) {
        string nodeName = node.dialogueName.ToLower();
        
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
            ++RepeatedNamesAmount;
            ungroupedNodes[nodeName].Nodes[0].SetErrorColor(errorColor);
        }
    }

    public void RemoveUngroupedNode(DSNode node) {
        string nodeName = node.dialogueName.ToLower();
        ungroupedNodes[nodeName].Nodes.Remove(node);
        node.ResetColor();

        if (ungroupedNodes[nodeName].Nodes.Count == 1) {
            --RepeatedNamesAmount;
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
            dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();
            
            if (string.IsNullOrEmpty(dsGroup.title)) {
                if (!string.IsNullOrEmpty(dsGroup.oldTitle)) {
                    ++RepeatedNamesAmount;
                }
            } else {
                if (string.IsNullOrEmpty(dsGroup.oldTitle)) {
                    --RepeatedNamesAmount;
                }
            }
            
            RemoveGroup(dsGroup);
            
            dsGroup.oldTitle = dsGroup.title;
            AddGroup(dsGroup);
        };
    }

    public void AddGroupedNode(DSNode node, DSGroup group) {
        string nodeName = node.dialogueName.ToLower();
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
            ++RepeatedNamesAmount;
            groupedNodes[group][nodeName].Nodes[0].SetErrorColor(errorColor);
        }
    }
    
    public void RemoveGroupedNode(DSNode node, Group group) {
        string nodeName = node.dialogueName.ToLower();
        node.group = null;
        
        groupedNodes[group][nodeName].Nodes.Remove(node);
        node.ResetColor();

        if (groupedNodes[group][nodeName].Nodes.Count == 1) {
            --RepeatedNamesAmount;
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

    private void OnGraphViewChanged() {
        graphViewChanged = changes => {
            if (changes.edgesToCreate != null) {
                foreach (Edge edge in changes.edgesToCreate) {
                    DSNode nextNode = (DSNode) edge.input.node;
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;
                    choiceData.nodeId = nextNode.id;
                }
            }

            if (changes.elementsToRemove != null) {
                Type edgeType = typeof(Edge);

                foreach (GraphElement element in changes.elementsToRemove) {
                    if (element.GetType() != edgeType) {
                        continue;
                    }
                    
                    Edge edge = (Edge) element;
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;
                    choiceData.nodeId = "";
                }
            }

            return changes;
        };
    }

    public void ClearGraph() {
        graphElements.ForEach(element => RemoveElement(element));
        
        groups.Clear();
        groupedNodes.Clear();
        ungroupedNodes.Clear();

        nameErrorsAmount = 0;
    }
}