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
        public SerializedProperty selectedPageIndex;
        private EEPageEditor pageEditor = null;
        private ReorderableList reordlistEventPages;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            gameEvent = (GameEvent)target;
            gameEvent.RefreshPages();
            serializedObject.Update();
            selectedPageIndex = serializedObject.FindProperty("selectedPageIndex");
            reordlistEventPages = EEUtils.CreateReordableList(serializedObject, serializedObject.FindProperty("pages"), MakeReordWrapper());
        }

        private void OnDisable() {
            if (!pageEditor) DestroyImmediate(pageEditor);
        }

        public override void OnInspectorGUI() {
            gameEvent.RefreshPages();
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
            EEPageEditor pageEditor = GetPageEditor();
            if (pageEditor) {
                pageEditor.DrawInspectorGUI();
            } else {
                EditorGUILayout.HelpBox("Create or select a page.", MessageType.Info);
            }
        }

        private EEPageEditor GetPageEditor() {
            if (gameEvent.SelectedEventPage != null) {
                return (EEPageEditor)CreateEditor(gameEvent.SelectedEventPage);
            } else {
                return null;
            }
        }

        private ReordableCallbackWrapper MakeReordWrapper() {
            ReordableCallbackWrapper wrapper = new ReordableCallbackWrapper {

                // Draw Header
                header = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Pages", EditorStyles.boldLabel);
                },

                // Draw elements
                element = (Rect rect, int index, bool isActive, bool isFocused) => {
                    var element = serializedObject.FindProperty("pages").GetArrayElementAtIndex(index);
                    EEPage page = (EEPage)element.objectReferenceValue;
                    if (page) {
                        SerializedObject serializedEventPage = new SerializedObject(page);

                        EditorGUI.PropertyField(rect, serializedEventPage.FindProperty("displayName"), GUIContent.none);

                        if (GUI.changed) {
                            serializedEventPage.ApplyModifiedProperties();
                        }
                    }
                },

                // Add new element
                add = (ReorderableList list) => {
                    gameEvent.AddNewEventPage();
                    serializedObject.Update();
                    selectedPageIndex.intValue = gameEvent.pages.Count - 1;
                    list.index = gameEvent.pages.Count - 1;
                    serializedObject.ApplyModifiedProperties();
                },

                // Remove element
                remove = (ReorderableList list) => {
                    int newIndex = list.index;
                    gameEvent.RemoveEventPage(list.index);
                    serializedObject.Update();
                    if (list.index == gameEvent.pages.Count) newIndex = gameEvent.pages.Count - 1;

                    selectedPageIndex.intValue = newIndex;
                    list.index = newIndex;

                    serializedObject.ApplyModifiedProperties();
                },

                // Select element
                select = (ReorderableList list) => {
                    selectedPageIndex.intValue = list.index;
                    serializedObject.ApplyModifiedProperties();
                },

                // Reorder elements
                reorder = (ReorderableList list) => {
                    gameEvent.RefreshPages();
                }
            };
            return wrapper;
        }
        #endregion
    }
}
