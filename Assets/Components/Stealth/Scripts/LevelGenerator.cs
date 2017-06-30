using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
	public class LevelGenerator : MonoBehaviour {

		private float unit_start_x = -6f;
		private float unit_width = 8f;
		private LevelLibrary library;
		private Level level;
		private List <GameObject> units;
		public List <GameObject> levelcards;

		void Start ()
		{
			units = new List <GameObject> ();
			library = new LevelLibrary ();

			foreach (Transform child in transform)
			{
				if (child.CompareTag (Tags.DARK))
				{
					units.Add (child.gameObject);
				}
			}
		}

		public void GenerateLevelcard (int id, Transform parent)
		{
			if (library == null)
			{
				library = new LevelLibrary ();
			}

			level = library.GetLevel (id);

			int index = 0;
			if (level != null)
			{
				for (int i = 0; i < level.units.Length; i++)
				{
					if (level.units[i] >= 0 && level.units[i] < level.units.Length)
					{
						Vector3 position = parent.position;
						position.x = unit_start_x + unit_width * index;

						if (units == null)
						{
							units = new List <GameObject> ();
							library = new LevelLibrary ();

							foreach (Transform child in transform)
							{
								if (child.CompareTag (Tags.DARK))
								{
									units.Add (child.gameObject);
								}
							}
						}
						GameObject level_object = Instantiate (units[level.units[i]], position, parent.rotation, parent);

						// objectives
						float rate = Random.Range (0, 1f);
						if (rate >= 0.4f)
						{
							foreach (Transform child in level_object.transform)
							{
								if (child.CompareTag (Tags.OBJECTIVE_SMALL))
								{
									Destroy (child.gameObject);
								}
							}
						}
						if (rate >= 0.8f)
						{
							foreach (Transform child in level_object.transform)
							{
								if (child.CompareTag (Tags.OBJECTIVE))
								{
									Destroy (child.gameObject);
								}
							}
						}
					}
					index ++;
				}
			}
		}

		public List <GameObject> GenerateLevecards ()
		{
			levelcards = new List<GameObject> ();
			for (int i = 0; i < GetNumOfLevels (); i++)
			{
				GameObject levelcard = new GameObject ();
				levelcard.name = "Levelcard_" + i;
				levelcard.tag = Tags.LEVELCARD;
				levelcard.transform.position = transform.position;
				levelcard.transform.parent = transform;
				GenerateLevelcard (i, levelcard.transform);
				levelcards.Add (levelcard);
			}
			return levelcards;
		}

		public int GetLevelIdByDifficultyLevel (int difficulty_level)
		{
			if (levelcards == null)
			{
				levelcards = GenerateLevecards ();
			}

			List<int> appropriate_levelcards = new List<int> ();
			for (int i = 0; i < GetNumOfLevels (); i++)
			{
				if (library.levels[i].difficulty_min <= difficulty_level && library.levels[i].difficulty_max >= difficulty_level)
				{
					appropriate_levelcards.Add (i);
				}
			}

			if (appropriate_levelcards.Count < 1)
			{
				return levelcards.Count - 1;
			}

			return appropriate_levelcards[Random.Range (0, appropriate_levelcards.Count - 1)];
		}

		public int GetNumOfLevels ()
		{
			if (library == null)
			{
				library = new LevelLibrary ();
			}

			return library.levels.Count;
		}

		public int GetNumOfUnits (int id)
		{
			if (library != null)
			{
				if (id >= 0 && id < library.levels.Count)
				{
					return library.levels[id].units.Length;
				}
			}
			return 0;
		}

		public int GetNumOfGuards (int id)
		{
			if (library != null)
			{
				if (id >= 0 && id < library.levels.Count)
				{
					return library.levels[id].num_of_guards;
				}
			}
			return 0;
		}

		public float GetWidth (int id)
		{
			return unit_width * GetNumOfUnits (id);
		}
	}

	public class Level {

		public int id;
		public int difficulty_min;
		public int difficulty_max;
		public int num_of_guards;
		public int[] units;

		public Level (int id, int difficulty_min, int difficulty_max, int num_of_guards, int[] units)
	    {
	        this.id = id;
			this.difficulty_min = difficulty_min;
			this.difficulty_max = difficulty_max;
			this.num_of_guards = num_of_guards;
			this.units = units;
	    }
	}

	public class LevelLibrary {

		public List<Level> levels;

		public LevelLibrary ()
		{
			levels = new List<Level> ();
			levels.Add (new Level (0, 0, 0, 4, new int[] {0, 1, 1, 2, 1, 1, 0}));
			levels.Add (new Level (1, 1, 4, 6, new int[] {0, 1, 2, 1, 2, 1, 2, 1, 0}));
			levels.Add (new Level (2, 1, 10, 8, new int[] {1, 2, 3, 2, 1, 2, 3, 2, 1}));
			levels.Add (new Level (3, 2, 100, 10, new int[] {1, 3, 2, 2, 3, 3, 2, 2, 3, 1}));
			levels.Add (new Level (4, 2, 100, 12, new int[] {1, 3, 2, 3, 4, 3, 2, 3, 4, 3, 2, 3, 1}));
			levels.Add (new Level (5, 3, 100, 12, new int[] {1, 2, 3, 1, 4, 2, 3, 2, 4, 1, 3, 2, 1}));
			levels.Add (new Level (6, 3, 100, 12, new int[] {1, 2, 3, 1, 4, 2, 5, 6, 2, 4, 1, 3, 2, 1}));
			levels.Add (new Level (7, 4, 100, 12, new int[] {1, 2, 5, 6, 4, 2, 5, 6, 2, 4, 5, 6, 2, 1}));
			levels.Add (new Level (8, 4, 100, 14, new int[] {1, 3, 5, 6, 4, 2, 3, 5, 6, 3, 2, 4, 5, 6, 3, 1}));
			levels.Add (new Level (9, 5, 100, 14, new int[] {2, 3, 5, 6, 3, 2, 4, 5, 6, 4, 2, 3, 5, 6, 3, 2}));
		}

		public Level GetLevel (int id)
		{
			if (id >= 0 && id < levels.Count)
			{
				return levels[id];
			}
			else
			{
				return null;
			}
		}
	}
}
