using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEditor;
using UnityEditorInternal;
using LGP.Utils;
using UnityScript;

namespace LGP.EventEditor {
    [CustomEditor(typeof(EEPage))]
    public class EEPageEditor : Editor {
        #region Variables
        private EEPage page;
        private GameEvent gameEvent;
        private GameEventEditor eventEditor;
        private ReorderableList conditionList;

        private const string NO_FIELD_DETECTED = "No fields detected.";
        private const string SELECT_SCENEOBJECT = "Select a Scene Object.";
        #endregion

        #region Unity Methods
        private void OnEnable() {
            if (target == null) return;
            page = (EEPage)target;
            serializedObject.Update();
            if (conditionList == null) conditionList = MakeReordConditionList();
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
            conditionList.index = serializedObject.FindProperty("conditionIndex").intValue;
            conditionList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        private ReorderableList MakeReordConditionList() {
            ReorderableList reordList = new ReorderableList(serializedObject, serializedObject.FindProperty("conditions"), false, true, true, true);
            reordList.elementHeight = EditorGUIUtility.singleLineHeight * 2;
            // Draw Header
            reordList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Conditions", EditorStyles.boldLabel);
            };

            // Draw Element
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {

                // Draw Content
                if (reordList.elementHeight == 0) return;
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
                    Rect infoFieldRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height);
                    GUIContent emptyLabel = new GUIContent();
                    SerializedObject serialCondition = new SerializedObject(condition);
                    SerializedProperty serialGameObjectA = serialCondition.FindProperty("gameObjectA");
                    SerializedProperty serialGameObjectB = serialCondition.FindProperty("gameObjectB");

                    // Set Game Object A
                    EditorGUI.ObjectField(gameObjectARect, serialGameObjectA, emptyLabel);
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
                            var objectInfoA = Condition.GetConditionalFieldFromIndex(gameObjectA, null, condition.IndexA);
                            condition.type = Condition.GetConditionType(objectInfoA);
                            if (objectInfoA is FieldInfo field) {
                                Type component = field.DeclaringType;
                                condition.SetValue(field.GetValue(gameObjectA.GetComponent(component)), 0);
                            } else if (objectInfoA is PropertyInfo property) {
                                Type component = property.DeclaringType;
                                condition.SetValue(property.GetValue(gameObjectA.GetComponent(component)), 0);
                            } else if (objectInfoA is MethodInfo method) {
                                Type component = method.DeclaringType;
                                condition.SetValue(method.Invoke(gameObjectA.GetComponent(component), null), 0);
                            }

                            // Draw Condition Drop Down
                            if (condition.type == EConditionType.Boolean) {
                                condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(EBoolConditionMode)));
                            } else if (condition.type == EConditionType.Integer || condition.type == EConditionType.Float) {
                                condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(ENummeralCondition)));
                            } else {
                                condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(EStringConditionMode)));
                            }

                            // Conitional self defined Field or from Object.
                            EditorGUI.ObjectField(gameObjectBRect, serialGameObjectB, emptyLabel);
                            GameObject gameObjectB = (GameObject)serialGameObjectB.objectReferenceValue;
                            if (gameObjectB) {
                                // From another Object
                                Type typeFilter = Condition.GetObjectInfoType(objectInfoA);
                                string[] optionListB = Condition.GetConditionalFieldOptionList(gameObjectB, typeFilter);
                                if (optionListB.Length > 0) {
                                    // Fields Detected
                                    condition.IndexB = EditorGUI.Popup(objectFieldBRect, condition.IndexB, optionListB);
                                    var objectInfoB = Condition.GetConditionalFieldFromIndex(gameObjectB, typeFilter, condition.IndexB);

                                    if (objectInfoB is FieldInfo fieldB) {
                                        Type component = fieldB.DeclaringType;
                                        condition.SetValue(fieldB.GetValue(gameObjectB.GetComponent(component)), 1);
                                    } else if (objectInfoB is PropertyInfo property) {
                                        Type component = property.DeclaringType;
                                        condition.SetValue(property.GetValue(gameObjectB.GetComponent(component)), 1);
                                    } else if (objectInfoB is MethodInfo method) {
                                        Type component = method.DeclaringType;
                                        condition.SetValue(method.Invoke(gameObjectB.GetComponent(component), null), 1);
                                    }
                                } else {
                                    // No Fields Detected
                                    EditorGUI.LabelField(objectFieldBRect, NO_FIELD_DETECTED);
                                    condition.IndexB = -1;
                                    //condition.objectB = null;
                                }// end ObjectFieldB
                            } else {
                                // No defined GameObjectB => Offer user defined values
                                if (condition.type == EConditionType.Boolean) condition.SetValue(EditorGUI.Toggle(objectFieldBRect, condition.objectBool[1]), 1);
                                if (condition.type == EConditionType.Integer) condition.SetValue(EditorGUI.IntField(objectFieldBRect, condition.objectInt[1]), 1);
                                if (condition.type == EConditionType.Float) condition.SetValue(EditorGUI.FloatField(objectFieldBRect, condition.objectFloat[1]), 1);
                                if (condition.type == EConditionType.String) condition.SetValue(EditorGUI.TextField(objectFieldBRect, condition.objectString[1]), 1);
                            }// end GameobjectB
                        } else {
                            // No Conditional Fields detected in GameObjectA => Clear Conditions and GameObjectB
                            EditorGUI.LabelField(objectFieldARect, NO_FIELD_DETECTED);
                            condition.ClearCondition();
                            condition.gameObjectB = null;
                        }// End ObjectFieldA
                    } else {
                        EditorGUI.HelpBox(infoFieldRect, SELECT_SCENEOBJECT, MessageType.Info);
                    }// end GameObjectA 

                    // Draw Condition Status
                    if (!Application.isPlaying || true) {
                        if (!condition.IsValid) {
                            EditorGUI.DrawRect(conditionStatusRect, GameEventEditor.WARNING_COLOR);
                        } else {
                            EditorGUI.DrawRect(conditionStatusRect, condition.CheckCondition() ? GameEventEditor.CONDITION_TRUE_COLOR : GameEventEditor.CONDITION_FALSE_COLOR);
                        }
                    } 

                    if (GUI.changed) {
                        serialCondition.ApplyModifiedProperties();
                    }
                }
            };

            // Add Element
            reordList.onAddCallback = (ReorderableList list) => {
                EEPage page = (EEPage)target;
                page.AddCondition();
                serializedObject.Update();
                serializedObject.FindProperty("conditionIndex").intValue = page.conditions.Count - 1;
                serializedObject.ApplyModifiedProperties();
            };

            // Remove Element
            reordList.onRemoveCallback = (ReorderableList list) => {
                EEPage page = (EEPage)target;
                page.RemoveCondition(list.index);
                serializedObject.Update();
                if (list.index == page.conditions.Count) {
                    serializedObject.FindProperty("conditionIndex").intValue = page.conditions.Count - 1;
                }
                serializedObject.ApplyModifiedProperties();
            };

            // Select Element
            reordList.onSelectCallback = (ReorderableList list) => {
                serializedObject.FindProperty("conditionIndex").intValue = list.index;
                serializedObject.ApplyModifiedProperties();
            };



            return reordList;
        }
        #endregion
    }
}