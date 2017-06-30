using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fear
{
	public class CompleteController : MonoBehaviour {

		private float rad;
		private Vector3 offset;
		private float angle;

		void Awake ()
		{
			rad = Random.Range (0.15f, 0.25f);
			offset = new Vector3 (0, 1, 1);
			angle = Random.Range (0, 1f);
		}

		void Update () {
			angle += Time.deltaTime;
			transform.Find ("Sprite").localPosition
				= new Vector3 (Mathf.Sin (angle) * rad, Mathf.Cos (angle) * rad, Mathf.Cos (angle) * rad) + offset;
		}
	}
}
