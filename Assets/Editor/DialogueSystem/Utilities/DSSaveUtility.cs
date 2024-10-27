using System;
using System.Collections.Generic;
using System.Linq;
using DS.Data;
using DS.Data.Save;
using DS.Elements;
using DS.ScriptableObjects;
using DS.Windows;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DS.Utility {
    public static class DSSaveUtility {
        private static string graphFileName;
        private static string containerFolderPath;
        private static DSGraphView graphView;
        private static List<DSGroup> groups;
        private static List<DSNode> nodes;
        private static Dictionary<string, DSDialogueGroup> createdDialogueGroups;
        private static Dictionary<string, DSDialogue> createdDialogues;
        private static Dictionary<string, DSGroup> loadedGroups;
        private static Dictionary<string, DSNode> loadedNodes;
        
        public static void Initialize(DSGraphView dsGraphView, string graphName) {
            graphView = dsGraphView;
            graphFileName = graphName;
            containerFolderPath = "Assets/DialogueSystem/Dialogues/" + graphFileName;
            groups = new List<DSGroup>();
            nodes = new List<DSNode>();
            createdDialogueGroups = new Dictionary<string, DSDialogueGroup>();
            createdDialogues = new Dictionary<string, DSDialogue>();
            loadedGroups = new Dictionary<string, DSGroup>();
            loadedNodes = new Dictionary<string, DSNode>();
        }
        
        public static void Save() {
            CreateStaticFolders();
            GetElementFromGraphView();
            DSGraphSaveData graphData = CreateAsset<DSGraphSaveData>("Assets/Editor/DialogueSystem/Graphs", graphFileName);
            graphData.Initialize(graphFileName);
            
            DSDialogueContainer dialogueContainer = CreateAsset<DSDialogueContainer>(containerFolderPath, graphFileName);
            dialogueContainer.Initialize(graphFileName);

            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);
            
            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }

        private static void CreateStaticFolders() {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");
            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");
            CreateFolder("Assets/DialogueSystem/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder(containerFolderPath + "/Global", "Dialogues");
        }
        
        private static void CreateFolder(string path, string folderName){
            if (AssetDatabase.IsValidFolder(path + "/" + folderName)) {
                return;
            }
            
            AssetDatabase.CreateFolder(path, folderName);
        }

        private static void GetElementFromGraphView() {
            Type groupType = typeof(DSGroup);
            
            graphView.graphElements.ForEach(graphElement => {
                if (graphElement is DSNode node) {
                    nodes.Add(node);
                    return;
                }

                if (graphElement.GetType() == groupType) {
                    groups.Add((DSGroup) graphElement);
                    return;
                }
            });
        }

        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject {
            string fullPath = path + "/" + assetName + ".asset";
            T asset = LoadAsset<T>(path, assetName);

            if (asset == null) {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, fullPath);
            }
            
            return asset;
        }

        private static void SaveGroups(DSGraphSaveData graphData, DSDialogueContainer dialogueContainer) {
            List<string> groupNames = new List<string>();
            foreach (DSGroup group in groups) {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);
                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveData graphData) {
            DSGroupSaveData groupData = new DSGroupSaveData();
            groupData.id = group.id;
            groupData.name = group.title;
            groupData.position = group.GetPosition().position;
            
            graphData.groups.Add(groupData);
        }
        
        private static void SaveGroupToScriptableObject(DSGroup group, DSDialogueContainer dialogueContainer) {
            string groupName = group.title;
            CreateFolder(containerFolderPath + "/Groups", groupName);
            CreateFolder(containerFolderPath + "/Groups/" + groupName, "Dialogues");
            
            DSDialogueGroup dialogueGroup = CreateAsset<DSDialogueGroup>(containerFolderPath + "/Groups/" + groupName, groupName);
            dialogueGroup.Initialize(groupName);
            createdDialogueGroups.Add(group.id, dialogueGroup);
            dialogueContainer.dialogueGroups.Add(dialogueGroup, new List<DSDialogue>());
            
            SaveAsset(dialogueGroup);
        }

        private static void SaveAsset(Object asset) {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private static void SaveNodes(DSGraphSaveData graphData, DSDialogueContainer dialogueContainer) {
            List<string> ungroupedNodeNames = new List<string>();
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            
            foreach (DSNode node in nodes) {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);

                if (node.group != null) {
                    groupedNodeNames.AddItem(node.group.title, node.dialogueName);
                    continue;
                }
                
                ungroupedNodeNames.Add(node.dialogueName);
            }

            UpdateDialoguesChoicesConnections();
            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(DSNode node, DSGraphSaveData graphData) {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.choices);
            
            DSNodeSaveData nodeData = new DSNodeSaveData();
            nodeData.id = node.id;
            nodeData.name = node.dialogueName;
            nodeData.choices = choices;
            nodeData.text = node.dialogueText;
            nodeData.groupId = node.group?.id;
            nodeData.dialogueType = node.dialogueType;
            nodeData.position = node.GetPosition().position;
            
            graphData.nodes.Add(nodeData);
        }

        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices) {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();
            
            foreach (DSChoiceSaveData choice in nodeChoices) {
                DSChoiceSaveData choiceData = new DSChoiceSaveData();
                choiceData.text = choice.text;
                choiceData.nodeId = choice.nodeId;
                choices.Add(choiceData);
            }

            return choices;
        }

        private static void SaveNodeToScriptableObject(DSNode node, DSDialogueContainer dialogueContainer) {
            DSDialogue dialogue;

            if (node.group != null) {
                dialogue = CreateAsset<DSDialogue>(containerFolderPath + "/Groups/" + node.group.title + "/Dialogues", node.dialogueName);
                dialogueContainer.dialogueGroups.AddItem(createdDialogueGroups[node.group.id], dialogue);
            } else {
                dialogue = CreateAsset<DSDialogue>(containerFolderPath + "/Global/Dialogues", node.dialogueName);
                dialogueContainer.ungroupedDialogues.Add(dialogue);
            }
            
            dialogue.Initialize(node.dialogueName, node.dialogueText, ConvertNodeChoicesToDialogueChoices(node.choices), node.dialogueType, node.isStartingNode());
            createdDialogues.Add(node.id, dialogue);
            SaveAsset(dialogue);
        }

        private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices) {
            List<DSDialogueChoiceData> dialogueChoices = new List<DSDialogueChoiceData>();

            foreach (DSChoiceSaveData choice in nodeChoices) {
                DSDialogueChoiceData choiceData = new DSDialogueChoiceData();
                choiceData.text = choice.text;
                dialogueChoices.Add(choiceData);
            }
            
            return dialogueChoices;
        }
        
        private static void UpdateDialoguesChoicesConnections() {
            foreach (DSNode node in nodes) {
                DSDialogue dialogue = createdDialogues[node.id];
                for (int i = 0; i < dialogue.choices.Count; i++) {
                    DSChoiceSaveData choice = node.choices[i];

                    if (string.IsNullOrEmpty(choice.nodeId)) {
                        continue;
                    }
                    
                    dialogue.choices[i].nextDialogue = createdDialogues[choice.nodeId];
                    SaveAsset(dialogue);
                }
            }
        }
        
        private static void UpdateOldGroups(List<string> currentGroupNames, DSGraphSaveData graphData) {
            if (graphData.oldGroupNames != null && graphData.oldGroupNames.Count != 0) {
                List<string> groupsToRemove = graphData.oldGroupNames.Except(currentGroupNames).ToList();
                foreach (string groupToRemove in groupsToRemove) {
                    RemoveFolder(containerFolderPath + "/Groups/" + groupToRemove);
                }
            }
            
            graphData.oldGroupNames = new List<string>(currentGroupNames);
        }

        private static void RemoveFolder(string path) {
            FileUtil.DeleteFileOrDirectory(path + ".meta/");
            FileUtil.DeleteFileOrDirectory(path + "/");
        }
        
        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveData graphData) {
            if (graphData.oldUngroupedNames != null && graphData.oldUngroupedNames.Count != 0) {
                List<string> nodesToRemove = graphData.oldUngroupedNames.Except(currentUngroupedNodeNames).ToList();
                foreach (string nodeToRemove in nodesToRemove) {
                    RemoveAsset(containerFolderPath + "/Global/Dialogues", nodeToRemove);
                }
            }
            
            graphData.oldUngroupedNames = new List<string>(currentUngroupedNodeNames);
        }

        private static void RemoveAsset(string path, string assetName) {
            AssetDatabase.DeleteAsset(path + "/" + assetName + ".asset");
        }
        
        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DSGraphSaveData graphData) {
            if (graphData.oldGroupedNames != null && graphData.oldGroupedNames.Count != 0) {
                foreach (KeyValuePair<string, List<string>> oldGroupedNodes in graphData.oldGroupedNames) {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNodes.Key)) {
                        nodesToRemove = oldGroupedNodes.Value.Except(currentGroupedNodeNames[oldGroupedNodes.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove) {
                        RemoveAsset(containerFolderPath + "/Groups/" + oldGroupedNodes.Key + "/Dialogues", nodeToRemove);
                    }
                }
            }
            
            graphData.oldGroupedNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }
        
        public static void Load() {
            DSGraphSaveData graphData = LoadAsset<DSGraphSaveData>("Assets/Editor/DialogueSystem/Graphs", graphFileName);

            if (graphData == null) {
                EditorUtility.DisplayDialog("Error", "The file at the following path could not be found: \n" + 
                                                     "Assets/Editor/DialogueSystem/Graphs/" + graphFileName, "OK");
                return;
            }
            
            DSEditorWindow.UpdateFileName(graphData.fileName);
            LoadGroups(graphData.groups);
            LoadNodes(graphData.nodes);
            LoadNodesConnections();
        }

        private static void LoadNodesConnections() {
            foreach (KeyValuePair<string, DSNode> loadedNode in loadedNodes) {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children()) {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) choicePort.userData;
                    
                    if (string.IsNullOrEmpty(choiceData.nodeId)) {
                        continue;
                    }
                    
                    DSNode nextNode = loadedNodes[choiceData.nodeId];
                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();
                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);
                    graphView.AddElement(edge);
                    loadedNode.Value.RefreshPorts();
                }
            }
        }

        private static void LoadNodes(List<DSNodeSaveData> nodes) {
            foreach (DSNodeSaveData nodeData in nodes) {
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.choices);
                DSNode node = graphView.CreateNode(nodeData.name, nodeData.dialogueType, nodeData.position, false);
                node.id = nodeData.id;
                node.choices = choices;
                node.dialogueText = nodeData.text;
                node.Draw();
                
                graphView.AddElement(node);
                loadedNodes.Add(node.id, node);

                if (string.IsNullOrEmpty(nodeData.groupId)) {
                    continue;
                }
                
                DSGroup group = loadedGroups[nodeData.groupId];
                node.group = group;
                group.AddElement(node);
            }
        }

        private static void LoadGroups(List<DSGroupSaveData> groups) {
            foreach (DSGroupSaveData groupData in groups) {
                DSGroup group = graphView.CreateGroup(groupData.name, groupData.position);
                group.id = groupData.id;
                loadedGroups.Add(group.id, group);
            }
        }
        
        private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject {
            string fullPath = path + "/" + assetName + ".asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }
    }
}
