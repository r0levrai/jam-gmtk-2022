using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
	public enum PlayerResources
	{
		Mana = 3,
		ManaCharging = 4
	}

	public Dice dice;
	public float initialTurnDuration = 2;
	public int halveDurationEvery = 6;
	public float lastTurnTime = 0;

	public Room room;

	[SerializeField] int turn;
	[SerializeField] int playerHP;
	[SerializeField] int[] playerResources;
	[SerializeField] int arrowsTraversedThisTurn;
	[SerializeField] int bossHP;
	[SerializeField] int bossActionIndex;
	[SerializeField] int bossActionLoopCount;
	private IEnumerator playTurns;

	private void Start()
	{
		playTurns = PlayTurns();
		StartCoroutine(playTurns);
	}

	IEnumerator PlayTurns()
	{
		playerHP = 1;
		playerResources = new int[5];
		bossHP = room.BossHP;
		for (turn = 0; ; turn++)
		{
			yield return new WaitForSeconds(initialTurnDuration * Mathf.Log(1 + turn/halveDurationEvery));

			// boss
			ActionOrLoop actionOrLoop = room.BossActions[bossActionIndex];
			while (actionOrLoop.action == BossAction.LoopTo)
			{
				if (actionOrLoop.times == 0 || bossActionLoopCount < actionOrLoop.times)
				{
					bossActionIndex = actionOrLoop.toElement;
					bossActionLoopCount++;
				}
				else
				{
					bossActionIndex++;
				}
				actionOrLoop = room.BossActions[bossActionIndex];
			}

			Resolve(actionOrLoop.action, dice.currentSkill);

			dice.Advance();
		}
	}

	void Resolve(BossAction bossAction, Skill playerAction)
	{
		// priority 1: dice ~~sheninigans~~ movements
		if (bossAction == BossAction.FlipDice) {
			dice.Flip();
			playerAction = dice.currentSkill;
		}
		if (bossAction == BossAction.RandomizeDice) {
			dice.Randomize();
			playerAction = dice.currentSkill;
		}
		// priority 2: defense, buffs and other statuses
		int bossDef = 0;
		int playerDef = 0;
		if (bossAction == BossAction.Def)
			bossDef = 1;
		if (playerAction.Effect == SkillEffect.Def)
			playerDef = 1;
		// mana
		if (playerAction.Effect == SkillEffect.Mana)
			playerResources[(int)PlayerResources.Mana] += playerAction.Value;
		if (playerAction.Effect == SkillEffect.ManaCharged)
			playerResources[(int)PlayerResources.ManaCharging]++;
		// arrows
		if (playerAction.Effect == SkillEffect.Move)
		{
			playerResources[(int)PlayerResources.Mana] += playerResources[(int)PlayerResources.ManaCharging];
			dice.nextSide = Dice.d6neighborSide[(int)dice.currentSide, playerAction.Value];
		}
		// priority 3: attacks
		if (bossAction == BossAction.Atk)
			playerHP -= Mathf.Max(0, 1 - playerDef);
		if (playerAction.Effect == SkillEffect.Atk)
			bossHP -= Mathf.Max(0, playerAction.Value - bossDef);
		if (playerAction.Effect == SkillEffect.AtkCharged)
			bossHP -= Mathf.Max(0, playerResources[3] - bossDef);
		// priority 0: make sure you don't forget the inspector >:{
		if (playerAction.Effect == SkillEffect.Pass)
			Debug.LogWarning($"Skill effect for skill '{playerAction.name}' is Pass. Did we forget to set it?");
	}

	void BattleEnd(bool won)
	{
		if (won)
		{
			print("Battle won :)");
		}
		else
		{
			print("Battle lost :/");
		}
	}
}
