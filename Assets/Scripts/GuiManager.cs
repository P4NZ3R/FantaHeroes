using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiManager : MonoBehaviour {
    public static GuiManager singleton;
    public UnityEngine.UI.GridLayoutGroup layout;
    public GameObject prefabHero;
    public int numColumn = 2;
    public int numRowForPages = 6;
    float pages = 3;

    public void Awake()
    {
        singleton = this;
    }

    public void Start()
    {
        pages = HeroesManager.singleton.numHeroes / (numColumn * numRowForPages * 1f);

        layout.cellSize = new Vector2((Screen.width- layout.padding.left-layout.padding.right-layout.spacing.x*(numColumn-1)) / numColumn ,
            (Screen.height*pages- layout.padding.top - layout.padding.bottom-layout.spacing.y*(numRowForPages*pages-1)) / (numRowForPages*pages));

        RectTransform rect = GetComponent<RectTransform>();
        rect.localPosition = rect.anchoredPosition = new Vector2(0, -Screen.height/2 * (pages-1));
        rect.sizeDelta = new Vector2(0, Screen.height * (pages-1));

        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < HeroesManager.singleton.CalculateUpdateMatch()-HeroesManager.singleton.matchMakingCount; i++)
        {
            HeroesManager.singleton.MatchMaking();
        }

        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < HeroesManager.singleton.heroes.Length; i++)
        {
            GameObject go = Instantiate(prefabHero, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(transform);
            Hero tmpHero = HeroesManager.singleton.heroes[i];


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
