using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using LGP.Utils;

namespace LGP.EventEditor {
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor {
        #region Variables
        private GameEvent gameEvent;
        public SerializedProperty selectedEventPageIndex;
        private GameEventPageEditor pageEditor;
        private ReorderableList reordlistEventPages;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            gameEvent = (GameEvent)target;
            gameEvent.Refresh();
            serializedObject.Update();
            selectedEventPageIndex = serializedObject.FindProperty("selectedEventPageIndex");
            reordlistEventPages = EEUtils.CreateReordableList(serializedObject, serializedObject.FindProperty("eventPages"), MakeReordWrapper());
        }

        private void OnDisable() {
            if (!pageEditor) DestroyImmediate(pageEditor);
        }

        public override void OnInspectorGUI() {
            gameEvent.Refresh();
            serializedObject.Update();
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) {
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }
        #endregion

        #region Methods
        private void DrawInspector() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            gameEvent.id = EditorGUILayout.TextField("Id:", string.Empty, EditorStyles.textField);
            gameEvent.displayName = EditorGUILayout.TextField("Name:", string.Empty, EditorStyles.textField);
            EditorGUILayout.EndVertical();
            reordlistEventPages.DoLayoutList();
            GameEventPageEditor pageEditor = GetPageEditor();
            if (pageEditor) {
                pageEditor.DrawInspectorGUI();
            }
        }
        

        private GameEventPageEditor GetPageEditor() {
            GameEvent gameEvent = (GameEvent)target;
            if (!pageEditor || !pageEditor.target || pageEditor.target != gameEvent.SelectedEventPage) {
                if (gameEvent.SelectedEventPage) {
                    if (pageEditor) DestroyImmediate(pageEditor);
                    pageEditor = (GameEventPageEditor)CreateEditor(gameEvent.SelectedEventPage);
                    pageEditor.SetEventEditor(this);
                }
            } else {
                //if (pageEditor) DestroyImmediate(pageEditor);
                //pageEditor = null;
            }
            return pageEditor;
        }

        private ReordableCallbackWrapper MakeReordWrapper() {
            ReordableCallbackWrapper wrapper = new ReordableCallbackWrapper();

            // Draw Header
            wrapper.header = (Rect rect) => {
                EditorGUI.LabelField(rect, "Pages", EditorStyles.boldLabel);
            };

            // Draw elements
            wrapper.element = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = serializedObject.FindProperty("eventPages").GetArrayElementAtIndex(index);
                GameEventPage page = (GameEventPage)element.objectReferenceValue;
                if (page) {
                    SerializedObject serializedEventPage = new SerializedObject(page);

                    EditorGUI.PropertyField(rect, serializedEventPage.FindProperty("displayName"), GUIContent.none);

                    if (GUI.changed) {
                        serializedEventPage.ApplyModifiedProperties();
                    }
                }
            };

            // Add new element
            wrapper.add = (ReorderableList list) => {
                gameEvent.AddNewEventPage();
                selectedEventPageIndex.intValue = gameEvent.eventPages.Count - 1;
                serializedObject.ApplyModifiedProperties();
            };

            // Remove element
            wrapper.remove = (ReorderableList list) => {
                GameEvent gameEvent = (GameEvent)target;
                DestroyImmediate(pageEditor);
                gameEvent.RemoveEventPage(list.index);
                if (gameEvent.eventPages.Count != 0) {
                    selectedEventPageIndex.intValue = 0;
                } else {
                    selectedEventPageIndex.intValue = -1;
                }
                serializedObject.ApplyModifiedProperties();
            };

            // Select element
            wrapper.select = (ReorderableList list) => {
                selectedEventPageIndex.intValue = list.index;
                serializedObject.ApplyModifiedProperties();
                GUI.changed = true;
            };

            // Reorder elements
            wrapper.reorder = (ReorderableList list) => {
                gameEvent.SortPages();
                Repaint();
            };

            return wrapper;
        }
        #endregion
    }
}
