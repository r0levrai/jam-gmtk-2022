using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
	private float diceRotationSpeed;
	private IEnumerator playTurns;

    public void StartBattle(Room room)
	{
		this.room = room;
		dice.rotateToMatchSides = true;
		diceRotationSpeed = dice.rotationSpeed;
		playTurns = PlayTurns();
		StartCoroutine(playTurns);
	}

	IEnumerator PlayTurns()
	{
		print("Battle started!");
		playerHP = 1;
		playerResources = new int[5];
		bossHP = room.BossHP;
		float speedMultiplier = 1;
		for (turn = 0; ; turn++)
		{
            PeakEnemy();

			speedMultiplier = 1 + (float) turn / halveDurationEvery;
			dice.rotationSpeed = diceRotationSpeed * speedMultiplier;
			yield return new WaitForSeconds(0.35f * initialTurnDuration / speedMultiplier);
			
			Resolve(GetNextBossAction(), dice.currentSkill);
			ui.Refresh();
			yield return new WaitForSeconds(0.65f * initialTurnDuration / speedMultiplier);

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
		if (bossAction == BossAction.Def4)
			bossDef = 4;
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
		{
			bossHP -= Mathf.Max(0, playerResources[(int)PlayerResources.Mana] - bossDef);
			playerResources[(int)PlayerResources.Mana] = 0;
		}

		// priority 4: loose mana charging
		if (playerAction.Effect != SkillEffect.Move && playerAction.Effect != SkillEffect.ManaCharged)
			playerResources[(int)PlayerResources.ManaCharging] = 0;

		// priority 0: make sure you don't forget the inspector >:{
		if (playerAction.Effect == SkillEffect.Pass && playerAction.name != "Pass")
			Debug.LogWarning($"Skill effect for skill '{playerAction.name}' is Pass. Did we forget to set it?");

		// battle end
		if (bossHP <= 0)
		{
			Stop();
			ui.BattleEnd(true);
		}
		else if (playerHP <= 0)
		{
			Stop();
			ui.BattleEnd(false);
		}
	}


    void PeakEnemy()
    {

    }

	public void Stop()
	{
		ui.Refresh();
		StopCoroutine(playTurns);
		dice.rotateToMatchSides = false;
		dice.rotationSpeed = diceRotationSpeed;
		dice.currentSide = Dice.Side.Front;
		dice.nextSide = Dice.Side.Up;
	}
}
