using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGP.EventEditor {
    /// <summary>
    /// A global manifest of switches, which can be defined by the user and game.
    /// </summary>
    public static class GlobalSwitch {

        #region Variabels
        private static Dictionary<string, bool> manifest = new Dictionary<string, bool>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new switch to the manifest. 
        /// Switch key has to be unique. 
        /// Default Value is always false.
        /// </summary>
        /// <param name="name">Name of the switch.</param>
        /// <returns>True, when the switch has been added successfully.</returns>
        public static bool Add(string name) {
            if (manifest.ContainsKey(name)) {
                manifest.Add(name, false);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Adds a new switch to the manifest.
        /// Switch key has to be unique.
        /// </summary>
        /// <param name="name">Name of the switch.</param>
        /// <param name="value">Default state of the value</param>
        /// <returns>True, when the switch has been added successfully.</returns>
        public static bool Add(string name, bool value) {
            if (manifest.ContainsKey(name)) {
                manifest.Add(name, false);
                return true;
            } else {
                return false;
            }
        }


        /// <summary>
        /// Removes the global swtich from the manifest.
        /// </summary>
        /// <param name="name">Name of the swtich to be removed.</param>
        /// <returns>True, when the swtich has been successfully removed.</returns>
        public static bool Remove(string name) {
            if (manifest.ContainsKey(name)) {
                manifest.Remove(name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the switch to a specific state.
        /// </summary>
        /// <param name="name">The name of the global Switch.</param>
        /// <param name="value">The sate which the global switch is going to be.</param>
        /// <returns>True, if the state of the switch has been successfully changed.</returns>
        public static bool Set(string name, bool value) {
            if (manifest.ContainsKey(name)) {
                if (manifest.TryGetValue(name, out bool globalSwitch)) {
                    globalSwitch = value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the value of the global switch with it's name.
        /// </summary>
        /// <param name="name">The name of the global switch.</param>
        /// <returns>The value of the global Switch</returns>
        public static bool Get(string name) {
            if (manifest.TryGetValue(name, out bool value)) {
                return value;
            }
            return false;
        }
        #endregion
    }
}
