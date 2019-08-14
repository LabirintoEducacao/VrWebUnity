using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CubeButton : MonoBehaviour, IGvrPointerHoverHandler {
    public Light spotLight;
    public string label;
    public string destination;
    public bool isDestinationVR;
    public TMPro.TextMeshPro textHolder;
    public Animator _anim;

    public GameObject canvasPanel;

    float minFlickTime = 3f;
    float maxFlickTime = 20f;
    int nextSceneLoad = -1;

    CubeButton[] buttons;

    private void OnValidate () {
        updateText ();
    }

    [ExecuteAlways, ExecuteInEditMode]
    void updateText () {
        if (this.textHolder != null)
            this.textHolder.text = label;
    }

    void Start () {
        this._anim = this.GetComponent<Animator> ();
        updateText ();
        Invoke ("flick", Random.Range (minFlickTime, maxFlickTime));
        if (canvasPanel != null) {
            canvasPanel.SetActive (false);
        }
        buttons = FindObjectsOfType<CubeButton> ();
    }

    void flick () {
        if (!this._anim.GetBool ("click")) {
            // not clicking, can flicker
            this._anim.SetTrigger ("flicker");
        }
        Invoke ("flick", Random.Range (minFlickTime, maxFlickTime));
    }

    void OnDestroy () {
        this._anim = null;
    }

    IEnumerator changeScene () {
        this.nextSceneLoad = 1; //loading
        Camera.main.GetComponent<EnableDisableVR> ().changeState (isDestinationVR, allowChangeScene);
        yield return new WaitForSeconds (0.1f);
        // SceneManager.LoadScene (destination);
    }

    void allowChangeScene(int teste) {
        this.nextSceneLoad = 2;
    }

    private void FixedUpdate() {
        if (this.nextSceneLoad == 2) {
            this.loadScene(0);
            this.nextSceneLoad = -1;
            StopAllCoroutines();
        }
    }

    void loadScene (int teste) {
        SceneManager.LoadScene (destination);
    }
#region mouse-touch-input

    [ExecuteAlways]
    void computeClick () {
        this._anim.SetBool ("click", false);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ui_interaction", "click", this.name);
        if (canvasPanel == null) {
            StartCoroutine (changeScene ());
        } else {
            changeButtonsState (false);
            canvasPanel.SetActive (true);
        }
    }
    void click () {
        this._anim.SetBool ("click", true);
    }

    void setInputOver (bool over) {
        this._anim.SetBool ("isOver", over);
    }
    private void OnMouseEnter () {
        this.setInputOver (true);
    }

    private void OnMouseExit () {
        this.setInputOver (false);
    }

    private void OnMouseDown () {
        click ();
    }

    public void OnGvrPointerHover (PointerEventData eventData) {
        this.setInputOver (true);
    }
#endregion
    public void afterCanvasOK () {
        canvasPanel.SetActive (false);
        changeButtonsState (true);
        if (System.String.IsNullOrEmpty (destination)) {
            return;
        } else {
            StartCoroutine (changeScene ()); // UnityBarCodeScanner has a problem with disable and destroy in the same frame
        }
        // changeScene ();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public void afterCanvasCancel () {
        canvasPanel.SetActive (false);
        changeButtonsState (true);
    }

    public void changeButtonsState (bool enable) {
        foreach (CubeButton btn in buttons) {
            btn.gameObject.SetActive (enable);
        }
    }
}
