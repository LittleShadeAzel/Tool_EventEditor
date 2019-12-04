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

        #region Statics
        public static Color32 WARNING_COLOR => new Color32(255, 255, 0, 128);       // Yellow
        public static Color32 CONDITION_TRUE_COLOR => new Color32(0, 255, 0, 128);  // Green
        public static Color32 CONDITION_FALSE_COLOR => new Color32(255, 0, 0, 128); // Red
        public static Color32 ACTIVE_PAGE_COLOR => new Color32(0, 0, 255, 128);     // Blue
        #endregion

        #region Variables
        private GameEvent gameEvent;
        public SerializedProperty selectedPageIndex;
        private EEPageEditor pageEditor;
        private ReorderableList reordlistEventPages;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            gameEvent = (GameEvent)target;
            gameEvent.RefreshPages();
            serializedObject.Update();
            selectedPageIndex = serializedObject.FindProperty("selectedPageIndex");
            reordlistEventPages = MakeReordList();
        }

        private void OnDisable() {
            if (pageEditor) DestroyImmediate(pageEditor);
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
            reordlistEventPages.DoLayoutList();
            EEPageEditor pageEditor = GetPageEditor();
            if (pageEditor) {
                pageEditor.DrawInspectorGUI();
            } else {
                EditorGUILayout.HelpBox(EEUtils.labels["CreatePage"], MessageType.Info);
            }
        }

        private EEPageEditor GetPageEditor() {
            if (gameEvent.SelectedEventPage != null) {
                return (EEPageEditor)CreateEditor(gameEvent.SelectedEventPage);
            } else {
                return null;
            }
        }

        private ReorderableList MakeReordList() {
            ReorderableList reordList = new ReorderableList(serializedObject, serializedObject.FindProperty("pages"), true, true, true, true);

            // Draw Header
            reordList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, EEUtils.labels["Pages"], EditorStyles.boldLabel);
            };

            // Draw elements
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = serializedObject.FindProperty("pages").GetArrayElementAtIndex(index);
                EEPage page = (EEPage)element.objectReferenceValue;
                if (page) {
                    float padding = 15;
                    Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                    Rect conditionStatusRect = new Rect(rect.x, rect.y, 15, rect.height);

                    SerializedObject serialPage = new SerializedObject(page);

                    EditorGUI.PropertyField(contentRect, serialPage.FindProperty("displayName"), GUIContent.none);
                    if (page == serializedObject.FindProperty("activePage").objectReferenceValue) {
                        EditorGUI.DrawRect(conditionStatusRect, ACTIVE_PAGE_COLOR);
                    } else {
                        EditorGUI.DrawRect(conditionStatusRect, page.CheckConditons() ? CONDITION_TRUE_COLOR : CONDITION_FALSE_COLOR);
                    }

                    if (GUI.changed) {
                        serialPage.ApplyModifiedProperties();
                    }
                }
            };

            // Add new element
            reordList.onAddCallback = (ReorderableList list) => {
                gameEvent.AddNewEventPage();
                serializedObject.Update();
                selectedPageIndex.intValue = gameEvent.pages.Count - 1;
                list.index = gameEvent.pages.Count - 1;
                serializedObject.ApplyModifiedProperties();
            };

            // Remove element
            reordList.onRemoveCallback = (ReorderableList list) => {
                int newIndex = list.index;
                gameEvent.RemoveEventPage(list.index);
                serializedObject.Update();
                if (list.index == gameEvent.pages.Count) newIndex = gameEvent.pages.Count - 1;

                selectedPageIndex.intValue = newIndex;
                list.index = newIndex;

                serializedObject.ApplyModifiedProperties();
            };

            // Select element
            reordList.onSelectCallback = (ReorderableList list) => {
                selectedPageIndex.intValue = list.index;
                serializedObject.ApplyModifiedProperties();
            };

            // Reorder elements
            reordList.onReorderCallback = (ReorderableList list) => {
                gameEvent.RefreshPages();
            };

            return reordList;
        }
        #endregion
    }
}
