using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	// Update is called once per frame
	private void OnTriggerExit(Collider other)
	{
		GameObject go = other.gameObject;
		Transportable transport = go.GetComponent<Transportable>();
		if (transport == null)
			return;
		if (!transport.Transportable)
			return;

		Vector3 currentPosition = go.transform.position;
		if (currentPosition.z > 16)
			currentPosition.z = -16.5f;
		else if (currentPosition.z < -16.5f)
			currentPosition.z = 16;
		if (currentPosition.x > 29.5)
			currentPosition.x = -29.5f;
		else if (currentPosition.x < -29.5f)
			currentPosition.x = 29.5f;
		go.transform.position = currentPosition;
	}
}
