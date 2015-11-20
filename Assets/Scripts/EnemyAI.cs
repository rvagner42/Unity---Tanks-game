using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : Tank {

	private NavMeshAgent			agent;
	private Transform				player;
	private	List<Tank>				tanks;
	private Transform				target;
	private UnityEngine.UI.Text		hpText;
	private UnityEngine.Canvas		canvas;

	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		GameObject[] tmp = GameObject.FindGameObjectsWithTag ("Tank");
		tanks = new List<Tank> ();
		foreach (GameObject t in tmp)
		{
			tanks.Add (t.GetComponent<Tank> ());
		}
		tanks.Add (player.GetComponent<Tank> ());
		hpText = transform.GetChild (1).GetChild (1).GetComponent<UnityEngine.UI.Text> ();
		canvas = transform.GetChild (1).GetComponent<UnityEngine.Canvas> ();
		StartCoroutine (SelectTarget ());
		StartCoroutine (Shoot ());
	}
	
	void Update ()
	{
		hpText.text = hp.ToString ();
		if (hp <= 0)
		{
			hp = 0;
			Instantiate(missile, transform.position, Quaternion.identity);
			StopCoroutine(Shoot ());
			StopCoroutine(SelectTarget ());
			Destroy (gameObject, 0.5f);
		}
		else
		{
			if (target == null)
			{
				StopCoroutine (SelectTarget ());
				StartCoroutine (SelectTarget ());
			}
			if (Vector3.Distance (transform.position, target.position) > 25.0f)
				agent.destination = target.position;
			else
			{
				agent.destination = transform.position;
				transform.LookAt (target.position);
			}
		}
		canvas.transform.LookAt (player.transform.position);
		canvas.transform.eulerAngles = canvas.transform.eulerAngles + new Vector3 (0.0f, 180.0f, 0.0f);
	}

	IEnumerator Shoot()
	{
		for (;;)
		{
			if (Vector3.Distance (transform.position, target.position) <= 45.0f)
			{
				RaycastHit hit;
				GetComponent<Collider> ().enabled = false;
				if (Physics.Raycast (transform.position, transform.forward, out hit))
				{
					Instantiate (bullet, hit.point, Quaternion.identity);
					if (hit.collider.tag == "Tank" || hit.collider.tag == "Player")
					{
						hit.collider.GetComponent<Tank> ().hp -= 5;
						if (hit.collider.GetComponent<Tank> ().hp <= 0)
						{
							StopCoroutine (SelectTarget ());
							StartCoroutine (SelectTarget ());
						}
					}
				}
				GetComponent<Collider> ().enabled = true;
			}
			yield return new WaitForSeconds (Random.Range (0.2f, 1.0f));
		}
	}

	IEnumerator SelectTarget()
	{
		for (;;)
		{
			tanks.RemoveAll (t => t == null);
			tanks.Remove (gameObject.GetComponent<Tank> ());
			if (target == null)
				target = tanks[0].transform;
			foreach (Tank t in tanks)
			{
				if (Vector3.Distance (target.position, transform.position) > Vector3.Distance (t.transform.position, transform.position))
					target = t.transform;
			}
			yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
		}
	}
}
