using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
	public class SoundManager : MonoBehaviour {

		private AudioSource opening;
		private AudioSource gaming;
		private AudioSource ending;
		private AudioSource saw;
		private AudioSource jump;
		private AudioSource wind;

		private bool fade_out_opening;
		private bool fade_in_gaming;
		private float max_opening_volume;
		private float max_gaming_volume;
		private float min_volume;
		private float opening_fade_speed;
		private float gaming_fade_speed;

		// Use this for initialization
		void Start () {
			opening = transform.Find ("Opening").GetComponent <AudioSource> ();
			gaming = transform.Find ("Gaming").GetComponent <AudioSource> ();
			ending = transform.Find ("Ending").GetComponent <AudioSource> ();
			saw = transform.Find ("Saw").GetComponent <AudioSource> ();
			jump = transform.Find ("Jump").GetComponent <AudioSource> ();
			wind = transform.Find ("Wind").GetComponent <AudioSource> ();

			fade_out_opening = false;
			fade_in_gaming = false;
			max_opening_volume = 1f;
			max_gaming_volume = 0.1f;
			min_volume = 0.01f;
			opening_fade_speed = max_opening_volume;
			gaming_fade_speed = max_gaming_volume;
			gaming.volume = min_volume;
		}

		void Update ()
		{
			if (fade_out_opening)
			{
				if (opening.volume > min_volume)
				{
					fadeOut (opening, min_volume, opening_fade_speed);
				}
				else
				{
					fade_out_opening = false;
					opening.Stop ();
					opening.volume = max_opening_volume;
				}
			}

			if (fade_in_gaming)
			{
				if (gaming.volume < max_gaming_volume)
				{
					fadeIn (gaming, max_gaming_volume, gaming_fade_speed);
				}
				else
				{
					fade_in_gaming = false;
					gaming.volume = max_gaming_volume;
				}
			}
		}

		public void PlayMusic (string name)
		{
			if (name == "opening")
			{
				opening.Play ();
			}
			if (name == "gaming")
			{
				// opening.Stop ();
				fade_out_opening = true;
				gaming.Play ();
				fade_in_gaming = true;
				ending.Stop ();
			}
			if (name == "ending")
			{
				gaming.Stop ();
				ending.Play ();
			}
		}

		public void PlaySound (string name)
		{
			if (name == "saw")
			{
				if (!saw.isPlaying)
				{
					saw.Play ();
				}
			}
			if (name == "jump")
			{
				if (!jump.isPlaying)
				{
					jump.Play ();
				}
			}
			if (name == "wind")
			{
				if (!wind.isPlaying)
				{
					wind.Play ();
				}
			}
		}

		void fadeIn (AudioSource audio, float max_volume, float fade_speed)
		{
			if (audio.volume < max_volume)
			{
				audio.volume += fade_speed * Time.deltaTime;
			}
		 }

		void fadeOut (AudioSource audio, float min_volume, float fade_speed)
		{
			if (audio.volume > min_volume)
			{
				audio.volume -= fade_speed * Time.deltaTime;
			}
		}
	}
}
