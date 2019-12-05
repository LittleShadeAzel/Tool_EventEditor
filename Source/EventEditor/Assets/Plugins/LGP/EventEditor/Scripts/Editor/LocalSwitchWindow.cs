using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using LGP.Utils;

namespace LGP.EventEditor {
    /// <summary>
    /// Window to display Local Switches of the selected GameEvent.
    /// </summary>
    public class LocalSwitchWindow : EditorWindow {

        #region Variables
        private GameEvent gameEvent;
        private SerializedObject serialObject;
        private ReorderableList switchlist;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            titleContent = new GUIContent(EEUtils.labels["LocalSwitches"]);
        }
        private void OnGUI() {
            DrawContent();
        }
        #endregion

        #region Methods
        public void Setup(GameEvent gameEvent) {
            this.gameEvent = gameEvent;
            serialObject = new SerializedObject(gameEvent);
            switchlist = MakeReordList();
        }

        private void DrawContent() {
            if (!gameEvent) return;
            // Draw Target
            gameEvent = (GameEvent)EditorGUILayout.ObjectField("Target", gameEvent, typeof(GameEvent), true);

            // Draw List
            //switchlist.DoLayoutList();
        }

        private ReorderableList MakeReordList() {
            // TO DO
            // First make a serializable Dictionary.

            return null;
        }
        #endregion
    }
}
