using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMidi;
public class Intro : MonoBehaviour
{
	public static Intro Instance;

	public RectTransform IntroText;
	public GameObject mainCanvas;
	public GameObject storyCanvas;
	public GameObject controlCanvas;
	public GameObject StartButton;
	public GameObject StoryButton;
	public GameObject ControlButton;
	public MidiPlayer midiPlayer;
	public string introSong;
	bool play = false;
	private void Awake()
	{
		Instance = this;
	}
	private void Start()
	{
		midiPlayer.StreamMidi(introSong);
	}
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
		StartButton.SetActive(false);
		StoryButton.SetActive(false);
		ControlButton.SetActive(false);
	}
	public void StoryTime()
	{
		mainCanvas.SetActive(false);
		storyCanvas.SetActive(true);
	}
	public void Controls()
	{
		mainCanvas.SetActive(false);
		controlCanvas.SetActive(true);
	}
}
