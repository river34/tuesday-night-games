using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class GuardController : MonoBehaviour {

		public bool is_player_in_sight;
		public bool is_in_sight;
		public float visiable_range;

		private Animator animator;
		private SpriteRenderer render;
		private Color original_color;
		private Transform player;
		private GameController game;
		private SightController sight;
		private Vector3 original_sight_rotation;
		private float speed;
		private float original_x;
		private bool is_distractor_in_range;
		public bool is_walking;
		public bool is_right;
		public bool is_right_last_frame;
		public bool is_looking;
		private float x_this_frame;
		private float x_last_frame;
		public float movement_speed;
		public float movement_range;
		private float destroy_delay;
		private float start_time;

		// Use this for initialization
		void Start () {
			animator = GetComponent <Animator> ();
			render = GetComponent <SpriteRenderer> ();
			original_color = render.color;
			is_player_in_sight = false;
			is_in_sight = false;
			visiable_range = 8f;
			game = GameObject.Find ("Root").GetComponent <GameController> ();
			sight = transform.Find ("Sight").gameObject.GetComponent <SightController> ();
			original_sight_rotation = transform.Find ("Sight").rotation.eulerAngles;
			speed = Random.Range (10f, 12f);
			original_x = transform.position.x;
			is_walking = true;
			is_looking = true;
			is_right = true;
			is_right_last_frame = true;
			x_this_frame = original_x;
			x_last_frame = original_x;
			movement_speed = Random.Range (0.15f, 0.25f);
			movement_range = Random.Range (10f, 15f);
			destroy_delay = 2f;
			start_time = Time.time;
		}

		// Update is called once per frame
		void Update ()
		{
			x_this_frame = transform.position.x;

			if (game.GetState () == States.END)
			{
				return;
			}

			if (is_player_in_sight) // if see a player
			{
				// rush to the player, kill the player, end the game
				Stop ();
				// Fly ();

				if (x_this_frame > x_last_frame)
				{
					is_right = true;
				}
				else
				{
					is_right = false;
				}

				if (is_right != is_right_last_frame)
				{
					Flip ();
				}

				if (player != null)
				{
	        		// transform.position = Vector3.MoveTowards (transform.position, player.position, speed * Time.deltaTime);
				}
			}
			else if (is_distractor_in_range) // if sense a distractor
			{

			}
			else // if nothing happens
			{
				// walk around in a range (defined by collider). turn around when reach the end, stop, look up the down
				if (is_walking)
				{
					// start walking animation
					Walk ();
					x_this_frame = original_x + Mathf.Sin ((Time.time - start_time) * movement_speed) * movement_range;
					transform.position = new Vector3 (x_this_frame, transform.position.y, transform.position.z);
					if (x_this_frame > x_last_frame)
					{
						is_right = true;
					}
					else
					{
						is_right = false;
					}

					if (is_right != is_right_last_frame)
					{
						Flip ();
					}
				}
				else
				{
					// stop walking animation
					Stop ();
				}

				if (is_looking)
				{
					float angle = original_sight_rotation.z + Mathf.Sin ((Time.time - start_time - Mathf.PI/4) * movement_speed) * -90f;
					if (!is_right)
					{
						angle = -angle;
					}
					RotateSight (angle);
				}
			}
		}

		void LateUpdate ()
		{
			is_right_last_frame = is_right;
			x_last_frame = x_this_frame;
		}

		public void SetPlayerInSight (Transform _player)
		{
			is_player_in_sight = true;
			player = _player;
			// sight.Hide ();
			// gameObject.GetComponent <Collider2D> ().enabled = false;
			// Destroy (gameObject, destroy_delay);
		}

		public void UnsetPlayerInSight ()
		{
			is_player_in_sight = false;
			player = null;
		}

		public float GetSightRange ()
		{
			return visiable_range;
		}

		void RotateSight (float angle)
		{
			sight.Rotate (angle);
		}

		public void SetInSight (bool _is_in_sight)
		{
			is_in_sight = _is_in_sight;
			if (is_in_sight)
			{
				// render.material.SetColor ("_Color", Color.white);
				render.color = Color.white;
			}
			else
			{
				// render.material.SetColor ("_Color", original_color);
				render.color = original_color;
			}
		}

		public void Flip ()
		{
			Vector3 scale = gameObject.transform.localScale;
			scale.x = - scale.x;
			gameObject.transform.localScale = scale;
		}

		void Walk ()
		{
			if (!animator.GetBool ("IsWalking"))
			{
				animator.SetBool ("IsWalking", true);
			}
		}

		void Stop ()
		{
			if (animator.GetBool ("IsWalking"))
			{
				animator.SetBool ("IsWalking", false);
			}
		}

		void Fly ()
		{
			if (!animator.GetBool ("IsFlying"))
			{
				animator.SetBool ("IsFlying", true);
			}
		}

		public void SetMovementRange (float _movement_range)
		{
			movement_range = _movement_range;
		}

		public void Init ()
		{
			UnsetPlayerInSight ();
		}
	}
}
