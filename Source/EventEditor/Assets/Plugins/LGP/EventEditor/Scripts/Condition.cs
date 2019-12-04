using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LGP.EventEditor {
    /// <summary>
    /// Container class to store condition data for a page. It has functionality to handle with object Info.
    /// </summary>
    [Serializable]
    public class Condition : ScriptableObject {
        #region Static Object Info Handlers
        /// <summary>
        /// Returns the Type of an info typed object.
        /// </summary>
        /// <param name="dynamicObject">ObjectType of either a FieldInfo, PropertyInfo or MethodInfo.</param>
        /// <returns>Returns the Field or return Type of the object info.</returns>
        public static Type GetObjectInfoType(object dynamicObject) {
            if (dynamicObject is FieldInfo field) return field.FieldType;
            if (dynamicObject is PropertyInfo property) return property.PropertyType;
            if (dynamicObject is MethodInfo method) return method.ReturnType;
            return null;
        }

        /// <summary>
        /// Returns the condition type of an object info.
        /// </summary>
        /// <param name="a">Info object type of either FieldInfo, PropertyInfo or MethodInfo.</param>
        /// <returns>Returns the Condition type of the object.</returns>
        public static EConditionObjectType GetConditionType(object a) {
            if (a is FieldInfo field) {
                if (field.FieldType == typeof(bool)) {
                    return EConditionObjectType.Boolean;
                } else if (field.FieldType == typeof(int)) {
                    return EConditionObjectType.Integer;
                } else if (field.FieldType == typeof(float)) {
                    return EConditionObjectType.Float;
                } else {
                    return EConditionObjectType.String;
                }
            } else if (a is PropertyInfo property) {
                if (property.PropertyType == typeof(bool)) {
                    return EConditionObjectType.Boolean;
                } else if (property.PropertyType == typeof(int)) {
                    return EConditionObjectType.Integer;
                } else if (property.PropertyType == typeof(float)) {
                    return EConditionObjectType.Float;
                } else {
                    return EConditionObjectType.String;
                }
            } else {
                MethodInfo method = (MethodInfo)a;
                if (method.ReturnType == typeof(bool)) {
                    return EConditionObjectType.Boolean;
                } else if (method.ReturnType == typeof(int)) {
                    return EConditionObjectType.Integer;
                } else if (method.ReturnType == typeof(float)) {
                    return EConditionObjectType.Float;
                } else {
                    return EConditionObjectType.String;
                }
            }
        }
        #endregion

        #region Static Condition Checks
        private static bool CheckBool(bool a, bool b, int mode) {
            switch (mode) {
                case 0: // Same
                    return a == b;
                case 1: // NotSame
                    return a != b;
                default:
                    return false;
            }
        }

        public static bool CheckFloat(float a, float b, int mode) {
            switch (mode) {
                case 0: // Equal
                    return a == b;
                case 1: // NotEqual
                    return a != b; ;
                case 2: // GreaterThan
                    return a > b;
                case 3: // GreaterOrEvenThan
                    return a >= b;
                case 4: // LesserThan
                    return a < b;
                case 5: // LesserOrEvenThan
                    return a <= b;
                default:
                    return false;
            }
        }

        public static bool CheckInt(int a, int b, int mode) {
            switch (mode) {
                case 0: // Equal
                    return a == b;
                case 1: // NotEqual
                    return a != b; ;
                case 2: // GreaterThan
                    return a > b;
                case 3: // GreaterOrEvenThan
                    return a >= b;
                case 4: // LesserThan
                    return a < b;
                case 5: // LesserOrEvenThan
                    return a <= b;
                default:
                    return false;
            }
        }

        public static bool CheckString(string a, string b, int mode) {
            switch (mode) {
                case 0: // Same
                    return a == b;
                case 1: // NotSame
                    return a != b;
                default:
                    return false;
            }
        }
        #endregion

        #region Static Field and Object Info List Handlers
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
                        string[] declaringType = field.DeclaringType.ToString().Split(char.Parse("."));
                        string fieldType = field.FieldType.ToString().Split(char.Parse("."))[1].Replace("Single", "Float");
                        result.Add(declaringType[declaringType.Length - 1] + "." + field.Name + " : " + fieldType);
                    }
                }
                PropertyInfo[] propertyInfos = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int k = 0; k < propertyInfos.Length; k++) {
                    PropertyInfo property = propertyInfos[k];
                    if (property.GetCustomAttribute(typeof(Conditional)) != null && (property.PropertyType == typeFilter || typeFilter == null)) {
                        string[] declaringType = property.DeclaringType.ToString().Split(char.Parse("."));
                        string fieldType = property.PropertyType.ToString().Split(char.Parse("."))[1].Replace("Single", "Float");
                        result.Add(declaringType[declaringType.Length - 1] + "." + property.Name + " : " + fieldType);
                    }
                }
                MethodInfo[] methodInfos = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                for (int l = 0; l < methodInfos.Length; l++) {
                    MethodInfo method = methodInfos[l];
                    if (method.GetCustomAttribute(typeof(Conditional)) != null && method.ReturnType != typeof(void) && !method.ContainsGenericParameters && method.GetParameters().Length == 0 && (method.ReturnType == typeFilter || typeFilter == null)) {
                        string[] declaringType = method.DeclaringType.ToString().Split(char.Parse("."));
                        string fieldType = method.ReturnType.ToString().Split(char.Parse("."))[1].Replace("Single", "Float");
                        result.Add(declaringType[declaringType.Length - 1] + "." + method.Name + " : " + fieldType);
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
        [SerializeField] private EEPage page;
        public EEPage Page { get => page; set => page = value; }
        [SerializeField] private GameObject gameObjectA, gameObjectB;
        public GameObject GameObjectA { get => gameObjectA; set => gameObjectA = value; }
        public GameObject GameObjectB { get => gameObjectB; set => gameObjectB = value; }
        private int indexA, indexB, conditionIndex = -1;
        public int IndexA { get => indexA == -1 ? 0 : indexA; set => indexA = value; }
        public int IndexB { get => indexB == -1 ? 0 : indexB; set => indexB = value; }

        [SerializeField] private string localSwitchKey;
        public string LocalSwitchKey { get => localSwitchKey; set => localSwitchKey = value; }
        [SerializeField] private bool localSwitchValue;
        public bool LocalSwitchValue { get => localSwitchValue; set => localSwitchValue = value; }

        [SerializeField] private string globalSwitchKey;
        public string GlobalSwtichKey { get => globalSwitchKey; set => globalSwitchKey = value; }
        [SerializeField] private bool gloablSwichValue;
        public bool GlobalSwitchValue { get => gloablSwichValue; set => gloablSwichValue = value; }

        [SerializeField] private EConditionObjectType objectType;
        public EConditionObjectType ObjectType { get => objectType; set => objectType = value; }
        [SerializeField] private EConditionType type;
        public EConditionType Type { get => type; set => type = value; }

        public int ConditionIndex { get => conditionIndex == -1 ? 0 : conditionIndex; set => conditionIndex = value; }
        private bool[] objectBool = new bool[2];
        public bool[] ObjectBool { get => objectBool; set => objectBool = value; }
        private float[] objectFloat = new float[2];
        public float[] ObjectFloat { get => objectFloat; set => objectFloat = value; }
        private int[] objectInt = new int[2];
        public int[] ObjectInt { get => objectInt; set => objectInt = value; }
        private string[] objectString = new string[2];
        public string[] ObjectString { get => objectString; set => objectString = value; }

        public bool IsValid { get => IsObjectValid || IsLocalSwitchValid; }
        public bool IsLocalSwitchValid { get => string.IsNullOrEmpty(LocalSwitchKey); }
        public bool IsGlobalSwtichValid { get => string.IsNullOrEmpty(GlobalSwtichKey); }
        public bool IsObjectValid { get => GetValue(0) != null && GetValue(1) != null && conditionIndex != -1; }
        public bool ExistLocalSwitch { get => Page.Owner.LocalSwitches.ContainsKey(LocalSwitchKey); }
        #endregion

        #region Methods
        /// <summary>
        /// Clears all condition data.
        /// </summary>
        public void ClearCondition() {
            indexA = -1;
            indexB = -1;
            conditionIndex = -1;
            localSwitchKey = string.Empty;
            localSwitchValue = false;
            globalSwitchKey = string.Empty;
            gloablSwichValue = false;
            objectBool = new bool[2];
            objectFloat = new float[2];
            objectInt = new int[2];
            objectString = new string[2];
        }

        /// <summary>
        /// Wrapper Method to set objects to the condition to check.
        /// </summary>
        /// <param name="value">The object of the condition.</param>
        /// <param name="index">Determiens position of the array the value will be stored in. Either 0 or 1.</param>
        public void SetValue(object value, int index) {
            index = Mathf.Clamp(index, 0, 1);
            if (ObjectType == EConditionObjectType.Boolean) objectBool[index] = (bool)value;
            if (ObjectType == EConditionObjectType.Integer) objectInt[index] = (int)value;
            if (ObjectType == EConditionObjectType.Float) objectFloat[index] = (float)value;
            if (ObjectType == EConditionObjectType.String) objectString[index] = (string)value;
        }

        /// <summary>
        /// Wrapper Method to get the object stored  in the condition.
        /// </summary>
        /// <param name="index">Position from the array stored in the condition. Either 0 or 1.</param>
        /// <returns>Returns the condition Object.</returns>
        public object GetValue(int index) {
            index = Mathf.Clamp(index, 0, 1);
            if (ObjectType == EConditionObjectType.Boolean) return objectBool[index];
            if (ObjectType == EConditionObjectType.Integer) return objectInt[index];
            if (ObjectType == EConditionObjectType.Float) return objectFloat[index];
            if (ObjectType == EConditionObjectType.String) return objectString[index];
            return null;
        }

        /// <summary>
        /// Wrapper Method to check the condition based on the type of the condition.
        /// </summary>
        /// <returns>True, if both object meet with the condition.</returns>
        public bool CheckCondition() {
            if (Type == EConditionType.LocalSwtich) 
                if (ExistLocalSwitch) return Page.Owner.GetLocalSwtich(localSwitchKey) == localSwitchValue;
            if (Type == EConditionType.GlobalSwtich) return false;
            if (Type == EConditionType.GameObject) {
                if (ObjectType == EConditionObjectType.Boolean) return CheckBool(objectBool[0], objectBool[1], conditionIndex);
                if (ObjectType == EConditionObjectType.Integer) return CheckInt(objectInt[0], objectInt[1], conditionIndex);
                if (ObjectType == EConditionObjectType.Float) return CheckFloat(objectFloat[0], objectFloat[1], conditionIndex);
                if (ObjectType == EConditionObjectType.String) return CheckString(objectString[0], objectString[1], conditionIndex);
            }
            return false;
        }
        #endregion
    }
}