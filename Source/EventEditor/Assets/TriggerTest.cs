using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LGP.EventEditor;

public class TriggerTest : MonoBehaviour {
    private void Update() {
        //GetComponent<GameEvent>().Interact();
    }

    public void ChangeScene() {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }
}
