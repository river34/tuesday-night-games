using UnityEngine;
using System.Collections;

namespace Fear
{
	public class Loader : MonoBehaviour
	{
		public GameObject game;

		void Awake ()
		{
			if (GameController.instance == null)
				Instantiate (game);
		}
	}
}
