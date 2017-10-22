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
    [HideInInspector]
    public bool showOnlyMyHeroes;

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
        HeroesManager.singleton.UpdateMatch();

        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        for (int j = 4; j >= 0; j--)
        {
            for (int i = 0; i < HeroesManager.singleton.heroes.Length; i++)
            {
                Hero tmpHero = HeroesManager.singleton.heroes[i];
                if ((int)tmpHero.rank != j || (showOnlyMyHeroes && !PlayerManager.singleton.myHeroes.Contains(tmpHero)))
                    continue;
                if (showOnlyMyHeroes)
                    GetComponent<RectTransform>().localPosition = new Vector3(0,-100000,0);
                GameObject go = Instantiate(prefabHero, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(transform);


                go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = tmpHero.id.ToString()+tmpHero.rank.ToString() +" ("+tmpHero.valueHero+") ["+tmpHero.Avg().ToString("F1")+"]";
                go.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "const.:"+tmpHero.constitution+"\nprecis.:"+tmpHero.precision+"\nchar.:"+tmpHero.charisma+"\nwin:"+tmpHero.winCount;
                go.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "force.:"+tmpHero.force+"\ndodge.:"+tmpHero.dodge+"\nagility:"+tmpHero.agility+"\nlose:"+tmpHero.loseCount;
                go.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = tmpHero.killCount>0 ? "kill: "+tmpHero.killCount : "";

                switch (tmpHero.rank)
                {
                    case HeroesManager.Rank.S:
                        go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.blue;
                        break;
                    case HeroesManager.Rank.A:
                        go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.red;
                        break;
                    case HeroesManager.Rank.B:
                        go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.green;
                        break;
                    case HeroesManager.Rank.C:
                        go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().color = Color.cyan;
                        break;
                }

                go.GetComponent<ButtonHero>().id = tmpHero.id;
            }
        }
    }

    public void ToggleShowOnlyMyHero()
    {
        showOnlyMyHeroes = !showOnlyMyHeroes;
        Refresh();
    }
}
