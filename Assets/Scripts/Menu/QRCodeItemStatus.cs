using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeItemStatus : MonoBehaviour {
	public Color notReady;
	public Color ready;
	public Sprite notReadyBG;
	public Sprite readyBG;
	QRCode qr;
	TextMeshProUGUI textfield;
	Image im;
	bool wasRead = true;
	public int index = -1;

	private void Start() {
		this.qr = GetComponentInParent<QRCode>();
		this.textfield = GetComponentInChildren<TextMeshProUGUI>();
		this.im = GetComponent<Image>();
	}

	void Update()
    {
        if ((this.qr != null) && (index >=0)) {
			this.textfield.text = index.ToString("00");
			if (this.wasRead) {
				if (this.qr.base64BitRead(index)) {
					this.wasRead = true;
					this.im.sprite = this.readyBG;
					this.textfield.color = this.ready;
				}
			}
		}
    }
}
