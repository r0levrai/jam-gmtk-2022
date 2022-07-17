using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Data : MonoBehaviour
{
	// Singleton
	public static Data Instance;
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(this);
		Instance = this;
	}

	public Sprite[] resources;
	public Color[] bgColors;
	public Color[] bgTextColors;
	public Color highlightColor = Color.yellow;
	public float highlightFreq = 1;
    public Skill[] skills;
}
