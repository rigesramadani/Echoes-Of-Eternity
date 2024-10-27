using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Elements {
    public class DSGroup : Group {
        public string id;
        public string oldTitle;
        private Color defultBorderColor;
        private float defultBorderWidth;

        public DSGroup(string groupTitle, Vector2 position) {
            id = Guid.NewGuid().ToString();
            title = groupTitle;
            oldTitle = groupTitle;
            SetPosition(new Rect(position, Vector2.zero));
            
            defultBorderColor = contentContainer.style.borderBottomColor.value;
            defultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color color) {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetStyle() {
            contentContainer.style.borderBottomColor = defultBorderColor;
            contentContainer.style.borderBottomWidth = defultBorderWidth;
        }
    }
}
