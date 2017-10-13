using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHero : MonoBehaviour {
    public int id;

    public void Start()
    {
        Hero hero = HeroesManager.singleton.SearchHero(id);
        if(PlayerManager.singleton.myHeroes.Contains(hero))
            GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        else if(hero.winCount+hero.loseCount<=2)
            GetComponent<UnityEngine.UI.Image>().color = Color.white;

        foreach (UnityEngine.UI.Text child in transform.GetComponentsInChildren<UnityEngine.UI.Text>())
        {
            child.fontSize =  Screen.height/(638/14);
        }
    }

    public void BuySellHero()
    {
        PlayerManager.singleton.BuySellHero(id);
        GuiManager.singleton.Refresh();
    }
}
