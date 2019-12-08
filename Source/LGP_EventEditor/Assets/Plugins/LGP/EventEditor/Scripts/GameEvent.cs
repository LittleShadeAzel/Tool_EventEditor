using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using LGP.Utils;

namespace LGP.EventEditor {


    [Serializable]
    public class InitialLocalSwitchDictionary : SerializedGenericDictionary<string, bool> { }

    [Serializable]
    public class LocalSwtichDictionary : SerializedGenericDictionary<string, bool> { }



    /// <summary>
    /// This compnent enables a gameObject to have an event editor. 
    /// A GameEvent contains pages which can change the gameObject depending on what page is currently active.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("LGP/Game Event")]
    public class GameEvent : MonoBehaviour {

        #region Statics
        /// <summary>
        /// A global List of all loaded Game Events inside active scenes. This List automaticly updates itself when gameEvents are enabled/disabled.
        /// </summary>
        private static List<GameEvent> gameEvents = new List<GameEvent>();
        public static List<GameEvent> GameEvents { get => gameEvents; }

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
        /// Interacts and starts the function process of the GameEvent found in the currently loaded Game Event list.
        /// </summary>
        /// <param name="gameEvent">The specified GameEvent to interact with.</param>
        public static void Interact(GameEvent gameEvent) {
            gameEvents.Find((GameEvent g) => {
                return g.Equals(gameEvent);
            });
        }

        /// <summary>
        /// Interacts and starts the function process of the GameEvent found in the currently loaded Game Event list.
        /// </summary>
        /// <param name="gameEvent">The name of the GameEvent to interact with.</param>
        public static void Interact(string gameEvent) {
            gameEvents.Find((GameEvent g) => {
                return (g.name == gameEvent);
            });
        }

        /// <summary>
        /// Gets a GameEvent from the list.
        /// </summary>
        /// <param name="gameEvent"></param>
        /// <returns>Name of the gameobject that holds the game object.</returns>
        public static GameEvent GetGameEvent(string gameEvent) {
            return gameEvents.Find((GameEvent g) => {
                return (g.name == gameEvent);
            });
        }


        #endregion

        #region Variables
        [SerializeField] private int selectedPageIndex = -1;
        public int SelectedPageIndex { get => selectedPageIndex; }
        [SerializeField] private EEPage activePage;
        public EEPage ActivePage { get => activePage; }
        public EEPage SelectedEventPage { get => selectedPageIndex >= 0 && selectedPageIndex < pages.Count ? pages[selectedPageIndex] : null; }
        public List<EEPage> pages = new List<EEPage>();
        [SerializeField] private LocalSwtichDictionary localSwitches = new LocalSwtichDictionary();
        public Dictionary<string, bool> LocalSwitches { get => localSwitches.Dictionary; set => localSwitches.Dictionary = value; }
        [SerializeField] private InitialLocalSwitchDictionary initialLocalSwitches = new InitialLocalSwitchDictionary();
        public Dictionary<string, bool> InitialLocalSwitches { get => initialLocalSwitches.Dictionary; }
        
        #endregion

        #region Unity Methods 
        private void OnEnable() {
            ResetLocalSwitches();
            AddSelfToGameEvents(this);
        }

        private void Update() {
            EEPage newPage = GetActivePage();
            if (activePage != newPage) {
                StopAllCoroutines();
                activePage = newPage;
                if (activePage == null) return;
                activePage.Setup();
                if (activePage.TriggerMode == ETriggerMode.Autorun) {
                    activePage.InvokeFunctions();
                } else if (activePage.TriggerMode == ETriggerMode.Custom) {
                    if (GetComponent<IEECustomTrigger>() != null && GetComponent<IEECustomTrigger>().CustomTrigger()) {
                        activePage.InvokeFunctions();
                    }
                }
            }
        }

        private void OnDisable() {
            ForceFunctionStop();
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

        #region Methods: Local Getter/Setters
        public void SetLocalSwtich(string value) {
            string[] args = value.Split(char.Parse(","));
            SetLocalSwtich(args[0], bool.Parse(args[1]));
        }

        public void SetLocalSwtich(string key, bool flag) {
            if (!LocalSwitches.ContainsKey(key)) {
                AddNewLocalSwtich(key, flag);
            } else {
                LocalSwitches[key] = flag;
            }
        }

        public void AddNewLocalSwitch(string key) {
            if (!LocalSwitches.ContainsKey(key)) {
                AddNewLocalSwtich(key, true);
            }
        }

        public void AddNewLocalSwtich(string key, bool flag) {
            if (!LocalSwitches.ContainsKey(key)) LocalSwitches.Add(key, flag);
        }

        public bool GetLocalSwtich(string key) {
            if (LocalSwitches.TryGetValue(key, out bool value)) return value;
            Debug.Log("Local Switch with Key [" + key + "] doesnt exist.");
            return false;
        }

        /// <summary>
        /// Resets the Local Switch catalog of this GameEvent to the inital values setup inside the Event Editor.
        /// </summary>
        public void ResetLocalSwitches() {
            LocalSwitches = new Dictionary<string, bool>(InitialLocalSwitches);
        }
        #endregion

        #region Methods: Function Process Flow Control
        /// <summary>
        /// Force a stop to the function invoking of the GameEvent.
        /// </summary>
        public void ForceFunctionStop() {
            if (!activePage) return;
            if (!activePage.IsReady) return;
            StopAllCoroutines();
            activePage.ForceStop();
        }

        /// <summary>
        /// Enable the function invoke to loop itself after completion. DANGER: CAN UNLEASH AN INFINITE LOOP!!!
        /// </summary>
        /// <param name="value"></param>
        public void LoopFunctions(bool value) {
            if (!activePage) return;
            if (!activePage.IsReady) return;
            activePage.IsLooping = value;
        }
        #endregion

        /// <summary>
        /// If the active page trigger is set to "Interaction", this function will invoke the functions.
        /// </summary>
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
