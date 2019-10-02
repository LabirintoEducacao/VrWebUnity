using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserStatus : MonoBehaviour
{
	public TextMeshProUGUI username;
	public GameObject logoutBTN;

	public void logout() {
		LoginHandler.handler.logout();
	}

    void Update()
    {
		logoutBTN.SetActive(LoginHandler.handler.isValidUser);
        if (LoginHandler.handler.isValidUser) {
			username.text = LoginHandler.handler.user.username;
		} else {
			username.text = "Visitante";
		}
    }
}
