using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
	public Battle battle;
    public Image movingBar, backgdoundBar, bossAvatar;
    public TMP_Text nameText, HPText, playerHPText, playerManaText;
	public GameObject victoryUI, defeatUI;
	public GameObject diceBuilderUI;

    public LayoutGroup playerSequence, enemySequence;
    public DiceFace uiElementPrefab;
	public DamagePopup damagePopupPrefab;
	public Canvas damagePopupCanvas;

	// Start is called before the first frame update
	void Start()
    {
        nameText.outlineWidth = 0.2f;
        nameText.outlineColor = new Color32(0, 0, 0, 255);
    }

	public void Init()
	{
		nameText.text = battle.room.BossName;
		bossAvatar.sprite = battle.room.BossSprite;
		Refresh();
	}

	// Update is called once per frame
	public void Refresh()
    {
		HPText.text = battle.bossHP.ToString() + "/" + battle.room.BossHP.ToString();
        playerHPText.text = battle.playerHP.ToString();
        playerManaText.text = battle.playerResources[(int)PlayerResources.Mana].ToString();

        Vector2 size = backgdoundBar.rectTransform.sizeDelta;
        Vector2 currSize = size;
        currSize.x = size.x * (float)battle.bossHP / battle.room.BossHP;
        movingBar.rectTransform.sizeDelta = currSize;

    }

	public void DamagePopup(int value, Color color, Transform parent)
	{
		print($"Popup {value} {color} {parent}");
		DamagePopup dp = Instantiate<DamagePopup>(damagePopupPrefab, parent);
		dp.text.text = value.ToString();
		dp.text.color = color;
	}
	public void BossDamagePopup(int value) => DamagePopup(value, Color.red, bossAvatar.transform);
	public void PlayerDamagePopup(int value) => DamagePopup(value, Color.red, damagePopupCanvas.transform);
	public void PlayerDamagePopup(int value, Color color) => DamagePopup(value, color, damagePopupCanvas.transform);

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


    public void ClearPlayerActions()
    {
        foreach (Transform child in playerSequence.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void AddPlayerAction(Skill s)
    {
        Skill skill = s;
        DiceFace uiElement = Instantiate<DiceFace>(uiElementPrefab, playerSequence.transform);
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
