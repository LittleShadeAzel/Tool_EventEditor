using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HelloWorld {
    [MenuItem("GameObject/Create HelloWorld")]
    private static void CreateHelloWorldGameObject() {
        if (EditorUtility.DisplayDialog("Hellow World", "do you really want to do this?", "Create", "Cancel")) {
            new GameObject("HelloWorld");
        }
    }
}
