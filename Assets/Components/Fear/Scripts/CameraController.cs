using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fear
{
	public class CameraController : MonoBehaviour {

		public Transform player;
		private Vector3 offset;
		private Vector3 original_position = new Vector3 (0, -5, -6);
		private Vector3 origianl_rotation = new Vector3 (-45, 0, 0);

		void Update ()
		{
			if (player == null)
			{
				return;
			}

			transform.position = player.position + offset;
		}

		public void InitCamera ()
		{
			transform.position = original_position;
			transform.eulerAngles = origianl_rotation;
			offset = transform.position - player.position;
		}
	}
}
