using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceFace : MonoBehaviour
{
	public TMP_Text value;
	public Image icon;
	public LayoutGroup resources;
	public GameObject resourcePrefab;
	public Image background;
	public Button button;

	private void Start()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public void SetSkill(Skill skill)
	{
		if (skill == null)
		{
			Debug.LogError("[DiceFace.SetSkill] skill is null");
			return;
		}
		background.color = Data.Instance.bgColors[(int)skill.Background];
		value.color = Data.Instance.bgTextColors[(int)skill.Background];
		value.text = skill.Value.ToString();
		icon.sprite = skill.Icon;

		foreach (Transform child in resources.transform)
		{
			//GameObject.DestroyImmediate(child.gameObject);
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
}
