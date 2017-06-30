using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fear
{
	public class States {
		public const int TITLE = 0;
		public const int INTRO = 10;
		public const int START = 15;
		public const int QUEST = 20;
		public const int COMPLETE = 30;
		public const int FAIL = 40;
		public const int END = 50;
	}

	public class GameController : MonoBehaviour {

		public static GameController instance = null;
		public CameraController cameraController;
		public GameObject player;

		[HideInInspector]
		public float playerStrength;
		[HideInInspector]
		public float playerFear;
		[HideInInspector]
		public int playerComplete;
		// [HideInInspector]
		public int level;

		// UI
		public GameObject UI_Title;
		public GameObject UI_Block;
		public GameObject UI_End;
		public GameObject UI_Game;
		public GameObject UI_Strength;
		public GameObject UI_Courage;
		public GameObject UI_Complete;
		public GameObject UI_Nav;
		public Image UI_Mask;
		private RectTransform UI_StrengthBar;
		private RectTransform UI_CourageBar;
		private RectTransform UI_CompleteBar;
		private float num2bar = 0.001f * 40;
		private float reverseMul = 0.001f;

		[HideInInspector]
		public float map_width;
		[HideInInspector]
		public float map_height;

		private GameObject playerObject;
		private GameObject mapObject;
		private MapGenerator mapGenerator;
		// private bool doingSetup;
		private float gameStartDelay = 0.5f;

		// quest
		[HideInInspector]
		public QuestManager questManager;

		// intro
		[HideInInspector]
		public IntroManager introManeger;

		// sound
		[HideInInspector]
		public SoundManager soundManager;
		public AudioClip MUS_intro;
		public AudioClip MUS_game;
		public AudioClip MUS_outro;

		// state
		// [HideInInspector]
		public int state;

		// color
		private Color halfBlack = new Color (0, 0, 0, 0.5f);
		private Color totalBlack = new Color (0, 0, 0, 0.85f);

		// time control
		private float time;
		private string nextQuest = "More Courage";
		private int nextLevel = 4;

		void Awake ()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				Destroy (gameObject);
			}

			// DontDestroyOnLoad (gameObject);

			mapGenerator = GetComponent <MapGenerator>();
			soundManager = GetComponent <SoundManager>();
			questManager = GetComponent <QuestManager>();
			introManeger = GetComponent <IntroManager>();

			UI_StrengthBar = UI_Strength.transform.Find ("Bar").gameObject.GetComponent<RectTransform>();
			UI_CourageBar = UI_Courage.transform.Find ("Bar").gameObject.GetComponent<RectTransform>();
			UI_CompleteBar = UI_Complete.transform.Find ("Bar").gameObject.GetComponent<RectTransform>();

			time = Time.time;
			state = States.TITLE;
		}

		void Update ()
		{
			if (state == States.TITLE)
			{
				if (!UI_Title.activeSelf)
				{
					UI_Title.SetActive (true);
				}

				if (Time.time - time > 1f && Input.anyKey)
				{
					StartIntro ();
					soundManager.PlayBackground (MUS_intro);
					time = Time.time;
					state = States.INTRO;
				}
			}
			else if (state == States.INTRO)
			{
				if (UI_Title.GetComponent <Image> ().color.a > Mathf.Epsilon)
				{
					UI_Title.GetComponent <Image> ().color -= new Color (0, 0, 0, Time.deltaTime);
				}

				if (Time.time - time > 1f && Input.GetKey ("p"))
				{
					EndIntro ();
					soundManager.PlayBackground (MUS_game);
					soundManager.PlayAmbience ();
					time = Time.time;
					state = States.START;
				}
			}
			else if (state == States.START)
			{
				InitGame ();
				time = Time.time;
				state = States.QUEST;
			}
			else if (state == States.QUEST)
			{
				UI_StrengthBar.sizeDelta = new Vector2 (playerStrength * num2bar, UI_StrengthBar.sizeDelta.y);
				UI_CourageBar.sizeDelta = new Vector2 ((1000 - playerFear) * num2bar, UI_CourageBar.sizeDelta.y);
				UI_CompleteBar.sizeDelta = new Vector2 (playerComplete * num2bar, UI_CompleteBar.sizeDelta.y);
				UI_Mask.color = Color.Lerp (halfBlack, totalBlack, playerFear * reverseMul);
			}
			else if (state == States.COMPLETE)
			{
				string[] completes = new string[1];
				completes[0] = "May courage always be with you";
				UI_End.transform.Find ("Text").GetComponent<Text>().text = completes [Random.Range (0, completes.Length)];
				UI_End.transform.Find ("Restart").GetComponent<Text>().text = "Space to replay";

				if (Time.time - time > 1 && Input.GetKey ("p"))
				{
					EndIntro ();
					nextLevel ++;
					time = Time.time;
					state = States.END;
				}
			}
			else if (state == States.FAIL)
			{
				string[] fails = new string[2];
				fails[0] = "Fear is temporary. You can do better";
				fails[1] = "A Kumu will never give up and neither shall you";
				UI_End.transform.Find ("Text").GetComponent<Text>().text = fails [Random.Range (0, fails.Length)];
				UI_End.transform.Find ("Restart").GetComponent<Text>().text = "Space to try again";
				nextLevel = Mathf.Max (1, nextLevel - 1);
				time = Time.time;
				state = States.END;
			}
			else if (state == States.END)
			{
				GameEnd ();

				if (Time.time - time > 1 && Input.GetKey ("space"))
				{
					soundManager.PlayBackground (MUS_game);
					InitGame (nextQuest, nextLevel);
					time = Time.time;
					state = States.QUEST;
				}
			}
		}

		public void FinishLines ()
		{
			if (state == States.INTRO)
			{
				soundManager.PlayBackground (MUS_game);
				soundManager.PlayAmbience ();
				time = Time.time;
				state = States.START;
			}
			else if (state == States.COMPLETE)
			{
				time = Time.time;
				state = States.END;
			}
		}

		public void GameOver ()
		{
			playerObject.GetComponent <PlayerController>().enabled = false;
			mapGenerator.enabled = false;
			soundManager.PlayBackground (MUS_outro);
			time = Time.time;
			state = States.FAIL;
		}

		public void GameComplete ()
		{
			playerObject.GetComponent <PlayerController>().enabled = false;
			mapGenerator.enabled = false;
			soundManager.PlayBackground (MUS_outro);
			time = Time.time;
			state = States.COMPLETE;
			StartComplete ();
		}

		void GameEnd ()
		{
			Destroy (playerObject);
			Destroy (mapObject);
			playerObject = null;
			mapObject = null;
			UI_Game.SetActive (false);
			// soundManager.StopBackground ();
			soundManager.StopAmbience ();
			UI_Block.SetActive (false);
			UI_End.SetActive (true);
			cameraController.enabled = false;
			questManager.enabled = false;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	    static public void CallbackInitialization ()
	    {
	        SceneManager.sceneLoaded += OnSceneLoaded;
	    }

	    static private void OnSceneLoaded (Scene arg0, LoadSceneMode arg1)
	    {
	        // instance.InitGame ();
	    }

		void InitGame (string questName, int questLevel)
		{
			InitGame ();
			questManager.CompleteQuest (questName);
		}

		void InitGame ()
		{
			// doingSetup = true;

			level = 1;

			UI_Block.SetActive (true);
			UI_End.SetActive (false);
			UI_Game.SetActive (false);

			if (playerObject == null)
			{
				playerObject = Instantiate (player);
				playerObject.transform.parent = transform.parent;
			}

			cameraController.player = playerObject.transform;
			cameraController.InitCamera ();
			cameraController.enabled = true;

			if (mapObject == null)
			{
				mapObject = new GameObject ("Map");
				mapObject.transform.parent = transform.parent;
			}

			playerStrength = 300;
			playerFear = 1000;
			playerComplete = 0;

			mapGenerator.InitMap ();
			map_width = mapGenerator.map_width;
			map_height = mapGenerator.map_height;

			questManager.enabled = true;
			questManager.InitQuest ();

			Invoke ("StartQuest", gameStartDelay);
		}

		void StartIntro ()
		{
			introManeger.enabled = true;
			introManeger.Init ("Intro");
			introManeger.StartLine ();
		}

		void EndIntro ()
		{
			UI_Title.SetActive (false);
			introManeger.End ();
		}

		void StartComplete ()
		{
			UI_Block.SetActive (true);
			introManeger.enabled = true;
			introManeger.Init ("Complete");
			introManeger.StartLine ();
		}

		void StartQuest ()
		{
			playerObject.GetComponent <PlayerController>().enabled = true;
			UI_Block.SetActive (false);
			UI_Game.SetActive (true);
			// doingSetup = false;
		}

		public void GenerateMap (float offset_x, float offset_y)
		{
			mapGenerator.GenerateMap (offset_x, offset_y);
		}

		public Map FindMap (float x, float y)
		{
			return mapGenerator.FindMap (x, y);
		}
	}
}
