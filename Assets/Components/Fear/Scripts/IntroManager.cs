using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fear
{
	public class VoiceOver {
		public int id;
		public AudioClip clip;
		public string line;
		public bool isStarted;
		public bool isFinished;
		public float length;

		public VoiceOver (int _id, AudioClip _clip, string _line)
		{
			id = _id;
			clip = _clip;
			line = _line;

			isStarted = false;
			isFinished = false;
			length = clip.length;
		}
	}

	public class IntroManager : MonoBehaviour {

		public GameObject UI_Intro;
		private Text text;
		private List<string> lines;
		private float interval = 0.25f;
		private int current;
		private float startTime;
		private float endTime;

		// sound
		private SoundManager soundManager;
		public bool useVoiceOver;
		public AudioClip[] introVO;
		public AudioClip[] outroVO;
		private List<VoiceOver> voiceOver = new List<VoiceOver> ();
		private VoiceOver currentVoice;

		void Awake ()
		{
			text = UI_Intro.transform.Find ("Text").GetComponent <Text> ();

			lines = new List<string> ();

			startTime = -1;
			endTime = 0;

			enabled = false;
		}

		public void Init (string tag)
		{
			lines.Clear ();

			if (tag == "Intro")
			{
				lines.Add ("Ima, it is time now. You are no longer a child.");
				lines.Add ("You need to prove that you are a true Kumu.");
				lines.Add ("Under the stars, you will go to the Fear Forest.");
				lines.Add ("You will discover the secret of courage and fear.");
				lines.Add ("Then, and only then, will you finally become a Kumu.");
			}
			else if (tag == "Complete")
			{
				lines.Add ("Ima, you are a true Kumu now.");
				lines.Add ("The Fear Forest has witnessed your courage.");
				lines.Add ("You already knew all the secrets a Kumu needs to know.");
				lines.Add ("It is time to go home.");
			}

			if (useVoiceOver)
			{
				voiceOver.Clear ();
				if (tag == "Intro")
				{
					if (introVO.Length == lines.Count)
					{
						for (int i = 0; i < introVO.Length; i++)
						{
							VoiceOver voice = new VoiceOver (i, introVO[i], lines[i]);
							voiceOver.Add (voice);
						}
					}
					else
					{
						useVoiceOver = false;
					}
				}
				if (tag == "Complete")
				{
					if (outroVO.Length == lines.Count)
					{
						for (int i = 0; i < outroVO.Length; i++)
						{
							VoiceOver voice = new VoiceOver (i, outroVO[i], lines[i]);
							voiceOver.Add (voice);
						}
					}
					else
					{
						useVoiceOver = false;
					}
				}
			}
		}

		public void StartLine ()
		{
			current = -1;
			startTime = Time.time;

			if (useVoiceOver)
			{
				foreach (VoiceOver voice in voiceOver)
				{
					if (!voice.isStarted && !voice.isFinished)
					{
						currentVoice = voice;
						break;
					}
				}
			}
		}

		void Update ()
		{
			if (soundManager == null)
			{
				soundManager = GameController.instance.soundManager;
			}

			if (startTime < 0)
			{
				return;
			}

			if (!UI_Intro.activeSelf)
			{
				UI_Intro.SetActive (true);
			}

			if (useVoiceOver)
			{
				if (currentVoice != null)
				{
					if (!currentVoice.isStarted)
					{
						currentVoice.isStarted = true;
						currentVoice.isFinished = false;
						soundManager.PlayVoice (currentVoice.clip);
						startTime = Time.time;
						endTime = currentVoice.length + 0.5f;
						text.text = lines [currentVoice.id];
					}
					else if (!currentVoice.isFinished)
					{
						if (Time.time - startTime > endTime)
						{
							currentVoice.isFinished = true;
							foreach (VoiceOver voice in voiceOver)
							{
								if (!voice.isStarted && !voice.isFinished)
								{
									currentVoice = voice;
									break;
								}
							}
							if (currentVoice.isFinished)
							{
								currentVoice = null;
							}
						}
						if (Time.time - startTime > 1 && (Input.anyKey || Input.GetMouseButton (0)))
						{
							currentVoice.isFinished = true;
							foreach (VoiceOver voice in voiceOver)
							{
								if (!voice.isStarted && !voice.isFinished)
								{
									currentVoice = voice;
									break;
								}
							}
							if (currentVoice.isFinished)
							{
								currentVoice = null;
							}
						}
					}
				}
				else
				{
					GameController.instance.FinishLines ();
					UI_Intro.SetActive (false);
					enabled = false;
				}
			}
			else
			{
				int next = (int) Mathf.Floor ((Time.time - startTime) * interval);
				if (current != next && next < lines.Count)
				{
					current = next;
					text.text = lines [next];
				}
				if (next >= lines.Count)
				{
					GameController.instance.FinishLines ();
					UI_Intro.SetActive (false);
					enabled = false;
				}
			}
		}

		public void End ()
		{
			startTime = -1;
			if (UI_Intro.activeSelf)
			{
				UI_Intro.SetActive (false);
			}
			enabled = false;
		}
	}
}
