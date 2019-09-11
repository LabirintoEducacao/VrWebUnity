using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRCodeItemList : MonoBehaviour
{
	QRCode qr;
	public GameObject QRItemStatusPrefab;

	private void OnEnable() {
		this.qr = GetComponentInParent<QRCode>();
		if (this.qr == null) {
			Debug.LogWarning("Can't create list without QrCode obj.");
			return;
		}
	}

	private void LateUpdate() {
		if (this.qr.base64bits != null) {
			//create children if not already created
			if (this.qr.base64bits.Count != this.transform.childCount) {
				//clear
				this.clear();
				for (int i = 0; i < this.qr.base64bits.Count; i++) {
					QRCodeItemStatus item = Instantiate(QRItemStatusPrefab, this.transform).GetComponent<QRCodeItemStatus>();
					item.index = i;
				}
			}
		}
	}
	void clear() {
		for (int i = this.transform.childCount - 1; i >= 0; i--) {
			Destroy(this.transform.GetChild(i).gameObject);
		}
	}

	private void OnDisable() {
		//clear children
		this.clear();
		this.qr = null;
	}
}
