using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceBuilder : MonoBehaviour
{
	public GameObject roomSelectUI;
	public Dice dice;
	public Battle battle;
	public LayoutGroup uiLayout, uiEnemyLayout;
	public DiceFace uiElementPrefab;
    public GameObject textBigPrefab, textSmallPrefab;
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
        resetButton.onClick.AddListener(ResetUI);
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
        if(room != null)
            SetEnemySequence();
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

    private void ResetUI()
    {
        
        for (int s = 0; s < dice.sides.Length; s++)
        {
            dice.SetSide(s, pass);
        }

        possibleFaces = room.DiceFacesAvailable;
        SetPossibleFaces(room.DiceFacesAvailable);
        highlightPossibleFaces(true);

        selected = null;
        selectedFace = -1;
        selectedDF = null;

        dice.HighlightFaces(false);

        dice.transform.rotation = Quaternion.identity;
    }

	public void SetRoom(Room room)
	{
        this.room = room;

        ResetUI();
        SetEnemySequence();
    }

    public void SetEnemySequence()
    {
        foreach (var child in uiEnemyLayout.GetComponentsInChildren<DiceFace>())
        {
            Destroy(child.gameObject);
        }

        List<Skill> bossSkills = new List<Skill>();
        List<int> starts = new List<int>(), ends = new List<int>(), value = new List<int>();
        int nbLoops = 0;
        for (int i = 0; i < room.BossActions.Length; i++)
        {
            ActionOrLoop action = room.BossActions[i];
            if (action.action != BossAction.LoopTo)
                bossSkills.Add(Data.Instance.skills[(int)action.action]);
            else
            {
                starts.Add(action.toElement - nbLoops);
                ends.Add(bossSkills.Count);
                value.Add(action.times);
                nbLoops++;
            }
        }


        for(int i = 0; i < bossSkills.Count + 1; i++)
        {
            for(int j = 0; j < starts.Count;  j++)
            {
                if(i == ends[j])
                {
                    GameObject uiElement1 = Instantiate<GameObject>(textBigPrefab, uiEnemyLayout.transform);
                    uiElement1.GetComponentInChildren<TMP_Text>().text = "]";

                    GameObject uiElement2 = Instantiate<GameObject>(textSmallPrefab, uiEnemyLayout.transform);
                    uiElement2.GetComponentInChildren<TMP_Text>().text = value[j] == 0 ? "x Inf" : "x " + value[j].ToString();
                }
                if(i == starts[j])
                {
                    GameObject uiElement3 = Instantiate<GameObject>(textBigPrefab, uiEnemyLayout.transform);
                    uiElement3.GetComponentInChildren<TMP_Text>().text = "[";
                }
               
            }
            if(i < bossSkills.Count)
            {
                Skill skill = bossSkills[i];
                DiceFace uiElement = Instantiate<DiceFace>(uiElementPrefab, uiEnemyLayout.transform);
                uiElement.SetSkill(skill);
                uiElement.button.onClick.AddListener(() => ShowSkill(skill));
            }
        }

        
    }

    void ShowSkill(Skill s)
    {
        skillCard.BuildInfo(s);
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
