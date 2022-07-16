using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
	public Dice dice;
	public float initialTurnDuration = 2;
	public int halveDurationEvery = 6;
	public float lastTurnTime = 0;

	public Room room;
	public float turn = 0;
	public int playerHP;
	public int[] playerResources;
	public int bossHP;
	public int bossActionIndex;
	public int bossActionLoopCount;
	IEnumerator playTurns;

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

			Resolve(actionOrLoop.action, dice.currentSkill;

			dice.Advance();
		}
	}

	void Resolve(BossAction bossAction, Skill playerAction)
	{
		int bossDef = 0;
		int playerDef = 0;
		if (bossAction == BossAction.FlipDice) {
			dice.Flip();
			playerAction = dice.currentSkill;
		}
		if (bossAction == BossAction.RandomizeDice) {
			dice.Randomize();
			playerAction = dice.currentSkill;
		}
		if (bossAction == BossAction.Def)
			bossDef = 1;
		if (playerAction.Effect == SkillEffect.Def)
			playerDef = 1;
		if (bossAction == BossAction.Atk)
			playerHP -= Mathf.Max(0, 1 - playerDef);
		if (playerAction.Effect == SkillEffect.Atk)
			bossHP -= Mathf.Max(0, playerAction.Value - bossDef);
		if (playerAction.Effect == SkillEffect.AtkCharged)
			bossHP -= Mathf.Max(0, playerResources[3] - bossDef);
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
