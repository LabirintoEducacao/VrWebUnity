using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserStatus : MonoBehaviour
{
	public TextMeshProUGUI username;
	public string baseText;
	public GameObject logoutBTN;
	public GameObject loginBTN;
	public void logout() {
		LoginHandler.handler.logout();
	}

    void FixedUpdate()
    {
		if (logoutBTN != null)
			logoutBTN.SetActive(LoginHandler.handler.isValidUser);
		if (loginBTN != null)
			loginBTN.SetActive(!LoginHandler.handler.isValidUser);

		if (LoginHandler.handler.isValidUser) {
			username.text = baseText+LoginHandler.handler.user.username;
		} else {
			username.text = baseText+"Visitante";
		}
    }
}
