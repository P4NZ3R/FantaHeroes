using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiManager : MonoBehaviour {
    public static GuiManager singleton;
    public GameObject prefabHero;

    public void Awake()
    {
        singleton = this;
    }

    public void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < HeroesManager.heroesManager.heroes.Length; i++)
        {
            GameObject go = Instantiate(prefabHero, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(transform);
            Hero tmpHero = HeroesManager.heroesManager.heroes[i];


            go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = tmpHero.id.ToString()+tmpHero.rank +" ("+tmpHero.valueHero+")";
            go.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "const.:"+tmpHero.constitutionMin+"-"+tmpHero.constitutionMax+"\nprec.:"+tmpHero.precisionMin+"-"+tmpHero.precisionMax+"\nwin:"+tmpHero.winCount;
            go.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "force.:"+tmpHero.forceMin+"-"+tmpHero.forceMax+"\ndodge.:"+tmpHero.dodgeMin+"-"+tmpHero.dodgeMax+"\nlose:"+tmpHero.loseCount;

            switch (tmpHero.rank)
            {
                case "S":
                    go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.blue;
                    break;
                case "A":
                    go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.red;
                    break;
                case "B":
                    go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.green;
                    break;
                case "C":
                    go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.cyan;
                    break;
            }

            go.GetComponent<ButtonHero>().id = tmpHero.id;
        }
    }
}
