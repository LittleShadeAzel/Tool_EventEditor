﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LGP.EventEditor;

public class TriggerTest : MonoBehaviour {
    private void Update() {
        GameEvent.Interact(GetComponent<GameEvent>());
    }

    public void DebugTest() {
        Debug.Log("Interact Test");
    }
}
