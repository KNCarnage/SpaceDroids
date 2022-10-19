using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Damageables, Transportable
{
	public GameObject afterburner = null;
	public GameObject muzzle = null;
	public GameObject[] projectileObject;
	public Texture[] currenSkin;
	public Rigidbody rb;
	public BlasterType currentBlaster = BlasterType.Single;

	public int maxHealth;
	public int maxBoost;
	public float speed;
	public float fireRate;

	List<ParticleSystem> burnt = new List<ParticleSystem>();
	List<ParticleSystem> muzzleFx = new List<ParticleSystem>();

	float nextFire;
	float viewDirection = 0;
	public bool boostStarted = false;

	bool valuesStored = false;
	Vector3 lastVelocity = Vector3.zero;
	Vector3 lastAngularVelocity = Vector3.zero;

	public MeshRenderer mr;
	public bool Transportable { get { return true; } }
	public enum BlasterType
	{
		Single,
		Triple,
		Wave
	}
	private void Awake()
	{
		destroy = false;
		audioSource = GetComponent<AudioSource>();
		mr = GetComponentInChildren<MeshRenderer>();
	}
	private void Start()
	{
		health = maxHealth;
		GameManager.SetObjectAndParticles(afterburner, burnt);
		GameManager.SetObjectAndParticles(muzzle, muzzleFx);
	}

	void Update()
    {
		if (GameManager.Paused)
			return;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			EnableBoost();
		}
		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			DisableBoost();
		}
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
		{
			if (Time.time > nextFire)
			{
				if (muzzle.activeSelf == false)
					muzzle.SetActive(true);
				else
				{
					foreach (ParticleSystem particle in muzzleFx)
						particle.Play();
				}
				Fire();
				nextFire = Time.time + 1 / fireRate;
			}
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			viewDirection -= (currentLevel + 1) * Time.deltaTime * 90;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			viewDirection += (currentLevel + 1) * Time.deltaTime * 90;

		transform.rotation = Quaternion.Euler(0, viewDirection, 0);
	}

	private void FixedUpdate()
	{
		if (GameManager.Paused)
		{
			if (!valuesStored)
			{
				lastVelocity = rb.velocity;
				rb.velocity = Vector3.zero;
				lastAngularVelocity = rb.angularVelocity;
				rb.angularVelocity = Vector3.zero;
				valuesStored = true;
			}
			return;
		}
		else if (valuesStored)
		{
			rb.velocity = lastVelocity;
			lastVelocity = Vector3.zero;
			rb.angularVelocity = lastAngularVelocity;
			lastAngularVelocity = Vector3.zero;
			valuesStored = false;
		}

		rb.position += speed * Time.fixedDeltaTime * transform.forward;

		if (boostStarted)
			rb.AddForce(5 * speed * Time.fixedDeltaTime * transform.forward, ForceMode.VelocityChange);	
	}
	void Fire()
	{
		audioSource.clip = shoot;
		audioSource.Play();
		switch (currentBlaster)
		{
			case BlasterType.Single:
				Instantiate(projectileObject[currentLevel], transform.position + transform.forward, transform.rotation);
			break;
			case BlasterType.Triple:
				Instantiate(projectileObject[currentLevel], transform.position - transform.forward * 1.2f + transform.right * 1.4f, transform.rotation);
				Instantiate(projectileObject[currentLevel], transform.position + transform.forward, transform.rotation);
				Instantiate(projectileObject[currentLevel], transform.position - transform.forward * 1.2f - transform.right * 1.4f, transform.rotation);
			break;
			case BlasterType.Wave:
				Instantiate(projectileObject[currentLevel], transform.position + transform.forward, transform.rotation);
			break;
		}

	}

	public void UpgradeLevel()
	{
		if (currentLevel < GameManager.Red)
		{
			currentLevel++;
			maxHealth *= 2;
			health = maxHealth;
			maxBoost += 20;
			speed += .5f;
			fireRate += .5f;
			mr.material.mainTexture = currenSkin[currentLevel];
		}
	}

	public void EnableBoost()
	{
		if (afterburner.activeSelf == false)
		{
			afterburner.SetActive(true);
			boostStarted = true;
		}
		else if (!boostStarted)
		{
			boostStarted = true;
			foreach (ParticleSystem particle in burnt)
				particle.Play();
		}
	}

	public void DisableBoost()
	{
		if (afterburner.activeSelf)
		{
			boostStarted = false;
			foreach (ParticleSystem particle in burnt)
				particle.Stop();
		}
	}
}
