using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
	public DiceFace[] sides = new DiceFace[6];
	public Skill[] skills = new Skill[6];

	void Start()
	{
		SetSides(skills);
	}

	public void SetSides(Skill[] skills)
	{
		for (int side = 0; side < sides.Length; side++)
		{
			SetSide(side, skills[side]);
		}
	}

	public DiceFace SetSide(int side, Skill skill)
	{
		sides[side].SetSkill(skill);
        skills[side] = skill;

        return sides[side];
	}

	public void HighlightFaces(bool enabled)
	{
		for (int side = 0; side < sides.Length; side++)
		{
			sides[side].SetHighlight(enabled);
		}
	}
    public void HighlightFace(int nb, bool enabled)
    {
        sides[nb].SetHighlight(enabled);
    }
}
