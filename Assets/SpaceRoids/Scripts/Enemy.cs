using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageables
{
	public GameObject[] Projectile;
	public GameObject muzzle = null;
	public Texture[] currenSkin;

	public float speed;
	public bool rotationByPath;
	public bool lookTarget;
	public bool loop;

	float nextFire;
	float nextLook;
	List<ParticleSystem> muzzleFx = new List<ParticleSystem>();
	public int shotChance;
	public float shotTimeMin, shotTimeMax;

	float currentPathPercent;
	Vector3[] pathPositions = new Vector3[8];
	public Color pathColor = Color.yellow;
	public Rigidbody rb;
	float distance = 0;
	bool fireNext = false;
	public void Awake()
	{
		currentPathPercent = 0;

		for (int i = 0; i < pathPositions.Length; i++)
		{
			Vector2 randomposition = UnityEngine.Random.insideUnitCircle * 15;
			if (i == 0)
				pathPositions[i] = new Vector3(UnityEngine.Random.Range(0, 2) * 60 - 30, -4f, UnityEngine.Random.Range(0, 2) * 40 - 20);
			else
			{
				if (i == (pathPositions.Length - 1))
					pathPositions[i] = new Vector3(UnityEngine.Random.Range(0, 2) * 60 - 30, -4f, UnityEngine.Random.Range(0, 2) * 40 - 20);
				else
					pathPositions[i] = new Vector3(randomposition.x * 3f, -4f, randomposition.y);
				distance += Vector2.Distance(pathPositions[i], pathPositions[i - 1]);
			}
		}
		transform.position = pathPositions[0];

		if (!rotationByPath)
			transform.rotation = Quaternion.identity;
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
		mr.material.mainTexture = currenSkin[currentLevel];
		health += 5 * currentLevel;
		speed += 5*currentLevel;
		if (muzzle != null)
			GameManager.SetObjectAndParticles(muzzle, muzzleFx);
	}

	void Update()
    {
		if (GameManager.Paused)
			return;

		if (Time.time > nextFire)
		{
			if (fireNext)
			{
				if (muzzle != null)
				{
					if (muzzle.activeSelf == false)
						muzzle.SetActive(true);
					else
					{
						foreach (ParticleSystem particle in muzzleFx)
							particle.Play();
					}
				}
				audioSource.clip = shoot;
				audioSource.Play();
				Instantiate(Projectile[currentLevel], transform.position + transform.forward, transform.rotation);
				fireNext = false;
			}
			fireNext = (UnityEngine.Random.value < (float)shotChance / 100);
			nextFire = Time.time + UnityEngine.Random.Range(shotTimeMin, shotTimeMax);
			if (fireNext)
				nextLook = nextFire - shotTimeMin;
			else
				nextLook = nextFire + shotTimeMin;
		}
	}

	private void FixedUpdate()
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		if (GameManager.Paused)
			return;

		currentPathPercent += Mathf.Lerp(0, 1, speed / (distance * 10) * Time.fixedDeltaTime);

		Vector3 currentPosition = transform.position;
		Vector3 nextPosition = GameManager.NewPositionByPath(pathPositions, currentPathPercent);
		Vector3 Vel = (nextPosition - currentPosition) / Time.fixedDeltaTime;

		rb.AddForce(Vel, ForceMode.VelocityChange);


		if (lookTarget)
		{
			Vector3 LookAt;
			if (Time.time > nextLook)
			{
				LookAt = GameManager.Instance.currentPlayer.transform.position - transform.position;
				transform.forward = Vector3.Lerp(transform.forward, LookAt, Time.fixedDeltaTime * 5);
			}
			else
			{
				LookAt = GameManager.Interpolate(GameManager.CreatePoints(pathPositions), currentPathPercent + 0.01f) - transform.position;
				transform.forward = Vector3.Lerp(transform.forward, LookAt, Time.fixedDeltaTime);
			}

		}
		else if (rotationByPath)
		{
			transform.right = GameManager.Interpolate(GameManager.CreatePoints(pathPositions), currentPathPercent + 0.01f) - transform.position;
			transform.Rotate(Vector3.up * 90);
		}

		if (currentPathPercent > 1)
		{
			if (loop)
				currentPathPercent = 0;
			else
			{
				Destroy(gameObject);
			}
		}
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

	private void OnDestroy()
	{
		GameManager.Instance.RemoveEnemy(gameObject);
	}
}
