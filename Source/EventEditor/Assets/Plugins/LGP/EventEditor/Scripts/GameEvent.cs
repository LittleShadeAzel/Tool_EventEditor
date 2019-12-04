using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;

namespace LGP.EventEditor {


    /// <summary>
    /// This class enables game objects to have an event editor. 
    /// An event contains one or many pages which can change the event processing depending on what page is currently active.
    /// </summary>
    public class GameEvent : MonoBehaviour {

        #region Delegates
        /// <summary>
        /// This delegate fires when a page becomes active.
        /// </summary>
        /// <param name="page">Returns the new active page</param>
        public delegate void OnPageActiveDelegate(EEPage page);
        #endregion

        #region Variables
        [SerializeField] private int selectedPageIndex = -1;
        public int SelectedPageIndex { get => selectedPageIndex; }
        [SerializeField] private EEPage activePage;
        public EEPage ActivePage { get => activePage; }
        public OnPageActiveDelegate onPageActive;
        public EEPage SelectedEventPage { get => selectedPageIndex >= 0 && selectedPageIndex < pages.Count ? pages[selectedPageIndex] : null; }
        public List<EEPage> pages = new List<EEPage>();
        [SerializeField] private Dictionary<string, bool> localSwtiches = new Dictionary<string, bool>();
        public Dictionary<string, bool> LocalSwitches { get => localSwtiches; }
        #endregion

        #region Unity Methods 
        private void OnEnable() {
            onPageActive += StartPageSetup;
        }

        private void Update() {
            CheckActivePage();
        }

        private void OnDisable() {
            StopAllCoroutines();
            activePage = null;
        }
        #endregion

        #region Method
        /// <summary>
        /// Rerfreshes Pages of the Event, to keep sorting order right. It also removes empty objects from the list.
        /// </summary>
        public void RefreshPages() {
            for (int i = 0; i < pages.Count; i++) {
                if (pages[i] == null) {
                    pages.RemoveAt(i);
                    continue;
                }
            }
        }

        /// <summary>
        /// Adds new a Page to the event with initial values.
        /// </summary>
        /// <returns>Returns the new page instance.</returns>
        public EEPage AddNewEventPage() {
            EEPage page = ScriptableObject.CreateInstance<EEPage>();
            Undo.RegisterCreatedObjectUndo(page, "Created Page");
            SerializedObject serialPage = new SerializedObject(page);
            //serializedPage.FindProperty("order").intValue = pages.Count - 1;
            serialPage.FindProperty("displayName").stringValue = "Page" + (pages.Count + 1);
            serialPage.FindProperty("owner").objectReferenceValue = this;
            serialPage.ApplyModifiedProperties();
            pages.Add(page);

            //page.displayName = "Page" + (eventPages.Count + 1);
            //page.order = eventPages.Count - 1;
            RefreshPages();
            return page;
        }

        /// <summary>
        /// Removes a page from the Event based on index.
        /// </summary>
        /// <param name="index">Position of the page to be removed.</param>
        public void RemoveEventPage(int index) {
            EEPage page = pages[index];
            pages.RemoveAt(index);
            Undo.DestroyObjectImmediate(page);
            RefreshPages();
        }

        /// <summary>
        /// Sets the active Page for this Event. The first page with its conditions beeing all true will be returend active.
        /// </summary>
        public EEPage GetActivePage() {
            for (int i = 0; i < pages.Count; i++) {
                EEPage page = pages[i];
                if (page.CheckConditons()) return page;
            }
            return null;
        }

        public void SetLocalSwtich(string value) {
            string[] args = value.Split(char.Parse(","));
            SetLocalSwtich(args[0], bool.Parse(args[1]));
        }
        public void SetLocalSwtich(string key, bool flag) {
            if (!localSwtiches.ContainsKey(key)) {
                AddNewLocalSwtich(key, flag);
            } else {
                localSwtiches[key] = flag;
            }
        }

        public void AddNewLocalSwitch(string key) {
            if (!localSwtiches.ContainsKey(key)) {
                AddNewLocalSwtich(key, true);
            }
        }

        private void AddNewLocalSwtich(string key, bool flag) {
            localSwtiches.Add(key, flag);
        }

        public bool GetLocalSwtich(string key) {
            if (localSwtiches.TryGetValue(key, out bool value)) return value;
            return false;
        }

        private void InvokePageFunctions() {
            if (!activePage) return;
            if (activePage.IsRunning) return;
            if (activePage.TriggerMode == ETriggerMode.Autorun) {
                activePage.InvokeFunctions();
            }
        }

        private void CheckActivePage() {
            EEPage newPage = GetActivePage();
            if (activePage != newPage) {
                StopAllCoroutines();
                activePage = newPage;
                if (onPageActive != null) onPageActive.Invoke(activePage);
                InvokePageFunctions();
            }
        }

        private void StartPageSetup(EEPage page) {
            activePage.Setup();
        }

        

        public void ForceFunctionStop() {
            if (!activePage) return;
            if (activePage.IsRunning) activePage.ForceStop();
        }
        #endregion
    }
}
