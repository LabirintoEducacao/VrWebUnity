using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
	public TMP_InputField loginTxt;
	public TMP_InputField pwdTxt;
	public TextMeshProUGUI warnMessage;
	public Button loginBtn;
	public CubeButton srcButton;

	private void OnEnable() {
		loginTxt.text = "";
		pwdTxt.text = "";
		warnMessage.text = "";
		loginBtn.interactable = false;
		LoginHandler.OnLoginCompleted += OnLoginCompleted;
	}

	private void OnDisable() {
		LoginHandler.OnLoginCompleted -= OnLoginCompleted;
	}

	public void fieldCompleted() {
		warnMessage.text = "";
		if ((loginTxt.text != "") && (pwdTxt.text != "")) {
			loginBtn.interactable = true;
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			//back button closes
			this.cancel();
		}
	}

	public void cancel() {
		srcButton.afterCanvasCancel();
	}

	public void login() {

		LoginHandler.handler.loginAsync(loginTxt.text, pwdTxt.text);
		loginBtn.interactable = false;
	}
	void OnLoginCompleted(UserInfo user) {
		if (user.uid.Equals("-1")) {
			warnMessage.text = user.username;
		} else {
			warnMessage.text = "Login OK!";
			Invoke("closeCanvasOK", 1f);
		}
	}

	void closeCanvasOK() {
		srcButton.afterCanvasOK();
	}
}
