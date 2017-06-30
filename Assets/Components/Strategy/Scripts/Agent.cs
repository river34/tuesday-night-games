using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strategy
{
	public class Agent : MonoBehaviour {

		// info
		public int id;					// unique id
		public string tag;				// Spirit, Ghost, SpiritTower, GhostTower
		public string cat;				// category
		public string name;				// name

		// stats
		public float damage;			// damage per time
		public float damageTime;		// 1 / damageFrequency
		public int num;					// number of enemies it can attack once
		// public float area;				// area of effect (m)
		public float speed;				// move speed (m/frame)
		public float health;			// initial health
		public float defense;			// defense power
		public float refresh;			// production speed (seconds)
		public int rate;
		public int cost;
		public int level;

		// controls
		public bool move;
		public bool right;
		public bool attack;
		public bool dead;
		public List<Agent> targets = new List<Agent>();
		public List<Agent> predators = new List<Agent>();
		public bool targetsLocked;
		public bool predatorsLocked;
		private float maxHealth;
		private float pace;
		private int n;
		public float attackTime = 0;

		// properties
		private Animator animator;
		private Renderer renderer;

		// game
		public AgentManager agentManager;
		public GameController game;

		// particle
		public GameObject hurtParticle;
		public GameObject dieParticle;

		// hit
		private Transform hitHolder;
		public GameObject hitText;

		void Awake ()
		{
			animator = GetComponent <Animator> ();
			if (animator == null)
			{
				animator = transform.Find ("Model").gameObject.GetComponent <Animator> ();
			}

			renderer = gameObject.GetComponent <Renderer> ();
			if (renderer == null)
			{
				renderer = transform.Find ("Model").gameObject.GetComponent <Renderer> ();
				transform.Find ("Model").gameObject.tag = tag;
			}

			gameObject.tag = tag;
			gameObject.name = name;

			// particle system
			if (hurtParticle == null)
			{
				// hurtParticle = transform.Find ("Hurt").gameObject.GetComponent <ParticleSystem> ();
			}
			if (hurtParticle != null) hurtParticle.SetActive (false);
			if (dieParticle != null) dieParticle.SetActive (false);

			// hitHolder
			if (hitHolder == null)
			{
				hitHolder = transform.Find ("Hit");
			}
		}

		void Start ()
		{
			maxHealth = health;

			if (tag == "Tree" && transform.Find ("Leaves") != null)
			{
				pace = maxHealth / transform.Find ("Leaves").childCount;
				n = 0;
			}

			dead = false;
		}

		void OnEnable ()
		{
			maxHealth = Mathf.Max (maxHealth, health);
			if (tag == "Tree" && transform.Find ("Leaves") != null)
			{
				pace = maxHealth / transform.Find ("Leaves").childCount;
			}
		}

		void Update ()
		{
			if (dead)
			{
				return;
			}

			if (move)
			{
				if (right)
				{
					transform.position += Vector3.right * Time.deltaTime * speed;
				}
				else
				{
					transform.position += Vector3.left * Time.deltaTime * speed;
				}
			}

			if (attack)
			{
				Stop ();
				if (damageTime > 0 && Time.time - attackTime > damageTime)
				{
					attackTime = Time.time;
					Attack ();
				}
			}
			else
			{
				Move ();
			}
		}

		public void Move ()
		{
			// Debug.Log (name + " : Move");
			if (tag != "Tree")
			{
				animator.SetBool ("Move", true);
				move = true;
			}
		}

		public void Stop ()
		{
			// Debug.Log (name + " : Stop");
			animator.SetBool ("Move", false);
			move = false;
		}

		public void Attack ()
		{
			animator.SetTrigger ("Attack");
			targetsLocked = true;
			foreach (Agent target in targets)
			{
				if (target != null && !target.dead && target.gameObject.activeSelf)
				{
					target.Hurt (this, damage);
				}
				else
				{
					StartCoroutine (AttemptRemoveTarget (target));
				}
			}
			targetsLocked = false;
		}

		public void Hurt (Agent predator, float damage)
		{
			if (hurtParticle != null)
			{
				hurtParticle.SetActive (true);
			}

			predatorsLocked = true;
			float healthLoss = damage * (1 - defense);

			if (hitText != null && hitHolder != null)
			{
				GameObject newHitText = Instantiate (hitText, hitHolder, false);
				newHitText.GetComponent <TextMesh> ().text = "-" + (int) Mathf.Round (healthLoss);
				StartCoroutine (ShowHitText (newHitText, 0.2f));
			}

			// Debug.Log (name + " : Hurt -" + healthLoss);
			if (!predators.Contains (predator))
			{
				predators.Add (predator);
			}
			animator.SetTrigger ("Hurt");
			health -= healthLoss;
			if (tag == "Ghost")
			{
				StartCoroutine (game.AttemptAddExp ((int) Mathf.Floor (healthLoss/2)));
			}
			if (tag == "Tree")
			{
				if (maxHealth - health > pace * n)
				{
					foreach (Transform child in transform)
					{
						if (child.gameObject.name == "Leaves")
						{
							foreach (Transform grandchild in child)
							{
								if (grandchild.gameObject.activeSelf)
								{
									grandchild.gameObject.SetActive (false);
									n ++;
									break;
								}
							}
						}
					}
				}
			}
			CheckIfDead ();
			predatorsLocked = false;
		}

		public void Die ()
		{
			if (!dead)
			{
				dead = true;
				Stop ();
				animator.SetTrigger ("Die");
				// dieParticle.SetActive (true);
				if (tag == "Tree")
				{
					foreach (Transform child in transform)
					{
						child.gameObject.SetActive (false);
					}
					NotATarget ();
					Destroy (GetComponent <BoxCollider2D> ());
					agentManager.Recycle (gameObject);
					game.EndDay ("Ghost", 2);
					// enabled = false;
				}
				else
				{
					if (gameObject.tag == "Ghost") game.numOfGhostsLeft -- ;
					if (gameObject.tag == "Spirit") game.numOfSpiritsLeft -- ;
					gameObject.tag = "Dead";
					NotATarget ();
					NotAPredator ();
					renderer.enabled = false;
					gameObject.GetComponent <Collider2D> ().enabled = false;
					agentManager.Recycle (gameObject);
					// enabled = false;
				}
				CheckIfEnd ();
			}
		}

		void NotATarget ()
		{
			Debug.Log (GetInstanceID () + " NotATarget");
			foreach (Agent predator in predators)
			{
				if (predator != null && predator.gameObject.activeSelf)
				{
					StartCoroutine (predator.AttemptRemoveTarget (this));
				}
			}
		}

		void NotAPredator ()
		{
			foreach (Agent target in targets)
			{
				if (target != null && target.gameObject.activeSelf)
				{
					StartCoroutine (target.AttemptRemovePredator (this));
				}
			}
		}

		public bool CheckIfDead ()
		{
			if (health <= Mathf.Epsilon)
			{
				Die ();
				return true;
			}
			return false;
		}

		public void CheckIfEnd ()
		{
			if (tag == "Ghost" && game.numOfGhostsLeft <= 0)
			{
				if (GameObject.Find ("Exit") != null) return;
				GameObject exit = new GameObject ("Exit");
				exit.tag = "SpiritExit";
				exit.transform.localPosition = transform.position + Vector3.left * 2;
				exit.transform.localScale = new Vector3 (1, 5, 1);
				exit.AddComponent <BoxCollider2D> ();
				exit.GetComponent <BoxCollider2D> ().isTrigger = true;
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			// Debug.Log (name + " : Meet " + other.gameObject.tag);
			if (tag == "Wall")
			{
				enabled = false;
			}
			else if (tag == "Spirit" && other.gameObject.CompareTag ("Ghost")
			|| tag == "Ghost" && other.gameObject.CompareTag ("Spirit")
			|| tag == "Ghost" && other.gameObject.CompareTag ("Tree"))
			{
				if (targets.Count < num)
				{
					Agent target = other.gameObject.GetComponent <Agent> ();
					// skip butterfly for now
					if (target == null || target.name == "Butterfly Spirit" && name != "Snow Man Ghost")
					{
						return;
					}
					if (target != null && enabled)
					{
						StartCoroutine (AttemptAddTarget (target));
					}
				}
			}
			else if (tag == "Spirit" && other.gameObject.CompareTag ("SpiritExit")
			|| tag == "Ghost" && other.gameObject.CompareTag ("GhostExit"))
			{
				game.EndDay (tag, 2);
				enabled = false;
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (tag == "Spirit" && other.gameObject.CompareTag ("Ghost")
			|| tag == "Ghost" && other.gameObject.CompareTag ("Spirit")
			|| tag == "Ghost" && other.gameObject.CompareTag ("Tree"))
			{
				Agent target = other.gameObject.GetComponent <Agent> ();
				if (target)
				{
					StartCoroutine (AttemptRemoveTarget (target));
				}
			}
		}

		public IEnumerator AttemptAddTarget (Agent target)
		{
			if (!enabled || !gameObject.activeSelf)
			{
				yield return null;
			}
			if (!target.enabled || !target.gameObject.activeSelf)
			{
				yield return null;
			}
			while (targetsLocked)
			{
				yield return new WaitForSeconds (0.05f);
			}
			// Debug.Log (name + " : Add to attack list");
			if (!targets.Contains (target))
			{
				targets.Add (target);
			}
			if (targets.Count > 0)
			{
				attack = true;
			}
		}

		public IEnumerator AttemptRemoveTarget (Agent target)
		{
			if (!enabled || !gameObject.activeSelf)
			{
				yield return null;
			}
			while (targetsLocked)
			{
				yield return new WaitForSeconds (0.05f);
			}
			// Debug.Log (name + " : Remove from attack list");
			targets.Remove (target);
			if (targets.Count <= 0)
			{
				attack = false;
			}
		}

		public IEnumerator AttemptRemovePredator (Agent predator)
		{
			if (!enabled || !gameObject.activeSelf)
			{
				yield return null;
			}
			while (predatorsLocked)
			{
				yield return new WaitForSeconds (0.05f);
			}
			predators.Remove (predator);
		}

		IEnumerator ShowHitText (GameObject newHitText, float delay)
		{
			float time = 0;
			while (time < delay)
			{
				newHitText.transform.position += Vector3.up * Time.deltaTime;
				time += Time.deltaTime;
				yield return new WaitForSeconds (Time.deltaTime);
			}
			Destroy (newHitText);
		}

		public void ExportToMeta (ref List<AgentMeta> agentMeta, int i)
		{
			AgentMeta meta = agentMeta[i];
			meta.tag = tag;
			meta.name = name;
			meta.damage = damage;
			meta.damageTime = damageTime;
			meta.num = num;
			meta.speed = speed;
			meta.health = health;
			meta.defense = defense;
			meta.level = level;
			meta.cost = cost;
			meta.gameObject = gameObject;
		}

		public void ExportToMeta (ref AgentMeta agentMeta)
		{
			AgentMeta meta = agentMeta;
			meta.tag = tag;
			meta.name = name;
			meta.damage = damage;
			meta.damageTime = damageTime;
			meta.num = num;
			meta.speed = speed;
			meta.health = health;
			meta.defense = defense;
			meta.level = level;
			meta.cost = cost;
			meta.gameObject = gameObject;
		}

		void OnDisable ()
		{
			// gameObject.SetActive (false);
		}

		IEnumerator DisableDelay (float delay)
		{
			yield return new WaitForSeconds (delay);
			enabled = false;
		}
	}
}
