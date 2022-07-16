using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public int HP, HPMax, playerHP, playerMana;
    public String bossName;
    public Image movingBar, backgdoundBar;
    public TMP_Text nameText, HPText, playerHPText, playerManaText;


    // Start is called before the first frame update
    void Start()
    {
        nameText.outlineWidth = 0.2f;
        nameText.outlineColor = new Color32(0, 0, 0, 255);
    }

    // Update is called once per frame
    void Update()
    {
        nameText.text = bossName;
        HPText.text = HP.ToString() + "/" + HPMax.ToString();
        playerHPText.text = playerHP.ToString();
        playerManaText.text = playerMana.ToString();

        Vector2 size = backgdoundBar.rectTransform.sizeDelta;
        Vector2 currSize = size;
        currSize.x = size.x * (float)HP / (float)HPMax;
        movingBar.rectTransform.sizeDelta = currSize;

    }
}
