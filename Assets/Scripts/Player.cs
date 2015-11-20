using UnityEngine;
using System.Collections;

public class Player : Tank {

	private CharacterController		controller;
	private Transform				canon;
	private Vector3					direction = Vector3.zero;
	private float					rot_tank = 0.0f;
	private float					rot_canon = 0.0f;
	private float					boost_gauge = 500.0f;
	private float					speed;

	private UnityEngine.UI.Text		hpText;
	private UnityEngine.UI.Text		nbMissilesText;
	private UnityEngine.UI.Image	crossHair;
	
	public float					normal_speed;
	public float					boost_speed;
	public float					rotate_speed;
	public float					sensibility;
	public int						nbMissiles;

	void Start ()
	{
		controller = GetComponent<CharacterController> ();
		canon = transform.GetChild (0);
		speed = normal_speed;
		hpText = transform.GetChild (0).GetChild (1).GetChild (1).GetComponent<UnityEngine.UI.Text> ();
		nbMissilesText = transform.GetChild (0).GetChild (1).GetChild (3).GetComponent<UnityEngine.UI.Text> ();
		crossHair = transform.GetChild (0).GetChild (1).GetChild (4).GetComponent<UnityEngine.UI.Image> ();
		StartCoroutine(RecoverBoost());
	}
	
	void Update ()
	{
		Move ();
		Shoot ();
		CheckTarget ();
		hpText.text = hp.ToString ();
		nbMissilesText.text = nbMissiles.ToString ();
		if (hp <= 0)
		{
			hp = 0;
			Application.LoadLevel (Application.loadedLevel);
		}
	}

	void FixedUpdate()
	{
		controller.SimpleMove (direction.normalized * speed * Time.deltaTime);
		direction = Vector3.zero;
	}

	void Move()
	{
		if (Input.GetKey ("w"))
			direction += transform.forward;
		if (Input.GetKey ("s"))
			direction += -transform.forward;
		if (Input.GetKey ("d"))
			rot_tank += rotate_speed * Time.deltaTime;
		if (Input.GetKey ("a"))
			rot_tank -= rotate_speed * Time.deltaTime;
		if (Input.GetKeyDown ("left shift") && boost_gauge > 250.0f)
		{
			StopCoroutine(RecoverBoost());
			StartCoroutine(UseBoost());
			speed = boost_speed;
		}
		if (Input.GetKeyUp ("left shift"))
		{
			StopCoroutine(UseBoost());
			StartCoroutine(RecoverBoost());
			speed = normal_speed;
		}

		transform.localEulerAngles = new Vector3 (0.0f, rot_tank, 0.0f);
		rot_canon += Input.GetAxis ("Mouse X") * speed * Time.deltaTime;
		canon.localEulerAngles = new Vector3 (0.0f, rot_canon, 0.0f);
	}

	void Shoot()
	{
		if (Input.GetMouseButtonDown (0))
		{
			RaycastHit hit;
			if (Physics.Raycast(canon.position, canon.forward, out hit, 50.0f))
			{
				Instantiate(bullet, hit.point, Quaternion.identity);
				if (hit.collider.tag == "Tank")
					hit.collider.GetComponent<Tank> ().hp -= 5;
			}
		}
		if (Input.GetMouseButtonDown (1) && nbMissiles > 0)
		{
			RaycastHit hit;
			if (Physics.Raycast(canon.position, canon.forward, out hit, 100.0f))
			{
				Instantiate(missile, hit.point, Quaternion.identity);
				if (hit.collider.tag == "Tank")
					hit.collider.GetComponent<Tank> ().hp -= 25;
				nbMissiles -= 1;
			}
		}
	}

	void CheckTarget()
	{
		RaycastHit hit;
		if (Physics.Raycast (canon.position, canon.forward, out hit, 100.0f) && hit.transform.tag == "Enemy")
			crossHair.color = new Color (1.0f, 0.0f, 0.0f);
		else
			crossHair.color = new Color (1.0f, 1.0f, 1.0f);
	}

	IEnumerator UseBoost()
	{
		for (;;)
		{
			if (boost_gauge > 0.0f)
				boost_gauge -= 1.0f;
			else
			{
				speed = normal_speed;
				break;
			}
			yield return new WaitForSeconds(0.005f);
		}
	}

	IEnumerator RecoverBoost()
	{
		for (;;)
		{
			if (boost_gauge < 500.0f)
				boost_gauge += 0.5f;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
