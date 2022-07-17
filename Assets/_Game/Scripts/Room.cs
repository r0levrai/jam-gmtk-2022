using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum BossAction
{
	Pass,
	LoopTo,
	Atk,
	Def,
	RandomizeDice,
	FlipDice,
	Def4,
	PivotDiceLeftOrRight,
}

[System.Serializable]  // necessary for inspector UI
public class ActionOrLoop
{
	public BossAction action;

	[ConditionalField(nameof(action), false, BossAction.LoopTo)]
	public int toElement;

	[ConditionalField(nameof(action), false, BossAction.LoopTo)]
	[Tooltip("0 to loop forever")]
	public int times = 0;
}

[CreateAssetMenu(fileName = "Room N", menuName = "ScriptableObjects/CreateRoom", order = 1)]
public class Room : ScriptableObject
{
	public string BossName;
	public Sprite BossSprite;
	[MultilineAttribute] public string GameDesignGoal;
	public int BossHP;
	public ActionOrLoop[] BossActions = new ActionOrLoop[1];
	public Skill[] DiceFacesAvailable = new Skill[6];
	public Skill RoomIcon;
}
