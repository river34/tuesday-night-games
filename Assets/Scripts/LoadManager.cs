using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour {

    public GameObject TitlePage;
    public GameObject SelectPage;
    public GameObject LoadPage;
    public GameObject GamePage;
    public GameObject EndPage;
    public GameObject EventSystems;

    Status status;
    string currentGame;
    Resolution defaultResolution;

    enum Status {Title, Select, Game, End};

    void Start ()
    {
        defaultResolution = Screen.currentResolution;
        Init ();
    }

    void Init ()
    {
        status = Status.Title;
    }

    void Update ()
    {
        if (status == Status.Title)
        {
            TitlePage.SetActive (true);
            if (Input.anyKey)
            {
                status = Status.Select;
            }
        }
        if (status == Status.Select)
        {
            TitlePage.SetActive (false);
            SelectPage.SetActive (true);
            GamePage.SetActive (false);
            EventSystems.SetActive (true);
        }
        if (status == Status.Game)
        {
            EventSystems.SetActive (false);
            if (Input.GetKeyDown (KeyCode.Escape))
            {
                if (currentGame != null)
                {
                    UnloadGame (currentGame);
                    status = Status.Select;
                }
            }
        }
        if (status == Status.End)
        {

        }
    }

    public void UnloadGame (string game)
    {
        SceneManager.UnloadSceneAsync (game);
        currentGame = null;
        Screen.SetResolution(defaultResolution.width, defaultResolution.height, Screen.fullScreen);
    }

    public void LoadGame (string game)
    {
        SceneManager.LoadScene (game, LoadSceneMode.Additive);
        status = Status.Game;
        currentGame = game;
        StartCoroutine(Show(LoadPage.GetComponent<Image>(), SelectPage, GamePage));
    }

    public void SetLoadText (string text)
    {
        LoadPage.GetComponentInChildren<Text>().text = text;
    }

    IEnumerator Show (Image load, GameObject hide, GameObject show)
    {
        Color color = load.color;
        load.gameObject.SetActive (true);

        /*
        color.a = 0;
        load.color = color;
        load.gameObject.SetActive (true);

        while (load.color.a < 1 - float.Epsilon)
        {
            color.a += Time.deltaTime * 2;
            load.color = color;
            yield return new WaitForSeconds (Time.deltaTime);
        }
        */

        color.a = 1;
        load.color = color;

        if (hide != null) hide.SetActive (false);

        while (load.color.a > 0.1)
        {
            color.a -= Time.deltaTime;
            load.color = color;
            yield return new WaitForSeconds (Time.deltaTime);
        }
        color.a = 0;
        load.color = color;
        load.gameObject.SetActive (false);

        if (show != null) show.SetActive (true);
    }
}
