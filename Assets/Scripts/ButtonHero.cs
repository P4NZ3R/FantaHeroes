using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHero : MonoBehaviour {
    public int id;

    public void Start()
    {
        if(PlayerManager.singleton.myHeroes.Contains(HeroesManager.singleton.SearchHero(id)))
            GetComponent<UnityEngine.UI.Image>().color = Color.grey;
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
