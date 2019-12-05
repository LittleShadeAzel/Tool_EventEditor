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
        private EEPage page;
        private GameEventEditor eventEditor;
        private ReorderableList conditionList;
        private GenericMenu menu = new GenericMenu();
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


        public void DrawInspectorGUI() {
            serializedObject.Update();
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) {
                EditorUtility.SetDirty(target);
            }
        }

        public void DrawInspector() {
            Rect contentRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EEPage page = (EEPage)target;

            // Draw Header
            EditorGUILayout.LabelField(page.DisplayName, EditorStyles.boldLabel);

            // Draw Conditions
            conditionList.index = serializedObject.FindProperty("conditionIndex").intValue;
            conditionList.DoLayoutList();

            // Draw Trigger
            EditorGUILayout.LabelField(EEUtils.labels["Trigger"], EditorStyles.boldLabel);
            serializedObject.FindProperty("isCoroutine").boolValue = EditorGUILayout.Toggle(EEUtils.labels["RunAsCoroutine"], page.IsCoroutine);

            SerializedProperty serialTriggerIndex = serializedObject.FindProperty("triggerIndex");
            serialTriggerIndex.intValue = EditorGUILayout.Popup(page.TriggerIndex, Enum.GetNames(typeof(ETriggerMode)));
            EditorGUILayout.Space();

            // Draw Function
            EditorGUILayout.LabelField(EEUtils.labels["Functions"], EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("unityEvents"));
            EditorGUILayout.EndVertical();
        }

        private ReorderableList MakeReordConditionList() {
            ReorderableList reordList = new ReorderableList(serializedObject, serializedObject.FindProperty("conditions"), false, true, true, true);
            reordList.elementHeight = EditorGUIUtility.singleLineHeight * 2;
            // Draw Header
            reordList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, EEUtils.labels["Conditions"], EditorStyles.boldLabel);
            };

            // Draw Element
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {

                // Draw Content
                if (reordList.elementHeight == 0) return;
                var element = serializedObject.FindProperty("conditions").GetArrayElementAtIndex(index);
                Condition condition = (Condition)element.objectReferenceValue;
                if (condition) {
                    SerializedObject serialCondition = new SerializedObject(condition);
                    float padding = 15;
                    Rect conditionStatusRect = new Rect(rect.x, rect.y, padding, rect.height);
                    #region Type of GameObject
                    if (condition.Type == EConditionType.GameObject) {
                        // Setup
                        Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                        Rect gameObjectARect = new Rect(contentRect.x, contentRect.y, contentRect.width / 2, contentRect.height / 2);
                        Rect objectFieldARect = new Rect(contentRect.x, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect conditionModeRect = new Rect(contentRect.x + objectFieldARect.width, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect gameObjectBRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height / 2);
                        Rect objectFieldBRect = new Rect(contentRect.x + objectFieldARect.width * 2, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect infoFieldRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height);
                        GUIContent emptyLabel = new GUIContent();
                        SerializedProperty serialGameObjectA = serialCondition.FindProperty("gameObjectA");
                        SerializedProperty serialGameObjectB = serialCondition.FindProperty("gameObjectB");

                        // Set Game Object A
                        EditorGUI.ObjectField(gameObjectARect, serialGameObjectA, emptyLabel);
                        GameObject gameObjectA = condition.GameObjectA;
                        if (gameObjectA) {
                            // Conditional Field Object A
                            string[] optionListA = Condition.GetConditionalFieldOptionList(gameObjectA, null);
                            if (optionListA.Length > 0) {
                                // Set object Field A from Gameobject
                                int newIndex = EditorGUI.Popup(objectFieldARect, condition.IndexA, optionListA);
                                if (condition.IndexA != newIndex) {
                                    // If gameObjectA changes reset values
                                    condition.ClearCondition();
                                    condition.IndexA = newIndex;
                                    var objectInfoA = Condition.GetConditionalFieldFromIndex(gameObjectA, null, condition.IndexA);
                                    condition.ObjectType = Condition.GetConditionType(objectInfoA);
                                    if (objectInfoA is FieldInfo field) {
                                        condition.SetValue(field.GetValue(gameObjectA.GetComponent(field.DeclaringType)), 0);
                                    } else if (objectInfoA is PropertyInfo property) {
                                        condition.SetValue(property.GetValue(gameObjectA.GetComponent(property.DeclaringType)), 0);
                                    } else if (objectInfoA is MethodInfo method) {
                                        condition.SetValue(method.Invoke(gameObjectA.GetComponent(method.DeclaringType), null), 0);
                                    }
                                    condition.SetValue(condition.GetValue(0), 1);
                                }
                                // Draw Condition Drop Down
                                if (condition.ObjectType == EConditionObjectType.Boolean) {
                                    condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(EBoolConditionMode)));
                                } else if (condition.ObjectType == EConditionObjectType.Integer || condition.ObjectType == EConditionObjectType.Float) {
                                    condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(ENummeralCondition)));
                                } else {
                                    condition.ConditionIndex = EditorGUI.Popup(conditionModeRect, condition.ConditionIndex, Enum.GetNames(typeof(EStringConditionMode)));
                                }

                                // Conitional self defined Field or from Object.
                                EditorGUI.ObjectField(gameObjectBRect, serialGameObjectB, emptyLabel);
                                GameObject gameObjectB = (GameObject)serialGameObjectB.objectReferenceValue;
                                if (gameObjectB) {
                                    // From another Object
                                    Type typeFilter = Condition.GetObjectInfoType(Condition.GetConditionalFieldFromIndex(gameObjectA, null, condition.IndexA));
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
                                        EditorGUI.LabelField(objectFieldBRect, EEUtils.labels["NoFields"]);
                                        condition.IndexB = -1;
                                    }// end ObjectFieldB
                                } else {
                                    // No defined GameObjectB => Offer user defined values
                                    if (condition.ObjectType == EConditionObjectType.Boolean) condition.SetValue(EditorGUI.Toggle(objectFieldBRect, condition.ObjectBool[1]), 1);
                                    if (condition.ObjectType == EConditionObjectType.Integer) condition.SetValue(EditorGUI.IntField(objectFieldBRect, condition.ObjectInt[1]), 1);
                                    if (condition.ObjectType == EConditionObjectType.Float) condition.SetValue(EditorGUI.FloatField(objectFieldBRect, condition.ObjectFloat[1]), 1);
                                    if (condition.ObjectType == EConditionObjectType.String) condition.SetValue(EditorGUI.TextField(objectFieldBRect, condition.ObjectString[1]), 1);
                                }// end GameobjectB
                            } else {
                                // No Conditional Fields detected in GameObjectA => Clear Conditions and GameObjectB
                                EditorGUI.LabelField(objectFieldARect, EEUtils.labels["NoFields"]);
                                condition.ClearCondition();
                                condition.GameObjectB = null;
                            }// End ObjectFieldA
                        } else {
                            EditorGUI.HelpBox(infoFieldRect, EEUtils.labels["SelectObject"], MessageType.Info);
                        }// end GameObjectA 

                    }
                    #endregion

                    #region Type of Local Swtich
                    if (condition.Type == EConditionType.LocalSwtich) {
                        Rect contentRect = new Rect(rect.x + padding, rect.y, rect.width - padding, rect.height);
                        Rect labelRect = new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height / 2);
                        Rect keyRect = new Rect(contentRect.x, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect infoFieldRect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height);
                        Rect label2Rect = new Rect(contentRect.x + keyRect.width, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect toggleRect = new Rect(contentRect.x + keyRect.width * 2, contentRect.y + contentRect.height / 2, contentRect.width / 3, contentRect.height / 2);
                        Rect infoField2Rect = new Rect(contentRect.x + contentRect.width / 2, contentRect.y, contentRect.width / 2, contentRect.height / 2);

                        EditorGUI.LabelField(labelRect, EEUtils.labels["LocalSwitch"]);
                        if (condition.LocalSwitchKey == string.Empty) {
                            EditorGUI.HelpBox(infoFieldRect, EEUtils.labels["DefineLocalSwitch"], MessageType.Info);
                        } else {
                            if (!condition.ExistLocalSwitch) EditorGUI.LabelField(labelRect, EEUtils.labels["LocalSwitchNotExisting"]);
                            EditorGUI.LabelField(label2Rect, EEUtils.labels["is"]);
                            condition.LocalSwitchValue = EditorGUI.Toggle(toggleRect, condition.LocalSwitchValue);
                        }
                        condition.LocalSwitchKey = EditorGUI.TextArea(keyRect, condition.LocalSwitchKey);
                    }
                    #endregion

                    #region Type Global Swtich
                    if (condition.Type == EConditionType.GlobalSwtich) {
                        // TO DO
                        // Implemented same as Local Swtiches.
                    }
                    #endregion

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

            // Remove Element
            reordList.onRemoveCallback = (ReorderableList list) => {
                EEPage page = (EEPage)target;
                page.RemoveCondition(list.index);
                serializedObject.Update();
                if (list.index == page.Conditions.Count) {
                    serializedObject.FindProperty("conditionIndex").intValue = page.Conditions.Count - 1;
                }
                serializedObject.ApplyModifiedProperties();
            };

            // Select Element
            reordList.onSelectCallback = (ReorderableList list) => {
                serializedObject.FindProperty("conditionIndex").intValue = list.index;
                serializedObject.ApplyModifiedProperties();
            };

            // Add Element from Drop Down
            reordList.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
                EEPage page = (EEPage)target;
                Condition condition = page.AddCondition();
                serializedObject.Update();
                serializedObject.FindProperty("conditionIndex").intValue = page.Conditions.Count - 1;
                serializedObject.ApplyModifiedProperties();
                menu.AddItem(new GUIContent(EEUtils.labels["LocalSwitch"]), false, OnAddLocalSwtich, condition);
                //menu.AddItem(new GUIContent(EEUtils.labels["GlobalSwitch"]), false, OnAddGlobalSwtich, condition);// TO DO: Yet to be implemented
                menu.AddItem(new GUIContent(EEUtils.labels["GameObject"]), false, OnAddObjectSelected, condition);
                menu.ShowAsContext();
            };

            return reordList;
        }

        private void OnAddLocalSwtich(object condition) {
            SerializedObject serialCondition = new SerializedObject((Condition)condition);
            serialCondition.FindProperty("type").enumValueIndex = 0;
            serialCondition.ApplyModifiedProperties();
        }

        private void OnAddGlobalSwtich(object condition) {
            SerializedObject serialCondition = new SerializedObject((Condition)condition);
            serialCondition.FindProperty("type").enumValueIndex = 1;
            serialCondition.ApplyModifiedProperties();
        }

        private void OnAddObjectSelected(object condition) {
            SerializedObject serialCondition = new SerializedObject((Condition)condition);
            serialCondition.FindProperty("type").enumValueIndex = 2;
            serialCondition.ApplyModifiedProperties();
        }
        #endregion
    }
}