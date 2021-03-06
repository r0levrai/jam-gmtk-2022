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
	int _bossHP;
	public int bossHP {
		get { return _bossHP; }
		set { if (initialized) ui.BossDamagePopup(value - _bossHP); _bossHP = value; }
	}
	int _playerHP;
	public int playerHP {
		get { return _playerHP; }
		set { if (initialized) ui.PlayerDamagePopup(value - _playerHP); _playerHP = value; }
	}
	public int[] playerResources = new int[5];
	public void setMana(int value) {
		ui.PlayerDamagePopup(value - playerResources[(int)PlayerResources.Mana],
							 Data.Instance.bgColors[(int)PlayerResources.Mana]);
		playerResources[(int)PlayerResources.Mana] = value;
	}
	int getMana() => playerResources[(int)PlayerResources.Mana];

	private bool initialized = false;
	private int bossActionIndex;
	private int bossActionLoopCount;
	private float diceRotationSpeed;
	private IEnumerator playTurns;

    public void StartBattle(Room room)
	{
		this.room = room;
		ui.Init();
		dice.rotateToMatchSides = true;
		diceRotationSpeed = dice.rotationSpeed;
		playTurns = PlayTurns();
		StartCoroutine(playTurns);
	}

	IEnumerator PlayTurns()
	{
		initialized = false;
		print("Battle started!");
		playerHP = 1;
		playerResources = new int[5];
		bossHP = room.BossHP;
		bossActionIndex = 0;
		bossActionLoopCount = 0;
		float speedMultiplier = 1;
		initialized = true;
		for (turn = 0; ; turn++)
		{
            PeakEnemy();
            PeakPlayer();

			speedMultiplier = Mathf.Min(10, 1 + (float) turn / halveDurationEvery);
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
			if (actionOrLoop.times == 0 || bossActionLoopCount + 1 < actionOrLoop.times)
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
		if (bossAction == BossAction.PivotDiceLeftOrRight) {
			dice.Pivot();
			playerAction = dice.currentSkill;
		}

		// priority 2: defense, buffs and other statuses
		int bossDef = 0;
		int playerDef = 0;
		if (bossAction == BossAction.Def)
			bossDef = 1;
		if (bossAction == BossAction.Def4)
			bossDef = 3;
		if (playerAction.Effect == SkillEffect.Def)
			playerDef = 1;
		// mana
		if (playerAction.Effect == SkillEffect.Mana)
			setMana(getMana() + playerAction.Value);
		if (playerAction.Effect == SkillEffect.ManaCharged)
			playerResources[(int)PlayerResources.ManaCharging]++;
		// arrows
		if (playerAction.Effect == SkillEffect.Move)
		{
			setMana(getMana() + playerResources[(int)PlayerResources.ManaCharging]);
			dice.nextSide = Dice.d6neighborSide[(int)dice.currentSide, playerAction.Value];
		}

		// priority 3: attacks
		if (bossAction == BossAction.Atk)
			playerHP -= Mathf.Max(0, 1 - playerDef);
		if (playerAction.Effect == SkillEffect.Atk && getMana() >= playerAction.Resources[(int)PlayerResources.Mana])
		{
			setMana(getMana() - playerAction.Resources[(int)PlayerResources.Mana]);
			bossHP -= Mathf.Max(0, playerAction.Value - bossDef);
		}
		if (playerAction.Effect == SkillEffect.AtkCharged)
		{
			bossHP -= Mathf.Max(0, getMana() - bossDef);
			setMana(0);
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
            bossHP = 0;
            StartCoroutine(BattleEnd(true));
		}
		else if (playerHP <= 0)
		{
            playerHP = 0;
            StartCoroutine(BattleEnd(false));
		}
	}


    void PeakEnemy()
    {
        int saveBossIndex = bossActionIndex;
        int saveBossLoopCount = bossActionLoopCount;

        ui.ClearBossActions();

        for (int i = 0; i < 10; i++)
        {
            BossAction ba = GetNextBossAction();
            ui.AddBossAction(ba);
            bossActionIndex++;
        }

        bossActionIndex = saveBossIndex;
        bossActionLoopCount = saveBossLoopCount;
    }

    void PeakPlayer()
    {
        dice.SaveCurrent();

        ui.ClearPlayerActions();

        for (int i = 0; i < 10; i++)
        {
            ui.AddPlayerAction(dice.currentSkill);
            
            if (dice.currentSkill.Effect == SkillEffect.Move)
                dice.nextSide = Dice.d6neighborSide[(int)dice.currentSide, dice.currentSkill.Value];

            dice.Advance();

        }

        dice.LoadCurrent();
    }

	IEnumerator BattleEnd(bool won)
	{
		Stop();
		yield return new WaitForSeconds(2);
		ui.BattleEnd(won);
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
