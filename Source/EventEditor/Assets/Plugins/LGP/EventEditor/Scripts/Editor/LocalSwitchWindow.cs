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

        public static LocalSwitchWindow instance;

        [MenuItem("Window/Event Editor/Local Switch")]
        public static void Open() {
            instance = (LocalSwitchWindow)GetWindow(typeof(LocalSwitchWindow));
            instance.titleContent = new GUIContent(EEUtils.labels["LocalSwitches"]);
        }

        #region Variables
        private GameEvent gameEvent;
        private SerializedObject serialObject;
        private ReorderableList switchlist;
        private int listIndex;
        #endregion

        #region Unity Methods
        private void OnGUI() {
            DrawContent();
            if (GUI.changed) {
                Repaint();
            }
        }
        #endregion

        #region Methods
        public void Setup(GameEvent gameEvent) {
            if (gameEvent == null) return;
            serialObject = new SerializedObject(gameEvent);
            serialObject.Update();
            this.gameEvent = gameEvent;
            switchlist = MakeReordList();
            switchlist.index = listIndex;
            serialObject.ApplyModifiedProperties();
        }

        private void DrawContent() {
            // Draw Target
            GameEvent newGameEvent = gameEvent;
            newGameEvent = (GameEvent)EditorGUILayout.ObjectField("Target", newGameEvent, typeof(GameEvent), true);
            if (newGameEvent != gameEvent) {
                Setup(newGameEvent);
            }

            // Draw List
            GUILayout.BeginScrollView(Vector2.right);
            if (gameEvent) {
                Setup(gameEvent);
                switchlist.DoLayoutList();
            } else {
                GUILayout.Space(15f);
                GUILayout.Label(EEUtils.labels["SelectGameEvent"]);
            }
            GUILayout.EndScrollView();
        }

        private ReorderableList MakeReordList() {
            ReorderableList reordList = new ReorderableList(serialObject, serialObject.FindProperty("localSwitches").FindPropertyRelative("keys"), false, true, true, true);

            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = serialObject.FindProperty("localSwitches").FindPropertyRelative("keys").GetArrayElementAtIndex(index);
                string key = element.stringValue;
                if (key != null) {
                    float padding = 15;
                    Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                    Rect keyRect = new Rect(contentRect.x, contentRect.y, contentRect.width / 2, contentRect.height);
                    Rect valueRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height);
                    
                    Dictionary<string, bool> dic = gameEvent.LocalSwitches;
                    string newKey = key;
                    newKey = EditorGUI.TextField(keyRect, newKey);
                    dic[key] = EditorGUI.Toggle(valueRect, dic[key]);
                    

                    if (GUI.changed) {
                        if (key != newKey) {
                            bool tempValue = dic[key];
                            dic.Remove(key);
                            if (!dic.ContainsKey(newKey)) dic.Add(newKey, tempValue);
                        }
                        serialObject.ApplyModifiedProperties();
                    }
                }
            };

            reordList.onAddCallback = (ReorderableList list) => {
                gameEvent.AddNewLocalSwitch(string.Empty);
                listIndex = list.index;
                serialObject.ApplyModifiedProperties();
            };
            
            reordList.onRemoveCallback = (ReorderableList list) => {
                string key = serialObject.FindProperty("localSwitches").FindPropertyRelative("keys").GetArrayElementAtIndex(list.index).stringValue;
                gameEvent.LocalSwitches.Remove(key);
                listIndex = list.index;
                serialObject.ApplyModifiedProperties();
            };

            reordList.onSelectCallback = (ReorderableList list) => {
                listIndex = list.index;
                serialObject.ApplyModifiedProperties();
            };

            return reordList;
        }
        #endregion
    }
}
