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
    public SkillCard skillCard;
    public Button resetButton, removeButton;

    public Skill pass;

    void Start()
	{
		for (int side = 0; side < dice.sides.Length; side++)
		{
			int s = side;
			dice.sides[side].button.onClick.AddListener(() => EditFaceDest(s));
		}
		SetPossibleFaces(possibleFaces);
		highlightPossibleFaces(true);

        resetButton.onClick.AddListener(() => ResetUI(possibleFaces));

    }

    private void ResetUI(Skill[] possibleFaces)
    {
        SetPossibleFaces(possibleFaces);
        highlightPossibleFaces(true);

        for (int s = 0; s < dice.sides.Length; s++)
        {
            dice.SetSide(s, pass);
        }
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
			uiElement.button.onClick.AddListener(() => EditFaceSource(skill, uiElement));
		}
		this.possibleFaces = possibleFaces;
		this.selected = null;
        this.selectedDF = null;
    }


	Skill selected = null;
    int selectedFace = -1;
    DiceFace selectedDF = null;
	void EditFaceSource(Skill skill, DiceFace diceFace)
	{
		//print(skill);
        if(selectedFace == -1)
        {
            selected = skill;
            selectedDF = diceFace;
            highlightPossibleFaces(false);
            dice.HighlightFaces(true);
            skillCard.BuildInfo(skill);
        }
        else
        {
            selected = skill;
            selectedDF = diceFace;
            EditFaceDest(selectedFace);
            dice.HighlightFaces(false);
            skillCard.BuildInfo(skill);
            selectedFace = -1;
        }
		

    }

	void EditFaceDest(int side)
	{
		//print(side);
		if (selected != null)
		{
            if (!dice.skills[side].Equals(pass))
            {
                Skill prevSkill = dice.skills[side];
                DiceFace uiElement = Instantiate<DiceFace>(uiElementPrefab, uiLayout.transform);
                uiElement.SetSkill(prevSkill);
                uiElement.button.onClick.AddListener(() => EditFaceSource(prevSkill, uiElement));
            }
            dice.SetSide(side, selected);
            highlightPossibleFaces(true);
            selected = null;
            Destroy(selectedDF.gameObject);
            dice.HighlightFaces(false);

        }
        else
        {
            dice.HighlightFaces(false);
            selectedFace = side;
            dice.HighlightFace(side, true);
        }
		

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
