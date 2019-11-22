using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using LGP.Utils;

namespace LGP.EventEditor {
    [CustomEditor(typeof(GameEventPage))]
    public class GameEventPageEditor : Editor {
        #region Variables
        public GameEventPage page;
        public GameEvent gameEvent;
        public GameEventEditor eventEditor;
        public ReorderableList conditionList;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            page = (GameEventPage)target;
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
            GameEventPage page = (GameEventPage)target;
            EditorGUILayout.LabelField(page.displayName, EditorStyles.boldLabel);
            conditionList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        private ReordableCallbackWrapper MakeReordConditionWrapper() {
            ReordableCallbackWrapper wrapper = new ReordableCallbackWrapper();

            // Draw Header
            wrapper.header = (Rect rect) => {
                EditorGUI.LabelField(rect, "Conditions", EditorStyles.boldLabel);
            };

            // Draw Element
            wrapper.element = (Rect rect, int index, bool isActive, bool isFocused) => {
                // Padding
                float padding = 15;
                rect.x += padding;
                rect.width -= padding;

                // Element
                var element = serializedObject.FindProperty("conditions").GetArrayElementAtIndex(index);
                Condition condition = (Condition)element.objectReferenceValue;
                if (condition) {
                    SerializedObject serialCondition = new SerializedObject(condition);
                    SerializedProperty serialObjectA = serialCondition.FindProperty("gameObjectA");
                    SerializedProperty serialObjectB = serialCondition.FindProperty("gameObjectB");
                    // Set GameObjectB
                    Rect rectObjectA = new Rect(rect.x, rect.y, rect.width / 2, rect.height / 2);
                    GUIContent emptyLabel = new GUIContent();
                    EditorGUI.ObjectField(rectObjectA, serialObjectA, emptyLabel);
                    GameObject gameObjectA = condition.gameObjectA;
                    if (gameObjectA) {
                        // Conditional Field Object A; Set ObjectB null
                        Rect rectDropDownA = new Rect(rect.x, rect.y + rect.height / 2, rect.width / 3, rect.height / 2);
                        int newIndex = EditorGUI.Popup(rectDropDownA, condition.indexA, Condition.GetConditionalFieldOptionList(gameObjectA, null));
                        if (condition.indexA != newIndex) {
                            condition.indexA = newIndex;
                            condition.indexB = -1;
                            condition.conditionIndex = 0;
                            condition.objectB = null;
                        }
                        condition.objectA = Condition.GetConditionalFieldFromIndex(gameObjectA, null, condition.indexA);

                        // Condition Drop Down
                        EConditionType type = Condition.CheckCondition(condition.objectA);
                        Rect rectCondition = new Rect(rect.x + rectDropDownA.width, rect.y + rect.height / 2, rect.width / 3, rect.height / 2);
                        if (type == EConditionType.Boolean) {
                            condition.conditionIndex = EditorGUI.Popup(rectCondition, condition.conditionIndex, Enum.GetNames(typeof(EBoolConditionMode)));
                        } else if (type == EConditionType.Integer) {
                            condition.conditionIndex = EditorGUI.Popup(rectCondition, condition.conditionIndex, Enum.GetNames(typeof(ENummeralCondition)));
                        } else {
                            condition.conditionIndex = EditorGUI.Popup(rectCondition, condition.conditionIndex, Enum.GetNames(typeof(EStringConditionMode)));
                        }

                        // Conitional self defined Field or from Object.
                        Rect rectObjectB = new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height / 2);
                        EditorGUI.ObjectField(rectObjectB, serialObjectB, emptyLabel);
                        GameObject gameObjectB = (GameObject)serialObjectB.objectReferenceValue;

                        Rect rectFieldB = new Rect(rect.x + rectDropDownA.width * 2, rect.y + rect.height / 2, rect.width / 3, rect.height / 2);
                        if (gameObjectB) {
                            // From another Object
                            condition.indexB = EditorGUI.Popup(rectFieldB, condition.indexB, Condition.GetConditionalFieldOptionList(gameObjectB, Condition.GetObjectType(condition.objectA)));
                        } else {
                            // User Defined
                            if (type == EConditionType.Boolean) condition.objectB = EditorGUI.Toggle(rectFieldB, (condition.objectB ?? false));
                            if (type == EConditionType.Integer) condition.objectB = EditorGUI.IntField(rectFieldB, (condition.objectB ?? 0));
                            if (type == EConditionType.Float) condition.objectB = EditorGUI.FloatField(rectFieldB, (condition.objectB ?? 0f));
                            if (type == EConditionType.String) condition.objectB = EditorGUI.TextField(rectFieldB, (condition.objectB ?? string.Empty));
                        }
                    }
                    if (GUI.changed) {
                        serialCondition.ApplyModifiedProperties();
                    }
                }
            };


            // Add Element
            wrapper.add = (ReorderableList list) => {
                page.AddCondition();
                serializedObject.ApplyModifiedProperties();
            };

            // Remove Element
            wrapper.remove = (ReorderableList list) => {
                page.RemoveCondition(list.index);
                serializedObject.ApplyModifiedProperties();
                //Repaint();
            };

            return wrapper;
        }



        private int MakeConditionDropDown(EConditionType type, Rect rect) {
            if (type == EConditionType.Boolean) {
                return EditorGUI.Popup(rect, 0, Enum.GetNames(typeof(EBoolConditionMode)));
            } else if (type == EConditionType.Integer) {
                return EditorGUI.Popup(rect, 0, Enum.GetNames(typeof(ENummeralCondition)));
            } else {
                return EditorGUI.Popup(rect, 0, Enum.GetNames(typeof(EStringConditionMode)));
            }
        }
        #endregion
    }
}