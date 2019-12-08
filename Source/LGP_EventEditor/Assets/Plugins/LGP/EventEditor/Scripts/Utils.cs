using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGP.Utils {

    /// <summary>
    /// Custom Dictionary that can be serialized. It has to be inherted to avoid the generics. Because Generics cannot be serialized.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public abstract class SerializedGenericDictionary<K, V> : ISerializationCallbackReceiver {
        [SerializeField] private List<K> keys = new List<K>();
        [SerializeField] private List<V> values = new List<V>();
        private Dictionary<K, V> dictionary = new Dictionary<K, V>();
        public Dictionary<K, V> Dictionary { get => dictionary; set => dictionary = value; }

        public void OnAfterDeserialize() {
            var c = keys.Count;
            dictionary = new Dictionary<K, V>(c);
            for (var i = 0; i != Math.Min(keys.Count, values.Count); i++) {
                dictionary.Add(keys[i], values[i]);
            }
        }

        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var kvp in dictionary) {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
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
            {"DefineGlobalSwitch", "Define a Global Switch."},
            {"LocalSwitchNotExisting", "Local Switch (Doesn't exist at the moment)" },
            {"GlobalSwitchNotExisting", "Global Switch (Doesn't exist at the moment)" },
            {"GlobalSwitch", "Global Switch" },
            {"GlobalSwitches", "Global Switches" },
            {"GameObject", "Game Object" },
            {"is", "is" },
            {"Trigger", "Trigger" },
            {"RunAsCoroutine", "Run as Coroutine" },
            {"Setup", "Setup" },
            {"Functions", "Functions" },
            {"SelectGameEvent", "Select a Game Event from the Scene." },
            {"SelectGlobalSwitch", "Select a Global Switch object from the Asset database." },
            {"GameEvent", "Game Event" },
            {"Key", "Key" },
            {"Value", "Value" },
            {"RuntimeValue", "Runtime Value" },
            {"InitialValue", "Initial Value" }
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
