using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGP.Utils {
    public static class DebugEventEditor {

        public static bool isDebugActive = true;

        public static void EventDebug(object content) {
            Debug.Log(content);
        }
    }
}
