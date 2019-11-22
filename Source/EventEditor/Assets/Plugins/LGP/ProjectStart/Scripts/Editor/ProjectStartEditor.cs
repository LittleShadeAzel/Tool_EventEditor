using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LGP.Utils;
namespace LGP.ProjectStart {
    public class ProjectStartEditor : EditorWindow {
        #region Variables
        public static ProjectStartEditor window;
        
        [MenuItem("Window/Project Starter")]
        public static ProjectStartEditor CreateWindow() {
            if (window == null) {
                window = CreateWindow<ProjectStartEditor>("Project Starter");
            } else {
                DestroyImmediate(window);
            }
            return window;
        }
        #endregion

        #region Unity Methods
        private void OnEnable() {
        }

        private void OnGUI() {
            IOUtils.CreateFolder("Assets/New Folder");
            
        }
        #endregion

        #region Methods
        #endregion
    }
}
