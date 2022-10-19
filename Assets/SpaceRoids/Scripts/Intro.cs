using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
	public RectTransform IntroText;
	public GameObject Button;
	bool play = false;

    // Update is called once per frame
    void Update()
    {
		if (play)
		{
			Vector3 currentScale = IntroText.localScale;
			if (currentScale.x > 40)
			{
				SceneManager.LoadScene("SpaceDroids", LoadSceneMode.Additive);
				Destroy(gameObject);
			}
			else
				IntroText.localScale = new Vector3(currentScale.x * (1.0f + Time.deltaTime * 2), currentScale.y * (1.0f + Time.deltaTime * 2), currentScale.z * (1.0f + Time.deltaTime * 2));
		}
	}

	public void StartGame()
	{
		play = true;
		Button.SetActive(false);
	}
}
