using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fear
{
	public class Map {

		public int[,] grid;
		public float up;
		public float right;
		public float bottom;
		public float left;
		public Transform holder;
	}

	public class MapGenerator : MonoBehaviour {

		public int width;
		public int height;
		public float size;

		public string seed;
		public bool useRandomSeed;

		[Range(0,100)]
		public int randomFillMap;
		[Range(0,100)]
		public int randomFillTree;

		public GameObject[] landTile;
		public GameObject[] treeTiles;
		public GameObject[] strengthTiles;
		public GameObject[] courageTiles;
		public GameObject[] wisdomTreeTiles;
		public GameObject[] spiritTreeTiles;
		public GameObject[] monsterTiles;

		[HideInInspector]
		public float map_width;
		[HideInInspector]
		public float map_height;

		private Map map;
		private List<Map> maps = new List<Map>();
		private Transform mapHolder;
		private Transform subMapHolder;
		public Transform poolHolder;
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		private bool noMonster;
		private bool noStrength;
		private bool noCourage;
		private bool noWisdomTree;
		private bool noSpiritTree;

		public List<GameObject> lands = new List <GameObject> ();
		public List<GameObject> trees = new List <GameObject> ();
		public List<GameObject> monsters = new List <GameObject> ();
		public List<GameObject> strengths = new List <GameObject> ();

		void Awake ()
		{
			map_width = width * size;
			map_height = height * size;
		}

		public void RemoveMap (float offset_x, float offset_y)
		{
			Map map = FindMap (offset_x, offset_y);
			maps.Remove (map);
		}

		public void GenerateMap (float offset_x, float offset_y)
		{
			if (FindMap (offset_x, offset_y) != null)
			{
				return;
			}

			// Debug.Log (trees.Count + ", " + strengths.Count + ", " + monsters.Count);

			if (mapHolder == null)
			{
				mapHolder = GameObject.Find ("Map").transform;
			}

			GameObject subMapObject = new GameObject ("SubMap");
			// subMapObject.layer = 9;
			subMapObject.AddComponent <MapController> ();
			subMapObject.transform.SetParent (mapHolder);
			subMapHolder = subMapObject.transform;
			subMapHolder.position = new Vector3 (offset_x, offset_y, 0);

			gridPositions.Clear ();
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}

			map = new Map ();
			map.grid = new int[width,height];
			map.holder = subMapHolder;
			RandomFillMap ();

			for (int i = 0; i < 5; i ++) {
				SmoothMap ();
			}

			RandomFillTree ();

			map.up = map_height/2 + offset_y;
			map.right = map_width/2 + offset_x;
			map.bottom = -map_height/2 + offset_y;
			map.left = -map_width/2 + offset_x;

			GameObject tile;
			GameObject tileChoice;

			// lands
			tileChoice = landTile[Random.Range (0, landTile.Length)];
			tile = Instantiate (tileChoice) as GameObject;
			tile.transform.position = new Vector3 (-map_width/2 + offset_x, -map_height/2 + offset_y, 0f);
			tile.transform.localScale = new Vector3 (map_width, map_height, 1);
			tile.transform.SetParent (subMapHolder);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					// trees
					if (map.grid[x,y] == 2)
					{
						if (trees.Count > 0)
						{
							tile = trees[0];
							trees.RemoveAt (0);
							tile.SetActive (true);
						}
						else
						{
							tileChoice = treeTiles[Random.Range (0, treeTiles.Length)];
							tile = Instantiate (tileChoice) as GameObject;
						}
						tile.transform.position = new Vector3 (-map_width/2 + x * size + offset_x, -map_height/2 + y * size + offset_y, 0f);
						tile.transform.SetParent (subMapHolder);
						gridPositions.Remove (new Vector3 (x, y, 0f));
					}
				}
			}

			if (!noWisdomTree)
			{
				LayoutObjectAtRandom (wisdomTreeTiles, -1, 1, offset_x, offset_y);
			}

			if (!noSpiritTree)
			{
				LayoutObjectAtRandom (spiritTreeTiles, -2, 1, offset_x, offset_y);
			}

			if (!noStrength)
			{
				int strengthCount = Mathf.Max (8, 15 - GameController.instance.level);
				LayoutObjectAtRandom (strengthTiles, strengthCount - 3, strengthCount, offset_x, offset_y, "Strength");
			}

			if (!noCourage)
			{
				LayoutObjectAtRandom (courageTiles, 1, 1, offset_x, offset_y);
			}

			if (!noMonster)
			{
				// int monsterCount = (int) Mathf.Log (GameController.instance.level, 2f);
				int monsterCount = Mathf.Max (1, GameController.instance.level);
				LayoutObjectAtRandom (monsterTiles, monsterCount, monsterCount, offset_x, offset_y, "Monster");
			}

			maps.Add (map);
		}

		void RandomFillMap ()
		{
			if (useRandomSeed) {
				seed = Time.time.ToString();
			}

			System.Random pseudoRandom = new System.Random (seed.GetHashCode ());

			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					map.grid[x,y] = (pseudoRandom.Next (0,100) < randomFillMap)? 1: 0;
				}
			}
		}

		void SmoothMap ()
		{
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					int neighbourWallTiles = GetSurroundingWallCount(x,y);

					if (neighbourWallTiles > 4)
						map.grid[x,y] = 1;
					else if (neighbourWallTiles < 4)
						map.grid[x,y] = 0;

				}
			}
		}

		int GetSurroundingWallCount (int gridX, int gridY)
		{
			int wallCount = 0;
			for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
				for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
					if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
						if (neighbourX != gridX || neighbourY != gridY) {
							wallCount += map.grid[neighbourX,neighbourY];
						}
					}
					else {
						wallCount ++;
					}
				}
			}

			return wallCount;
		}

		void RandomFillTree ()
		{
			if (useRandomSeed) {
				seed = Time.time.ToString();
			}

			System.Random pseudoRandom = new System.Random(seed.GetHashCode());

			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					if (map.grid[x,y] > 0)
					{
						map.grid[x,y] = (pseudoRandom.Next(0,100) < randomFillTree)? 2: 1;
					}
				}
			}
		}

		public Map FindMap (float x, float y)
		{
			foreach (Map _map in maps)
			{
				if (_map.holder == null)
				{
					maps.Remove (_map);
					return null;
				}
				if (x >= _map.left && x <= _map.right &&
					y <= _map.up && y >= _map.bottom)
				{
					if (_map.holder.childCount <= 0)
					{
						return null;
					}
					return _map;
				}
			}
			return null;
		}

		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum, float offset_x, float offset_y, string tag)
		{
			Vector3 randomPosition;
			GameObject tile;
			if (tag == "Strength")
			{
				int objectCount = Random.Range (minimum, maximum+1);
				for (int i = 0; i < objectCount; i++)
				{
					randomPosition = RandomPosition ();
					randomPosition.x = -map_width/2 + randomPosition.x * size + offset_x;
					randomPosition.y = -map_height/2 + randomPosition.y * size + offset_y;
					randomPosition.z = 0f;
					if (strengths.Count > 0)
					{
						tile = strengths[0];
						strengths.RemoveAt (0);
					}
					else
					{
						GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
						tile = Instantiate (tileChoice);
					}
					tile.transform.position = randomPosition;
					tile.transform.SetParent (subMapHolder);
				}
			}
			else if (tag == "Monster")
			{
				int objectCount = Random.Range (minimum, maximum+1);
				for (int i = 0; i < objectCount; i++)
				{
					randomPosition = RandomPosition ();
					randomPosition.x = -map_width/2 + randomPosition.x * size + offset_x;
					randomPosition.y = -map_height/2 + randomPosition.y * size + offset_y;
					randomPosition.z = 0f;
					if (monsters.Count > 0)
					{
						tile = monsters[0];
						monsters.RemoveAt (0);
					}
					else
					{
						GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
						tile = Instantiate (tileChoice);
					}
					tile.transform.position = randomPosition;
					tile.transform.SetParent (subMapHolder);
				}
			}
			else
			{
				LayoutObjectAtRandom (tileArray, minimum, maximum, offset_x, offset_y);
			}
		}

		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum, float offset_x, float offset_y)
		{
			int objectCount = Random.Range (minimum, maximum+1);
			Vector3 randomPosition;
			GameObject tileChoice;
			GameObject tile;
			for (int i = 0; i < objectCount; i++)
			{
				randomPosition = RandomPosition ();
				randomPosition.x = -map_width/2 + randomPosition.x * size + offset_x;
				randomPosition.y = -map_height/2 + randomPosition.y * size + offset_y;
				randomPosition.z = 0f;

				tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				tile = Instantiate (tileChoice, randomPosition, Quaternion.identity);
				tile.transform.SetParent (subMapHolder);
			}
		}

		Vector3 RandomPosition ()
		{
			int randomIndex = Random.Range (0, gridPositions.Count);
			Vector3 randomPosition = gridPositions[randomIndex];
			gridPositions.RemoveAt (randomIndex);
			return randomPosition;
		}

		public void InitMap ()
		{
			map = null;
			noMonster = true;
			noStrength = true;
			noCourage = true;
			noWisdomTree = true;
			noSpiritTree = true;
			maps.Clear ();
			trees.Clear ();
			strengths.Clear ();
			monsters.Clear ();

			if (poolHolder == null)
			{
				GameObject poolObject = new GameObject ("Pool");
				poolObject.transform.parent = transform.parent;
				poolHolder = poolObject.transform;
			}

			// tree pool
			if (trees.Count <= 0)
			{
				int objectCount = 30;
				GameObject tileChoice;
				GameObject tile;
				for (int i = 0; i < objectCount; i++)
				{
					tileChoice = treeTiles[Random.Range (0, treeTiles.Length)];
					tile = Instantiate (tileChoice);
					tile.SetActive (false);
					tile.transform.SetParent (poolHolder);
					trees.Add (tile);
				}
			}

			// strength pool
			if (strengths.Count <= 0)
			{
				int objectCount = 10;
				GameObject tileChoice;
				GameObject tile;
				for (int i = 0; i < objectCount; i++)
				{
					tileChoice = strengthTiles[Random.Range (0, strengthTiles.Length)];
					tile = Instantiate (tileChoice);
					tile.SetActive (false);
					tile.transform.SetParent (poolHolder);
					strengths.Add (tile);
				}
			}

			// monster pool
			if (monsters.Count <= 0)
			{
				int objectCount = 10;
				GameObject tileChoice;
				GameObject tile;
				for (int i = 0; i < objectCount; i++)
				{
					tileChoice = monsterTiles[Random.Range (0, monsterTiles.Length)];
					tile = Instantiate (tileChoice);
					tile.SetActive (false);
					tile.transform.SetParent (poolHolder);
					monsters.Add (tile);
				}
			}

			GenerateMap (0, 0);
		}

		public void SetNoMonster (bool _noMonster)
		{
			noMonster = _noMonster;
		}

		public void SetNoStrength (bool _noStrength)
		{
			noStrength = _noStrength;
		}

		public void SetNoCourage (bool _noCourage)
		{
			noCourage = _noCourage;
		}

		public void SetNoWisdomTree (bool _noWisdomTree)
		{
			noWisdomTree = _noWisdomTree;
		}

		public void SetNoSpiritTree (bool _noSpiritTree)
		{
			noSpiritTree = _noSpiritTree;
		}

		/*
		void OnDrawGizmos() {
			if (map != null) {
				for (int x = 0; x < width; x ++) {
					for (int y = 0; y < height; y ++) {
						Gizmos.color = (map.grid[x,y] == 1)?Color.black:Color.white;
						Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
						Gizmos.DrawCube(pos,Vector3.one);
					}
				}
			}
		}
		*/
	}
}
