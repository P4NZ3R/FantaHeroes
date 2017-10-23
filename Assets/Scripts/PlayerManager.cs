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
        if (PlayerPrefs.GetInt("cash"+(int)HeroesManager.singleton.activeTournament) == 0)
            cash = 11;
        else
            cash = PlayerPrefs.GetInt("cash"+(int)HeroesManager.singleton.activeTournament);

        for (int i = 0; i < HeroesManager.singleton.heroes.Length; i++)
        {
            if (PlayerPrefs.GetInt((int)HeroesManager.singleton.activeTournament + ","+HeroesManager.singleton.heroes[i].id.ToString()) == 1)
                myHeroes.Add(HeroesManager.singleton.heroes[i]);
        }
        Refresh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Refresh()
    {
        if (!text)
            return;
        text.text = "["+(int)HeroesManager.singleton.activeTournament+"] "+"you have: "+(cash-1).ToString()+"$ ("+(cash-1+HeroesValue()).ToString()+")";
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
            PlayerPrefs.SetInt((int)HeroesManager.singleton.activeTournament + "," +idHero,0);
        }
        else
        {
            if (cash > valueHero)
            {
                cash -= valueHero;//buy hero
                myHeroes.Add(tmpHero);
                PlayerPrefs.SetInt((int)HeroesManager.singleton.activeTournament + "," +idHero,1);
            }
        }
        PlayerPrefs.SetInt("cash"+(int)HeroesManager.singleton.activeTournament,cash);
        Refresh();
    }

    public int HeroesValue()
    {
        int value = 0;
        foreach (Hero hero in myHeroes)
        {
            value += hero.valueHero;
        }
        return value;
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("cash"+(int)HeroesManager.singleton.activeTournament, 11);
        foreach (Hero hero in myHeroes)
        {
            PlayerPrefs.SetInt((int)HeroesManager.singleton.activeTournament + "," +hero.id,0);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void OnHeroDie(Hero hero)
    {
        HeroesManager mng = HeroesManager.singleton;
        int heroesDead = (mng.heroCount + 1) - mng.tournaments[mng.activeTournament].numHeroes;
        mng.avgHeroLife = (mng.avgHeroLife * (heroesDead-1) + hero.winCount+hero.loseCount) / heroesDead;
        mng.avgHeroWin = (mng.avgHeroWin * (heroesDead-1) + hero.winCount) / heroesDead;
        mng.avgHeroDeath = (mng.avgHeroDeath * (heroesDead-1) +hero.loseCount) / heroesDead;
        myHeroes.Remove(hero);
        PlayerPrefs.SetInt((int)HeroesManager.singleton.activeTournament + "," +hero.id,0);
    }
}
