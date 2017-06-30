using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
	public class PlayerController : MonoBehaviour {

		// position
		private bool is_up;
		private bool is_up_last_frame;
		private bool is_on_ground;
		private bool is_in_sky;
		private bool is_going_down;
		private float position_y;
		private float position_y_last_frame;
		public float up_speed;
		private float max_up_speed;
		public float down_speed;
		private float gravity;
		public float right_speed;
		private float max_y;
		private float min_y;
		private float max_x;
		private float min_x;
		private Vector3 initial_position;

		// animation
		private bool is_attacking;
		private bool is_attacking_last_frame;
		private bool is_killed;
		private bool is_dead;
		private Animator animator;
		public Transform attack_timer;
		public GameObject attack_count_down;
		public Transform cd_timer;
		public GameObject cd_count_down;
		private float timer_position_offset;

		// cd time
		public float attack_time_limit;
		private float attack_start_time;
		private float attack_time;
		public float cd_time_limit;
		private float cd_start_time;
		private float cd_time;
		private bool is_attacking_triggered;

		// game controller
		private GameController game;
		private bool is_notified;
		public bool is_wasd;
		public bool is_arrows;

		// Use this for initialization
		void Start () {
			// game controller
			game = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();
			is_wasd = true;
			is_arrows = true;

			// position
			is_up = false;
			is_up_last_frame = false;
			is_on_ground = false;
			is_in_sky = false;
			is_going_down = false;
			max_up_speed = 10f;
			up_speed = 0f;
			down_speed = 0f;
			gravity = 16f;
			right_speed = 0f;
			max_y = 1.2f;
			min_y = -1.5f;
			max_x = 3f;
			min_x = -2f;
			initial_position = new Vector3 (min_x, max_y, -2f);
			transform.position = initial_position;
			position_y = transform.position.y;
			position_y_last_frame = transform.position.y;
			up_speed = max_up_speed;

			// animation
			is_attacking = false;
			is_attacking_last_frame = false;
			is_killed = false;
			is_dead = false;
			animator = GetComponent <Animator> ();
			timer_position_offset = 0.4f;

			// cd time
			attack_time_limit = Mathf.Max (2f, 3f - 0.2f * game.GetDifficultyLevel ());
			attack_start_time = Time.time;
			attack_time = Time.time;
			cd_time_limit = Mathf.Min (1.5f, 1f + 0.1f * game.GetDifficultyLevel ());
			cd_start_time = Time.time;
			cd_time = Time.time;
			is_attacking_triggered = false;

			is_notified = false;
		}

		// Update is called once per frame
		void Update () {
			if (!is_dead)
			{
				CheckControls ();
				UpdateStatus ();
				UpdateMovement ();
				UpdateAnimation ();
			}
		}

		void LateUpdate () {
			LateUpdateStatues ();
		}

		void CheckControls ()
		{
			if (is_wasd)
			{
				if (Input.GetKey ("w"))
				{
					is_up = true;
				}
				else
				{
					is_up = false;
				}

				if (Input.GetKey ("d"))
				{
					is_attacking = true;
				}
				else
				{
					is_attacking = false;
				}
			}

			if (is_arrows)
			{
				if (Input.GetKey ("up"))
				{
					is_up = true;
				}
				else
				{
					is_up = false;
				}

				if (Input.GetKey ("right"))
				{
					is_attacking = true;
				}
				else
				{
					is_attacking = false;
				}
			}

			if (is_wasd && is_arrows)
			{
				if (Input.GetKey ("w") || Input.GetKey ("up"))
				{
					is_up = true;
				}
				else
				{
					is_up = false;
				}

				if (Input.GetKey ("d") || Input.GetKey ("right"))
				{
					is_attacking = true;
				}
				else
				{
					is_attacking = false;
				}
			}

			if (Input.GetKey ("p")) // debug
			{
				game.ShowDebug (gameObject.name, string.Format ("x: {0}, y: {1}, z: {2}", transform.position.x, transform.position.y, transform.position.z));
			}
			else
			{
				game.HideDebug ();
			}
		}

		void UpdateStatus ()
		{
			position_y = transform.position.y;

			if (position_y - position_y_last_frame < 0 || is_in_sky)
			{
				is_going_down = true;
			}
			else
			{
				is_going_down = false;
			}

			if (is_going_down)
			{
				is_up = false;
			}

			// cd time
			if (!is_attacking_last_frame)
			{
				attack_start_time = Time.time;
			}
			if (is_attacking_last_frame)
			{
				cd_start_time = Time.time;
			}
			attack_time = Time.time - attack_start_time;
			if (is_attacking && attack_time >= attack_time_limit)
			{
				is_attacking = false;
			}
			cd_time = Time.time - cd_start_time;
			if (is_attacking && is_attacking_triggered && !animator.GetBool ("Attack") && cd_time <= cd_time_limit)
			{
				is_attacking = false;
			}
			if (is_attacking_triggered && !animator.GetBool ("Attack") && cd_time > cd_time_limit)
			{
				is_attacking_triggered = false;
			}

			if (is_up && !is_up_last_frame)
			{
				game.PlayJumpSound ();
			}
		}

		void UpdateMovement ()
		{
			if (is_up && !is_in_sky)
			{
				up_speed -= gravity * Time.deltaTime;
				if (up_speed < 0)
				{
					up_speed = 0;
				}
				right_speed = 0.8f;
				transform.position += Vector3.up * Time.deltaTime * up_speed + Vector3.right * Time.deltaTime * right_speed;
			}
			else if (!is_up && !is_on_ground)
			{
				down_speed += gravity * Time.deltaTime;
				right_speed = 0.8f;
				transform.position += Vector3.down * Time.deltaTime * down_speed + Vector3.right * Time.deltaTime * right_speed;
			}

			if (is_on_ground || is_in_sky)
			{
				down_speed = 0;
				up_speed = max_up_speed;
				right_speed = -0.5f;
				transform.position += Vector3.right * Time.deltaTime * right_speed;
			}

			attack_timer.position = transform.position + Vector3.up * timer_position_offset;
			cd_timer.position = transform.position + Vector3.up * timer_position_offset;

			// fix the problem that player disppears sometime by limiting the position
			if (transform.position.x > max_x + 2f)
			{
				transform.position = new Vector3 (max_x, transform.position.y, transform.position.z);
			}
			if (transform.position.x < min_x - 2f)
			{
				transform.position = new Vector3 (min_x, transform.position.y, transform.position.z);
			}
			if (transform.position.y > max_y + 2f)
			{
				transform.position = new Vector3 (transform.position.x, max_y, transform.position.z);
			}
			if (transform.position.y < min_y - 2f)
			{
				transform.position = new Vector3 (transform.position.x, min_y, transform.position.z);
			}
		}

		void UpdateAnimation ()
		{
			if (is_attacking)
			{
				// play attack animation
				// if collision detected, ... -> implement in OnCollisionEnter2D
				if (!animator.GetBool ("Attack"))
				{
					is_attacking_triggered = true;
					animator.SetBool ("Attack", true);
					// timer_animator.SetBool ("AttackCountDown", true);
				}

				// generate attack timer
				if (attack_time < attack_time_limit / 3) // 3
				{
					if (attack_timer.childCount <= 0)
					{
						for (int i = 0; i < 3; i++)
						{
							Vector3 position = attack_timer.position;
							position.x += (i-1) * 0.2f;
							position.y += (i%2-1) * 0.06f;
							Instantiate (attack_count_down, position, attack_timer.rotation, attack_timer);
						}
					}
				}
				else if (attack_time < attack_time_limit / 3 * 2) // 2
				{
					if (attack_timer.childCount >= 3)
					{
						Destroy (attack_timer.GetChild (2).gameObject);
					}
				}
				else // 1
				{
					if (attack_timer.childCount >= 2)
					{
						Destroy (attack_timer.GetChild (1).gameObject);
					}
				}
			}
			else // !is_attacking
			{
				if (animator.GetBool ("Attack"))
				{
					// end attack timer
					animator.SetBool ("Attack", false);
					// timer_animator.SetBool ("AttackCountDown", false);
					// start cd timer
					// timer_animator.SetTrigger ("CDCountDown");

					// generate cd timer
					if (cd_timer.childCount <= 0)
					{
						for (int i = 0; i < 3; i++)
						{
							Vector3 position = cd_timer.position;
							position.x += (i-1) * 0.2f;
							position.y += (i%2-1) * 0.06f;
							Instantiate (cd_count_down, position, cd_timer.rotation, cd_timer);
						}
					}
				}

				// cd timer
				if (cd_time < cd_time_limit / 3) // 3
				{
					//
				}
				else if (cd_time < cd_time_limit / 3 * 2) // 2
				{
					if (cd_timer.childCount >= 3)
					{
						Destroy (cd_timer.GetChild (2).gameObject);
					}
				}
				else if (cd_time < cd_time_limit) // 1
				{
					if (cd_timer.childCount >= 2)
					{
						Destroy (cd_timer.GetChild (1).gameObject);
					}
				}
				else // 0
				{
					foreach (Transform child in cd_timer)
					{
						Destroy (child.gameObject);
					}
				}

				if (attack_timer.childCount > 0)
				{
					foreach (Transform child in attack_timer)
					{
						Destroy (child.gameObject);
					}
				}
			}

			if (cd_time > cd_time_limit)
			{
				foreach (Transform child in cd_timer)
				{
					Destroy (child.gameObject);
				}
			}

			if (is_killed && !is_dead)
			{
				animator.ResetTrigger ("Revived");
				animator.SetTrigger ("Killed");
				is_dead = true;
				attack_timer.gameObject.SetActive (false);
				cd_timer.gameObject.SetActive (false);
			}
		}

		void LateUpdateStatues ()
		{
			is_up_last_frame = is_up;
			is_attacking_last_frame = is_attacking;
			position_y_last_frame = position_y;

			if (!is_notified && is_dead)
			{
				game.End ();
				is_notified = true;
			}
		}

		void OnCollisionEnter2D (Collision2D other)
		{
			if (other.gameObject.name == "Ground")
			{
				is_on_ground = true;
			}
			if (other.gameObject.name == "Sky")
			{
				is_in_sky = true;
			}
		}

		void OnCollisionExit2D (Collision2D other)
		{
			if (other.gameObject.name == "Ground")
			{
				is_on_ground = false;
			}
			if (other.gameObject.name == "Sky")
			{
				is_in_sky = false;
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag (Tags.ENEMY))
			{
				if (is_attacking)
				{
					other.gameObject.tag = "Finish";
					other.gameObject.GetComponent <EnemyController> ().KilledByPlayer ();
					if (gameObject.name == "Player_0")
					{
						game.AddKills0 ();
					}
					else if (gameObject.name == "Player_1")
					{
						game.AddKills1 ();
					}
					game.PlaySawSound ();
				}
				else
				{
					is_killed = true;
				}
			}
		}

		public bool GetAttacking ()
		{
			return is_attacking;
		}

		public void Init ()
		{
			is_notified = false;
			is_dead = false;
			is_killed = false;
			animator.SetTrigger ("Revived");
			transform.position = initial_position;
			position_y = transform.position.y;
			position_y_last_frame = transform.position.y;
			attack_timer.gameObject.SetActive (true);
			cd_timer.gameObject.SetActive (true);
		}

		public void SetControl (bool _is_wasd, bool _is_arrows)
		{
			is_wasd = _is_wasd;
			is_arrows = _is_arrows;
		}

		public void ChangeDifficultyLevel (int difficulty_level)
		{
			attack_time_limit = Mathf.Max (2f, 3f - 0.2f * game.GetDifficultyLevel ());
			cd_time_limit = Mathf.Min (1.5f, 1f + 0.1f * game.GetDifficultyLevel ());
		}
	}
}
