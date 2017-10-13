using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager singleton;
    public int cash;

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

        for (int i = 0; i < HeroesManager.heroesManager.heroes.Length; i++)
        {
            if (PlayerPrefs.GetInt(HeroesManager.heroesManager.heroes[i].id.ToString()) == 1)
                myHeroes.Add(HeroesManager.heroesManager.heroes[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BuySellHero(int idHero)
    {
        Hero tmpHero = HeroesManager.heroesManager.SearchHero(idHero);
        int valueHero = tmpHero.valueHero;
        if (myHeroes.Contains(tmpHero))
        {
            cash += valueHero-1;//sell hero
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
    }
}
