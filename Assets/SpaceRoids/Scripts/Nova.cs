using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nova : MonoBehaviour
{
	public float lifeTime = 1.8f;
	float time = 0f;
	void Update()
    {
		Vector3 currentScale = transform.localScale;
		transform.localScale = new Vector3(currentScale.x * (1.0f + Time.deltaTime * 4), currentScale.y, currentScale.z * (1.0f + Time.deltaTime * 4));

		time += Time.deltaTime;

		if (time >= lifeTime)
		{
			GameManager.Instance.ChangeLevel();
			Destroy(gameObject);
		}
	}
}
