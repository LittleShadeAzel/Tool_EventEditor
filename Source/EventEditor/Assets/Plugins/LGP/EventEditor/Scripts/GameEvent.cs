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
        public string id;
        public string displayName;
        public GameEventPage activeEventPage;
        [SerializeField] private int selectedEventPageIndex = -1;
        public GameEventPage SelectedEventPage { get => selectedEventPageIndex >= 0 && selectedEventPageIndex < eventPages.Count ? eventPages[selectedEventPageIndex] : null; }
        [SerializeField] public List<GameEventPage> eventPages = new List<GameEventPage>();
        #endregion

        #region Unity Methods 

        #endregion

        #region Method
        public void Refresh() {
            int index = 0;
            //List<GameEventPage> list = new List<GameEventPage>(GetComponents<GameEventPage>());
            //list.Sort((a, b) => a.order.CompareTo(b.order));
            //eventPages = list;
            for (int i = 0; i < eventPages.Count; i++) {
                SerializedObject page = new SerializedObject(eventPages[i]);
                page.FindProperty("order").intValue = index++;
                page.ApplyModifiedProperties();
                //eventPages[i].order = index++;
            }
        }

        public void SortPages() {
            // Make a sorted list
            int index = 0;
            for (int i = 0; i < eventPages.Count; i++) {
                SerializedObject page = new SerializedObject(eventPages[i]);
                page.FindProperty("order").intValue = index++;
                page.ApplyModifiedProperties();
                //eventPages[i].order = index++;
            }
        }

        public GameEventPage AddNewEventPage() {
            GameEventPage page = ScriptableObject.CreateInstance<GameEventPage>();
            SerializedObject serializedPage = new SerializedObject(page);

            serializedPage.FindProperty("order").intValue = eventPages.Count - 1;
            serializedPage.FindProperty("displayName").stringValue = "Page" + (eventPages.Count + 1);
            serializedPage.ApplyModifiedProperties();
            //page.displayName = "Page" + (eventPages.Count + 1);
            //page.order = eventPages.Count - 1;
            Refresh();
            return page;
        }

        public void RemoveEventPage(int index) {
            GameEventPage eventPage = eventPages[index];
            Undo.DestroyObjectImmediate(eventPage);
            Refresh();
        }
        #endregion
    }
}
