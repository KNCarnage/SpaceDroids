using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{
	public float speed;
	MeshRenderer mr;
	MaterialPropertyBlock materialParameters;
	Vector2 TextureScale;
	float offset = 0;
	private void Awake()
	{
		mr = GetComponent<MeshRenderer>();
		TextureScale = mr.material.mainTextureScale;
		materialParameters = new MaterialPropertyBlock();
	}
	void Update()
	{
		offset += speed * Time.deltaTime;
		mr.GetPropertyBlock(materialParameters);
		materialParameters.SetVector("_MainTex_ST", new Vector4(TextureScale[0], TextureScale[1], 0, offset));
		mr.SetPropertyBlock(materialParameters);
	}
}
