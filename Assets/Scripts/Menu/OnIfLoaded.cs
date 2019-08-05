using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnIfLoaded : MonoBehaviour {
    void FixedUpdate () {
        TextMeshProUGUI textMesh = this.GetComponent<TMPro.TextMeshProUGUI> ();
        if (DataManager.manager.mazeLD == null) {
            textMesh.text = "";
        } else {
            textMesh.text = "Data Loaded!";
        }
        // this.GetComponent<TMPro.TextMeshProUGUI> ().enabled = (DataManager.manager.mazeLD != null);
        this.GetComponent<Animator> ().SetFloat ("random", Random.Range (0f, 6f));
    }
}
