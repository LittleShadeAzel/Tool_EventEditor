using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace LGP.EE {
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor {
        #region Variables
        private GameEvent gameEvent;
        private int selectedEventPageIndex;
        private ReorderableList reordlistEventPages;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            gameEvent = (GameEvent)target;
            gameEvent.Refresh();
            serializedObject.Update();
            reordlistEventPages = DrawReorderListPages();
        }

        public override void OnInspectorGUI() {
            gameEvent.Refresh();
            serializedObject.Update();
            DrawInspector();
            if (GUI.changed) {
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }
        #endregion

        #region Methods
        private void DrawInspector() {
            DrawEventHeader();
            reordlistEventPages.DoLayoutList();
            DrawEventPage();
        }

        private void DrawEventHeader() {
            EditorGUILayout.LabelField("Event", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            gameEvent.id = EditorGUILayout.TextField("Id:", string.Empty, EditorStyles.textField);
            gameEvent.displayName = EditorGUILayout.TextField("Name:", string.Empty, EditorStyles.textField);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private ReorderableList DrawReorderListPages() {
            ReorderableList reordList = new ReorderableList(serializedObject, serializedObject.FindProperty("eventPages"), true, true, true, true);
            reordList.displayAdd = true;
            reordList.displayRemove = true;
            // Draw header
            reordList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Event Pages", EditorStyles.boldLabel);
            };

            // Draw elements
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = reordList.serializedProperty.GetArrayElementAtIndex(index);
                GameEventPage page = (GameEventPage)element.objectReferenceValue;
                if (page) {
                    SerializedObject serializedEventPage = new SerializedObject(page);

                    EditorGUI.PropertyField(rect, serializedEventPage.FindProperty("displayName"), GUIContent.none);

                    if (GUI.changed) {
                        serializedEventPage.ApplyModifiedProperties();
                    }
                }
            };

            // Add new slement
            reordList.onAddCallback = (ReorderableList list) => {
                gameEvent.AddNewEventPage();
                Repaint();
            };

            // Remove element
            reordList.onRemoveCallback = (ReorderableList list) => {
                gameEvent.RemoveEventPage(list.index);
                Repaint();
            };

            // Select element
            reordList.onSelectCallback = (ReorderableList list) => {
                gameEvent.SetActivePage(list.index);
                serializedObject.ApplyModifiedProperties();
                Repaint();
            };

            // Reorder elements
            reordList.onReorderCallback = (ReorderableList list) => {
                gameEvent.SortPages();
                Repaint();
            };

            return reordList;
        }

        private void DrawEventPage() {
            if (gameEvent.activeEventPage == null) return;
            EditorGUILayout.LabelField(gameEvent.activeEventPage.displayName, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}
