using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;

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
        [SerializeField] public List<GameEventPage> eventPages = new List<GameEventPage>();
        private UnityEvent functions = new UnityEvent();

        #endregion

        #region Unity Methods 

        #endregion

        #region Method
        public void Refresh() {
            int index = 0;
            List<GameEventPage> list = new List<GameEventPage>(GetComponents<GameEventPage>());
            list.Sort((a, b) => a.order.CompareTo(b.order));
            eventPages = list;
            for (int i = 0; i < eventPages.Count; i++) {
                eventPages[i].order = index++;
            }
        }

        public void SortPages() {
            // Make a sorted list
            int index = 0;
            for (int i = 0; i < eventPages.Count; i++) {
                eventPages[i].order = index++;
            }
        }

        public GameEventPage AddNewEventPage() {
            GameEventPage page = Undo.AddComponent<GameEventPage>(gameObject);
            page.displayName = "Page" + (eventPages.Count + 1);
            page.order = eventPages.Count() - 1;
            Refresh();
            SetActivePage(eventPages.Count - 1);
            return page;
        }

        public void RemoveEventPage(int index) {
            GameEventPage eventPage = eventPages[index];
            Undo.DestroyObjectImmediate(eventPage);
            Refresh();
            if (eventPages.Count != 0) {
                activeEventPage = eventPages[0];
            } else {
                activeEventPage = null;
            }
            
        }

        public void SetActivePage(int index) {
            activeEventPage = eventPages[index];
        }

        #endregion
    }
}
