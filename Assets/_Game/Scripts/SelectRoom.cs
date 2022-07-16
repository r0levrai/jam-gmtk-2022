using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRoom : MonoBehaviour
{
	public DiceBuilder diceBuilder;
	public LayoutGroup uiLayout;
	public DiceFace uiElementPrefab;
	public Room[] data;
	public int unlocked = -1;

	private void Start()
	{
		diceBuilder.gameObject.SetActive(false);
		RefreshList();
	}

	public void RefreshList()
    {
		foreach (var child in uiLayout.GetComponentsInChildren<DiceFace>())
		{
			Destroy(child.gameObject);
		}

		if (unlocked < 1)
			unlocked = int.MaxValue;
		for (int i = 0; i < Mathf.Min(data.Length, unlocked); i++)
		{
			Room room = data[i];
			DiceFace uiElement = Instantiate<DiceFace>(uiElementPrefab, uiLayout.transform);
			room.RoomIcon.Value = i + 1;
			uiElement.SetSkill(room.RoomIcon);
			uiElement.button.onClick.AddListener( () => LoadRoom(room) );
		}
    }

	public void LoadRoom(Room room)
	{
		diceBuilder.gameObject.SetActive(true);
		this.gameObject.SetActive(false);
	}
}
