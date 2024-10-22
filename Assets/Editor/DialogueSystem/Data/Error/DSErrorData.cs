using UnityEngine;

namespace DS.Data.Error {
    public class DSErrorData {
        public Color color;

        public DSErrorData() {
            GenerateRandomColor();
        }

        private void GenerateRandomColor() {
            color = new Color32((byte) Random.Range(65, 255), 
                                    (byte) Random.Range(50, 175), 
                                    (byte) Random.Range(50, 175), 
                                    255);
        }
    }
}