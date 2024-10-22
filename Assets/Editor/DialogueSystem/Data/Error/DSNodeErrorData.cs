using System.Collections.Generic;
using DS.Elements;
using UnityEngine;

namespace DS.Data.Error {
    public class DSNodeErrorData {
        public DSErrorData ErrorData;
        public List<DSNode> Nodes;

        public DSNodeErrorData() {
            ErrorData = new DSErrorData();
            Nodes = new List<DSNode>();
        }
    }
}