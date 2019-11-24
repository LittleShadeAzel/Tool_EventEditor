using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using UnityEditor;

namespace LGP.EventEditor {
    /// <summary>
    /// A page represents one "state" an event can be. Pages have their own conditions, setups, trigger and functions.
    /// </summary>
    public class EEPage : ScriptableObject {
        #region Variables
        public string displayName;
        public int order;
        [SerializeField] public List<Condition> conditions = new List<Condition>();
        #endregion

        #region UnityMethods
        #endregion

        #region Methods
        public Condition AddCondition() {
            Condition condition = CreateInstance<Condition>();
            conditions.Add(condition);
            return condition;
        }

        public void RemoveCondition(int index) {
            Condition condition = conditions[index];
            conditions.RemoveAt(index);
            Undo.DestroyObjectImmediate(condition);
        }
        #endregion
    }
}
