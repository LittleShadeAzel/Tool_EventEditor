using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace LGP.EventEditor {

    /// <summary>
    /// This compnent enables a gameObject to have an event editor. 
    /// A GameEvent contains pages which can change the gameObject depending on what page is currently active.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameEvent : MonoBehaviour {

        #region Statics
        /// <summary>
        /// A global List of all loaded Game Events inside active scenes. This List automaticly updates itself when gameEvents are enabled/disabled.
        /// </summary>
        private static List<GameEvent> gameEvents = new List<GameEvent>();
        
        #region Statics: List<GameEvent> Getter/Setter
        private static void AddSelfToGameEvents(GameEvent gameEvent) {
            if (!gameEvents.Contains(gameEvent)) gameEvents.Add(gameEvent);
        }

        private static void RemoveSelfFromGameEvents(GameEvent gameEvent) {
            if (gameEvents.Contains(gameEvent)) gameEvents.Remove(gameEvent);
        }

        /// <summary>
        /// Manually clears the List of all GameEvents. This is usually Called when the list has to refresh itself. 
        /// </summary>
        public static void ClearGameEvents() {
            gameEvents.Clear();
        }

        /// <summary>
        /// Manually refreshes the List of all GameEvents inside active scenes. Usually the Game Events autoamticly add to or remove themselfs from the list.
        /// </summary>
        public static void RefreshGameEvents() {
            gameEvents.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                GameObject[] gameObjects = scene.GetRootGameObjects();
                for (int j = 0; j < gameObjects.Length; j++) {
                    GameObject go = gameObjects[j];
                    if (!go.activeInHierarchy) continue;
                    if (go.GetComponent<GameEvent>()) {
                        gameEvents.Add(go.GetComponent<GameEvent>());
                    } else {
                        if (go.GetComponentInChildren<GameEvent>()) gameEvents.Add(go.GetComponentInChildren<GameEvent>());
                    }
                }
            }
        }
        #endregion
        
        /// <summary>
        /// Interacts and starts the function process of the GameEvent.
        /// </summary>
        /// <param name="gameEvent">The GameEvent to interact with.</param>
        public static void Interact(GameEvent gameEvent) {
            gameEvent.Interact();
        }
        #endregion

        #region Delegates
        //OnPageBeforeActive;
        //OnPageBeforeFunction
        //OnPageAfterFunction
        #endregion

        #region Variables
        [SerializeField] private int selectedPageIndex = -1;
        public int SelectedPageIndex { get => selectedPageIndex; }
        [SerializeField] private EEPage activePage;
        public EEPage ActivePage { get => activePage; }
        public EEPage SelectedEventPage { get => selectedPageIndex >= 0 && selectedPageIndex < pages.Count ? pages[selectedPageIndex] : null; }
        public List<EEPage> pages = new List<EEPage>();
        [SerializeField] private Dictionary<string, bool> localSwtiches = new Dictionary<string, bool>();
        public Dictionary<string, bool> LocalSwitches { get => localSwtiches; }
        #endregion

        #region Unity Methods 
        private void Awake() {
            Debug.Log("Subscribe to Scene Manager");
            RefreshGameEvents();
        }

        private void OnEnable() {
            AddSelfToGameEvents(this);
        }

        private void Update() {
            Debug.Log(gameEvents.Count);
            UpdateActivePage();
        }

        private void FixedUpdate() {
            UpdateCheckCustomTrigger();
        }

        private void OnDisable() {
            StopAllCoroutines();
            activePage = null;
            RemoveSelfFromGameEvents(this);
        }
        #endregion

        #region Methods

        #region Methods: Page Getter/Setter
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
        #endregion

        #region Methods: Local/Global Swtiches Getter/Setters
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

        public void ClearLocalSwtiches() {
            localSwtiches.Clear();
        }
        #endregion

        #region Methods: Function Process Flow Control
        public void ForceFunctionStop() {
            if (!activePage) return;
            if (!activePage.IsReady) return;
            activePage.ForceStop();
        }

        public void LoopFunctions(bool value) {
            if (!activePage) return;
            if (!activePage.IsReady) return;
            activePage.IsLooping = value;
        }
        #endregion

        private void UpdateActivePage() {
            EEPage newPage = GetActivePage();
            if (activePage != newPage) {
                StopAllCoroutines();
                activePage = newPage;
                activePage.Setup();
                InvokeAutoRun();
            }
        }

        private void InvokeAutoRun() {
            if (!activePage) return;
            if (!activePage.IsReady) return;
            if (activePage.TriggerMode == ETriggerMode.Autorun) {
                activePage.InvokeFunctions();
            }
        }

        private void UpdateCheckCustomTrigger() {
            if (!activePage) return;
            if (!activePage.IsReady) return;
            if (activePage.TriggerMode == ETriggerMode.Custom) {
                if (GetComponent<IEECustomTrigger>() != null && GetComponent<IEECustomTrigger>().CustomTrigger()) {
                    activePage.InvokeFunctions();
                }
            }
        }

        public void Interact() {
            if (!activePage) return;
            if (!activePage.IsReady) return;
            if (activePage.TriggerMode == ETriggerMode.Interaction) {
                activePage.InvokeFunctions();
            }
        }
        #endregion
    }
}
