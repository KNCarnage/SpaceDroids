using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageables : MonoBehaviour
{
	public bool alwayDrop;
	public int dropRatio;
	public GameObject[] dropObjects;
	public int health;
	public int currentLevel;
	public GameObject destructionVFX;
	public GameObject hitEffect;
	public AudioClip shoot;
	public AudioClip detroyed;
	public bool destroy = true;
	[HideInInspector]
	public AudioSource audioSource;
	public void GetDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			health = 0;
			GameManager.Create2DSound(detroyed, 0.5f);
			Destruction();
		}
		else
		{
			audioSource.clip = GameManager.Instance.hit;
			audioSource.Play();
			Instantiate(hitEffect, new Vector3(transform.position.x, -3f, transform.position.z), Quaternion.identity, transform);
		}

	}
	void Destruction()
	{
		Instantiate(destructionVFX, transform.position, Quaternion.identity);
		if (destroy)
		{
			if (Random.value < (dropRatio / 100f))
			{
				GameManager.Create2DSound(GameManager.Instance.dropPickup, 0.5f); 
				GameObject go = Instantiate(dropObjects[0], transform.position, Quaternion.identity);
				PickUp pu = go.GetComponent<PickUp>();
				if (pu != null)
					pu.itemLevel = currentLevel;
			}
			else if (alwayDrop)
			{
				GameManager.Create2DSound(GameManager.Instance.dropPickup, 0.5f);
				GameObject go;
				if (Random.value < 0.5f)
				{
					if (dropObjects.GetLength(0) > 2)
						go = Instantiate(dropObjects[2], transform.position, Quaternion.identity);
					else if (dropObjects.GetLength(0) > 1)
						go = Instantiate(dropObjects[1], transform.position, Quaternion.identity);
					else
						go = Instantiate(dropObjects[0], transform.position, Quaternion.identity);
				}
				else if (dropObjects.GetLength(0) > 1)
					go = Instantiate(dropObjects[1], transform.position, Quaternion.identity);
				else
					go = Instantiate(dropObjects[0], transform.position, Quaternion.identity);
				PickUp pu = go.GetComponent<PickUp>();
				if (pu != null)
					pu.itemLevel = currentLevel;
			}
			GameManager.Instance.Score += 10;
			Destroy(gameObject);
		}
		else
		{
			GameManager.Instance.GameOverScreen.SetActive(true);
			gameObject.SetActive(false);
		}
	}

	void OnCollisionEnter(Collision other)
	{
		int damage;
		Proyectile po = other.gameObject.GetComponent<Proyectile>();
		if (po != null)
			damage = po.damage;
		else
			damage = 1;
		GetDamage(damage);
	}
}
