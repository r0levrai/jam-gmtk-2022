using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{

    public TMP_Text text;
	public float duration = 0.7f;
	public float distanceUp = 115;

	float t = 0;
	Vector3 startPos;

	public void OnEnable()
	{
		startPos = transform.localPosition;
	}

	public float EaseIn(float x) => x*x*x;  // https://easings.net/#easeInCubic
	public float EaseOut(float x) => 1 - (1 - x) * (1 - x); // https://easings.net/#easeOutQuad

	private void Update()
	{
		t += Time.deltaTime;
		transform.localPosition = Vector3.Lerp(startPos, startPos + distanceUp * Vector3.up, EaseOut(t/duration));
		text.alpha = 1 - EaseIn(t/duration);
		if (t > 1)
			Destroy(this.gameObject);
	}
}
