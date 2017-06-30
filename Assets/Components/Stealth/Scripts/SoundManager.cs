using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class SoundManager : MonoBehaviour {

		public AudioSource background;
		public AudioSource effect;

		private float lowPitchRange = 0.8f;
		private float highPitchRange = 1f;

		public void PlayBackground ()
		{
			if (!background.isPlaying)
			{
				background.Play ();
			}
		}

		public void StopBackground ()
		{
			if (background.isPlaying)
			{
				background.Stop ();
			}
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
