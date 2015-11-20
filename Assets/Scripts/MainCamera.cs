using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	private Transform				canon;
	private Vector3					offset;

	void Start () 
	{
		canon = transform.parent.transform;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update ()
	{
		transform.LookAt (canon.position);
	}
}
