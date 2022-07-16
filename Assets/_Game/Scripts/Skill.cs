using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillColor
{
	White,
	Red,
	Black,
	Blue,
	Green
}

public enum SkillEffect
{
	Pass,
	Def,
	Atk,
	Mana,
	AtkCharged,
	ManaCharged,
	Move,
}

[CreateAssetMenu(fileName = "New Skill", menuName = "ScriptableObjects/CreateSkill", order = 1)]
public class Skill : ScriptableObject
{
	[MultilineAttribute] public string Description;
	public SkillEffect Effect;
	public int Value;
	public int[] Resources = new int[6];
	public Sprite Icon;
	public SkillColor Background;
}

/*
public class Skills : MonoBehaviour
{
	public static Dictionary<SkillColor, (Color, Color)> SkillColors = new Dictionary<SkillColor, (Color, Color)>
	{
		{ SkillColor.White, (new Color(0xF6, 0xF7, 0xEB), new Color(0x00, 0x00, 0x00)) },
		{ SkillColor.Red,   (new Color(0xE9, 0x4F, 0x37), new Color(0xFF, 0xFF, 0xFF)) },
		{ SkillColor.Black, (new Color(0x39, 0x3E, 0x41), new Color(0xFF, 0xFF, 0xFF)) },
		{ SkillColor.Blue,  (new Color(0x3F, 0x88, 0xC5), new Color(0xFF, 0xFF, 0xFF)) },
		{ SkillColor.Green, (new Color(0x5A, 0xC4, 0xAE), new Color(0x00, 0x00, 0x00)) }
	};

	// Singleton
	public static Skills Instance;
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}
		Instance = this;
	}

	public Dictionary<SkillColor, Color> SkillColors = new Dictionary<SkillColor, Color>
	{
		{ White, Color },
		{ "key2", "value2" }
	}
}*/
