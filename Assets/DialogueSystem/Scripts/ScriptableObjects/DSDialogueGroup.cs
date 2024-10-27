using UnityEngine;

namespace DS.ScriptableObjects {
    public class DSDialogueGroup : ScriptableObject {
        public string groupName;

        public void Initialize(string groupName) {
            this.groupName = groupName;
        }
    }
}