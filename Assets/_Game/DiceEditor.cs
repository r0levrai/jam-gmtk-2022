using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceEditor : MonoBehaviour
{
	public DiceFace[] sides = new DiceFace[6];
	public Skill[] skills = new Skill[6];

	void Start()
	{
		for (int side = 0; side < sides.Length; side++)
		{
			sides[side].SetSkill(skills[side]);
			int s = side;
			sides[side].button.onClick.AddListener( () => EditFace(s) );
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
			transform.Rotate(Vector3.Cross(dMouse, Camera.main.transform.forward), dMouse.magnitude, Space.World);
		}
	}

	public void EditFace(int side)
	{
		print(side);
	}
}
