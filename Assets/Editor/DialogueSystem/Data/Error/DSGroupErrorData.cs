using System.Collections.Generic;
using DS.Elements;

namespace DS.Data.Error {
    public class DSGroupErrorData {
        public DSErrorData ErrorData;
        public List<DSGroup> Groups;

        public DSGroupErrorData() {
            ErrorData = new DSErrorData();
            Groups = new List<DSGroup>();
        }
    }
}