using System;
using System.Collections.Generic;
using DS.Enums;
using UnityEngine;

namespace DS.Data.Save {
    
    [Serializable]
    public class DSNodeSaveData {
        public string id;
        public string name;        
        public string text;
        public List<DSChoiceSaveData> choices;
        public string groupId;
        public DSDialogueType dialogueType;
        public Vector2 position;
    }
}
