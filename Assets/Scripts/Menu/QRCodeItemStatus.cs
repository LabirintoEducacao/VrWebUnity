using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeItemStatus : MonoBehaviour
{
	public Color notReady;
	public Color ready;
	QRCode qr;
	Text textfield;
	Image im;
	public int index = -1;

	private void Start() {
		this.qr = GetComponentInParent<QRCode>();
		this.textfield = GetComponentInChildren<Text>();
		this.im = GetComponent<Image>();
	}

	void Update()
    {
        if ((this.qr != null) && (index >=0)) {
			this.textfield.text = index.ToString("00");
			this.im.color = (this.qr.base64BitRead(index) ? this.ready : this.notReady);
		}
    }
}
