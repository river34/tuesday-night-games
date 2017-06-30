using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fear
{
	public class MapController : MonoBehaviour {

		private MapGenerator map;
		private Transform player;
		// private BoxCollider2D boxCollider;
		private float maxDistance = 900f;

		void Start ()
		{
			// boxCollider = gameObject.AddComponent <BoxCollider2D> ();
			// boxCollider.isTrigger = true;
			map = GameObject.FindGameObjectWithTag ("GameController").GetComponent <MapGenerator> ();
			// boxCollider.size = new Vector2 (map.map_width, map.map_height);
		}

		void Update ()
		{
			if (player == null)
			{
				GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
				if (playerObject != null)
				{
					player = playerObject.transform;
					float maxView = playerObject.GetComponent <PlayerController> ().maxView;
					maxDistance = maxView * maxView * 16;
				}
			}

			if (player == null)
			{
				return;
			}

			Vector3 distance = player.position - transform.position;

			if (distance.sqrMagnitude > maxDistance)
			{
				map.RemoveMap (transform.position.x, transform.position.z);
				// Destroy (gameObject);

				// recycle gameobjects
				foreach (Transform child in gameObject.transform)
				{
					if (child.gameObject.CompareTag ("Tree"))
					{
						map.trees.Add (child.gameObject);
						child.SetParent (map.poolHolder);
						child.gameObject.SetActive (false);
					}
					else if (child.gameObject.CompareTag ("Strength"))
					{
						map.strengths.Add (child.gameObject);
						child.SetParent (map.poolHolder);
						child.gameObject.SetActive (false);
					}
					else if (child.gameObject.CompareTag ("Monster"))
					{
						map.monsters.Add (child.gameObject);
						child.SetParent (map.poolHolder);
						child.gameObject.SetActive (false);
					}
				}
				Destroy (gameObject);
			}
		}
	}
}
