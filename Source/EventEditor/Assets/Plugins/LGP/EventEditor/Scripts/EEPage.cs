using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using UnityEditor;
using UnityEngine.Events;

namespace LGP.EventEditor {
    /// <summary>
    /// A page represents one "state" a GameEvent can be. Pages have their own conditions, setups, triggers and functions.
    /// </summary>
    [Serializable]
    public class EEPage : ScriptableObject {
        #region Variables
        [SerializeField] private GameEvent owner;
        public GameEvent Owner { get => owner; set => owner = value; }
        [SerializeField] private string displayName;
        public string DisplayName { get => displayName; }
        [SerializeField] private int conditionIndex = -1;
        public int ConditionIndex { get => conditionIndex; }
        [SerializeField] private List<Condition> conditions = new List<Condition>();
        public List<Condition> Conditions { get => conditions; }
        [SerializeField] private UnityEvent unityEvents = new UnityEvent();
        public UnityEvent UnityEvents { get => unityEvents; }
        [SerializeField] private int triggerIndex;
        public int TriggerIndex { get => triggerIndex; }
        private bool isLooping = false;
        public bool IsLooping { get => isLooping; set => isLooping = value; }
        [SerializeField] private bool isCoroutine;
        public bool IsCoroutine { get => isCoroutine; }
        private bool isRunning;
        private bool isForcingStop = false;
        public bool IsReady { get => !isRunning; }
        public ETriggerMode TriggerMode {
            get {
                if (Enum.TryParse<ETriggerMode>(Enum.GetNames(typeof(ETriggerMode))[triggerIndex], out ETriggerMode result)) return result;
                return ETriggerMode.Interaction;
            }
        }
        #endregion

        #region Methods
        #region Methods: Condition Getters/Setters
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
        #endregion

        #region Methods: Setup Handlers
        public void Setup() {
            // Handle Setup
        }
        #endregion

        #region Methods: Function Handlers
        public void InvokeFunctions() {
            if (!isCoroutine) {
                RunFunctionsProcedural();
            } else {
                owner.StartCoroutine(RunFunctionsParallel());
            }
        }

        public void RunFunctionsProcedural() {
            isRunning = true;
            do {
                if (isForcingStop) break;
                if (unityEvents.GetPersistentEventCount() > 0) unityEvents.Invoke();
            } while (isLooping);
            isForcingStop = false;
            isRunning = false;
        }


        public IEnumerator RunFunctionsParallel() {
            isRunning = true;
            do {
                if (isForcingStop) break;
                if (unityEvents.GetPersistentEventCount() > 0) unityEvents.Invoke();
                yield return null;
            } while (isLooping);
            isForcingStop = false;
            isRunning = false;
        }

        public void ForceStop() {
            isForcingStop = true;
            isRunning = false;
        }
        #endregion
        #endregion
    }
}
