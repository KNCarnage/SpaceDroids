using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayer : MonoBehaviour
{
	public RectTransform HealthBar;
	public RectTransform BoostBar;
	public Image HealthImage;
	public TextMeshProUGUI labelHealth;
	Color BaseColor;
	float lastHealth;
	float currentBoost;

	private void Start()
	{
		BaseColor = HealthImage.color;
		lastHealth = GameManager.Instance.currentPlayer.health / 100f;
		currentBoost = GameManager.Instance.currentPlayer.maxBoost / 100f;
	}
	void Update()
	{
		if (GameManager.Paused)
			return;

//Health
		{
			float currentHealth = GameManager.Instance.currentPlayer.health / 100f;
			if (currentHealth != lastHealth)
			{
				Color currentColor = HealthImage.color;
				Vector2 RedGreen;
				RedGreen.x = Mathf.Lerp(BaseColor.r, 1f, (GameManager.Instance.currentPlayer.maxHealth - GameManager.Instance.currentPlayer.health) / ((float)GameManager.Instance.currentPlayer.maxHealth));
				RedGreen.y = Mathf.Lerp(BaseColor.g, 0f, (GameManager.Instance.currentPlayer.maxHealth - GameManager.Instance.currentPlayer.health) / ((float)GameManager.Instance.currentPlayer.maxHealth));

				Vector3 localScale = HealthBar.localScale;
				currentColor.r = Mathf.Lerp(currentColor.r, RedGreen.x, 2 * Time.deltaTime);
				currentColor.g = Mathf.Lerp(currentColor.g, RedGreen.y, 2 * Time.deltaTime);
				lastHealth = Mathf.Lerp(lastHealth, currentHealth, 2 * Time.deltaTime);
				if (Mathf.Abs(currentHealth - lastHealth) < .001f)
				{
					lastHealth = currentHealth;
					currentColor.r = RedGreen.x;
					currentColor.g = RedGreen.y;
				}
				localScale.x = lastHealth;
				HealthImage.color = currentColor;
				HealthBar.localScale = localScale;
				UpdateLabelHealth(GameManager.Instance.currentPlayer.health, GameManager.Instance.currentPlayer.maxHealth);
			}
		}
//Boost
		{
			float maxBoost = GameManager.Instance.currentPlayer.maxBoost / 100f;
			bool boostEnabled = GameManager.Instance.currentPlayer.boostStarted;
			if ((boostEnabled) || (currentBoost != maxBoost))
			{
				Vector3 localScale = BoostBar.localScale;
				if (boostEnabled)
					currentBoost = Mathf.Lerp(currentBoost, 0, Time.deltaTime / 2);
				else
					currentBoost = Mathf.Lerp(currentBoost, maxBoost, Time.deltaTime / 4);
				if (currentBoost < .01f)
					GameManager.Instance.currentPlayer.DisableBoost();
				localScale.x = currentBoost;
				BoostBar.localScale = localScale;
			}
		}
	}

	private void UpdateLabelHealth(int healthNow, int healthMax)
	{
		labelHealth.text = $"{healthNow}/{healthMax}";
	}
}