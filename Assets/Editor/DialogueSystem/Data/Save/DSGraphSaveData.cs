using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save {
    public class DSGraphSaveData : ScriptableObject {
        public string fileName;
        public List<DSGroupSaveData> groups;
        public List<DSNodeSaveData> nodes;
        public List<string> oldGroupNames;
        public List<string> oldUngroupedNames;
        public SerializableDictionary<string, List<string>> oldGroupedNames;

        public void Initialize(string fileName) {
            this.fileName = fileName;
            groups = new List<DSGroupSaveData>();
            nodes = new List<DSNodeSaveData>();
        }
    }
}