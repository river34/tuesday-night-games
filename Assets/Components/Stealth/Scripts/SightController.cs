using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class SightController : MonoBehaviour {

		// Use this for initialization
		void Start ()
		{
			//
		}

		// Update is called once per frame
		void Update ()
		{
			//
		}

		public void Rotate (float angle)
		{
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}

		public void Flip (bool is_right)
		{
			Vector3 scale = gameObject.transform.localScale;
			scale.x = - scale.x;
			if (is_right)
			{
				scale.x = Mathf.Abs (scale.x);
			}
			else
			{
				scale.x = -1 * Mathf.Abs (scale.x);
			}
			gameObject.transform.localScale = scale;
		}

		public Vector3 GetPosition ()
		{
			return transform.position;
		}

		public void Hide ()
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive (false);
			}
		}

		public void Show ()
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive (true);
			}
		}
	}
}
