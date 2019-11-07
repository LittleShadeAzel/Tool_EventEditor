using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace LGP.EE {
    /// <summary>
    /// This class enables game objects to have an event editor. 
    /// An event contains one or many pages which can change the event processing depending on what page is currently active.
    /// </summary>
    public class GameEvent : MonoBehaviour {
        #region Variables
        public string id;
        public string displayName;
        public GameEventPage activeEventPage;
        [SerializeField] private List<GameEventPage> eventPages = new List<GameEventPage>();
        private UnityEvent functions = new UnityEvent();

        #endregion

        #region Unity Methods 

        #endregion

        #region Method
        public void Refresh() {
            eventPages = new List<GameEventPage>(GetComponents<GameEventPage>());
        }

        public GameEventPage AddNewEventPage() {
            GameEventPage page = Undo.AddComponent<GameEventPage>(gameObject);
            page.displayName = "Page" + (eventPages.Count + 1);
            Refresh();
            SetActivePage(eventPages.Count - 1);
            return page;
        }

        public void RemoveEventPage(int index) {
            GameEventPage eventPage = eventPages[index];
            if (activeEventPage.Equals(eventPage)) {
                if (eventPages.Count > 0) {
                    activeEventPage = eventPages[0];
                } else {
                    activeEventPage = null;
                }
            }
            Undo.DestroyObjectImmediate(eventPage);
            Refresh();
        }

        public void SetActivePage(int index) {
            activeEventPage = eventPages[index];
        }

        #endregion
    }
}
