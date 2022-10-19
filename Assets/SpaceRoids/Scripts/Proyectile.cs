using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
	public float speed;
	public int damage;
	Collider col = null;
	public bool isWave = false;
	void Start()
    {
		col = GetComponent<Collider>();
		Invoke("EnableCollider", .1f);
	}
	void EnableCollider()
	{
		col.enabled = true;
	}
	void Update()
    {
		if (isWave)
		{
			Vector3 currentScale = transform.localScale;
			if (currentScale.sqrMagnitude < 8)
				transform.localScale = new Vector3(currentScale.x*(1.0f + Time.deltaTime * 4), currentScale.y, currentScale.z * (1.0f + Time.deltaTime/2 * 4));
		}
		transform.position += transform.forward * speed * Time.deltaTime;
	}
}
