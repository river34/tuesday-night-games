using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class CameraController : MonoBehaviour {

		public Transform player;
		private float right_offset = -3f;
		private float left_offset = 6f;
		// private float speed;

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
			if (player.position.x - transform.position.x > right_offset)
			{
				transform.position = new Vector3 (player.position.x - right_offset, transform.position.y, transform.position.z);
			}
			else if (transform.position.x - player.position.x > left_offset)
			{
				transform.position = new Vector3 (player.position.x + left_offset, transform.position.y, transform.position.z);
			}
		}
	}
}
