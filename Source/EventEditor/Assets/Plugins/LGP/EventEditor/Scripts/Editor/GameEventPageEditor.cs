using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LGP.EE {
    [CustomEditor(typeof(GameEventPage))]
    public class GameEventPageEditor : Editor {
        #region Unity Methods
        protected override void OnHeaderGUI() {
            //base.OnHeaderGUI();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
        #endregion
    }
}
