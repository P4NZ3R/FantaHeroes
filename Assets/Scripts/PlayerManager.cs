using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager singleton;
    public int cash;
    public UnityEngine.UI.Text text;

    public List<Hero> myHeroes = new List<Hero>();

    void Awake()
    {
        singleton = this;
    }

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetInt("cash") == 0)
            cash = 11;
        else
            cash = PlayerPrefs.GetInt("cash");

        for (int i = 0; i < HeroesManager.singleton.heroes.Length; i++)
        {
            if (PlayerPrefs.GetInt(HeroesManager.singleton.heroes[i].id.ToString()) == 1)
                myHeroes.Add(HeroesManager.singleton.heroes[i]);
        }
        Refresh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Refresh()
    {
        text.text = "you have: "+(cash-1).ToString()+"$";
        text.fontSize = Screen.height / (638 / 14);
    }

    public void BuySellHero(int idHero)
    {
        if (HeroesManager.singleton.UpdateMatch())
            return;
        Hero tmpHero = HeroesManager.singleton.SearchHero(idHero);
        int valueHero = tmpHero.valueHero;
        if (myHeroes.Contains(tmpHero))
        {
            cash += valueHero;//sell hero
            myHeroes.Remove(tmpHero);
            PlayerPrefs.SetInt(idHero.ToString(),0);
        }
        else
        {
            if (cash > valueHero)
            {
                cash -= valueHero;//buy hero
                myHeroes.Add(tmpHero);
                PlayerPrefs.SetInt(idHero.ToString(),1);
            }
        }
        PlayerPrefs.SetInt("cash",cash);
        Refresh();
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("cash", 0);
        foreach (Hero hero in myHeroes)
        {
            PlayerPrefs.SetInt(hero.id.ToString(),0);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
