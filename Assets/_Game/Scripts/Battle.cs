using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerResources
{
	Mana = 3,
	ManaCharging = 4
}

public class Battle : MonoBehaviour
{
	public Dice dice;
	public BattleUI ui;
	public float initialTurnDuration = 2;
	public int halveDurationEvery = 6;

	public Room room;

	public int turn;
	public int bossHP;
	public int playerHP;
	public int[] playerResources;

	private int bossActionIndex;
	private int bossActionLoopCount;
	private IEnumerator playTurns;

	public void StartBattle(Room room)
	{
		this.room = room;
		playTurns = PlayTurns();
		StartCoroutine(playTurns);
	}

	IEnumerator PlayTurns()
	{
		print("Battle started!");
		playerHP = 1;
		playerResources = new int[5];
		bossHP = room.BossHP;
		for (turn = 0; ; turn++)
		{
			ui.Refresh();
			yield return new WaitForSeconds(initialTurnDuration / (1 + (float)turn/halveDurationEvery));
			
			Resolve(GetNextBossAction(), dice.currentSkill);

			bossActionIndex++;
			dice.Advance();
		}
	}

	BossAction GetNextBossAction()
	{
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
		return actionOrLoop.action;
	}

	void Resolve(BossAction bossAction, Skill playerAction)
	{
		print($"Player used {playerAction}, Boss used {bossAction}");

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
		if (playerAction.Effect == SkillEffect.Pass && playerAction.name != "Pass")
			Debug.LogWarning($"Skill effect for skill '{playerAction.name}' is Pass. Did we forget to set it?");

		// battle end
		if (bossHP <= 0)
		{
			BattleEnd(true);
		}
		else if (playerHP <= 0)
		{
			BattleEnd(false);
		}
	}

	void BattleEnd(bool won)
	{
		ui.Refresh();
		StopCoroutine(playTurns);
		ui.BattleEnd(won);
	}
}
