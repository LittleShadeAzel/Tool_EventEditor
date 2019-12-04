using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;



namespace LGP.Utils {
    public static class EEUtils {

        public static Dictionary<string, string> labels = new Dictionary<string, string>() {
            {"CreatePage", "Create or select a page."},
            {"Pages", "Pages"},
            {"Conditions", "Conditions" },
            {"NoFields", "No fields detected." },
            {"SelectObject", "Select a scene object." },
            {"LocalSwitch", "Local Swtich" },
            {"DefineLocalSwitch", "Define a Local Switch."},
            {"LocalSwitchNotExisting", "Local Switch (Doesn't exits at the moment)" },
            {"GlobalSwitch", "Global Switch" },
            {"GameObject", "Game Object" },
            {"is", "is" },
            {"Trigger", "Trigger" },
            {"RunAsCoroutine", "Run as Coroutine" },
            {"Setup", "Setup" },
            {"Functions", "Functions" }
        };


        public static bool isDebugActive = false;
        public static void Debug(object content) {
            if (!isDebugActive) return;
            UnityEngine.Debug.Log(content);
        }
    }

    public static class IOUtils {
        public static string[] GetFullPathAsArray(string fullPath) {
            if (string.IsNullOrEmpty(fullPath)) return null;
            return fullPath.Split('/');
        }

        public static string GetFileName(string fullPath) {
            if (string.IsNullOrEmpty(fullPath)) return null;
            string[] pathArr = GetFullPathAsArray(fullPath);
            return pathArr[pathArr.Length - 1];
        }

        public static string GetFilePath(string fullPath) {
            if (string.IsNullOrEmpty(fullPath)) return null;
            string[] pathArr = GetFullPathAsArray(fullPath);
            Array.Resize<string>(ref pathArr, pathArr.Length - 1);
            string result = "";
            for (int i = 0; i < pathArr.Length; i++) {
                if (i == pathArr.Length - 1) {
                    result += pathArr[i];
                    break;
                }
                result += pathArr[i] + "/";
            }
            return result;
        }


        public static bool Exists(string fullPath) {
            return Directory.Exists(GetFilePath(fullPath));
        }

        public static void CreateFolder(string fullPath) {
            if (!Exists(fullPath)) return;
            Directory.CreateDirectory(fullPath);
        }

        public static void CreateFolder(string path, string name) {
            CreateFolder(path + "/" + name);
        }
    }
}
