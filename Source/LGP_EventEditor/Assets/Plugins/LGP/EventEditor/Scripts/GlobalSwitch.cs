using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using LGP.Utils;

namespace LGP.EventEditor {

    [Serializable]
    public class GlobalSwitchStorage : SerializedGenericDictionary<string, bool> { }

    /// <summary>
    /// A global manifest of switches, which can be defined by the user and game.
    /// </summary>
    [CreateAssetMenu()]
    [Serializable]
    public class GlobalSwitch : ScriptableObject {

        #region Variabels
        [SerializeField] private GlobalSwitchStorage manifest = new GlobalSwitchStorage();
        public Dictionary<string, bool> Manifest { get => manifest.Dictionary; }
        #endregion

        #region Methods
        public void SetGlobalSwtich(string value) {
            string[] args = value.Split(char.Parse(","));
            SetGlobalSwtich(args[0], bool.Parse(args[1]));
        }

        public void SetGlobalSwtich(string key, bool flag) {
            if (!Manifest.ContainsKey(key)) {
                AddNewGlobalSwtich(key, flag);
            } else {
                Manifest[key] = flag;
            }
        }

        public void AddNewGlobalSwitch(string key) {
            if (!Manifest.ContainsKey(key)) {
                AddNewGlobalSwtich(key, true);
            }
        }

        private void AddNewGlobalSwtich(string key, bool flag) {
            if (!Manifest.ContainsKey(key)) Manifest.Add(key, flag);
        }

        public bool GetGlobalSwtich(string key) {
            if (Manifest.TryGetValue(key, out bool value)) return value;
            Debug.Log("Global Switch with Key [" + key + "] doesnt exist.");
            return false;
        }
        #endregion
    }
}
