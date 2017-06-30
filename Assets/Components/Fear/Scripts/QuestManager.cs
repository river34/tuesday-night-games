using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fear
{
	public class Quest
	{
		// assigned values
		public int id;				// index
		public string name;			// give it a name
		public string text;			// short description
		public string tag;			// tag of objects to interact with
		public int num;				// num of objects to interact with
		public bool isOpen;			// is available now
		public string[] next;		// name of the next quest; null if any
		public float minStrength;
		public float maxStrength;
		public float minFear;
		public float maxFear;
		public bool strengthStar;
		public bool courageStar;
		public bool wisdomTree;
		public bool spiritTree;
		public bool monster;
		public int level;
		public AudioClip voiceOver;

		public bool isFinished;
		public float startTime;
		public float finishTime;
		public List<GameObject> objects;

		public Quest (int _id, string _name, string _text, string _tag, int _num, bool _isOpen, string[] _next,
			float _minStrength, float _maxStrength, float _minFear, float _maxFear,
			bool _strengthStar, bool _courageStar, bool _wisdomTree, bool _spiritTree, bool _monster,
			int _level, AudioClip _voiceOver)
		{
			id = _id;
			name = _name;
			text = _text;
			tag = _tag;
			num = _num;
			isOpen = _isOpen;
			next = _next;
			minStrength = _minStrength;
			maxStrength = _maxStrength;
			minFear = _minFear;
			maxFear = _maxFear;
			strengthStar = _strengthStar;
			courageStar = _courageStar;
			wisdomTree = _wisdomTree;
			spiritTree = _spiritTree;
			monster = _monster;
			level = _level;
			voiceOver = _voiceOver;

			isFinished = false;
			startTime = Time.deltaTime;
			objects = new List<GameObject> ();
		}

		public Quest (int _id, string _name, string _text, string _tag, int _num, bool _isOpen, string[] _next,
			float _minStrength, float _maxStrength, float _minFear, float _maxFear,
			bool _strengthStar, bool _courageStar, bool _wisdomTree, bool _spiritTree, bool _monster, int _level)
		{
			id = _id;
			name = _name;
			text = _text;
			tag = _tag;
			num = _num;
			isOpen = _isOpen;
			next = _next;
			minStrength = _minStrength;
			maxStrength = _maxStrength;
			minFear = _minFear;
			maxFear = _maxFear;
			strengthStar = _strengthStar;
			courageStar = _courageStar;
			wisdomTree = _wisdomTree;
			spiritTree = _spiritTree;
			monster = _monster;
			level = _level;

			voiceOver = null;
			isFinished = false;
			startTime = Time.deltaTime;
			objects = new List<GameObject> ();
		}
	}

	public class QuestManager : MonoBehaviour {

		public List<Quest> quests;

		// UI
		public GameObject UI_Quest;
		private Text questName;
		private Text questText;

		// current quest
		private Quest quest;

		// map generator
		private MapGenerator map;

		// sound manager
		private SoundManager soundManager;
		public AudioClip VO_Strength;
		public AudioClip VO_MoreStrength;
		public AudioClip VO_WisdomTree;
		public AudioClip VO_Courage;
		public AudioClip VO_MoreCourage;
		public AudioClip VO_Ghost;
		public AudioClip VO_StrengthDrop;
		public AudioClip VO_CannotSee;
		public AudioClip VO_MoreGhosts;
		public AudioClip VO_Everywhere;
		public AudioClip VO_Solution;
		public AudioClip VO_Disappear;
		public AudioClip VO_Closer;
		public AudioClip VO_Ask;
		public AudioClip VO_Reflection;
		public AudioClip VO_Spirit;

		void Awake ()
		{
			soundManager = gameObject.GetComponent<SoundManager> ();
			map = gameObject.GetComponent<MapGenerator> ();
			questName = UI_Quest.transform.Find ("Name").gameObject.GetComponent<Text>();
			questText = UI_Quest.transform.Find ("Text").gameObject.GetComponent<Text>();

			quests = new List<Quest> ();

			InitQuest ();
		}

		public void InitQuest ()
		{
			quests.Clear ();
			string thisName;
			string[] nextName;

			// quest : strength
			thisName = "Strength";
			nextName = new string[1];
			nextName[0] = "More Strength";
			quest = new Quest (quests.Count, thisName,
			"Ima needs physical strength to explore the forest.\r\nYellow Stars replenish Ima's strength.",
			"Strength", 1, true, nextName,
			-1, -1, -1, -1,
			true, false, false, false, false,
			1, VO_Strength);
			quests.Add (quest);

			// quest : more strength
			thisName = "More Strength";
			nextName = new string[1];
			nextName[0] = "Wisdom Tree";
			quest = new Quest (quests.Count, thisName,
			"The stronger Ima is, the brighter she will appear. \r\nShift / Space / Right Click to move faster.",
			"Strength", 1, false, nextName,
			950, 1000, -1, -1,
			true, false, false, false, false,
			1, VO_MoreStrength);
			quests.Add (quest);

			// quest : find the wisdom tree
			thisName = "Wisdom Tree";
			nextName = new string[1];
			nextName[0] = "Courage";
			quest = new Quest (quests.Count, thisName,
			"However, physical strength is not the only thing Ima needs. \r\nA Kumu can always ask the Wisdom Tree for advice.",
			"WisdomTree", 1, false, nextName,
			-1, -1, -1, -1,
			true, false, true, false, false,
			2, VO_WisdomTree);
			quests.Add (quest);

			// quest : collect red star
			thisName = "Courage";
			nextName = new string[1];
			nextName[0] = "More Courage";
			quest = new Quest (quests.Count, thisName,
			"Wisdom Tree: One needs courage to survive the Fear Forest. \r\nRed Stars provide courage.",
			"Courage", 1, false, nextName,
			-1, -1, -1, -1,
			true, true, false, false, false,
			3, VO_Courage);
			quests.Add (quest);

			// quest : courage
			thisName = "More Courage";
			nextName = new string[3];
			nextName[0] = "Ghosts 1";
			nextName[1] = "Ghosts 2";
			nextName[2] = "Ghosts 3";
			quest = new Quest (quests.Count, thisName,
			"Courage allows Ima to see clearly. \r\nThat is exactly what Ima needs in this darkness.",
			"Courage", 1, false, nextName,
			-1, -1, 0, 800,
			true, true, false, false, false,
			3, VO_MoreCourage);
			quests.Add (quest);

			// quest : ghosts 1
			thisName = "Ghosts 1";
			nextName = new string[1];
			nextName[0] = "Strength Drop";
			quest = new Quest (quests.Count, thisName,
			"Wisdom Tree:  Ghosts wonder in the Fear Forest.\r\nThey will take away Ima's physical strength.",
			"Monster", 1, false, nextName,
			501, 800, -1, -1,
			true, false, false, false, true,
			4, VO_Ghost);
			quests.Add (quest);

			// quest : ghosts 2
			thisName = "Ghosts 2";
			nextName = new string[1];
			nextName[0] = "Cannot See";
			quest = new Quest (quests.Count, thisName,
			"Wisdom Tree:  Ghosts wonder in the Fear Forest.\r\nThey will take away Ima's physical strength.",
			"Monster", 1, false, nextName,
			-1, -1, 901, 1000,
			true, false, false, false, true,
			4, VO_Ghost);
			quests.Add (quest);

			// quest : ghosts 3
			thisName = "Ghosts 3";
			nextName = new string[1];
			nextName[0] = "More Ghosts";
			quest = new Quest (quests.Count, thisName,
			"Wisdom Tree:  Ghosts wonder in the Fear Forest.\r\nThey will take away Ima's physical strength.",
			"Monster", 5, false, nextName,
			801, 1000, -1, -1,
			true, false, false, false, true,
			4, VO_Ghost);
			quests.Add (quest);

			// quest : Strength Drop
			thisName = "Strength Drop";
			nextName = new string[1];
			nextName[0] = "More Ghosts";
			quest = new Quest (quests.Count, thisName,
			"Ima is being consumed by Fear.  Ima's strength is dropping.",
			"Monster", 1, false, nextName,
			801, 1000, -1, -1,
			true, false, false, false, true,
			4, VO_StrengthDrop);
			quests.Add (quest);

			// quest : Cannot See
			thisName = "Cannot See";
			nextName = new string[1];
			nextName[0] = "More Ghosts";
			quest = new Quest (quests.Count, thisName,
			"Ima lost all courage. Ima can no longer see.",
			"Monster", 5, false, nextName,
			801, 1000, -1, -1,
			true, false, false, false, true,
			4, VO_CannotSee);
			quests.Add (quest);

			// quest : More Ghosts
			thisName = "More Ghosts";
			nextName = new string[2];
			nextName[0] = "Everywhere 1";
			nextName[1] = "Everywhere 2";
			quest = new Quest (quests.Count, thisName,
			"More Ghosts are coming...",
			"Monster", 5, false, nextName,
			801, 1000, -1, -1,
			true, false, false, false, true,
			5, VO_MoreGhosts);
			quests.Add (quest);

			// quest : Everywhere 1
			thisName = "Everywhere 1";
			nextName = new string[2];
			nextName[0] = "Solution 1";
			nextName[1] = "Solution 2";
			quest = new Quest (quests.Count, thisName,
			"Ghosts are everywhere.  What will Ima do?",
			"Monster", 5, false, nextName,
			601, 800, 801, 1000,
			true, false, false, false, true,
			6, VO_Everywhere);
			quests.Add (quest);

			// quest : Everywhere 2
			thisName = "Everywhere 2";
			nextName = new string[1];
			nextName[0] = "Disappear";
			quest = new Quest (quests.Count, thisName,
			"Ghosts are everywhere.  What will Ima do?",
			"Monster", 5, false, nextName,
			801, 1000, 301, 600,
			true, false, false, false, true,
			6, VO_Everywhere);
			quests.Add (quest);

			// quest : Solution 1
			thisName = "Solution 1";
			nextName = new string[1];
			nextName[0] = "Disappear";
			quest = new Quest (quests.Count, thisName,
			"There must be a way to deal with them.",
			"Monster", 1, false, nextName,
			801, 1000, 301, 600,
			true, false, false, false, true,
			6, VO_Solution);
			quests.Add (quest);

			// quest : Solution 2
			thisName = "Solution 2";
			nextName = new string[1];
			nextName[0] = "Closer";
			quest = new Quest (quests.Count, thisName,
			"There must be a way to deal with them.",
			"Monster", 5, false, nextName,
			801, 1000, 801, 1000,
			true, false, false, false, true,
			6, VO_Solution);
			quests.Add (quest);

			// quest : Closer
			thisName = "Closer";
			nextName = new string[1];
			nextName[0] = "Disappear";
			quest = new Quest (quests.Count, thisName,
			"What happens if Ima approaches the Ghosts?",
			"Monster", 1, false, nextName,
			601, 800, 301, 600,
			true, false, false, false, true,
			6, VO_Closer);
			quests.Add (quest);

			// quest : Disappear
			thisName = "Disappear";
			nextName = new string[1];
			nextName[0] = "Ask";
			quest = new Quest (quests.Count, thisName,
			"Is that true? Some Ghosts are fading.",
			"Monster", 1, false, nextName,
			801, 1000, 0, 300,
			true, false, false, false, true,
			7, VO_Disappear);
			quests.Add (quest);

			// quest : secret
			thisName = "Ask";
			nextName = new string[1];
			nextName[0] = "Reflection";
			quest = new Quest (quests.Count, thisName,
			"That is unusual.  Maybe Wisdom Tree knows the answer.",
			"WisdomTree", 1, false, nextName,
			-1, -1, -1, -1,
			true, false, true, false, true,
			8, VO_Ask);
			quests.Add (quest);

			// quest : Reflection
			thisName = "Reflection";
			nextName = new string[1];
			nextName[0] = "Spirit";
			quest = new Quest (quests.Count, thisName,
			"Wisdom Tree:  A true Kumu shall not be afraid of Ghosts. \r\nThey are nothing more than a reflection of your own fear.",
			"Monster", 1, false, nextName,
			801, 1000, 0, 100,
			true, false, false, false, true,
			9, VO_Reflection);
			quests.Add (quest);

			// quest : spirit
			thisName = "Spirit";
			nextName = new string[0];
			quest = new Quest (quests.Count, thisName,
			"Wisdom Tree:  Now go find the Sacred Tree, and spirits will guide you further on.",
			"SpiritTree", 1, false, nextName,
			-1, -1, -1, -1,
			true, false, false, true, false,
			10, VO_Spirit);
			quests.Add (quest);

			if (quests == null || quests.Count <= 0)
			{
				return;
			}

			enabled = false;
		}

		void OnEnable ()
		{
			foreach (Quest quest in quests)
			{
				if (quest.isOpen && !quest.isFinished)
				{
					UI_Quest.SetActive (true);
					quest.isOpen = true;
					quest.objects.Clear ();
					questName.text = quest.name;
					questText.text = quest.text;
					StartQuest (quest.id);
					break;
				}
			}
		}

		public void CompleteQuest (string completeName)
		{
			// Debug.Log ("Force Complete " + completeName);
			Quest completeQuest = FindByName (completeName);

			if (completeQuest == null)
				return;

			int id = completeQuest.id;

			quests[id].isFinished = true;
			quests[id].finishTime = Time.deltaTime;
			GameController.instance.level = quests[id].level;

			foreach (Quest quest in quests)
			{
				if (quest.isOpen)
				{
					quest.isOpen = false;
				}
			}

			if (quests[id].next.Length > 0)
			{
				foreach (string name in quests[id].next)
				{
					quest = FindByName (name);
					if (quest == null)
					{
						return;
					}
					quest.isOpen = true;
					quest.objects.Clear ();
					questName.text = quest.name;
					questText.text = quest.text;
					StartQuest (quest.id);
				}
			}
			else
			{
				GameController.instance.GameComplete ();
			}
		}

		public void CompleteQuest (int id)
		{
			if (id > quests.Count)
			{
				return;
			}

			if (quests[id].isFinished || !quests[id].isOpen)
			{
				return;
			}

			// Debug.Log ("Complete " + quests[id].name);
			quests[id].isFinished = true;
			quests[id].finishTime = Time.deltaTime;
			GameController.instance.level = quests[id].level;

			foreach (Quest quest in quests)
			{
				if (quest.isOpen)
				{
					quest.isOpen = false;
				}
			}

			if (quests[id].next.Length > 0)
			{
				foreach (string name in quests[id].next)
				{
					quest = FindByName (name);
					if (quest == null)
					{
						return;
					}
					quest.isOpen = true;
					quest.objects.Clear ();
					questName.text = quest.name;
					questText.text = quest.text;
					StartQuest (quest.id);
				}
			}
			else
			{
				GameController.instance.GameComplete ();
			}

			// debug
			foreach (Quest quest in quests)
			{
				if (quest.isOpen)
				{
					// Debug.Log ("Open: " + quest.name);
				}
			}
		}

		void StartQuest (int id)
		{
			if (id > quests.Count)
			{
				return;
			}

			if (quests[id].isFinished || !quests[id].isOpen)
			{
				return;
			}

			if (quests[id].voiceOver != null)
			{
				soundManager.PlayVoice (quests[id].voiceOver);
			}

			if (quests[id].strengthStar)
			{
				map.SetNoStrength (false);
			}
			else
			{
				map.SetNoStrength (true);
			}

			if (quests[id].courageStar)
			{
				map.SetNoCourage (false);
			}
			else
			{
				map.SetNoCourage (true);
			}

			if (quests[id].wisdomTree)
			{
				map.SetNoWisdomTree (false);
			}
			else
			{
				map.SetNoWisdomTree (true);
			}

			if (quests[id].spiritTree)
			{
				map.SetNoSpiritTree (false);
			}
			else
			{
				map.SetNoSpiritTree (true);
			}

			if (quests[id].monster)
			{
				map.SetNoMonster (false);
			}
			else
			{
				map.SetNoMonster (true);
			}

			quests[id].startTime = Time.time;
		}

		public bool CheckForComplete (int id, PlayerController player)
		{
			if (id > quests.Count)
			{
				return false;
			}

			if (quests[id].isFinished || !quests[id].isOpen)
			{
				return false;
			}

			if (quests[id].objects.Count >= quests[id].num)
			{
				if (quests[id].minStrength != -1 && quests[id].maxStrength != -1 && quests[id].minFear != -1 && quests[id].maxFear != -1)
				{
					if (player.strength >= quests[id].minStrength && player.strength <= quests[id].maxStrength)
					{
						if (player.fear >= quests[id].minFear && player.fear <= quests[id].maxFear)
						{
							CompleteQuest (id);
							return true;
						}
					}
				}
				else if (quests[id].minStrength != -1 && quests[id].maxStrength != -1)
				{
					if (player.strength >= quests[id].minStrength && player.strength <= quests[id].maxStrength)
					{
						CompleteQuest (id);
						return true;
					}
				}
				else if (quests[id].minFear != -1 && quests[id].maxFear != -1)
				{
					if (player.fear >= quests[id].minFear && player.fear <= quests[id].maxFear)
					{
						CompleteQuest (id);
						return true;
					}
				}
				else
				{
					CompleteQuest (id);
					return true;
				}
			}

			return false;
		}

		public int FindIDByName (string name)
		{
			if (quests == null || quests.Count <= 0)
			{
				return -1;
			}

			foreach (Quest quest in quests)
			{
				if (quest.name == name)
				{
					return quest.id;
				}
			}

			return -1;
		}

		public Quest FindByName (string name)
		{
			if (quests == null || quests.Count <= 0)
			{
				return null;
			}

			foreach (Quest quest in quests)
			{
				if (quest.name == name)
				{
					return quest;
				}
			}

			return null;
		}
	}
}
