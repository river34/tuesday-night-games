using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fear
{
	public class MonsterController : MonoBehaviour {

		public float speed;

		private Transform target;
		private PlayerController player;
		private bool FindPlayer;
		private bool lastFindPlayer;
		private float farDistance;
		// private float closeDistance;
		private float angle;
		private float rad;
		private SpriteRenderer render;
		private float reverseMul = 0.001f;
		private float maxScale = 1.6f;
		private float scaleSpeed = 0.3f;
		private float radSpeed = 0.1f;
		private float maxRad = 0.5f;
		// private Color highClear = Color.clear * 0.75f;
		private Color halfClear = Color.clear * 0.5f;
		private Color lowClear = Color.clear * 0.25f;
		private GameObject shadow;
		private Transform mapHolder;

		private MapGenerator map;

		void Awake ()
		{
			render = transform.Find ("Sprite").GetComponent <SpriteRenderer>();
			farDistance = 15f * 15f;
			// closeDistance = Random.Range (1f, 4f);

			// map generator
			map = GameObject.FindGameObjectWithTag ("GameController").GetComponent <MapGenerator> ();;
		}

		void Update () {
			if (target == null)
			{
				GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
				if (playerObject != null)
				{
					target = playerObject.transform;
					player = playerObject.GetComponent <PlayerController> ();
				}
			}

			if (target == null)
			{
				return;
			}

			if (mapHolder == null)
			{
				// shadow
				mapHolder = GameObject.Find ("Map").transform;
			}

			if (mapHolder == null)
			{
				return;
			}

			Vector3 distance = Vector3.zero;
			distance.x = target.position.x - transform.position.x;
			distance.y = target.position.y - transform.position.y;
			float sqrLen =  distance.sqrMagnitude;

			if (sqrLen < farDistance)
			{
				if (shadow == null)
				{
					shadow = new GameObject ("Shadow");
					shadow.tag = "Shadow";
					shadow.AddComponent <CircleCollider2D> ();
					shadow.GetComponent <CircleCollider2D> ().isTrigger = true;
					shadow.GetComponent <CircleCollider2D> ().radius = 6f;
					shadow.transform.SetParent (mapHolder);
					shadow.layer = LayerMask.NameToLayer ("Ignore Raycast");
				}

				// Vector3 targetPosition = target.position;
				// targetPosition.y = transform.position.y;
				// Vector3 offset = Vector3.zero;
				// offset.x = targetPosition.x - transform.position.x;
				// offset.y = targetPosition.y - transform.position.y;
				// sqrLen =  offset.sqrMagnitude;

				// if (sqrLen < closeDistance)
				// {
				// 	FindPlayer = true;
				// }
				// else
				// {
				// 	FindPlayer = false;
				// }

				transform.position = Vector3.MoveTowards (transform.position, target.position, speed * Time.deltaTime);

				if (!FindPlayer)
				{
					transform.Find ("Sprite").localPosition = Vector3.zero;
				}
				else
				{
					if (GameController.instance.playerFear > player.high_fear)
					{
						if (transform.localScale.x < maxScale)
						{
							transform.localScale += Vector3.one * GameController.instance.playerFear * reverseMul * Time.deltaTime * scaleSpeed;
						}
					}
					else if (GameController.instance.playerFear > player.mid_fear)
					{
						render.color = Color.Lerp (render.color, halfClear, (1f - GameController.instance.playerFear * reverseMul) * Time.deltaTime);
					}
					else if (GameController.instance.playerFear > player.low_fear)
					{
						render.color = Color.Lerp (render.color, lowClear, (1f - GameController.instance.playerFear * reverseMul) * Time.deltaTime);
					}
					else if (GameController.instance.playerFear >= player.min_fear)
					{
						render.color = Color.Lerp (render.color, Color.clear, (1f - GameController.instance.playerFear * reverseMul) * Time.deltaTime);
					}

					// transform.position = new Vector3 (transform.position.x, Mathf.Sin(Time.time * 5f) * .2f, transform.position.z);
					if (!lastFindPlayer)
					{
						angle = 0;
						rad = 0;
					}
					angle += Time.deltaTime;
					if (rad < maxRad)
					{
						rad += Time.deltaTime * radSpeed;
					}
					transform.Find ("Sprite").localPosition = new Vector3 (Mathf.Sin (angle) * rad, Mathf.Cos (angle) * rad, 0);
				}
			}

			if (shadow != null)
			{
				shadow.transform.position = transform.position;
			}
		}

		void LateUpdate ()
		{
			lastFindPlayer = FindPlayer;

			if (transform.localScale.x < 0.1 || render.color.a < 0.1)
			{
				player.RemoveMonster (gameObject, shadow);
				Destroy (shadow);
				// Destroy (gameObject);

				// recycle gameobjects
				map.monsters.Add (gameObject);
				gameObject.transform.SetParent (map.poolHolder);
				gameObject.SetActive (false);
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Player"))
			{
				FindPlayer = true;
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Player"))
			{
				FindPlayer = false;
			}
		}
	}
}
