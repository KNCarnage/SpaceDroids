using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootValues
{
	[Range(0, 100)]
	public int shotChance;

	public float shotTimeMin, shotTimeMax;
}
public class GameManager : MonoBehaviour
{
	public Player currentPlayer;
	[SerializeField]
	public GameObject[] Enemy;
	[SerializeField]
	public GameObject[] SpaceShips;
	public static GameManager Instance;

	public UIInfo uIInfo;

	public AudioClip hit;
	public AudioClip dropPickup;

	public int startCount;

	public float timeBetween;

	public static bool Paused { get { return Instance.paused; } }

	bool UFOsent = true;
	bool paused = false;
	Level level = Level.LVL1;
	GameObject currentEnemy;

	List<GameObject> CurrentEnemies = new List<GameObject>();
	public GameObject UFO;

	public const int Blue = 0;
	public const int Green = 1;
	public const int Yellow = 2;
	public const int Red = 3;

	int Round = 1;
	public int Score = 0;
	enum Level
	{
		LVL1,
		LVL2,
		LVL3,
		LVL4,
		LVL5,
		LVL6,
		LVL7,
		LVL8,
		LVL9,
		LVL10
	}
	private void Awake()
	{
		Instance = this;
	}
	private void Start()
	{
		currentEnemy = Enemy[0];
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		StartCoroutine(CreateEnemyWave(currentEnemy, startCount, timeBetween));
	}

	private void Update()
	{
		if (paused)
			return;

		if ((CurrentEnemies.Count == 1) && (!UFOsent))
		{
			Instantiate(UFO);
			UFOsent = true;
		}
		if (CurrentEnemies.Count == 0)
		{
			Round++;
			uIInfo.UpdateLevelInfo((int)(level+1), Round);
			switch (level)
			{
				default:
				case Level.LVL1:
					if (startCount < 6)
						StartCoroutine(CreateEnemyWave(Enemy[0], startCount++, timeBetween));
					else if (startCount < 10)
					{
						StartCoroutine(CreateEnemyWave(Enemy[0], startCount++, timeBetween));
						StartCoroutine(CreateEnemyWave(Enemy[1], startCount - 4, timeBetween,false));
					}
					else
					{
						StartCoroutine(CreateEnemyWave(Enemy[1], startCount++, timeBetween));
						StartCoroutine(CreateEnemyWave(Enemy[2], startCount - 4, timeBetween,false));
						StartCoroutine(CreateEnemyWave(Enemy[0], startCount - 6, timeBetween, false));
					}
					break;

			}

		}
	}
	IEnumerator CreateEnemyWave(GameObject go, int num, float timeDelay, bool newWave = true) 
	{
		if (newWave)
		{
			CurrentEnemies = new List<GameObject>();
			if ((num % 2) == 0)
			{
//				Debug.Log(num + " / " + num % 2);
				UFOsent = false;
			}
		}
		for (int i = 0; i < num; i++)
		{
			GameObject newEnemy = Instantiate(go);
			CurrentEnemies.Add(newEnemy);
			yield return new WaitForSeconds(timeDelay);
		}
		
	}

	public void RemoveEnemy(GameObject enemy)
	{
		if (CurrentEnemies.Contains(enemy))
			CurrentEnemies.Remove(enemy);
	}
	void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			paused = false;
		}
		else
			paused = true;
	}
	public static void SetObjectAndParticles(GameObject go, List<ParticleSystem> list)
	{
		go.SetActive(false);

		Transform goTransform = go.transform;
		for (int i = 0; i < goTransform.childCount; i++)
		{
			ParticleSystem particleSystem = goTransform.GetChild(i).gameObject.transform.GetComponent<ParticleSystem>();
			if (particleSystem)
				list.Add(particleSystem);
		}
	}

	public static Vector3 NewPositionByPath(Vector3[] pathPos, float percent)
	{
		return Interpolate(CreatePoints(pathPos), percent);
	}

	public static Vector3 Interpolate(Vector3[] path, float t)
	{
		int numSections = path.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
		float u = t * numSections - currPt;
		Vector3 a = path[currPt];
		Vector3 b = path[currPt + 1];
		Vector3 c = path[currPt + 2];
		Vector3 d = path[currPt + 3];
		return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
	}

	public static Vector3[] CreatePoints(Vector3[] path)
	{
		Vector3[] pathPositions;
		Vector3[] newPathPos;
		int dist = 2;
		pathPositions = path;
		newPathPos = new Vector3[pathPositions.Length + dist];
		Array.Copy(pathPositions, 0, newPathPos, 1, pathPositions.Length);
		newPathPos[0] = newPathPos[1] + (newPathPos[1] - newPathPos[2]);
		newPathPos[newPathPos.Length - 1] = newPathPos[newPathPos.Length - 2] + (newPathPos[newPathPos.Length - 2] - newPathPos[newPathPos.Length - 3]);
		if (newPathPos[1] == newPathPos[newPathPos.Length - 2])
		{
			Vector3[] LoopSpline = new Vector3[newPathPos.Length];
			Array.Copy(newPathPos, LoopSpline, newPathPos.Length);
			LoopSpline[0] = LoopSpline[LoopSpline.Length - 3];
			LoopSpline[LoopSpline.Length - 1] = LoopSpline[2];
			newPathPos = new Vector3[LoopSpline.Length];
			Array.Copy(LoopSpline, newPathPos, LoopSpline.Length);
		}
		return (newPathPos);
	}

	public static void Create2DSound(AudioClip clip, float minDistance)
	{
		GameObject sound = new GameObject("2dsound");
		sound.transform.position = Instance.transform.position;

		AudioSource audioSource = sound.AddComponent<AudioSource>();

		audioSource.clip = clip;
		audioSource.loop = false;
		audioSource.spatialBlend = 0;
		audioSource.minDistance = minDistance;
		audioSource.Play();

		sound.AddComponent<DestroyAfterSoundPlayed>();
	}
}
