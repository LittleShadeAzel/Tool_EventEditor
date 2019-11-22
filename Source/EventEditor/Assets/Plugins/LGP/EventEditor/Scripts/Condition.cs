using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LGP.EventEditor {
    /// <summary>
    /// It is used in the EventEditor class to store condition data.
    /// </summary>
    [Serializable]
    public class Condition : ScriptableObject {

        #region Condition Checks
        public static dynamic GetObjectType(dynamic dynamicObject) {
            if (dynamicObject is FieldInfo) return (dynamicObject as FieldInfo).FieldType;
            if (dynamicObject is PropertyInfo) return (dynamicObject as PropertyInfo).PropertyType;
            if (dynamicObject is MethodInfo) return (dynamicObject as MethodInfo).ReturnType;
            return null;
        }

        public static EConditionType CheckCondition(object a) {
            if (a is FieldInfo) {
                FieldInfo field = (FieldInfo)a;
                if (field.FieldType == typeof(bool)) {
                    return EConditionType.Boolean;
                } else if (field.FieldType == typeof(int)) {
                    return EConditionType.Integer;
                } else if (field.FieldType == typeof(float)) {
                    return EConditionType.Float;
                } else {
                    return EConditionType.String;
                }
            } else if (a is PropertyInfo) {
                PropertyInfo property = (PropertyInfo)a;
                if (property.PropertyType == typeof(bool)) {
                    return EConditionType.Boolean;
                } else if (property.PropertyType == typeof(int)) {
                    return EConditionType.Integer;
                } else if (property.PropertyType == typeof(float)) {
                    return EConditionType.Float;
                } else {
                    return EConditionType.String;
                }
            } else {
                MethodInfo method = (MethodInfo)a;
                if (method.ReturnType == typeof(bool)) {
                    return EConditionType.Boolean;
                } else if (method.ReturnType == typeof(int)) {
                    return EConditionType.Integer;
                } else if (method.ReturnType == typeof(float)) {
                    return EConditionType.Float;
                } else {
                    return EConditionType.String;
                }
            }
        }

        public static bool CheckBool(bool a, bool b, EBoolConditionMode mode) {
            switch (mode) {
                case EBoolConditionMode.Same:
                    return a == b;
                case EBoolConditionMode.NotSame:
                    return a != b;
                default:
                    return false;
            }
        }

        public static bool CheckNummeral(dynamic number1, dynamic number2, ENummeralCondition mode) {
            float a = (float)number1;
            float b = (float)number2;
            switch (mode) {
                case ENummeralCondition.Equal:
                    return a == b;
                case ENummeralCondition.NotEqual:
                    return a != b;;
                case ENummeralCondition.GreaterThan:
                    return a > b;
                case ENummeralCondition.GreaterOrEvenThan:
                    return a >= b;
                case ENummeralCondition.LesserThan:
                    return a < b;
                case ENummeralCondition.LesserOrEavenThan:
                    return a <= b;
                default:
                    return false;
            }
        }

        public static bool CheckString(string a, string b, EStringConditionMode mode) {
            switch (mode) {
                case EStringConditionMode.Same:
                    return a == b;
                case EStringConditionMode.NotSame:
                    return a != b;
                default:
                    return false;
            }
        }
        #endregion

        #region Get Field and Method
        // Important: Field -> Property -> Method
        public static string[] GetConditionalFieldOptionList(GameObject gameObject, Type typeFilter) {
            List<string> result = new List<string>();
            Component[] components = gameObject.GetComponents(typeof(Component));
            for (int i = 0; i < components.Length; i++) {
                Component component = components[i];
                FieldInfo[] fieldInfos = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int j = 0; j < fieldInfos.Length; j++) {
                    FieldInfo field = fieldInfos[j];
                    if (field.GetCustomAttribute(typeof(Conditional)) != null && (field.FieldType == typeFilter || typeFilter == null)) {
                        result.Add(field.Name + " : " + field.FieldType.ToString().Split(char.Parse("."))[1]);
                    }
                }
                PropertyInfo[] propertyInfos = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int k = 0; k < propertyInfos.Length; k++) {
                    PropertyInfo property = propertyInfos[k];
                    if (property.GetCustomAttribute(typeof(Conditional)) != null && (property.PropertyType == typeFilter || typeFilter == null)) {
                        result.Add(property.Name + " : " + property.PropertyType.ToString().Split(char.Parse("."))[1]);
                    }
                }
                MethodInfo[] methodInfos = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int l = 0; l < methodInfos.Length; l++) {
                    MethodInfo method = methodInfos[l];
                    if (method.GetCustomAttribute(typeof(Conditional)) != null && method.ReturnType != typeof(void) && !method.ContainsGenericParameters && method.GetParameters().Length == 0 && (method.ReturnType == typeFilter || typeFilter == null)) {
                        result.Add(method.Name + "() : " + method.ReturnType.ToString().Split(char.Parse("."))[1]); // Function Format for clarity.
                    }
                }
            }
            return result.ToArray();
        }

        public static object GetConditionalFieldFromIndex(GameObject gameObject, Type typeFilter, int index) {
            FieldInfo[] fieldList = GetConditionalFieldList(gameObject, typeFilter);
            PropertyInfo[] propertyList = GetConditionalPropertyList(gameObject, typeFilter);
            MethodInfo[] methodInfos = GetConditionalMethodList(gameObject, typeFilter);
            int fieldLength = fieldList.Length;
            int propertyLength = propertyList.Length;
            int methodLength = methodInfos.Length;

            Utils.EEUtils.Debug("Index: " + index + " | " + fieldLength + " |  " + propertyLength + " | " + methodLength + " |");
            if (index < fieldLength) {
                Utils.EEUtils.Debug("Field " + index);
                return fieldList[index];
            } else if (index < fieldLength + propertyLength) {
                index -= fieldLength;
                Utils.EEUtils.Debug("Property " + index);
                return propertyList[index];
            } else if (index < fieldLength + propertyLength + methodLength) {
                index -= fieldLength + propertyLength;
                Utils.EEUtils.Debug("Method " + index);
                return methodInfos[index];
            } else {
                Utils.EEUtils.Debug("GetConitionalFieldFromIndex: Index is out of bounds " + index);
                return null;
            }

        }

        // Field List
        public static FieldInfo[] GetConditionalFieldList(GameObject gameObject, Type typeFilter) {
            List<FieldInfo> result = new List<FieldInfo>();
            Component[] components = gameObject.GetComponents(typeof(Component));
            for (int i = 0; i < components.Length; i++) {
                Component component = components[i];
                FieldInfo[] fieldInfos = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int j = 0; j < fieldInfos.Length; j++) {
                    FieldInfo field = fieldInfos[j];
                    if (field.GetCustomAttribute(typeof(Conditional)) != null && (field.FieldType == typeFilter || typeFilter == null)) {
                        result.Add(field);
                    }
                }
            }
            return result.ToArray();
        }

        // Property List
        public static PropertyInfo[] GetConditionalPropertyList(GameObject gameObject, Type typeFilter) {
            List<PropertyInfo> result = new List<PropertyInfo>();
            Component[] components = gameObject.GetComponents(typeof(Component));
            for (int i = 0; i < components.Length; i++) {
                Component component = components[i];
                PropertyInfo[] fieldInfos = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int j = 0; j < fieldInfos.Length; j++) {
                    PropertyInfo property = fieldInfos[j];
                    if (property.GetCustomAttribute(typeof(Conditional)) != null && (property.PropertyType == typeFilter || typeFilter == null)) {
                        result.Add(property);
                    }
                }
            }
            return result.ToArray();
        }

        // Method List
        public static MethodInfo[] GetConditionalMethodList(GameObject gameObject, Type typeFilter) {
            List<MethodInfo> result = new List<MethodInfo>();
            Component[] components = gameObject.GetComponents(typeof(Component));
            for (int i = 0; i < components.Length; i++) {
                Component component = components[i];
                MethodInfo[] methodInfos = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int j = 0; j < methodInfos.Length; j++) {
                    MethodInfo method = methodInfos[j];
                    if (method.GetCustomAttribute(typeof(Conditional)) != null && method.ReturnType != typeof(void) && !method.ContainsGenericParameters && method.GetParameters().Length == 0 && (method.ReturnType == typeFilter || typeFilter == null)) {
                        result.Add(method);
                    }
                }
            }
            return result.ToArray();
        }

        #endregion

        #region Variables
        [SerializeField]
        public GameObject gameObjectA, gameObjectB;
        public int indexA, indexB, conditionIndex;
        public dynamic objectA, objectB;
        #endregion

        #region Methods
        
        #endregion
    }

    public enum ENummeralCondition {
       Equal,
       NotEqual,
       GreaterThan,
       GreaterOrEvenThan,
       LesserThan,
       LesserOrEvenThan
    }

    public enum EBoolConditionMode {
        Same,
        NotSame
    }

    public enum EStringConditionMode {
        Same,
        NotSame
    }

    public enum EConditionType {
        Boolean,
        Integer,
        Float,
        String
    }
}
