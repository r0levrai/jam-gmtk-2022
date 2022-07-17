using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
	public Battle battle;
    public Image movingBar, backgdoundBar;
    public TMP_Text nameText, HPText, playerHPText, playerManaText;
	public GameObject victoryUI, defeatUI;
	public GameObject diceBuilderUI;

    public LayoutGroup playerSequence, enemySequence;
    public DiceFace uiElementPrefab;

    // Start is called before the first frame update
    void Start()
    {
        nameText.outlineWidth = 0.2f;
        nameText.outlineColor = new Color32(0, 0, 0, 255);
    }

    // Update is called once per frame
    public void Refresh()
    {
		nameText.text = battle.room.BossName;
        HPText.text = battle.bossHP.ToString() + "/" + battle.room.BossHP.ToString();
        playerHPText.text = battle.playerHP.ToString();
        playerManaText.text = battle.playerResources[(int)PlayerResources.Mana].ToString();

        Vector2 size = backgdoundBar.rectTransform.sizeDelta;
        Vector2 currSize = size;
        currSize.x = size.x * (float)battle.bossHP / battle.room.BossHP;
        movingBar.rectTransform.sizeDelta = currSize;

    }


    public void ClearBossActions()
    {
        foreach (Transform child in enemySequence.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public void AddBossAction(BossAction ba)
    {
        Skill skill = Data.Instance.skills[(int)ba];
		DiceFace uiElement = Instantiate<DiceFace>(uiElementPrefab, enemySequence.transform);
		uiElement.SetSkill(skill);
    }


    public void BattleEnd(bool won)
	{
		if (won)
		{
			print("Battle won :)");
			victoryUI.gameObject.SetActive(true);
		}
		else
		{
			print("Battle lost :/");
			defeatUI.gameObject.SetActive(true);
		}
	}

	public void Exit()  // called by a button
	{
		diceBuilderUI.gameObject.SetActive(true);
		victoryUI.gameObject.SetActive(false);
		defeatUI.gameObject.SetActive(false);
		this.gameObject.SetActive(false);
	}
}
