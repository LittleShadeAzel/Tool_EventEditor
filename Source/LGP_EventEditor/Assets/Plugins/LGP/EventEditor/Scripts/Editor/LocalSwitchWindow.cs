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
        #region Statics
        public static LocalSwitchWindow instance;

        [MenuItem("Tools/Event Editor/Local Switch")]
        public static void Open() {
            instance = (LocalSwitchWindow)GetWindow(typeof(LocalSwitchWindow));
            instance.titleContent = new GUIContent(EEUtils.labels["LocalSwitches"]);
        }
        #endregion

        #region Variables
        private GameEvent gameEvent;
        private SerializedObject serialObject;
        private ReorderableList switchlist;
        private Vector2 scrollPosition = Vector2.right;
        #endregion

        #region Unity Methods
        private void OnGUI() {
            if (gameEvent != null) serialObject.Update();
            DrawContent();
            if (gameEvent != null) serialObject.ApplyModifiedProperties();
            if (GUI.changed) {
                Repaint();
            }
        }
        #endregion

        #region Methods
        public void Setup(GameEvent gameEvent) {
            this.gameEvent = gameEvent;
            if (!gameEvent) return;
            serialObject = new SerializedObject(gameEvent);
            serialObject.Update();
            switchlist = MakeReordList();
        }


        private void DrawContent() {
            // Draw Target
            GameEvent newGameEvent = gameEvent;
            newGameEvent = (GameEvent)EditorGUILayout.ObjectField(EEUtils.labels["GameEvent"], newGameEvent, typeof(GameEvent), true);
            if (newGameEvent != gameEvent) Setup(newGameEvent);

            // Draw Local Switches
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (gameEvent) {
                switchlist.DoLayoutList();
            } else {
                GUILayout.Space(15f);
                GUILayout.Label(EEUtils.labels["SelectGameEvent"]);
            }
            GUILayout.EndScrollView();
        }

        private ReorderableList MakeReordList() {
            ReorderableList reordList = new ReorderableList(serialObject, serialObject.FindProperty("initialLocalSwitches").FindPropertyRelative("keys"), false, true, true, true);
            float padding = 15;

            // Draw Header
            reordList.drawHeaderCallback = (Rect rect) => {
                Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                Rect keyRect = new Rect(contentRect.x, contentRect.y, contentRect.width / 3, contentRect.height);
                Rect InitialValueRect = new Rect(contentRect.x + keyRect.width, contentRect.y, contentRect.width / 3, contentRect.height);
                Rect valueRect = new Rect(contentRect.x + keyRect.width + InitialValueRect.width, contentRect.y, contentRect.width / 3, contentRect.height);

                EditorGUI.LabelField(keyRect, EEUtils.labels["Key"], EditorStyles.boldLabel);
                EditorGUI.LabelField(InitialValueRect, EEUtils.labels["InitialValue"], EditorStyles.boldLabel);
                EditorGUI.LabelField(valueRect, EEUtils.labels["RuntimeValue"], EditorStyles.boldLabel);
            };

            // Draw Element
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = serialObject.FindProperty("initialLocalSwitches").FindPropertyRelative("keys").GetArrayElementAtIndex(index);
                string key = element.stringValue;
                if (key != null) {
                    Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                    Rect keyRect = new Rect(contentRect.x, contentRect.y, contentRect.width / 3, contentRect.height);
                    Rect InitialValueRect = new Rect(contentRect.x + keyRect.width, contentRect.y, contentRect.width / 3, contentRect.height);
                    Rect valueRect = new Rect(contentRect.x + keyRect.width + InitialValueRect.width, contentRect.y, contentRect.width / 3, contentRect.height);
                    Dictionary<string, bool> initalLocalSwitches = gameEvent.InitialLocalSwitches;
                    Dictionary<string, bool> localSwitches = gameEvent.LocalSwitches;
                    string newKey = key;

                    EditorGUI.BeginDisabledGroup(Application.isPlaying);
                    newKey = EditorGUI.TextField(keyRect, newKey);
                    initalLocalSwitches[key] = EditorGUI.Toggle(InitialValueRect, initalLocalSwitches[key]);
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(Application.isEditor);
                    localSwitches[key] = EditorGUI.Toggle(valueRect, localSwitches[key]);
                    EditorGUI.EndDisabledGroup();

                    if (GUI.changed) {
                        if (key != newKey) {
                            bool tempValue = initalLocalSwitches[key];
                            initalLocalSwitches.Remove(key);
                            if (!initalLocalSwitches.ContainsKey(newKey)) initalLocalSwitches.Add(newKey, tempValue);
                        }
                        gameEvent.ResetLocalSwitches();
                        serialObject.ApplyModifiedProperties();
                    }
                }
            };

            // Add
            reordList.onAddCallback = (ReorderableList list) => {
                if (Application.isPlaying) return;
                Dictionary<string, bool> dictionary = gameEvent.InitialLocalSwitches;
                if (!dictionary.ContainsKey(string.Empty)) dictionary.Add(string.Empty, true);
                gameEvent.ResetLocalSwitches();
                serialObject.ApplyModifiedProperties();
            };

            // Remove
            reordList.onRemoveCallback = (ReorderableList list) => {
                if (Application.isPlaying) return;
                string key = serialObject.FindProperty("initialLocalSwitches").FindPropertyRelative("keys").GetArrayElementAtIndex(list.index).stringValue;
                gameEvent.InitialLocalSwitches.Remove(key);
                gameEvent.ResetLocalSwitches();
                serialObject.ApplyModifiedProperties();
            };

            return reordList;
        }
        #endregion
    }
}
