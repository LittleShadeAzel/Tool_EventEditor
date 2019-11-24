using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using LGP.Utils;

namespace LGP.EventEditor {
    [CustomEditor(typeof(EEPage))]
    public class EEPageEditor : Editor {
        #region Variables
        public EEPage page;
        public GameEvent gameEvent;
        public GameEventEditor eventEditor;
        public ReorderableList conditionList;

        private const string NO_FIELD_DETECTED = "No fields detected.";
        private readonly Color32 WARNING_COLOR = new Color32(255, 128, 0, 128);
        #endregion

        #region Unity Methods
        private void OnEnable() {
            page = (EEPage)target;
            serializedObject.Update();
            conditionList = EEUtils.CreateReordableList(serializedObject, serializedObject.FindProperty("conditions"), MakeReordConditionWrapper());
            conditionList.draggable = false;
            conditionList.elementHeight = EditorGUIUtility.singleLineHeight * 2;
        }

        protected override void OnHeaderGUI() {
            //base.OnHeaderGUI();
        }

        public override void OnInspectorGUI() {
            //DrawInspector();
            //if (GUI.changed) Repaint();
        }
        #endregion

        #region Methods
        public void SetEventEditor(GameEventEditor editor) {
            eventEditor = editor;
            gameEvent = (GameEvent)editor.target;
        }

        public void DrawInspectorGUI() {
            serializedObject.Update();
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) {
                EditorUtility.SetDirty(target);
            }
        }

        public void DrawInspector() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EEPage page = (EEPage)target;
            EditorGUILayout.LabelField(page.displayName, EditorStyles.boldLabel);
            conditionList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        private ReordableCallbackWrapper MakeReordConditionWrapper() {
            ReordableCallbackWrapper wrapper = new ReordableCallbackWrapper {

                // Draw Header
                header = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Conditions", EditorStyles.boldLabel);
                },

                // Draw Element
                element = (Rect rect, int index, bool isActive, bool isFocused) => {

                    // Draw Content
                    var element = serializedObject.FindProperty("conditions").GetArrayElementAtIndex(index);
                    Condition condition = (Condition)element.objectReferenceValue;
                    if (condition) {

                        // Setup
                        float padding = 15;
                        Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                        Rect conditionStatusRect = new Rect(rect.x, rect.y, 15, rect.height);
                        Rect gameObjectARect = new Rect(contentRect.x, contentRect.y, contentRect.width / 2, contentRect.height / 2);
                        Rect objectFieldARect = new Rect(contentRect.x, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect conditionModeRect = new Rect(contentRect.x + objectFieldARect.width, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect gameObjectBRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height / 2);
                        Rect objectFieldBRect = new Rect(contentRect.x + objectFieldARect.width * 2, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        GUIContent emptyLabel = new GUIContent();
                        SerializedObject serialCondition = new SerializedObject(condition);
                        SerializedProperty serialObjectA = serialCondition.FindProperty("gameObjectA");
                        SerializedProperty serialObjectB = serialCondition.FindProperty("gameObjectB");

                        // Set Game Object A
                        EditorGUI.ObjectField(gameObjectARect, serialObjectA, emptyLabel);
                        GameObject gameObjectA = condition.gameObjectA;
                        if (gameObjectA) {
                            // Conditional Field Object A
                            string[] optionListA = Condition.GetConditionalFieldOptionList(gameObjectA, null);
                            if (optionListA.Length > 0) {
                                // Set object Field A from Gameobject
                                int newIndex = EditorGUI.Popup(objectFieldARect, condition.IndexA, optionListA);
                                if (condition.IndexA != newIndex) {
                                    // If gameObjectA changes reset values
                                    condition.ClearCondition();
                                }
                                condition.IndexA = newIndex;
                                condition.objectA = Condition.GetConditionalFieldFromIndex(gameObjectA, null, condition.IndexA);

                                // Draw Condition Drop Down
                                EConditionType type = Condition.CheckCondition(condition.objectA);
                                if (type == EConditionType.Boolean) {
                                    condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(EBoolConditionMode)));
                                } else if (type == EConditionType.Integer) {
                                    condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(ENummeralCondition)));
                                } else {
                                    condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(EStringConditionMode)));
                                }

                                // Conitional self defined Field or from Object.
                                EditorGUI.ObjectField(gameObjectBRect, serialObjectB, emptyLabel);
                                GameObject gameObjectB = (GameObject)serialObjectB.objectReferenceValue;
                                if (gameObjectB) {
                                    // From another Object
                                    string[] optionListB = Condition.GetConditionalFieldOptionList(gameObjectB, Condition.GetObjectInfoType(condition.objectA));
                                    if (optionListB.Length > 0) {
                                        // Fields Detected
                                        condition.IndexB = EditorGUI.Popup(objectFieldBRect, condition.IndexB, optionListB);
                                        condition.objectB = Condition.GetConditionalFieldFromIndex(gameObjectB, Condition.GetObjectInfoType(condition.objectA), condition.IndexB);
                                    } else {
                                        // No Fields Detected
                                        EditorGUI.LabelField(objectFieldBRect, NO_FIELD_DETECTED);
                                        condition.IndexB = -1;
                                        condition.objectB = null;
                                    }// end ObjectFieldB
                                } else {
                                    // No defined GameObjectB => Offer user defined values
                                    if (Condition.GetObjectInfoType(condition.objectB) != null) condition.objectB = null; // This fixes a dynamic runtime error.
                                    if (type == EConditionType.Boolean) condition.objectB = EditorGUI.Toggle(objectFieldBRect, (condition.objectB ?? false));
                                    if (type == EConditionType.Integer) condition.objectB = EditorGUI.IntField(objectFieldBRect, (condition.objectB ?? 0));
                                    if (type == EConditionType.Float) condition.objectB = EditorGUI.FloatField(objectFieldBRect, (condition.objectB ?? 0f));
                                    if (type == EConditionType.String) condition.objectB = EditorGUI.TextField(objectFieldBRect, (condition.objectB ?? string.Empty));
                                }// end GameobjectB
                            } else {
                                // No Conditional Fields detected in GameObjectA => Clear Conditions and GameObjectB
                                EditorGUI.LabelField(objectFieldARect, NO_FIELD_DETECTED);
                                condition.ClearCondition();
                                condition.gameObjectB = null;
                            }// End ObjectFieldA
                        }// end GameObjectA

                        // Draw Condition Status
                        if (!condition.ConditionViable) {
                            EditorGUI.DrawRect(conditionStatusRect, WARNING_COLOR);
                        }

                        if (GUI.changed) {
                            serialCondition.ApplyModifiedProperties();
                        }
                    }
                },

                // Add Element
                add = (ReorderableList list) => {
                    page.AddCondition();
                    serializedObject.ApplyModifiedProperties();
                },

                // Remove Element
                remove = (ReorderableList list) => {
                    page.RemoveCondition(list.index);
                    serializedObject.ApplyModifiedProperties();
                },
            };
            return wrapper;
        }
        #endregion
    }
}