using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class PlayerController : MonoBehaviour {

		private GameController game;
		private Animator animator;
		private SpriteRenderer render;
		private Color original_color;
		private Collider2D collider;
		private Vector2 original_collider_offset;
		private Transform sight;
		private Vector3 original_sight_position;
		private float speed;
		public bool is_in_light;
		private List<bool> is_in_light_buffer;
		public bool is_dead;

		// particles
		private ParticleSystem particle;
		private Color particle_color_in_light;
		private Color particle_color_in_dark;
		private ParticleSystem.EmissionModule emission;

		// movement
		public bool is_up;
		private bool is_up_last_frame;
		public bool is_in_sky;
		public bool is_on_ground;
		private float gravity;
		public float up_speed;
		public float down_speed;
		private float max_up_speed;
		private float min_x;
		private float min_y;
		private Vector3 original_position;
		private float up_time;
		private float up_time_limit;
		private float position_y_last_frame;
		public bool is_going_down;
		public GameObject shadow;

		private bool is_using_sightline;
		private float sightline_disable_time_start;
		private float sightline_disable_time_limit_base;
		private float sightline_disable_time_limit;
		private bool is_sightline_disabled;
		private bool is_sightline_disabled_last_frame;

		// sound
		public AudioClip[] stealSmallLight;
		public AudioClip[] stealBigLight;
		private SoundManager soundManager;

		// Use this for initialization
		void Start ()
		{
			game = GameObject.Find ("Root").GetComponent <GameController> ();
			animator = GetComponent <Animator> ();
			render = GetComponent <SpriteRenderer> ();
			original_color = render.material.GetColor ("_Color");
			collider = GetComponent <Collider2D> ();
			original_collider_offset = collider.offset;
	        sight = transform.Find ("Sight");
			original_sight_position = sight.localPosition;
			speed = 3f;
			is_in_light = false;
			is_in_light_buffer = new List<bool> ();
			for (int i = 0; i < 2; i++)
			{
				is_in_light_buffer.Add (is_in_light);
			}
			is_dead = false;

			// particles
			particle = transform.Find ("Particle System").gameObject.GetComponent <ParticleSystem> ();
			particle_color_in_light = Color.black;
			particle_color_in_dark = Color.white;
			emission = particle.emission;

			// movement
			is_up = false;
			is_up_last_frame = false;
			is_in_sky = false;
			is_on_ground = true;
			gravity = 16f;
			up_speed = 0f;
			down_speed = 0f;
			max_up_speed = 8f;
			min_x = -6f;
			min_y = -3f;
			original_position = transform.position;
			up_time = Time.time;
			up_time_limit = 0.5f;
			position_y_last_frame = transform.position.y;
			is_going_down = false;
			shadow = new GameObject ("Shadow");
			shadow.transform.parent = transform.parent;
			shadow.transform.position = transform.position;
			sight.transform.parent = transform.transform;

			// sightline
			is_using_sightline = false;
			sightline_disable_time_start = Time.time;
			sightline_disable_time_limit_base = 1f;
			sightline_disable_time_limit = 0;
			is_sightline_disabled = false;

			// sound
			soundManager = GameObject.Find ("Root").GetComponent <SoundManager> ();
		}

		// Update is called once per frame
		void Update ()
		{
			UpdateState ();
			UpdateMovement ();
		}

		void LateUpdate ()
		{
			is_in_light_buffer.Add (is_in_light);
			is_in_light_buffer.Remove (is_in_light_buffer[0]);
			is_up_last_frame = is_up;
			position_y_last_frame = transform.position.y;
			is_sightline_disabled_last_frame = is_sightline_disabled;
			if (sightline_disable_time_limit > 0)
			{
				sightline_disable_time_limit -= Time.deltaTime;
			}
			if (sightline_disable_time_limit < 0)
			{
				sightline_disable_time_limit = 0;
			}
			shadow.transform.position = transform.position;
		}

		public void Move (float x)
		{
			transform.position += Vector3.right * x * Time.deltaTime * speed;

			if (transform.position.x < min_x)
			{
				transform.position = new Vector3 (min_x, transform.position.y, transform.position.z);
			}
			if (transform.position.y < min_y)
			{
				transform.position = new Vector3 (transform.position.x, min_y, transform.position.z);
			}
		}

		void UpdateState ()
		{
			if (GetInLight ())
			{
				render.material.SetColor ("_Color", original_color);
				particle.startColor = particle_color_in_light;
			}
			else
			{
				render.material.SetColor ("_Color", Color.black);
				particle.startColor = particle_color_in_dark;
			}

			if (is_dead)
			{
				particle.startColor = Color.black;
				// particle.startSize = 3f;
				particle.startLifetime = 3f;
				particle.startSpeed = 1f;
				emission.rate = 200f;
				render.material.SetColor ("_Color", new Color32 (0, 0, 0, 0));
			}

			if (is_using_sightline)
			{
				if (is_sightline_disabled)
				{
					if (!is_sightline_disabled_last_frame)
					{
						sightline_disable_time_start = Time.time;
						if (sight.gameObject.activeSelf)
						{
							sight.gameObject.SetActive (false);
						}
					}
					if (!sight.gameObject.activeSelf && Time.time - sightline_disable_time_start >= sightline_disable_time_limit)
					{
						is_sightline_disabled = false;
					}
				}
				else
				{
					if (!sight.gameObject.activeSelf)
					{
						sight.gameObject.SetActive (true);
					}
				}
			}
			else
			{
				if (sight.gameObject.activeSelf)
				{
					sight.gameObject.SetActive (false);
				}
			}

			if (transform.position.y - position_y_last_frame < 0 || is_in_sky)
			{
				is_going_down = true;
			}

			if (transform.position.y <= min_y)
			{
				is_on_ground = true;
			}
			else
			{
				is_on_ground = false;
			}

			if (is_on_ground)
			{
				is_going_down = false;
			}

			if (is_going_down)
			{
				is_up = false;
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
				transform.position += Vector3.up * Time.deltaTime * up_speed;
			}
			else if (!is_up && !is_on_ground)
			{
				down_speed += gravity * Time.deltaTime;
				transform.position += Vector3.down * Time.deltaTime * down_speed;
			}

			if (is_on_ground)
			{
				down_speed = 0;
				up_speed = max_up_speed;
			}

			if (is_in_sky)
			{
				down_speed = 0;
				up_speed = 0;
			}
		}

		public void SetInLight (bool _is_in_light)
		{
			is_in_light = _is_in_light;
			// visual clue that the player is in light or in dark
			// print (Time.time + " " + is_in_light);
		}

		public bool GetInLight ()
		{
			return is_in_light_buffer[0] && is_in_light_buffer[1] && is_in_light;
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag (Tags.DARK))
			{
				// print ("OnTriggerEnter2D DARK");
				if (GetInLight())
				{
					SetInLight (false);
				}
			}
			if (other.gameObject.CompareTag (Tags.OBJECTIVE_SMALL))
			{
				game.AddScore (Tags.OBJECTIVE_SMALL);
				Destroy (other.gameObject);
				game.LightUp ();
				soundManager.RandomizeEffect (stealSmallLight);
			}
			if (other.gameObject.CompareTag (Tags.OBJECTIVE))
			{
				game.AddScore (Tags.OBJECTIVE);
				Destroy (other.gameObject);
				game.LightUp ();
				soundManager.RandomizeEffect (stealBigLight);
			}
			if (other.gameObject.name == "Ground")
			{
				is_on_ground = true;
			}
			if (other.gameObject.name == "Sky")
			{
				is_in_sky = true;
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (other.CompareTag (Tags.DARK))
			{
				// print ("OnTriggerExit2D DARK");
				if (!GetInLight())
				{
					SetInLight (true);
				}
			}
			if (other.gameObject.name == "Ground")
			{
				is_on_ground = false;
			}
			if (other.gameObject.name == "Sky")
			{
				is_in_sky = false;
			}
		}

		void OnTriggerStay2D (Collider2D other)
		{
			// initial check
			if (other.CompareTag (Tags.DARK))
			{
				if (GetInLight())
				{
					SetInLight (false);
				}
			}

			// recorrect
			if (other.CompareTag (Tags.LIGHT))
			{
				if (!GetInLight())
				{
					SetInLight (true);
				}
			}
		}

		public void Run ()
		{
			if (!animator.GetBool ("IsRunning"))
			{
				animator.SetBool ("IsRunning", true);
			}
			/*
			if (sight.localPosition.x < original_sight_position.x + 0.2f)
			{
				sight.localPosition += Vector3.right * Time.deltaTime * 1f;
			}
			if (sight.localPosition.y > original_sight_position.y - 0.2f)
			{
				sight.localPosition += Vector3.down * Time.deltaTime * 1f;
			}
			*/
		}

		public void Stop ()
		{
			if (animator.GetBool ("IsRunning"))
			{
				animator.SetBool ("IsRunning", false);
			}
			/*
			if (sight.localPosition.x > original_sight_position.x)
			{
				sight.localPosition += Vector3.left * Time.deltaTime * 1f;
			}
			if (sight.localPosition.y < original_sight_position.y)
			{
				sight.localPosition += Vector3.up * Time.deltaTime * 1f;
			}
			*/
		}

		public void Hide ()
		{
			if (!animator.GetBool ("IsHiding"))
			{
				animator.SetBool ("IsHiding", true);
				// collider.offset += Vector2.down;
				// sight.localPosition += Vector3.down;
			}
			if (collider.offset.y > original_collider_offset.y - 1f)
			{
				collider.offset += Vector2.down * Time.deltaTime * 8f;
			}
			if (sight.localPosition.y > original_sight_position.y - 1f)
			{
				sight.localPosition += Vector3.down * Time.deltaTime * 8f;
			}
		}

		public void Stand ()
		{
			if (animator.GetBool ("IsHiding"))
			{
				animator.SetBool ("IsHiding", false);
				// collider.offset = original_collider_offset;
				// sight.localPosition = original_sight_position;
			}
			if (collider.offset.y < original_collider_offset.y)
			{
				collider.offset += Vector2.up * Time.deltaTime * 5f;
			}
			if (sight.localPosition.y < original_sight_position.y)
			{
				sight.localPosition += Vector3.up * Time.deltaTime * 5f;
			}
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

		public void SetUp ()
		{
			if (!is_going_down)
			{
				is_up = true;
			}
			if (is_going_down)
			{
				is_up = false;
			}
			// is_up = true;
		}

		public void SetDown ()
		{
			is_up = false;
		}

		public void SetDie ()
		{
			is_dead = true;
		}

		public void Init ()
		{
			is_in_light = false;
			for (int i = 0; i < 2; i++)
			{
				is_in_light_buffer.Remove (is_in_light_buffer[0]);
				is_in_light_buffer.Add (is_in_light);
			}
			is_dead = false;

			is_up = false;
			is_in_sky = false;
			is_on_ground = true;
			gravity = 16f;
			up_speed = 0f;
			down_speed = 0f;
			transform.position = new Vector3 (game.GetPlayerRevivePositionX(), original_position.y, original_position.z);

			particle.startColor = Color.white;
			// particle.startSize = 3f;
			particle.startLifetime = 1f;
			particle.startSpeed = 0.1f;
			emission.rate = 10f;
			render.material.SetColor ("_Color", Color.black);
		}

		public void SetSightline ()
		{
			is_using_sightline = true;
		}

		public void UnsetSightline ()
		{
			is_using_sightline = false;
		}

		public void LightUp ()
		{
			// sightline
			is_sightline_disabled = true;
			sightline_disable_time_limit += sightline_disable_time_limit_base;
		}

		public float GetMinX ()
		{
			return min_x;
		}
	}
}
