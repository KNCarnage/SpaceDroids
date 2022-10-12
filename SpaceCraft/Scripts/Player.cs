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
	public int currentStrenght = GameManager.Blue;

	public int maxHealth;
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
	}
	private void Start()
	{
		maxHealth = health;
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
			viewDirection -= (currentStrenght + 1) * Time.deltaTime * 90;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			viewDirection += (currentStrenght + 1) * Time.deltaTime * 90;

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
				Instantiate(projectileObject[currentStrenght], transform.position + transform.forward, transform.rotation);
			break;
			case BlasterType.Triple:
				Instantiate(projectileObject[currentStrenght], transform.position - transform.forward * 1.2f + transform.right * 1.4f, transform.rotation);
				Instantiate(projectileObject[currentStrenght], transform.position + transform.forward, transform.rotation);
				Instantiate(projectileObject[currentStrenght], transform.position - transform.forward * 1.2f - transform.right * 1.4f, transform.rotation);
			break;
			case BlasterType.Wave:
				Instantiate(projectileObject[currentStrenght], transform.position + transform.forward, transform.rotation);
			break;
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
