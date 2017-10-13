using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHero : MonoBehaviour {
    public int id;

    public void Start()
    {
        if(PlayerManager.singleton.myHeroes.Contains(HeroesManager.heroesManager.SearchHero(id)))
            GetComponent<UnityEngine.UI.Image>().color = Color.grey;
    }

    public void BuySellHero()
    {
        PlayerManager.singleton.BuySellHero(id);
        GuiManager.singleton.Refresh();
    }
}
