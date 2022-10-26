using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Story : MonoBehaviour
{
	public GameObject DateObject;
	public GameObject TextObject;
	public GameObject ContinueObject;
	TextMeshProUGUI Date;
	TextMeshProUGUI Text;
	RectTransform ContinueText;
	public string storySong;

	string[] Dates = new string[4] {"June 25º 2135", "July 31º 2135", "May 23º 2136", "August 1º 2136" };
	string[] StoryText = new string[4] { "NASA Accidentally Detected Something Huge Traveling Towards Earth!\nNASA’s Neil Gehrels Swift noticed an object on March 12, when it exited a massive burst of X-rays. Follow-up studies by the European Space Agency’s XMM-Newton observatory and NASA’s NuSTAR telescope, led by Caltech and managed by the agency’s Jet Propulsion Laboratory.\nThis object of unkown characteristics is heading towards earth.",
	"The massive object arrived at Earth!Everyone on earth was anxious about the arrival of this massive object, which turn out to be an Alien Mothership created by an alien race known as the Klorgs.Klorgs began invading the plant, in order to secure their need for nitrogen.Meanwhile, the world began organizing itself into the new United Earth Federation, in order to confront the invaders.",
	"Earth First Victory!Just above verdum the United Earth Federation got its first victory. A valuable Klorgs dropship was shot down and important technology data was salvaged.The  United Earth Federation created a special taskforce to replicate Klorgs's technology with this new data.Now new war spaceships called 'Trexlors' were created with Earth/Klorg technology. These war machines are capable of interstellar travel as well as dynamic combat adaptation.Finally a ray of hope for humanity.",
	"PAYBACK TIME!\nFinally after a complete year enduring the alien invasion, everything is ready to take war on their homeworld.You have been selected as the earth's greatest pilot, it's your task to make them pay for daring to invade our planet.while you gaze at night's sky....\nALERT....ALERT....ALERT....\nThe Klorgs are making a final attempt to stop your mission. You jump into you TrexlorIt's a good night to kick some alien butt...."};

	int currentPage = 0;
	int maxPage;
	int currentNum = 0;
	string currentText = "";
	float updateTime = 0.15f;
	float time = 0f;
	TextState currentState = TextState.DATE;
	enum TextState
	{
		DATE,
		TEXT,
		CONTINUE
	}
	void Awake()
	{
		Date = DateObject.GetComponent<TextMeshProUGUI>();
		Text = TextObject.GetComponent<TextMeshProUGUI>();
		ContinueText = ContinueObject.GetComponent<RectTransform>();
		maxPage = Mathf.Min(Dates.GetLength(0), StoryText.GetLength(0));
	}

	private void OnEnable()
    {
		currentPage = 0;
		currentNum = 0;
		currentText = "";
		currentState = TextState.DATE;
		Intro.Instance.midiPlayer.Stop();
		Intro.Instance.midiPlayer.StreamMidi(storySong);
	}

	void Update()
    {
		time += Time.deltaTime;
		if (time >= updateTime)
		{
			time = 0;
			switch (currentState)
			{
				case TextState.DATE:
				{
					currentText += Dates[currentPage][currentNum++];
					Date.SetText(currentText);
					if (currentNum >= Dates[currentPage].Length)
					{
						currentState = TextState.TEXT;
						currentText = "";
						currentNum = 0;
						time = -.25f;
					}
				}
				break;
				case TextState.TEXT:
				{
					currentText += StoryText[currentPage][currentNum++];
					Text.SetText(currentText);
					if (currentNum >= StoryText[currentPage].Length)
					{
						ContinueObject.SetActive(true);
						currentState = TextState.CONTINUE;
						currentText = "";
						currentNum = 0;
					}
				}
				break;
				case TextState.CONTINUE:
				{
					if (ContinueText.localScale.x > 1.5f)
						ContinueText.localScale = Vector3.one;
				}
				break;
			}
		}
		if (currentState == TextState.CONTINUE)
		{
			Vector3 currentScale = ContinueText.localScale;
			ContinueText.localScale = new Vector3(currentScale.x * (1.0f + Time.deltaTime / 2), currentScale.y * (1.0f + Time.deltaTime / 2), currentScale.z * (1.0f + Time.deltaTime / 2));
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			switch (currentState)
			{
				case TextState.DATE:
				{
					Date.SetText(Dates[currentPage]);
					currentState = TextState.TEXT;
					currentText = "";
					currentNum = 0;
					time = -.25f;
				}
				break;
				case TextState.TEXT:
				{
					Text.SetText(StoryText[currentPage]);
					ContinueObject.SetActive(true);
					currentState = TextState.CONTINUE;
					currentText = "";
					currentNum = 0;
				}
				break;
				case TextState.CONTINUE:
				{
					ContinueObject.SetActive(false);
					Date.SetText("");
					Text.SetText("");
					ContinueText.localScale = Vector3.one;
					currentPage++;

					currentState = TextState.DATE;
					currentText = "";
					currentNum = 0;
					if (currentPage >= maxPage)
					{
						gameObject.SetActive(false);
						Intro.Instance.mainCanvas.SetActive(true);
						Intro.Instance.midiPlayer.Stop();
						Intro.Instance.midiPlayer.StreamMidi(Intro.Instance.introSong);
					}
				}
				break;
			}
		}
	}
}
