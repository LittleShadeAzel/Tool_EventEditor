using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using UnityEditor;
using UnityEngine.Events;

namespace LGP.EventEditor {
    /// <summary>
    /// A page represents one "state" an event can be. Pages have their own conditions, setups, trigger and functions.
    /// </summary>
    [Serializable]
    public class EEPage : ScriptableObject {
        #region Variables
        public GameEvent gameEvent;
        public string displayName;
        public int order;
        public int conditionIndex = -1;
        public List<Condition> conditions = new List<Condition>();
        public UnityEvent unityEvents = new UnityEvent();
        public int triggerIndex = -1;
        public bool isLooping;
        public bool isCoroutine;
        private bool isRunning = false;
        public bool IsRunning => isRunning;
        private bool isForcingStop = false;
        public ETriggerMode TriggerMode {
            get {
                if (Enum.TryParse<ETriggerMode>(Enum.GetNames(typeof(ETriggerMode))[triggerIndex], out ETriggerMode result)) return result;
                return ETriggerMode.Autorun;
            }
        }

        #endregion

        #region UnityMethods

        #endregion

        #region Methods
        /// <summary>
        /// Adds a condition the the page.
        /// </summary>
        /// <returns>Returns the new condition instance.</returns>
        public Condition AddCondition() {
            Condition condition = CreateInstance<Condition>();
            conditions.Add(condition);
            SerializedObject serializedCondition = new SerializedObject(condition);
            serializedCondition.FindProperty("page").objectReferenceValue = this;
            serializedCondition.ApplyModifiedProperties();
            return condition;
        }

        /// <summary>
        /// Removes condition of the page based on the index.
        /// </summary>
        /// <param name="index">Removes a condition at this position.</param>
        public void RemoveCondition(int index) {
            Condition condition = conditions[index];
            conditions.RemoveAt(index);
            Undo.DestroyObjectImmediate(condition);
        }

        /// <summary>
        /// Loops through conditions and checks their outcome.
        /// </summary>
        /// <returns>True when all conditions are met.</returns>
        public bool CheckConditons() {
            for (int i = 0; i < conditions.Count; i++) {
                Condition condition = conditions[i];
                if (!condition.IsValid) continue;
                if (!condition.CheckCondition()) return false;
            }
            return true;
        }

        public void ForceStop() {
            isForcingStop = true;
        }

        public void InvokeFunctions() {
            if (!isCoroutine) {
                RunFunctionsProcedural();
            }
        }

        public void RunFunctionsProcedural() {
            isRunning = true;
            do {
                if (unityEvents.GetPersistentEventCount() > 0) unityEvents.Invoke();
                if (isForcingStop) break;
            } while (isLooping);
            isForcingStop = false;
            isRunning = false;
        }


        public IEnumerator RunFunctionsParallel() {
            isRunning = true;
            do {
                if (unityEvents.GetPersistentEventCount() > 0) unityEvents.Invoke();
                if (isForcingStop) break;
                yield return null;
            } while (isLooping);
            isForcingStop = false;
            isRunning = false;
        }
        #endregion
    }
}
