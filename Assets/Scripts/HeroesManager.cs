using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroesManager : MonoBehaviour {
    public static HeroesManager singleton;

    public bool debugMode;
    public bool fightDebugMode;
    public int matchMakingCount;
//    public int matchCount;
    public int heroCount=1;
    public int seed;
    public int numHeroes;
    public Hero[] heroes;

    void Awake()
    {
        singleton = this;
        Random.InitState(seed);
        heroes = new Hero[numHeroes];
        for (int i = 0; i < numHeroes; i++)
        {
            heroes[i] = new Hero(i);
            heroes[i].ToString();
        }
        UpdateMatch();
    }

	// Use this for initialization
	void Start () {
        
	}
	
    void Update()
    {
        int num = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
        num = Input.GetKeyDown(KeyCode.LeftControl) ? 10 : num;
        num = Input.GetKeyDown(KeyCode.RightControl) ? 100 : num;

        if (num>0 && heroes.Length>1)
        {
            for (int i = 0; i < num; i++)
            {
                MatchMaking();
            }
            PrintAll();
            GuiManager.singleton.Refresh();
        }
    }

    public bool UpdateMatch()
    {
        bool updated = false;
        int k = CalculateUpdateMatch() - matchMakingCount;
        for (int i = 0; i < k && !fightDebugMode; i++)
        {
            MatchMaking();
            updated = true;
        }
        return updated;
    }

    public int CalculateUpdateMatch()
    {
        //return (System.DateTime.Today.Year-2017)*365*24 + (System.DateTime.Today.DayOfYear - 286)*24 + (System.DateTime.Now.Hour-19);
        return (System.DateTime.Today.DayOfYear - 286)*24*60 + (System.DateTime.Now.Hour-21)*60 + (System.DateTime.Now.Minute-0);
    }

    void PrintAll()
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            heroes[i].ToString();
        }
    }

    void Fight(Hero hero1,Hero hero2)
    {
//        matchCount++;
        Hero[] hero = new Hero[2];
        hero[0] = hero1;
        hero[1] = hero2;

        int[] damagedealt = new int[2];

        //step 1
        for (int i = 0; i < 10*2; i++)
        {
            int j = i % 2;
            int k = (j + 1) % 2;
            if (Random.Range(hero[j].precisionMin, hero[j].precisionMax) - Random.Range(hero[k].dodgeMin, hero[k].dodgeMax) > 0)
            {
                damagedealt[j] += Random.Range(Mathf.FloorToInt(hero[j].forceMin), hero[j].forceMax);
                if (fightDebugMode)
                    Debug.Log(hero[j].id+" deal damage, damage amount "+damagedealt[i%2]);
            }
            else
            {
                if (fightDebugMode)
                    Debug.Log(hero[j].id+" miss");
            }
        }
        if(fightDebugMode)
            Debug.Log("___ "+damagedealt[0]+" <-> "+damagedealt[1]+" ___");

        if (damagedealt[0] > damagedealt[1])
        {
            if(debugMode)
                Debug.Log(hero[0].id+hero[0].rank + " win the match against " + hero[1].id+hero[1].rank);
            hero[0].Win();
            hero[1].Lose();
        }
        else if (damagedealt[1] > damagedealt[0])
        {
            if(debugMode)
                Debug.Log(hero[1].id+hero[1].rank + " win the match against " + hero[0].id+hero[0].rank);
            hero[1].Win();
            hero[0].Lose();
        }

        if (damagedealt[0] > Random.Range(hero[1].constitutionMin*3,hero[1].constitutionMax*3)+20)
        {
            if(debugMode)
                Debug.LogWarning("hero "+hero[1].id + " death");
            heroes[hero[1].arrayPos] = new Hero(hero[1].arrayPos);
        }
        if (damagedealt[1] > Random.Range(hero[0].constitutionMin*3,hero[0].constitutionMax*3)+20)
        {
            if(debugMode)
                Debug.LogWarning("hero "+hero[0].id + " death");
            heroes[hero[0].arrayPos] = new Hero(hero[0].arrayPos);
        }
    }

    public Hero SearchHero(int id)
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i].id == id)
                return heroes[i];
        }
        return null;
    }

    public void Shuffle(Hero[,] heroes) 
    {
        Hero tmp;
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < heroes.GetLength(1); i++) {
                int rnd = Random.Range(0, heroes.GetLength(1));
                tmp = heroes[j,rnd];
                heroes[j,rnd] = heroes[j,i];
                heroes[j,i] = tmp;
            }
        }
    }

    public void MatchMaking()
    {
        Hero[,] tmpHeroes = new Hero[5,heroes.Length];
        int[] index = new int[5];

        for (int i = 0; i < heroes.Length; i++)
        {
            switch (heroes[i].rank)
            {
                case "S":
                    tmpHeroes[4,index[4]] = heroes[i];
                    index[4]++;
                    break;
                case "A":
                    tmpHeroes[3,index[3]] = heroes[i];
                    index[3]++;
                    break;
                case "B":
                    tmpHeroes[2,index[2]] = heroes[i];
                    index[2]++;
                    break;
                case "C":
                    tmpHeroes[1,index[1]] = heroes[i];
                    index[1]++;
                    break;
                case "D":
                    tmpHeroes[0,index[0]] = heroes[i];
                    index[0]++;
                    break;
            }
        }
        Shuffle(tmpHeroes);
        for (int j = 0; j < 5; j++)
        {
            Hero fighter1=null;
            for (int i = 0; i < heroes.Length; i++)
            {
                if (tmpHeroes[j,i] != null)
                {
                    if (fighter1 == null)
                        fighter1 = tmpHeroes[j,i];
                    else
                    {
                        Fight(fighter1, tmpHeroes[j,i]);
                        fighter1 = null;
                    }
                }
            }
        }
        if(debugMode)
            Debug.Log("-------------------- match:" + matchMakingCount + " end");
        matchMakingCount++;
    }
}

