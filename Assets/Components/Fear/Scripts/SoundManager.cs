using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fear
{
	public class SoundManager : MonoBehaviour {

		public AudioSource background;
		public AudioSource panic;
		public AudioSource ambience;
		public AudioSource effect;
		public AudioSource voice;

		private float lowPitchRange = 0.8f;
		private float highPitchRange = 1f;

		public void PlayAmbience ()
		{
			if (!ambience.isPlaying)
			{
				ambience.Play ();
			}
		}

		public void StopAmbience  ()
		{
			if (ambience.isPlaying)
			{
				ambience.Stop ();
			}
		}

		public void PlayBackground (AudioClip clip)
		{
			background.clip = clip;
			background.Play ();
		}

		public void StopBackground ()
		{
			background.Stop ();
		}

		public void PlayPanic ()
		{
			if (!panic.isPlaying)
			{
				panic.Play ();
			}
		}

		public void StopPanic ()
		{
			if (panic.isPlaying)
			{
				panic.Stop ();
			}
		}

		public void PlayVoice (AudioClip clip)
		{
			voice.clip = clip;
	        voice.PlayDelayed (1f);
		}

		public void RandomizeEffect (params AudioClip[] clips)
	    {
	        int randomIndex = Random.Range (0, clips.Length);
	        float randomPitch = Random.Range (lowPitchRange, highPitchRange);
	        effect.pitch = randomPitch;
	        effect.clip = clips[randomIndex];
	        effect.Play ();
	    }
	}
}
