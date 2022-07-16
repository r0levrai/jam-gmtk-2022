using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCard : MonoBehaviour
{

    public TMP_Text title, description;
    public Image icon;
    public LayoutGroup resources;
    public GameObject resourcePrefab, container;
    private Skill skill;

    public Image background;

    bool active = false;

    public void BuildInfo(Skill s)
    {
        if(!active)
        {
            container.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
            active = true;
        }

        skill = s;
        background.color = Data.Instance.bgColors[(int)skill.Background];
        title.text = skill.name;
        description.text = String.Format(skill.Description, skill.Value);
        icon.sprite = skill.Icon;

        foreach (Transform child in resources.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int resource = 0; resource < skill.Resources.Length; resource++)
        {
            for (int n = 0; n < skill.Resources[resource]; n++)
            {
                GameObject res = Instantiate<GameObject>(resourcePrefab, resources.transform);
                res.GetComponent<Image>().sprite = Data.Instance.resources[resource];
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        container.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
