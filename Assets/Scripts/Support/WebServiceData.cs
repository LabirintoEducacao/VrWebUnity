using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WebRequestInfo", menuName = "LARCOM/Web Request Config", order = 1)]
public class WebServiceData : ScriptableObject
{
	public string baseURL;
	public string loginPath;
	public string getRoomPath;
	public string getMazePath;
	public string loginURL {
		get {
			return baseURL + loginPath;
		}
	}

	public string getRoomURL {
		get {
			return baseURL + getRoomPath;
		}
	}

	public string getMazeURL {
		get {
			return baseURL + getMazePath;
		}
	}
}
