using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class BackgroundController : MonoBehaviour {

		public List <Transform> backgrounds;
		private float start_speed = 0f;
		private float base_speed = 0.1f;
		private float start_x = 0f;
		private float length_x = 29f;
		private bool is_moving;
		private bool is_right;

		// Use this for initialization
		void Start () {
			// get all child gameobject with tag "background"
			backgrounds = new List <Transform>();
			foreach (Transform child in transform)
			{
				if (child.CompareTag (Tags.BACKGROUND))
				{
					transform.position = new Vector3 (start_x, transform.position.y, transform.position.z);
					backgrounds.Add (child);
				}
			}
			is_moving = false;
			is_right = false;
		}

		// Update is called once per frame
		void Update () {
			if (is_moving)
			{
				if (is_right)
				{
					if (backgrounds.Count <= 0)
					{
						return;
					}

					int index = 0;
					foreach (Transform background in backgrounds)
					{
						if (background.childCount <= 0)
						{
							continue;
						}
						Vector3 speed = Vector3.left * Time.deltaTime * (start_speed + base_speed * (backgrounds.Count - index) * (backgrounds.Count - index));
						background.GetChild(0).position += speed;

						if (background.childCount > 1)
						{
							background.GetChild(1).position += speed;
						}
						if (background.GetChild(0).position.x <= start_x)
						{
							if (background.childCount == 2)
							{
								if (background.GetChild(1).position.x <= background.position.x - length_x)
								{
									Destroy (background.GetChild(1).gameObject);
								}
							}
							if (background.childCount < 2)
							{
								Vector3 position = background.position;
								position.x += length_x;
								Instantiate (background.GetChild(0).gameObject, position, background.rotation, background);
							}
						}
						if (background.childCount > 1 && background.GetChild(0).position.x <= start_x - length_x)
						{
							Destroy (background.GetChild(0).gameObject);
						}
						index ++;
					}
				}
				else
				{
					if (backgrounds.Count <= 0)
					{
						return;
					}

					int index = 0;
					foreach (Transform background in backgrounds)
					{
						if (background.childCount <= 0)
						{
							continue;
						}
						Vector3 speed = Vector3.right * Time.deltaTime * (start_speed + base_speed * (backgrounds.Count - index) * (backgrounds.Count - index));
						background.GetChild(0).position += speed;
						if (background.childCount > 1)
						{
							background.GetChild(1).position += speed;
						}
						if (background.GetChild(0).position.x >= start_x)
						{
							if (background.childCount == 2)
							{
								if (background.GetChild(1).position.x >= background.position.x + length_x)
								{
									Destroy (background.GetChild(1).gameObject);
								}
							}
							if (background.childCount < 2)
							{
								Vector3 position = background.position;
								position.x -= length_x;
								Instantiate (background.GetChild(0).gameObject, position, background.rotation, background);
							}
						}
						if (background.GetChild(0).position.x >= start_x + length_x && background.childCount > 1)
						{
							Destroy (background.GetChild(0).gameObject);
						}
						if (background.GetChild(0).position.x >= start_x + length_x && background.childCount > 1)
						{
							Destroy (background.GetChild(0).gameObject);
						}
						index ++;
					}
				}
			}
		}

		public void SetMoving (bool _is_moving, bool _is_right)
		{
			is_moving = _is_moving;
			is_right = _is_right;
		}
	}
}
