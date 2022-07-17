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
		{ Side.Up,    Side.Right, Side.Down,  Side.Left },
		{ Side.Down,  Side.Right, Side.Up,    Side.Left },
		{ Side.Front, Side.Right, Side.Back,  Side.Left },
		{ Side.Back,  Side.Right, Side.Front, Side.Left },
		{ Side.Up, Side.Front, Side.Down, Side.Back  },
		{ Side.Up, Side.Back,  Side.Down, Side.Front },
	};
	public static Vector3[] d6sideDirections = new Vector3[] {
		Vector3.back, Vector3.forward, Vector3.up, Vector3.down, Vector3.left, Vector3.right
	};  // forward dir from the camera is dice's back, back is dice's front

	public DiceFace[] sides = new DiceFace[6];
	public Skill[] skills = new Skill[6];
	public Side currentSide = Side.Front;
	public Side nextSide = Side.Up;

	public Skill currentSkill => skills[(int)currentSide];

	public bool rotateToMatchSides = false;
	public float rotationSpeed = 4;

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

    Side savedCurrent, savedNext;
    public void SaveCurrent()
    {
        savedCurrent = currentSide;
        savedNext = nextSide;
    }

    public void LoadCurrent()
    {
        currentSide = savedCurrent;
        nextSide = savedNext;
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

	public void Randomize()  // assume d6
	{
		currentSide = (Side) Random.Range(0, 6);
		nextSide = d6neighborSide[(int)currentSide, Random.Range(0, d6neighborSide.GetLength(1))];
	}

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

	public Quaternion GetRotationFromSides()
		=> Quaternion.Inverse( Quaternion.LookRotation(
			d6sideDirections[(int)currentSide],
			d6sideDirections[(int)nextSide]
		));

	public Quaternion GetRotationFromSides(Transform lookAt)
		=> Quaternion.LookRotation(
			lookAt.position - this.transform.position,
			lookAt.up
		) * GetRotationFromSides();

	private void Update()  // assume d6
	{
		if (rotateToMatchSides)
		{
			Transform lookAt = Camera.main.transform;
			transform.rotation = Quaternion.Slerp(transform.rotation, GetRotationFromSides(lookAt), rotationSpeed * Time.deltaTime);
		}
	}
}