[System.Serializable]
public class Hero {
    //const
    int highMinValue=6;
    int highMaxValue=9+1;
    int lowMinValue=2;
    int lowMaxValue=4+1;
    //attribute
    public int constitutionMin;
    public int constitutionMax;
    public int forceMin;
    public int forceMax;

    public int precisionMin;
    public int precisionMax;
    public int dodgeMin;
    public int dodgeMax;

    //values
    public int id;
    public int arrayPos;
    public string rank;
    public int valueHero;
    public int winCount;
    public int loseCount;

    //power

    public Hero (int pos)
    {
        constitutionMin = Random.Range(lowMinValue, lowMaxValue);
        constitutionMax = Random.Range(highMinValue, highMaxValue);
        forceMin = Random.Range(lowMinValue, lowMaxValue);
        forceMax = Random.Range(highMinValue, highMaxValue);

        precisionMin = Random.Range(lowMinValue, lowMaxValue);
        precisionMax = Random.Range(highMinValue, highMaxValue);
        dodgeMin = Random.Range(lowMinValue, lowMaxValue);
        dodgeMax = Random.Range(highMinValue, highMaxValue);

        id = HeroesManager.singleton.heroCount++;
        arrayPos = pos;

        constitutionMin = constitutionMin > constitutionMax ? constitutionMax-1 : constitutionMin;
        forceMin = forceMin > forceMax ? forceMax-1 : forceMin;
        precisionMin = precisionMin > precisionMax ? precisionMax-1 : precisionMin;
        dodgeMin = dodgeMin > dodgeMax ? dodgeMax-1 : dodgeMin;

        UpdateRank();
    }

    public void Win()
    {
        winCount++;
        UpdateRank();
    }

    public void Lose()
    {
        loseCount++;
        UpdateRank();
    }

    public void UpdateRank()
    {
        if (winCount - loseCount < 2 && rank!="C")
        {
            rank = "D";
            valueHero = 5;
        }
        else if (winCount - loseCount < 4 && rank!="B")
        {
            rank = "C";
            valueHero = 7;
        }
        else if (winCount - loseCount < 6 && rank!="A")
        {
            rank = "B";
            valueHero = 10;
        }
        else if (winCount - loseCount < 10 && rank!="S")
        {
            rank = "A";
            valueHero = 15;
        }
        else
        {
            rank = "S";
            valueHero = 25 + (winCount - loseCount-9)*2;
        }
    }

    public string ToString(bool stamp=false)
    {
        string tmp = "id=" + id.ToString() +
                     ", win=" + winCount.ToString() +
                     ", lose=" + loseCount.ToString() +
                     ", constitution=" + constitutionMin + "-" + constitutionMax +
                     ", force=" + forceMin + "-" + forceMax +
                     ", precision=" + precisionMin + "-" + precisionMax +
                     ", dodge=" + dodgeMin + "-" + dodgeMax;
        if (stamp)
            Debug.Log(tmp);
        return tmp;
    }
}