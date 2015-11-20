using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {

	void Start () {
		Destroy (gameObject, 2.0f);
	}
}
