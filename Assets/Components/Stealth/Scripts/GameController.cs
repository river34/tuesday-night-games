using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stealth
{
	public class GameController : MonoBehaviour {

		// guards
		private GameObject[] guards;
		public GameObject guard;
		private Transform guards_transform;

		// player
		private Transform player_transform;
		private PlayerController player;

		// camera
		private Transform camera;

		// score
		private int objective_score;
		private int objective_small_score;
		private int score;
		private int punish_score;

		// mask
		public SpriteRenderer mask;
		private Color objective_small_color;
		private Color objective_color;
		private Color death_color;
		private bool is_blinked;
		private Color blink_color;
		private float blink_start_time;

		// game controls
		public int difficulty_level;
		private InputController input;
		private float start_time;
		public bool is_using_sightline;
		private bool is_start;
		private bool is_start_time_set;
		private float player_revive_position_x;

		// levels
		public List <GameObject> levelcards;
		public LevelGenerator generator;
		public Transform levels_transform;
		private GameObject levelcard;
		private float levelcard_start_x;
		private float levelcard_end_x;
		private int levelcard_id;
		private bool is_respawn;
		private float player_level_offset_x;

		// UI
		public GameObject UI_title;
		public GameObject UI_score;
		public GameObject UI_end;

		// tutorial
		public Transform tutorials;
		public GameObject tutorial_text;
		private List <Transform> tutorial_texts;
		private string[] tutorial_strings;
		private float tutorial_speed;
		private float tutorial_text_start_position_x;
		private float tutorial_text_end_position_x;
		private float tutorial_close_x_start;
		private float tutorial_close_x_end;
		private float tutorial_close_speed;
		private float color_speed;
		private bool skip_tutorial;
		private bool is_tutorial_finished;

		// state machine
		public int state;
		public int highest_score;

		// sound
		private SoundManager soundManager;

		// Use this for initialization
		void Start ()
		{
			// player
			player_transform = transform.Find ("Player");
			player = player_transform.gameObject.GetComponent <PlayerController> ();

			// camera
			camera = transform.Find ("Main Camera");

			// score
			objective_score = 10;
			objective_small_score = 1;
			score = 0;
			punish_score = 20;

			// mask
			objective_small_color = new Color32 (255, 255, 255, 0);
			objective_color = new Color32 (255, 255, 255, 0); // new Color32 (255, 81, 81, 0); // FF5151FF
			death_color = new Color (0, 0, 0, 0);
			is_blinked = true;
			blink_start_time = Time.time;

			// game controls
			difficulty_level = 0;
			input = gameObject.GetComponent <InputController> ();
			start_time = Time.time;
			is_using_sightline = true;
			is_start = false;
			is_start_time_set = false;
			player_revive_position_x = player_transform.position.x;

			// levels
			levelcards = generator.GenerateLevecards ();
			levelcard_start_x = -3f;
			levelcard_end_x = 12f;
			levelcard_id = -1;
			levelcard = null;
			is_respawn = false;
			player_level_offset_x = player.GetMinX () - levelcard_start_x;

			// guards
			guards_transform = transform.Find ("Guards");

			// tutorial
			tutorials = camera.Find ("Tutorials");
			tutorial_texts = new List <Transform> ();
			tutorial_text_start_position_x = 6f;
			tutorial_text_end_position_x = -20f;
			tutorial_close_x_start = 3f;
			tutorial_close_x_end = -8f;
			tutorial_close_speed = 1.2f;
			color_speed = 0.3f;
			skip_tutorial = false;
			is_tutorial_finished = false;
			tutorial_speed = 6f;
			SetTutorial ();

			// state machine
			state = 0;
			score = 0;
			highest_score = 0;

			// sound
			soundManager = GetComponent <SoundManager> ();
		}

		// Update is called once per frame
		void Update ()
		{
			if (blink_color != null)
			{
				if (!is_blinked)
				{
					Blink (blink_color);
				}
			}

			if (is_using_sightline)
			{
				input.SetSightline ();
				player.SetSightline ();
			}
			else
			{
				input.UnsetSightline ();
				player.UnsetSightline ();
			}

			// state machine
			if (state == States.TITLE)
			{
				ShowTitle ();
				HideTutorial ();
				HideUI ();
				HideEnd ();
				// UpdateLevel ();
				// go to next stage
				if (is_start && !is_start_time_set)
				{
					start_time = Time.time;
					is_start_time_set = true;
				}
				if (is_start && Time.time - start_time > 1f)
				{
					soundManager.PlayBackground ();
					state = States.TUTORIAL;
				}
			}
			else if (state == States.TUTORIAL)
			{
				HideTitle ();
				ShowTutorial ();
				HideUI ();
				HideEnd ();
				// UpdateLevel ();
				// update tutorial
				UpdateTutorial ();
				// finish tutorial, go to next stage
				if (is_tutorial_finished)
				{
					state = States.GAME;
				}
				// skip tutorial, go to next stage
				if (skip_tutorial)
				{
					state = States.GAME;
				}
			}
			else if (state == States.GAME)
			{
				HideTitle ();
				HideTutorial ();
				ShowUI ();
				HideEnd ();
				// update ui for score
				UI_score.GetComponent <Text> ().text = "steals " + score;
				UpdateLevel ();
			}
			else if (state == States.END)
			{
				HideTitle ();
				HideTutorial ();
				HideUI ();
				ShowEnd ();
				// update ui for highest score
				UI_end.GetComponent <Text> ().text = "record " + highest_score;
			}
		}

		public void PlayerIsDiscoveredByGuard (Transform _player)
		{
			/*
			if (guards == null)
			{
				guards = GameObject.FindGameObjectsWithTag (Tags.GUARD);
			}
			if (guards!= null)
			{
				foreach (GameObject guard in guards)
				{
					guard.GetComponent <GuardController> ().SetPlayerInSight (_player);
				}
			}
			*/
			if (state == States.GAME)
			{
				player.SetDie ();
				SetPlayerRevivePositionX ();
				End ();
			}
		}

		public void AddScore (string tag)
		{
			if (tag == Tags.OBJECTIVE)
			{
				score += objective_score;
				blink_color = objective_color;
				is_blinked = false;
				blink_start_time = Time.time;
			}
			else if (tag == Tags.OBJECTIVE_SMALL)
			{
				score += objective_small_score;
				blink_color = objective_small_color;
				is_blinked = false;
				blink_start_time = Time.time;
			}
		}

		public void Blink (Color color)
		{
			float a = Mathf.Sin ((Time.time - blink_start_time) * 2);
			if (a < 0)
			{
				is_blinked = true;
				return;
			}
			color.a = a * 0.8f;
			mask.color = color;
		}

		public void End ()
		{
			soundManager.StopBackground ();
			blink_color = death_color;
			is_blinked = false;
			blink_start_time = Time.time;
			highest_score = Mathf.Max (highest_score, score);
			state = States.END;
		}

		public int GetDifficultyLevel ()
		{
			return difficulty_level;
		}

		void UpdateLevel ()
		{
			// pick one levelcard
			if (levelcard == null)
			{
				// generate new levelcard
				Vector3 position = levels_transform.position;
				position.x = levelcard_start_x;
				position.y = 0;
				position.z = -2f;

				// generate guards
				if (!is_respawn)
				{
					levelcard_id = generator.GetLevelIdByDifficultyLevel (difficulty_level);
					levelcard_end_x = levelcard_start_x + generator.GetWidth (levelcard_id);
					levelcard = Instantiate (levelcards[levelcard_id], position, levels_transform.rotation, levels_transform);
					int num_of_guards = 0;
					float guard_range = generator.GetWidth (levelcard_id) / generator.GetNumOfGuards (levelcard_id);
					while (num_of_guards < generator.GetNumOfGuards (levelcard_id))
					{
						GameObject new_guard = Instantiate (guard, levelcard.transform);
						new_guard.transform.parent = guards_transform;
						position = new_guard.transform.position;
						position.x = levelcard_start_x + guard_range * num_of_guards;
						position.y = -2.5f;
						new_guard.transform.position = position;
						new_guard.GetComponent <GuardController> ().SetMovementRange (guard_range / 2);
						num_of_guards ++;
					}
				}
				else
				{
					levelcard = Instantiate (levelcards[levelcard_id], position, levels_transform.rotation, levels_transform);
					is_respawn = false;
				}
			}
			else
			{
				float offset_x = levelcard_end_x - camera.position.x;
				if (offset_x < 20f)
				{
					levelcard_start_x = levelcard_end_x;
					levelcard = null;
					difficulty_level ++;
				}
			}
		}

		public void Restart ()
		{
			score = Mathf.Max (0, score - punish_score);
			difficulty_level = 0;

			input.Init ();
			player.Init ();

			foreach (Transform guard_transform in guards_transform)
			{
				guard_transform.GetComponent <GuardController> (). Init ();
			}

			RespawnLevel ();
			soundManager.PlayBackground ();
			state = States.GAME;
		}

		void HideTitle ()
		{
			if (UI_title.activeSelf)
			{
				UI_title.SetActive (false);
			}
		}

		void ShowTitle ()
		{
			if (!UI_title.activeSelf)
			{
				UI_title.SetActive (true);
			}
		}

		void HideUI ()
		{
			if (UI_score.activeSelf)
			{
				UI_score.SetActive (false);
			}
		}

		void ShowUI ()
		{
			if (!UI_score.activeSelf)
			{
				UI_score.SetActive (true);
			}
		}

		void HideEnd ()
		{
			if (UI_end.activeSelf)
			{
				UI_end.SetActive (false);
			}
		}

		void ShowEnd ()
		{
			if (!UI_end.activeSelf)
			{
				UI_end.SetActive (true);
			}
		}

		void HideTutorial ()
		{
			if (tutorials.gameObject.activeSelf)
			{
				tutorials.gameObject.SetActive (false);
			}
		}

		void ShowTutorial ()
		{
			if (!tutorials.gameObject.activeSelf)
			{
				tutorials.gameObject.SetActive (true);
			}
		}

		void SetTutorial ()
		{
			if (is_using_sightline)
			{
				tutorial_strings = new string[4];
				tutorial_strings[0] = "this is a story about the light stealer";
				tutorial_strings[1] = "who steals light but remains in shadow";
				tutorial_strings[2] = "use wasd or arrows to move";
				tutorial_strings[3] = "use mouse to look around";
			}
			else
			{
				tutorial_strings = new string[3];
				tutorial_strings[0] = "this is a story about the light stealer";
				tutorial_strings[1] = "who steals light but remains in shadow";
				tutorial_strings[2] = "use wasd or arrows to move";
			}

			foreach (string tutorial_string in tutorial_strings)
			{
				GameObject new_tutorial_text = Instantiate (tutorial_text, tutorials);
				tutorial_texts.Add (new_tutorial_text.transform);
				new_tutorial_text.transform.localPosition += Vector3.right * tutorial_text_start_position_x;
				new_tutorial_text.GetComponent <TextMesh> ().color = new Color (1f, 1f, 1f, 0);
				new_tutorial_text.GetComponent <TextMesh> ().text = tutorial_string;
			}
		}

		void UpdateTutorial ()
		{
			if (tutorial_texts.Count > 0)
			{
				Transform tutorial_text = tutorial_texts[0];
				if (tutorial_text.localPosition.x > tutorial_text_end_position_x)
				{
					if (tutorial_text.localPosition.x > tutorial_close_x_start)
					{
						tutorial_text.localPosition += Vector3.left * Time.deltaTime * tutorial_speed;
					}
					else if (tutorial_text.localPosition.x > tutorial_close_x_end)
					{
						tutorial_text.localPosition += Vector3.left * Time.deltaTime * tutorial_close_speed;
						Color color = tutorial_text.gameObject.GetComponent <TextMesh> ().color;
						color.a += Time.deltaTime * color_speed;
						tutorial_text.gameObject.GetComponent <TextMesh> ().color = color;
					}
					else
					{
						tutorial_text.localPosition += Vector3.left * Time.deltaTime * tutorial_speed;
						Color color = tutorial_text.gameObject.GetComponent <TextMesh> ().color;
						color.a -= Time.deltaTime * color_speed * 3;
						tutorial_text.gameObject.GetComponent <TextMesh> ().color = color;
					}
				}
				else
				{
					tutorial_texts.Remove (tutorial_text);
					Destroy (tutorial_text.gameObject);
				}
			}
			else
			{
				// Destroy (tutorials.gameObject);
				is_tutorial_finished = true;
			}
		}

		public void SkipTutorial ()
		{
			skip_tutorial = true;
		}

		public int GetState ()
		{
			return state;
		}

		public void UseSightline ()
		{
			is_using_sightline = true;
		}

		public void NotUseSightline ()
		{
			is_using_sightline = false;
		}

		public void RespawnLevel ()
		{
			// replace current level card with a new one
			Destroy (levelcard);
			levelcard = null;
			is_respawn = true;
		}

		public void LightUp ()
		{
			player.LightUp ();
		}

		public void StartGame ()
		{
			is_start = true;
		}

		public void SetPlayerRevivePositionX ()
		{
			player_revive_position_x = levelcard_start_x + player_level_offset_x;
		}

		public float GetPlayerRevivePositionX ()
		{
			return player_revive_position_x;
		}

		public float GeMinX ()
		{
			return levelcard_start_x + player_level_offset_x;
		}
	}
}
