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
		InvokeRepeating("fieldCompleted", 1f, 0.2f);
	}

	private void OnDisable() {
		LoginHandler.OnLoginCompleted -= OnLoginCompleted;
		CancelInvoke();
	}

	public void fieldCompleted() {
		//warnMessage.text = "";
		if ((loginTxt.text != "") && (pwdTxt.text != "")) {
			loginBtn.interactable = true;
		} else {
			loginBtn.interactable = false;
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			//back button closes
			this.cancel();
		}
	}

	public void cancel() {
		if (srcButton != null)
			srcButton.afterCanvasCancel();
	}

	public void login() {
		_ = LoginHandler.handler.loginAsync(loginTxt.text, pwdTxt.text);
		loginBtn.interactable = false;
		CancelInvoke("fieldCompleted");
	}
	void OnLoginCompleted(UserInfo user) {
		if (user.uid.Equals("-1")) {
			warnMessage.text = user.username;
			InvokeRepeating("fieldCompleted", 0.2f, 0.2f);
		} else {
			warnMessage.text = "Login OK!";
			Invoke("closeCanvasOK", 2f);
		}
	}

	void closeCanvasOK() {
		if (srcButton != null)
			srcButton.afterCanvasOK();
		else {
			//rever como fazer
			GetComponentInParent<Animator>().SetTrigger("Back");
		}
	}
}
