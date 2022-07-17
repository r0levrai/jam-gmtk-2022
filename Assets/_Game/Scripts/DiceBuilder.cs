using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceBuilder : MonoBehaviour
{
	public GameObject roomSelectUI;
	public Dice dice;
	public Battle battle;
	public LayoutGroup uiLayout;
	public DiceFace uiElementPrefab;
	public Skill pass;
	public Skill[] possibleFaces;
	public SkillCard skillCard;
    public Button resetButton, removeButton, resetCameraButton, startButton;
	public Room room;

	Skill selected = null;
    int selectedFace = -1;
    DiceFace selectedDF = null;

    void Start()
	{
        resetButton.onClick.AddListener(() => ResetUI(possibleFaces));
        removeButton.onClick.AddListener(RemoveFace);
        removeButton.gameObject.SetActive(false);

        resetCameraButton.onClick.AddListener(ResetCamera);
        startButton.onClick.AddListener(StartBattle);
    }

	private void OnEnable()
	{
		for (int side = 0; side < dice.sides.Length; side++)
		{
			int s = side;
			dice.sides[side].button.onClick.AddListener(() => EditFaceDest(s));
		}
		SetPossibleFaces(possibleFaces);
		highlightPossibleFaces(true);
		ResetCamera();
	}

	public void Back()
	{
		roomSelectUI.gameObject.SetActive(true);
		this.gameObject.SetActive(false);
	}


    private void ResetCamera()
    {
		dice.transform.rotation = dice.GetRotationFromSides(Camera.main.transform);
    }

    private void StartBattle()
    {
		if (room == null)
			return;

		dice.HighlightFaces(false);
		selectedFace = -1;
		highlightPossibleFaces(false);

		battle.gameObject.SetActive(true);
		battle.ui.gameObject.SetActive(true);
		battle.StartBattle(room);
		this.gameObject.SetActive(false);
	}

    private void RemoveFace()
    {
        if(selectedFace != -1)
        {
            selected = pass;
            EditFaceDest(selectedFace);
            selectedFace = -1;
            dice.HighlightFaces(false);
            removeButton.gameObject.SetActive(false);
            skillCard.Desactivate();
        }
    }

    private void ResetUI(Skill[] possibleFaces)
    {
        SetPossibleFaces(possibleFaces);
        highlightPossibleFaces(true);

        for (int s = 0; s < dice.sides.Length; s++)
        {
            dice.SetSide(s, pass);
        }

        dice.transform.rotation = Quaternion.identity;
    }

	public void SetRoom(Room room)
	{
		SetPossibleFaces(room.DiceFacesAvailable);
		this.room = room;
	}

    public void SetPossibleFaces(Skill[] possibleFaces)
	{
		foreach (var child in uiLayout.GetComponentsInChildren<DiceFace>())
		{
			Destroy(child.gameObject);
		}
		List<Skill> possibleFacesLeft = new List<Skill>(possibleFaces);
		foreach (var skill in dice.skills)
		{
			possibleFacesLeft.Remove(skill);
		}
		foreach (var skill in possibleFacesLeft)
		{
			DiceFace uiElement = Instantiate<DiceFace>(uiElementPrefab, uiLayout.transform);
			uiElement.SetSkill(skill);
			uiElement.button.onClick.AddListener(() => EditFaceSource(skill, uiElement));
		}
		this.possibleFaces = possibleFaces;
		this.selected = null;
        this.selectedDF = null;
    }


	
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
            removeButton.gameObject.SetActive(false);
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
            if(selectedDF != null)
                Destroy(selectedDF.gameObject);
            dice.HighlightFaces(false);

        }
        else
        {
            dice.HighlightFaces(false);
            selectedFace = side;
            skillCard.BuildInfo(dice.skills[side]);
            dice.HighlightFace(side, true);
            removeButton.gameObject.SetActive(true);
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
