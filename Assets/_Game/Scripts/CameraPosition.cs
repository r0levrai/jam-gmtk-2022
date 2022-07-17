using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
	new public Transform camera;

	public Transform[] targets;
	public float speed = 2;

	public SelectRoom selectRoom;

	[SerializeField] Transform lastTarget;

	private void Update()
	{
		foreach (var target in targets)
		{
			if (target.gameObject.activeInHierarchy)
			{
				if (target.name == "RoomSelectionCameraPosition" && !selectRoom.gameObject.activeInHierarchy)
					continue;  // hack since I can't make RoomSelectionCameraPosition under SelectRoom in time
				camera.position = Vector3.Lerp(camera.position, target.position, speed * Time.deltaTime);
				camera.rotation = Quaternion.Slerp(camera.rotation, target.rotation, speed * Time.deltaTime);
				if (target != lastTarget)
				{
					lastTarget = target;
					print($"Camera switching to {target.position}");
				}
				break;
			}
		}
	}
}
