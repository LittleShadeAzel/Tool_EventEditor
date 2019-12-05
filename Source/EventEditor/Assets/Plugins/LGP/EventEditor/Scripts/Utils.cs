using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGP.Utils {

    public class SerialDictionary<K, V> : ISerializationCallbackReceiver {
        [SerializeField] private K[] keys;
        public K[] Keys { get => keys; set => keys = value; }
        [SerializeField] private V[] values;
        public V[] Values { get => values; set => values = value; }

        private Dictionary<K, V> dictionary = new Dictionary<K, V>();
        public Dictionary<K,V> Dictionary { get => dictionary; set => dictionary = value; }
 
        public void OnAfterDeserialize() {
            var c = keys.Length;
            dictionary = new Dictionary<K, V>(c);
            for (int i = 0; i < c; i++) {
                dictionary[keys[i]] = values[i];
            }
            keys = null;
            values = null;
        }

        public void OnBeforeSerialize() {
            var c = dictionary.Count;
            keys = new K[c];
            values = new V[c];
            int i = 0;
            using (var e = dictionary.GetEnumerator())
            while (e.MoveNext()) {
                var kvp = e.Current;
                keys[i] = kvp.Key;
                values[i] = kvp.Value;
                i++;
            }
        }
    }


    public static class EEUtils {

        public static Dictionary<string, string> labels = new Dictionary<string, string>() {
            {"CreatePage", "Create or select a page."},
            {"Pages", "Pages"},
            {"Conditions", "Conditions" },
            {"NoFields", "No fields detected." },
            {"SelectObject", "Select a scene object." },
            {"LocalSwitch", "Local Switch" },
            {"LocalSwitches", "Local Switches" },
            {"DefineLocalSwitch", "Define a Local Switch."},
            {"LocalSwitchNotExisting", "Local Switch (Doesn't exist at the moment)" },
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
