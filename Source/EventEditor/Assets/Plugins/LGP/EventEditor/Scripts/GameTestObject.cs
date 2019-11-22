using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGP.EventEditor {
    public class GameTestObject : MonoBehaviour {
        #region Static Flieds
        // Bool
        public static bool staticPublicBool = true;
        [Conditional]
        public static bool staticConditionalPublicBool = true;
        private static bool staticPrivateBool = true;
        [Conditional]
        private static bool staticConditionalPrivateBool = true;

        // Int
        public static int staticPublicInt = 1;
        [Conditional]
        public static int staticConditionalPublicInt = 1;
        private static int staticPrivateInt = 1;
        [Conditional]
        private static int staticConditionalPrivateInt = 1;

        // Float
        public static int staticPublicFloat = 1;
        [Conditional]
        public static int staticConditionalPublicFloat = 1;
        private static int staticPrivateFloat = 1;
        [Conditional]
        private static int staticConditionalPrivateFloat = 1;

        // String
        public static string staticPublicString = "Test";
        [Conditional]
        public static string staticConditionalPublicString = "Test";
        private static string staticPrivateString = "Test";
        [Conditional]
        private static string staticConditionalPrivateString = "Test";
        #endregion

        #region Static Methdos
        // Bool
        public static bool StaticPublicBoolFunction() => true;
        [Conditional]
        public static bool StaticConditionalPublicBoolFunction() => true;
        private static bool StaticPrivateBoolFunction() => true;
        [Conditional]
        private static bool StaticConditionalPrivateBoolFunction() => true;

        // Int
        public static int StaticPublicIntFunction() => 1;
        [Conditional]
        public static int StaticConditionalPublicIntFunction() => 1;
        private static int StaticPrivateIntFunction() => 1;
        [Conditional]
        private static int StaticConditionalPrivateIntFunction() => 1;

        // Float
        public static int StaticPublicFloatFunction() => 1;
        [Conditional]
        public static int StaticConditionalPublicFloatFunction() => 1;
        private static int StaticPrivateFloatFunction() => 1;
        [Conditional]
        private static int StaticConditionalPrivateFloatFunction() => 1;

        // String
        public static string StaticPublicStringFunction() => "Test";
        [Conditional]
        public static string StaticConditionalPublicStringFunction() => "Test";
        private static string StaticPrivateStringFunction() => "Test";
        [Conditional]
        private static string StaticConditionalPrivateStringFunction() => "Test";
        #endregion

        #region Fields
        // Bool
        public bool publicBool = true;
        [Conditional]
        public bool conditionalPublicBool = true;
        private bool privateBool = true;
        [Conditional]
        private bool conditionalPrivateBool = true;

        // Int
        public int publicInt = 1;
        [Conditional]
        public int conditionalPublicInt = 1;
        private int privateInt = 1;
        [Conditional]
        private int conditionalPrivateInt = 1;

        // Float
        public int publicFloat = 1;
        [Conditional]
        public int conditionalPublicFloat = 1;
        private int privateFloat = 1;
        [Conditional]
        private int conditionalPrivateFloat = 1;

        // String
        public string publicString = "Test";
        [Conditional]
        public string conditionalPublicString = "Test";
        private string privateString = "Test";
        [Conditional]
        private string conditionalPrivateString = "Test";
        #endregion

        #region Properties
        // Bool
        private bool PrivateBoolProperty => true;
        [Conditional]
        private bool ConditionalPrivateBoolProperty => true;
        public bool PublicBoolProperty => true;
        [Conditional]
        public bool ConditionalPublicBoolProperty => true;

        // Int
        private int PrivateIntProperty => 1;
        [Conditional]
        private int ConditionalPrivateIntProperty => 1;
        public int PublicIntProperty => 1;
        [Conditional]
        public int ConditionalPublicIntProperty => 1;

        // Float
        private float PrivatefloatProperty => 1f;
        [Conditional]
        private float ConditionalPrivatefloatProperty => 1f;
        public float PublicfloatProperty => 1f;
        [Conditional]
        public float ConditionalPublicfloatProperty => 1f;

        // String
        private string PrivateStringProperty => "Test";
        [Conditional]
        private string ConditionalPrivateStringProperty => "Test";
        public string PublicStringProperty => "Test";
        [Conditional]
        public string ConditionalPublicStringProperty => "Test";
        #endregion

        #region Functions
        // Bool
        public bool PublicBoolFunction() => true;
        [Conditional]
        public bool ConditionalPublicBoolFunction() => true;
        private bool PrivateBoolFunction() => true;
        [Conditional]
        private bool ConditionalPrivateBoolFunction() => true;

        // Int
        public int PublicIntFunction() => 1;
        [Conditional]
        public int ConditionalPublicIntFunction() => 1;
        private int PrivateIntFunction() => 1;
        [Conditional]
        private int ConditionalPrivateIntFunction() => 1;

        // Float
        public int PublicFloatFunction() => 1;
        [Conditional]
        public int ConditionalPublicFloatFunction() => 1;
        private int PrivateFloatFunction() => 1;
        [Conditional]
        private int ConditionalPrivateFloatFunction() => 1;

        // String
        public string PublicStringFunction() => "Test";
        [Conditional]
        public string ConditionalPublicStringFunction() => "Test";
        private string PrivateStringFunction() => "Test";
        [Conditional]
        private string ConditionalPrivateStringFunction() => "Test";
        #endregion
    }
}
