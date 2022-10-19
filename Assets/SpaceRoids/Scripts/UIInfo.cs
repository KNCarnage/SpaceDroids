using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInfo : MonoBehaviour
{
	public TextMeshProUGUI labelInfo;
	public TextMeshProUGUI labelScore;
	int currentScore;
	private void Start()
	{
		currentScore = GameManager.Instance.Score;
	}
	public void UpdateLevelInfo(int Level, int Round)
    {
		labelInfo.text = $"Level Nº{Level} Round Nº{Round}";
	}

	void Update()
	{
		if (GameManager.Paused)
			return;

		if (currentScore != GameManager.Instance.Score)
		{
			currentScore = GameManager.Instance.Score;
			labelScore.text = "Score: "+currentScore.ToString("00000000000");
		}
	}
}
