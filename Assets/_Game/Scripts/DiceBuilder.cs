using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceBuilder : MonoBehaviour
{
	public Dice dice;
	public LayoutGroup uiLayout;
	public DiceFace uiElementPrefab;
	public Skill[] possibleFaces;

	void Start()
	{
		for (int side = 0; side < dice.sides.Length; side++)
		{
			int s = side;
			dice.sides[side].button.onClick.AddListener(() => EditFaceDest(s));
		}
		SetPossibleFaces(possibleFaces);
		highlightPossibleFaces(true);
	}

	public void SetPossibleFaces(Skill[] possibleFaces)
	{
		foreach (var child in uiLayout.GetComponentsInChildren<DiceFace>())
		{
			Destroy(child.gameObject);
		}
		for (int i = 0; i < possibleFaces.Length; i++)
		{
			Skill skill = possibleFaces[i];
			DiceFace uiElement = Instantiate<DiceFace>(uiElementPrefab, uiLayout.transform);
			uiElement.SetSkill(skill);
			uiElement.button.onClick.AddListener(() => EditFaceSource(skill));
		}
		this.possibleFaces = possibleFaces;
		this.selected = null;
	}

	Skill selected = null;
	void EditFaceSource(Skill skill)
	{
		//print(skill);
		selected = skill;
		highlightPossibleFaces(false);
		dice.HighlightFaces(true);
	}

	void EditFaceDest(int side)
	{
		//print(side);
		if (selected != null)
		{
			dice.SetSide(side, selected);
		}
		dice.HighlightFaces(false);
	}

	void highlightPossibleFaces(bool enabled)
	{
		foreach (var child in uiLayout.GetComponentsInChildren<DiceFace>())
		{
			child.SetHighlight(enabled);
		}
	}

	Vector3 mouse;
	Vector3 lastMouse = Vector3.zero;
	Vector3 dMouse;
	void Update()
    {
		/*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 1000))
			print(hit);*/
		mouse = Input.mousePosition;
		mouse.z = 0;
		dMouse = mouse - lastMouse;
		lastMouse = mouse;
		if (Input.GetMouseButton(1))
		{
			dice.transform.Rotate(Vector3.Cross(dMouse, Camera.main.transform.forward), dMouse.magnitude, Space.World);
		}
	}
}
