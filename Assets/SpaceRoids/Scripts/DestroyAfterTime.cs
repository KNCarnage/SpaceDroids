using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	public float lifeTime = 1;
	float time = 0f;

	void Update()
	{
		time += Time.deltaTime;

		if (time >= lifeTime)
			Destroy(gameObject);
	}
}