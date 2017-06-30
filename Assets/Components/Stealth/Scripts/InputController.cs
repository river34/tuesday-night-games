using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class InputController : MonoBehaviour {

		public SightController sight;
		public PlayerController player;
		public BackgroundController background;

		private Camera camera;
		private GameController game;
		private float max_angle;
		private float min_angle;
		private bool is_right;
		private bool is_right_last_frame;
		private float speed;
		private bool is_using_mouse;

		// Use this for initialization
		void Start () {
			max_angle = 45f;
			min_angle = 0f;
			is_right = true;
			is_right_last_frame = true;
			speed = 6f;
			game = GetComponent <GameController> ();
			camera = GetComponentInChildren <Camera> ();
			is_using_mouse = false;
		}

		// Update is called once per frame
		void Update ()
		{
			if (Input.GetKey ("h"))
			{
				game.NotUseSightline ();
			}
			else if (Input.GetKey ("j"))
			{
				game.UseSightline ();
			}

			if (game.GetState () == States.TITLE)
			{
				if (Input.anyKey)
				{
					// move
					game.StartGame ();
				}
				return;
			}

			if (game.GetState () == States.END)
			{
				if (Input.GetKey ("space"))
				{
					// move
					game.Restart ();
				}
				return;
			}

			if (game.GetState () == States.TUTORIAL)
			{
				if (Input.GetKey ("space"))
				{
					// move
					game.SkipTutorial ();
				}
			}

			if (game.GetState () == States.TUTORIAL || game.GetState () == States.GAME)
			{
				if (Input.GetKey ("w") || Input.GetKey ("up") || Input.GetKey ("space"))
				{
					player.Stand ();
					player.SetUp ();
				}
				else
				{
					player.SetDown ();
				}

				if (Input.GetKey ("a") || Input.GetKey ("left"))
				{
					// player.SetDown ();
					// set direction to left
					is_right = false;
					// start running animation
					player.Run ();
					// move
					player.Stand ();
					player.Move (-1f);
					background.SetMoving (true, false);
					// level.SetMoving (true, false);
				}
				else if (Input.GetKey ("d") || Input.GetKey ("right"))
				{
					// player.SetDown ();
					// set direction to right
					is_right = true;
					// start running animation
					player.Run ();
					// move
					player.Stand ();
					player.Move (1f);
					background.SetMoving (true, true);
					// level.SetMoving (true, true);
				}
				else
				{
					// stop running animation
					player.Stop ();
					background.SetMoving (false, false);
					// level.SetMoving (false, false);

					if (Input.GetKey ("s") || Input.GetKey ("down"))
					{
						player.Hide ();
					}
					else
					{
						player.Stand ();
					}
				}

				if (is_using_mouse) // Input.GetMouseButton (0) - mouse left button down
				{
					Vector3 position = camera.WorldToScreenPoint (sight.GetPosition ());
					Vector3 direction = Input.mousePosition - position;
					float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;

					/*
					if (angle > max_angle)
					{
						angle = max_angle;
					}
					else if (angle < min_angle)
					{
						angle = min_angle;
					}
					*/

					sight.Rotate (angle);
				}
			}

			/*
			if (is_right_last_frame != is_right)
			{
				// sight.Flip ();
				player.Flip (is_right);
			}
			*/
			player.Flip (is_right);
		}

		void LateUpdate ()
		{
			is_right_last_frame = is_right;
		}

		public void Init ()
		{
			is_right = true;
			is_right_last_frame = true;
		}

		public void SetSightline ()
		{
			is_using_mouse = true;
		}

		public void UnsetSightline ()
		{
			is_using_mouse = false;
		}
	}
}
