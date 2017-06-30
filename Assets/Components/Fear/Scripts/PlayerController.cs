using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fear
{
	public class PlayerController : MonoBehaviour {

		// camera
		private Camera camera;

		// map
		private Map map;
		public bool map_up;
		public bool map_right;
		public bool map_bottom;
		public bool map_left;
		public float maxView = 15f;

		// strength
		public float strength;
		private int min_strength = 0;
		private int max_strength = 1000;
		private int strengthGain = 300;
		private int strengthLoss = 5;

		// fear
		public float fear;
		public int min_fear = 0;
		public int low_fear = 100;
		public int mid_fear = 300;
		public int high_fear = 600;
		public int max_fear = 1000;
		private int fearGain = 5;
		private int fearLoss = 5;
		private int courageGain = 100;

		// position
		private Vector3 last_position;

		// input
		public float speed;
		// public float jump_speed;
		public float walk_speed;
		public float run_speed;
		// private float up_speed = 0;
		// private float down_speed = 0;
		// private float gravity = 10f;
		private bool is_moving;
		// private bool is_running;
		private bool is_left;
		private bool is_up;
		private List<GameObject> monsters = new List<GameObject>();
		private List<GameObject> shadows = new List<GameObject>();

		// quest
		private QuestManager questManager;

		// sprite
		private Transform sprite;
		private SpriteRenderer render;
		private Animator animator;
		private Color halfClear;

		// completes
		// public GameObject complete;
		// private List<GameObject> completes = new List<GameObject>();

		// navigation
		private GameObject UI_Nav;
		private RectTransform UI_Arrow;
		private List<string> navList = new List<string>();
		private Vector2 thisViewpoint;
		private Vector3 thisViewpoint3D;
		private RectTransform canvasRect;
		private float canvasWidth;
		private float canvasHeight;
		// public GameObject foot;
		// private LineRenderer line;

		private float reverseMul = 0.001f;
		// int mask = (1 << 9);

		// sound effects
		private SoundManager soundManager;
		public AudioClip[] footstep;
		public AudioClip[] pickUpStrength;
		public AudioClip[] pickUpCourage;
		public AudioClip[] meetWisdom;
		public AudioClip[] meetSpirit;

		void Awake ()
		{
			camera = Camera.main;
			sprite = transform.Find ("Sprite");
			animator = sprite.GetComponent <Animator>();
			render = sprite.GetComponent <SpriteRenderer>();
			halfClear = Color.yellow;
			halfClear.a = 0.2f;
			questManager = GameController.instance.questManager;
			UI_Nav = GameController.instance.UI_Nav;
			UI_Arrow = UI_Nav.transform.Find ("Arrow").gameObject.GetComponent <RectTransform> ();
			canvasRect = UI_Nav.GetComponent <RectTransform> ();
			canvasWidth = canvasRect.rect.width;
			canvasHeight = canvasRect.rect.height;
			navList.Add ("WisdomTree");
			navList.Add ("Courage");
			navList.Add ("SpiritTree");
		}

		void Start ()
		{
			soundManager = GameController.instance.soundManager;
			strength = GameController.instance.playerStrength;
			fear = GameController.instance.playerFear;
			last_position = transform.position;
			render.color = Color.Lerp (halfClear, Color.yellow, strength * reverseMul);
			thisViewpoint = camera.WorldToViewportPoint (transform.position);
			thisViewpoint3D = new Vector3 (thisViewpoint.x * canvasWidth, thisViewpoint.y * canvasHeight, 0);

			// line = foot.GetComponent <LineRenderer> ();
			// line.SetVertexCount (2);
			// line.useWorldSpace = false;
		}

		// Update is called once per frame
		void Update () {

			UpdateInput ();
			UpdateMap ();
			UpdateStatus ();
			UpdateNavigate ();
		}

		void LateUpdate ()
		{
			if (last_position != transform.position)
			{
				is_moving = true;
				animator.SetBool ("IsWalking", true);
				animator.SetBool ("IsUp", true);
				if (transform.position.x <= last_position.x)
				{
					is_left = true;
				}
				else
				{
					is_left = false;
				}
				if (transform.position.y >= last_position.y)
				{
					is_up = true;
					// animator.SetBool ("IsUp", true);
				}
				else
				{
					is_up = false;
					// animator.SetBool ("IsUp", false);
				}
			}
			else
			{
				is_moving = false;
				animator.SetBool ("IsWalking", false);
				is_left = false;
				is_up = false;
			}

			last_position = transform.position;

			if (is_moving && is_left)
			{
				sprite.localScale = new Vector3 (1, 1, 1);
			}
			else if (is_moving && !is_left)
			{
				sprite.localScale = new Vector3 (-1, 1, 1);
			}

			if (is_moving && is_up)
			{

			}
			else if (is_moving && !is_up)
			{

			}

			// if (is_moving)
			// {
			// 	soundManager.RandomizeEffect (footstep);
			// }

			CheckIfGameOver ();

			if (questManager != null && questManager.quests.Count > 0)
			{
				foreach (Quest quest in questManager.quests)
				{
					questManager.CheckForComplete (quest.id, this);
				}
			}
		}

		// reach border of the current map
		void ReachBoarder ()	// up - 0, right - 1, bottom - 2, left - 3, none - -1
		{
			if (transform.position.y > map.up - maxView)
			{
				map_up = true;
			}
			else
			{
				map_up = false;
			}
			if (transform.position.x > map.right - maxView)
			{
				map_right = true;
			}
			else
			{
				map_right = false;
			}
			if (transform.position.y < map.bottom + 4.5f)
			{
				map_bottom = true;
			}
			else
			{
				map_bottom = false;
			}
			if (transform.position.x < map.left + maxView)
			{
				map_left = true;
			}
			else
			{
				map_left = false;
			}
		}

		void UpdateInput ()
		{
			speed = walk_speed;
			// is_running = false;
			Vector3 movement = Vector3.zero;

			// keyboard input
			if (Input.GetKey("left shift") || Input.GetKey("right shift") || Input.GetKey("space"))
			{
				speed = run_speed;
				// is_running = true;
			}

			if (Input.GetKey("w") || Input.GetKey("up"))
			{
				movement += Vector3.up;
			}
			else if (Input.GetKey("s") || Input.GetKey("down"))
			{
				movement += Vector3.down;
			}

			if (Input.GetKey("a") || Input.GetKey("left"))
			{
				movement += Vector3.left;
			}
			else if (Input.GetKey("d") || Input.GetKey("right"))
			{
				movement += Vector3.right;
			}

			/*
			// jump
			if (Input.GetKey("space"))
			{
				if (transform.position.y <= 0)
				{
					up_speed = jump_speed;
				}
			}

			if (up_speed > 0)
			{
				up_speed -= gravity * Time.deltaTime;
				transform.position += Vector3.up * up_speed * Time.deltaTime;
			}
			else
			{
				up_speed = 0;
			}

			if (up_speed == 0 && transform.position.y > 0)
			{
				down_speed += gravity * Time.deltaTime;
				transform.position += Vector3.down * down_speed * Time.deltaTime;
			}

			if (down_speed > 0 && transform.position.y <= 0)
			{
				down_speed = 0;
				transform.position = new Vector3 (transform.position.x, 0, transform.position.z);
			}
			*/

			// mouse input
			if (Input.GetMouseButton (1))
			{
				speed = run_speed;
				// is_running = true;
			}

			if (Input.GetMouseButton (0))
			{
				// Ray camRay = camera.ScreenPointToRay (Input.mousePosition);
	            // RaycastHit floorHit;
	            // if (Physics.Raycast (camRay, out floorHit, 100, mask))
	            // {
	            //     Vector3 position = floorHit.point - transform.position;
	            //     position.z = 0f;
				// 	movement = position.normalized;
	            // }

				// 2D for webgl
				Vector3 screenPoint = Input.mousePosition;
				movement = screenPoint - thisViewpoint3D;
				// Debug.Log (screenPoint + ", " + thisViewpoint3D);
				movement = movement.normalized;
			}

			transform.position += movement * speed * Time.deltaTime;
		}

		void UpdateMap ()
		{
			if (map == null)
			{
				map = GameController.instance.FindMap (transform.position.x, transform.position.y);
			}

			if (map == null)
			{
				return;
			}

			ReachBoarder();

			if (map_up)
			{
				GameController.instance.GenerateMap (map.left + GameController.instance.map_width/2, map.up + GameController.instance.map_height/2);
			}
			if (map_right)
			{
				GameController.instance.GenerateMap (map.right + GameController.instance.map_width/2, map.bottom + GameController.instance.map_height/2);
			}
			if (map_bottom)
			{
				GameController.instance.GenerateMap (map.left + GameController.instance.map_width/2, map.bottom - GameController.instance.map_height/2);
			}
			if (map_left)
			{
				GameController.instance.GenerateMap (map.left - GameController.instance.map_width/2, map.bottom + GameController.instance.map_height/2);
			}

			if (map_up && map_right)
			{
				GameController.instance.GenerateMap (map.right + GameController.instance.map_width/2, map.up + GameController.instance.map_height/2);
			}
			if (map_right && map_bottom)
			{
				GameController.instance.GenerateMap (map.right + GameController.instance.map_width/2, map.bottom - GameController.instance.map_height/2);
			}
			if (map_bottom && map_left)
			{
				GameController.instance.GenerateMap (map.left - GameController.instance.map_width/2, map.bottom - GameController.instance.map_height/2);
			}
			if (map_left && map_up)
			{
				GameController.instance.GenerateMap (map.left - GameController.instance.map_width/2, map.up + GameController.instance.map_height/2);
			}

			if (transform.position.y > map.up || transform.position.y < map.bottom ||
				transform.position.x > map.right || transform.position.x < map.left)
			{
				map = GameController.instance.FindMap (transform.position.x, transform.position.y);
			}
		}

		void UpdateStatus ()
		{
			if (fear > mid_fear && shadows.Count > 0)
			{
				soundManager.PlayPanic ();
			}
			else
			{
				soundManager.StopPanic ();
			}

			// Debug.Log (monsters.Count + ", " + shadows.Count + ", " + strength + ", " + fear);
			if (is_moving)
			{
				if (monsters.Count > 0 && fear > high_fear)
				{
					LossStrength (strengthLoss * (monsters.Count * 4 + 1) * Time.deltaTime);
				}
				else if (monsters.Count > 0 && fear > mid_fear)
				{
					LossStrength (strengthLoss * (monsters.Count * 4 + 1) * Time.deltaTime);
				}
				else if (monsters.Count > 0 && fear > low_fear)
				{
					LossStrength (strengthLoss * (monsters.Count * 3 + 1) * Time.deltaTime);
				}
				else if (monsters.Count > 0)
				{
					LossStrength (strengthLoss * (monsters.Count * 2 + 1) * Time.deltaTime);
				}
				else
				{
					LossStrength (strengthLoss * Time.deltaTime);
				}
			}
			else
			{
				if (monsters.Count > 0 && fear > high_fear)
				{
					LossStrength (strengthLoss * monsters.Count * 4 * Time.deltaTime);
				}
				else if (monsters.Count > 0 && fear > mid_fear)
				{
					LossStrength (strengthLoss * monsters.Count * 4 * Time.deltaTime);
				}
				else if (monsters.Count > 0 && fear > low_fear)
				{
					LossStrength (strengthLoss * monsters.Count * 3 * Time.deltaTime);
				}
				else if (monsters.Count > 0)
				{
					LossStrength (strengthLoss * monsters.Count * 2 * Time.deltaTime);
				}
			}

			/*
			if (is_running)
			{
				if (monsters.Count > 0)
				{
					GainFear (fearGain * monsters.Count * Time.deltaTime);
				}
			}
			else
			{
				if (monsters.Count > 0)
				{
					LossFear (fearLoss * monsters.Count * Time.deltaTime);
				}
			}
			*/

			if (shadows.Count > 0)
			{
				float fear = 0;
				fear -= fearLoss * monsters.Count;
				fear += fearGain * (shadows.Count - monsters.Count);
				// Debug.Log (fearLoss * monsters.Count + ", " + fearGain * (shadows.Count - monsters.Count));
				if (fear > 0)
				{
					GainFear (fear * Time.deltaTime);
				}
				else
				{
					LossFear (-fear * Time.deltaTime);
				}
			}
		}

		public void GainStrength (int gain)
		{
			strength += gain;

			if (strength > max_strength)
			{
				strength = max_strength;
			}

			render.color = Color.Lerp (Color.yellow, halfClear, strength * reverseMul);

			GameController.instance.playerStrength = strength;
		}

		public void LossStrength (float loss)
		{
			strength -= loss;
			if (strength <= min_strength)
			{
				strength = min_strength;
			}
			// CheckIfGameOver ();

			render.color = Color.Lerp (halfClear, Color.yellow, strength * reverseMul);

			GameController.instance.playerStrength = strength;
		}

		public void GainCourage (float gain)
		{
			LossFear (gain);
		}

		public void GainFear (float gain)
		{
			fear += gain;
			if (fear > max_fear)
			{
				fear = max_fear;
			}
			GameController.instance.playerFear = fear;
		}

		public void LossFear (float loss)
		{
			fear -= loss;
			if (fear < min_fear)
			{
				fear = min_fear;
			}
			GameController.instance.playerFear = fear;
		}

		private void CheckIfGameOver ()
		{
			if (strength <= min_strength)
			{
				GameController.instance.GameOver ();
			}
		}

		void OnDisable ()
		{
			if (soundManager != null)
			{
				soundManager.StopPanic ();
			}
		}

		void OnDestroy ()
		{
			if (soundManager != null)
			{
				soundManager.StopPanic ();
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Strength"))
			{
				soundManager.RandomizeEffect (pickUpStrength);
				GainStrength (strengthGain);
				Destroy (other.gameObject);
			}
			if (other.gameObject.CompareTag ("Courage"))
			{
				soundManager.RandomizeEffect (pickUpCourage);
				GainCourage (courageGain);
				Destroy (other.gameObject);
			}
			if (other.gameObject.CompareTag ("Monster"))
			{
				monsters.Add (other.gameObject);
			}
			if (other.gameObject.CompareTag ("Shadow"))
			{
				shadows.Add (other.gameObject);
			}
			if (other.gameObject.CompareTag ("WisdomTree"))
			{
				soundManager.RandomizeEffect (meetWisdom);
			}
			if (other.gameObject.CompareTag ("SpiritTree"))
			{
				soundManager.RandomizeEffect (meetSpirit);
			}

			if (questManager != null && questManager.quests.Count > 0)
			{
				foreach (Quest quest in questManager.quests)
				{
					if (quest.isFinished)
					{
						continue;
					}
					if (!quest.isOpen)
					{
						continue;
					}
					if (quest.tag != null && other.gameObject.CompareTag (quest.tag))
					{
						questManager.quests[quest.id].objects.Add (other.gameObject);
						// Debug.Log (quest.id + " " + questManager.quests[quest.id].objects.Count);
						// bool completeNew = questManager.CheckForComplete (quest.id, this);
						// if (completeNew)
						// {
						// 	GameObject completeObject = Instantiate (complete, transform, false);
						// 	completeObject.AddComponent <CompleteController> ();
						// 	completes.Add (completeObject);
						// }
					}
				}
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Monster"))
			{
				monsters.Remove (other.gameObject);
			}
			if (other.gameObject.CompareTag ("Shadow"))
			{
				shadows.Remove (other.gameObject);
			}
		}

		public void RemoveMonster (GameObject monster, GameObject shadow)
		{
			monsters.Remove (monster);
			shadows.Remove (shadow);
		}

		void UpdateNavigate ()
		{
			GameObject[] gameObjects = null;

			if (questManager != null && questManager.quests.Count > 0)
			{
				foreach (Quest quest in questManager.quests)
				{
					if (quest.isFinished)
					{
						continue;
					}
					if (!quest.isOpen)
					{
						continue;
					}
					// find the nearest object
					if (quest.tag != null)
					{
						if (!navList.Contains (quest.tag))
						{
							break;
						}
						gameObjects = GameObject.FindGameObjectsWithTag (quest.tag);
						break;
					}
				}
			}

			if (gameObjects != null && gameObjects.Length > 0)
			{
				float distance = -1;
				GameObject nearest = null;
				foreach (GameObject gameObject in gameObjects)
				{
					Vector3 offset = gameObject.transform.position - transform.position;
					float newDistance = offset.sqrMagnitude;
					if (distance == -1 || nearest == null)
					{
						distance = newDistance;
						nearest = gameObject;
					}
					else
					{
						if (newDistance < distance)
						{
							distance = newDistance;
							nearest = gameObject;
						}
					}
				}

				if (nearest != null)
				{
					if (!UI_Nav.activeSelf)
						UI_Nav.SetActive (true);

					Vector2 targetViewpoint = camera.WorldToViewportPoint (nearest.transform.position);
					float angle = Mathf.Atan2 (targetViewpoint.y - thisViewpoint.y, targetViewpoint.x - thisViewpoint.x) * Mathf.Rad2Deg;
					UI_Arrow.localRotation = Quaternion.AngleAxis (angle, Vector3.forward);

					// footstep lead to the objects
					// if (!foot.activeSelf)
					// 	foot.SetActive (true);
					// Vector3 Start = Vector3.zero;
					// Vector3 End = nearest.transform.position - transform.position;
					// line.SetPosition (0, Start);
					// line.SetPosition (1, End);
					// performce is too bad
				}
				else
				{
					// if (foot.activeSelf)
					// 	foot.SetActive (false);
					if (UI_Nav.activeSelf)
						UI_Nav.SetActive (false);
				}
			}
			else
			{
				if (UI_Nav.activeSelf)
					UI_Nav.SetActive (false);
			}
		}
	}
}
