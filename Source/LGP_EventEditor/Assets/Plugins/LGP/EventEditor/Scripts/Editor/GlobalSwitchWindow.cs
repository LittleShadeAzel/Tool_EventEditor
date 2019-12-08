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
    public class GlobalSwitchWindow : EditorWindow {
        #region Statics
        public static GlobalSwitchWindow instance;

        [MenuItem("Tools/Event Editor/Global Switch")]
        public static void Open() {
            instance = (GlobalSwitchWindow)GetWindow(typeof(GlobalSwitchWindow));
            instance.titleContent = new GUIContent(EEUtils.labels["GlobalSwitches"]);
            instance.Setup(null);

        }

        private static void RepaintInspector(System.Type type) {
            Editor[] ed = Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++) {
                if (ed[i].GetType() == type) ed[i].Repaint();
            }
        }
        #endregion

        #region Variables
        private GlobalSwitch globalSwtichInstance;
        private SerializedObject serialObject;
        private ReorderableList switchlist;
        private Vector2 scrollPosition = Vector2.right;
        #endregion

        #region Unity Methods
        private void OnGUI() {
            if (serialObject != null) serialObject.Update();
            DrawContent();
            if (serialObject != null) serialObject.ApplyModifiedProperties();
            if (GUI.changed) {
                RepaintInspector(typeof(GameEventEditor));
                Repaint();
            }
        }
        #endregion

        #region Methods
        public void Setup(GlobalSwitch globalSwitch) {
            globalSwtichInstance = globalSwitch;
            if (!globalSwitch) return;
            serialObject = new SerializedObject(globalSwitch);
            serialObject.Update();
            switchlist = MakeReordList();
        }

        private void DrawContent() {
            // Draw Object Field
            GlobalSwitch newGlobals = globalSwtichInstance;
            newGlobals = (GlobalSwitch)EditorGUILayout.ObjectField(EEUtils.labels["GlobalSwitch"], newGlobals, typeof(GlobalSwitch), true);
            if (newGlobals != globalSwtichInstance) {
                Setup(newGlobals);
            }

            // Draw Local Switches
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (globalSwtichInstance != null) {
                switchlist.DoLayoutList();
            } else {
                GUILayout.Space(15f);
                GUILayout.Label(EEUtils.labels["SelectGlobalSwitch"]);
            }
            GUILayout.EndScrollView();
        }

        private ReorderableList MakeReordList() {
            ReorderableList reordList = new ReorderableList(serialObject, serialObject.FindProperty("manifest").FindPropertyRelative("keys"), false, true, true, true);
            float padding = 15;

            // Draw Header
            reordList.drawHeaderCallback = (Rect rect) => {
                Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                Rect keyRect = new Rect(contentRect.x, contentRect.y, contentRect.width / 2, contentRect.height);
                Rect valueRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height);

                EditorGUI.LabelField(keyRect, EEUtils.labels["Key"], EditorStyles.boldLabel);
                EditorGUI.LabelField(valueRect, EEUtils.labels["Value"], EditorStyles.boldLabel);
            };

            // Draw Element
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = serialObject.FindProperty("manifest").FindPropertyRelative("keys").GetArrayElementAtIndex(index);
                string key = element.stringValue;
                if (key != null) {
                    Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                    Rect keyRect = new Rect(contentRect.x, contentRect.y, contentRect.width / 2, contentRect.height);
                    Rect valueRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height);

                    Dictionary<string, bool> dic = globalSwtichInstance.Manifest;
                    string newKey = key;
                    EditorGUI.BeginDisabledGroup(Application.isPlaying);
                    newKey = EditorGUI.TextField(keyRect, newKey);
                    dic[key] = EditorGUI.Toggle(valueRect, dic[key]);
                    EditorGUI.EndDisabledGroup();

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

            // Add
            reordList.onAddCallback = (ReorderableList list) => {
                if (Application.isPlaying) return;
                if (!globalSwtichInstance.Manifest.ContainsKey(string.Empty)) globalSwtichInstance.Manifest.Add(string.Empty, true);
                serialObject.ApplyModifiedProperties();
            };

            // Remove
            reordList.onRemoveCallback = (ReorderableList list) => {
                if (Application.isPlaying) return;
                string key = serialObject.FindProperty("manifest").FindPropertyRelative("keys").GetArrayElementAtIndex(list.index).stringValue;
                globalSwtichInstance.Manifest.Remove(key);
                serialObject.ApplyModifiedProperties();
            };

            return reordList;
        }
        #endregion
    }
}
