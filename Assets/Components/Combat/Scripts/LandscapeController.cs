using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
	public class LandscapeController : MonoBehaviour {

		public List <Transform> landscapes;
		private float base_speed = 0.2f;
		private float start_x = 2.45f;
		private float end_x = -9.55f;
		private float length_x = 12f;

		// Use this for initialization
		void Start () {
			// get all child gameobject with tag "landscape"
			landscapes = new List <Transform>();
			foreach (Transform child in transform)
			{
				if (child.CompareTag (Tags.LANDSCAPE))
				{
					transform.position = new Vector3 (start_x, transform.position.y, transform.position.z);
					landscapes.Add (child);
				}
			}
		}

		// Update is called once per frame
		void Update () {
			int index = 0;
			foreach (Transform landscape in landscapes)
			{
				landscape.GetChild(0).position += Vector3.left * Time.deltaTime * base_speed * (landscapes.Count - index);
				if (landscape.childCount > 1)
				{
					landscape.GetChild(1).position += Vector3.left * Time.deltaTime * base_speed * (landscapes.Count - index);
				}
				if (landscape.GetChild(0).position.x <= start_x && landscape.childCount < 2)
				{
					Vector3 position = landscape.position;
					position.x += length_x;
					Instantiate (landscape.GetChild(0).gameObject, position, landscape.rotation, landscape);
				}
				if (landscape.GetChild(0).position.x <= end_x && landscape.childCount > 1)
				{
					Destroy (landscape.GetChild(0).gameObject);
				}
				index ++;
			}
		}
	}
}
