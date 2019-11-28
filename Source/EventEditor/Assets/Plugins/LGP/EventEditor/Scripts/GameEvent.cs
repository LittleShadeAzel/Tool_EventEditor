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
        #region Variables
        [SerializeField]public string displayName;
        public EEPage activeEventPage;
        [SerializeField] private int selectedPageIndex = -1;
        public EEPage SelectedEventPage { get => selectedPageIndex >= 0 && selectedPageIndex < pages.Count ? pages[selectedPageIndex] : null; }
        [SerializeField] public List<EEPage> pages = new List<EEPage>();
        #endregion

        #region Unity Methods 
        private void OnEnable() {
            activeEventPage = GetActivePage();
            if (activeEventPage != null)
            Debug.Log(activeEventPage.displayName);
        }

        private void OnDisable() {
            
        }
        #endregion

        #region Method
        public void RefreshPages() {
            // Needed to remove empty nodes and get the sort right.
            int index = 0;
            for (int i = 0; i < pages.Count; i++) {
                if (pages[i] == null) {
                    pages.RemoveAt(i);
                    continue;
                }
                SerializedObject page = new SerializedObject(pages[i]);
                page.FindProperty("order").intValue = index++;
                page.ApplyModifiedProperties();
            }
        }

        public EEPage AddNewEventPage() {
            EEPage page = ScriptableObject.CreateInstance<EEPage>();
            Undo.RegisterCreatedObjectUndo(page, "Created Page");
            SerializedObject serializedPage = new SerializedObject(page);
            serializedPage.FindProperty("order").intValue = pages.Count - 1;
            serializedPage.FindProperty("displayName").stringValue = "Page" + (pages.Count + 1);        
            serializedPage.ApplyModifiedProperties();
            pages.Add(page);
            //page.displayName = "Page" + (eventPages.Count + 1);
            //page.order = eventPages.Count - 1;
            RefreshPages();
            return page;
        }

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
        #endregion
    }
}
