using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Transportable
{
	public Texture[] currenSkin;
	public PickUpType pickUpType;
	public int itemLevel = 0;
	public float speed;
	public bool teleport = false;
	public AudioClip audioClip;
	float currentPathPercent;
	
	Vector3[] pathPositions = new Vector3[6];
	public Color pathColor = Color.green;
	public bool Transportable { get { return teleport; } }

	public enum PickUpType
	{
		Health,
		Weapon,
		Upgrade
	}
	public void Start()
	{
		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.material.mainTexture = currenSkin[itemLevel];
		speed *= (itemLevel + 1);

		currentPathPercent = 0;

		for (int i = 0; i < pathPositions.Length; i++)
		{
			Vector2 randomposition = Random.insideUnitCircle * 15;
			if (i == 0)
				pathPositions[i] = transform.position;
			else
			{
				if (i == (pathPositions.Length - 1))
					pathPositions[i] = new Vector3(Random.Range(0, 2) * 60 - 30, -4f, Random.Range(0, 2) * 40 - 20);
				else
					pathPositions[i] = new Vector3(randomposition.x * 3f, -4f, randomposition.y);
			}
		}
		transform.position = pathPositions[0];
		transform.rotation = Quaternion.identity;
	}

	private void Update()
	{
		if (GameManager.Paused)
			return;

		currentPathPercent += speed / 100 * Time.deltaTime;
		transform.position = GameManager.NewPositionByPath(pathPositions, currentPathPercent);

		if (currentPathPercent > 1)
			Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		Player player = other.gameObject.GetComponent<Player>();
		if (player == null)
			return;
		GameManager.Create2DSound(audioClip, 0.5f);
		switch (pickUpType)
		{
			case PickUpType.Health:
				player.health += 2 * (itemLevel + 1);
				if (player.health > player.maxHealth)
					player.health = player.maxHealth;
				GameManager.Instance.Score += 2 * (itemLevel + 1);
				break;
			case PickUpType.Weapon:
			{
				if (player.currentBlaster == Player.BlasterType.Wave)
						player.health = player.maxHealth;
				else
				{
					GameObject newPlayer;
					if (player.currentBlaster == Player.BlasterType.Single)
						newPlayer = Instantiate(GameManager.Instance.SpaceShips[1], player.transform.position, Quaternion.identity);
					else
						newPlayer = Instantiate(GameManager.Instance.SpaceShips[2], player.transform.position, Quaternion.identity);
					newPlayer.transform.forward = player.gameObject.transform.forward;
					GameManager.Instance.currentPlayer = newPlayer.GetComponent<Player>();
					GameManager.Instance.currentPlayer.rb.velocity = player.rb.velocity;
					GameManager.Instance.currentPlayer.rb.angularVelocity = player.rb.angularVelocity;
					if (player.currentLevel > GameManager.Blue)
					{
						GameManager.Instance.currentPlayer.currentLevel = player.currentLevel;
						GameManager.Instance.currentPlayer.maxHealth = player.maxHealth;
						GameManager.Instance.currentPlayer.maxBoost = player.maxBoost;
						GameManager.Instance.currentPlayer.speed += .5f * player.currentLevel;
						GameManager.Instance.currentPlayer.fireRate += .5f * player.currentLevel;
						GameManager.Instance.currentPlayer.mr.material.mainTexture = GameManager.Instance.currentPlayer.currenSkin[player.currentLevel];
					}
					player.gameObject.SetActive(false);
					GameManager.Instance.Score += 5 * (itemLevel + 1);
					Destroy(player.gameObject);
				}
			}
			break;
			case PickUpType.Upgrade:
			{
				GameManager.Instance.Score += 10 * (itemLevel + 1);
				player.UpgradeLevel();
			}
			break;
		}
		Destroy(gameObject);
	}
	void OnDrawGizmos()
	{
		Vector3[] newPathPositions = GameManager.CreatePoints(pathPositions);
		Vector3 previosPositions = GameManager.Interpolate(newPathPositions, 0);
		Gizmos.color = pathColor;
		int SmoothAmount = pathPositions.Length * 20;
		for (int i = 1; i <= SmoothAmount; i++)
		{
			float t = (float)i / SmoothAmount;
			Vector3 currentPositions = GameManager.Interpolate(newPathPositions, t);
			Gizmos.DrawLine(currentPositions, previosPositions);
			previosPositions = currentPositions;
		}
	}
}
