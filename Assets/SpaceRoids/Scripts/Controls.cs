using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
	public GameObject ContinueObject;
	RectTransform ContinueText;
	float updateTime = 0.15f;
	float time = 0f;
	private void Awake()
	{
		ContinueText = ContinueObject.GetComponent<RectTransform>();
	}
	void Update()
    {
		time += Time.deltaTime;
		if (time >= updateTime)
		{
			time = 0;
			if (ContinueText.localScale.x > 1.5f)
				ContinueText.localScale = Vector3.one;
		}
		Vector3 currentScale = ContinueText.localScale;
		ContinueText.localScale = new Vector3(currentScale.x * (1.0f + Time.deltaTime / 2), currentScale.y * (1.0f + Time.deltaTime / 2), currentScale.z * (1.0f + Time.deltaTime / 2));

		if (Input.GetKeyDown(KeyCode.Space))
		{
			gameObject.SetActive(false);
			Intro.Instance.mainCanvas.SetActive(true);
		}
	}
}
