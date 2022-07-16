using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
	public enum Side : int {
		Front, Back, Up, Down, Left, Right
	}
	public static Side[] opposedSide = new Side[] {
		Side.Back, Side.Front, Side.Down, Side.Up, Side.Right, Side.Left
	};
	public static Side[,] d6neighborSide = new Side[,] { // in clockwize order from the top of the face
		{ Side.Up, Side.Right, Side.Down, Side.Left },
		{ Side.Up, Side.Left, Side.Down, Side.Right },
		{ Side.Back, Side.Right, Side.Front, Side.Left },
		{ Side.Back, Side.Left, Side.Front, Side.Left },
		{ Side.Up, Side.Front, Side.Down, Side.Back },
		{ Side.Up, Side.Back, Side.Down, Side.Front },
	};

	public DiceFace[] sides = new DiceFace[6];
	public Skill[] skills = new Skill[6];
	public Side currentSide = Side.Front;
	public Side nextSide = Side.Up;

	public Skill currentSkill => skills[(int)currentSide];

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

	public void Advance()
	{
		(currentSide, nextSide) = (nextSide, opposedSide[(int)currentSide]);
	}

	public void Flip()
	{
		currentSide = opposedSide[(int)currentSide];
		nextSide    = opposedSide[(int)nextSide];
	}

	public void Randomize(int d=6)
	{
		if (d != 6)
			Debug.LogError("[Dice.Randomize] Neighbors faces for d!=6 unknown, randomization undefined");
		currentSide = (Side) Random.Range(0, d);
		nextSide = d6neighborSide[(int)currentSide, Random.Range(0, d6neighborSide.GetLength(1))];
	}
	public void Randomize() => Randomize(this.sides.Length);

	public Side[] GetCycle(Side currentSide, Side nextSide)
	{
		var cycle = new List<Side>();
		while (nextSide != cycle[0])
		{
			(currentSide, nextSide) = (nextSide, opposedSide[(int)currentSide]);
		}
		return cycle.ToArray();
	}
	public Side[] GetCycle() => GetCycle(this.currentSide, this.nextSide);
}
