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
//    [HideInInspector]
//    public int seed;
    [HideInInspector]
    public int numHeroes;
    public Hero[] heroes;

    public enum Rank {S,A,B,C,D}
    public enum TournamentType {hour,fiveMin,length}

    [HideInInspector]
    public int activeTournament;
    public Tournament[] tournaments;

    void Awake()
    {
        singleton = this;
        activeTournament = PlayerPrefs.GetInt("activeTournament");
        if (activeTournament > tournaments.Length)
        {
            activeTournament = 0;
            PlayerPrefs.SetInt("activeTournament",0);
        }
        numHeroes = tournaments[activeTournament].numHeroes;
        Random.InitState(activeTournament);
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

    public void ChangeTournament()
    {
        activeTournament = activeTournament + 1 >= tournaments.Length ? 0 : activeTournament + 1;
        PlayerPrefs.SetInt("activeTournament",activeTournament);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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
        switch (tournaments[activeTournament].tournamentType)
        {
            case TournamentType.hour:
                return (System.DateTime.Today.Year - 2017)*365*24 + (System.DateTime.Today.DayOfYear - 286)*24 + (System.DateTime.Now.Hour - 0);
            case TournamentType.fiveMin:
                return (System.DateTime.Today.Year - 2017)*365*24*12 + (System.DateTime.Today.DayOfYear - 289)*24*12 + (System.DateTime.Now.Hour-19)*12 + Mathf.FloorToInt(System.DateTime.Now.Minute/5);
            default:
                Debug.LogError("no activeTournament found");
            break;
        }
        return 0;
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
        for (int i = 0; i < 5*2; i++)
        {
            int j = i % 2;
            int k = (j + 1) % 2;
            if (Random.Range(hero[j].precisionMin, hero[j].precisionMax) - Random.Range(hero[k].dodgeMin, hero[k].dodgeMax) > 0)
            {
                damagedealt[j] += Random.Range(hero[j].forceMin, hero[j].forceMax);
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

        if (damagedealt[0] > Random.Range((hero[1].constitutionMin+5)*5,(hero[1].constitutionMax+5)*5)-Mathf.FloorToInt(hero[1].loseCount*tournaments[activeTournament].loseCountMultiplier))
        {
            if(debugMode)
                Debug.LogWarning("hero "+hero[1].id + " death");
            PlayerManager.singleton.OnHeroDie(hero[1]);
            heroes[hero[1].arrayPos] = new Hero(hero[1].arrayPos);
            hero[0].killCount++;
        }
        if (damagedealt[1] > Random.Range((hero[0].constitutionMin+5)*5,(hero[0].constitutionMax+5)*5)-Mathf.FloorToInt(hero[0].loseCount*tournaments[activeTournament].loseCountMultiplier))
        {
            if(debugMode)
                Debug.LogWarning("hero "+hero[0].id + " death");
            PlayerManager.singleton.OnHeroDie(hero[0]);
            heroes[hero[0].arrayPos] = new Hero(hero[0].arrayPos);
            hero[1].killCount++;
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
                case HeroesManager.Rank.S:
                    tmpHeroes[4,index[4]] = heroes[i];
                    index[4]++;
                    break;
                case HeroesManager.Rank.A:
                    tmpHeroes[3,index[3]] = heroes[i];
                    index[3]++;
                    break;
                case HeroesManager.Rank.B:
                    tmpHeroes[2,index[2]] = heroes[i];
                    index[2]++;
                    break;
                case HeroesManager.Rank.C:
                    tmpHeroes[1,index[1]] = heroes[i];
                    index[1]++;
                    break;
                case HeroesManager.Rank.D:
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
        PlayerManager.singleton.Refresh();
    }
}

[System.Serializable]
public class Hero {
    //const
    int highMinValue=7;
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
    public HeroesManager.Rank rank;
    public int valueHero;
    public int winCount;
    public int loseCount;
    public int killCount;
    public bool isNew;

    public HeroesManager.Rank Rank
    {
        get{ return rank;}
        set
        { 
            isNew = false;
            if (value != rank)
            {
                if (value < rank)
                {
                    UpgradeRandomStat(1, 0);
                    UpgradeRandomStat(1, 0);
                    UpgradeRandomStat(1, 0);
                    UpgradeRandomStat(1, 0);
                    UpgradeRandomStat(1, 0);
                    UpgradeRandomStat(0, 1);
                    rank = value;
                    isNew = true;
                }
            }
            //value hero
            valueHero = Mathf.Max(5+(winCount-loseCount)*(5-(int)rank),5*(5-(int)rank));
        }
    }
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

        rank = HeroesManager.Rank.D;
        UpdateRank();
        isNew = true;
    }

    public void Win()
    {
        winCount++;
        UpdateRank();
//        if((winCount)%5==0)
//            UpgradeRandomStat(1,0);
    }

    public void Lose()
    {
        loseCount++;
        UpdateRank();
        if((loseCount)%5==0)
            UpgradeRandomStat(-1,0);
    }

    public float Avg()
    {
        return (constitutionMin + constitutionMax + forceMin + forceMax + precisionMin + precisionMax + dodgeMin + dodgeMax) / 8f;
    }

    //il controllo per non scendere di rank viene fatto nel set
    public void UpdateRank()
    {
        int winLoseForRankUp = HeroesManager.singleton.tournaments[HeroesManager.singleton.activeTournament].winLoseForRankUp;
        if (winCount - loseCount < winLoseForRankUp*1)
        {
            Rank = HeroesManager.Rank.D;
        }
        else if (winCount - loseCount < winLoseForRankUp*2)
        {
            Rank = HeroesManager.Rank.C;
        }
        else if (winCount - loseCount < winLoseForRankUp*3)
        {
            Rank = HeroesManager.Rank.B;
        }
        else if (winCount - loseCount < winLoseForRankUp*4)
        {
            Rank=HeroesManager.Rank.A;
        }
        else
        {
            Rank=HeroesManager.Rank.S;
        }
    }

    void UpgradeRandomStat(int valueMax=1,int valueMin=0)
    {
        switch(Random.Range(0,4+1))
        {
            case 0:
                constitutionMax += valueMax;
                constitutionMin += valueMin;

                if (constitutionMax < highMinValue)
                    constitutionMax = highMinValue;
                if (constitutionMin > lowMaxValue)
                    constitutionMin = lowMaxValue;
                if (constitutionMin <= 0)
                    constitutionMin = 1;
                break;
            case 1:
                forceMax += valueMax;
                forceMin += valueMin;

                if (forceMax < highMinValue)
                    forceMax = highMinValue;
                if (forceMin > lowMaxValue)
                    forceMin = lowMaxValue;
                if (forceMin <= 0)
                    forceMin = 1;
                break;
            case 2:
                precisionMax += valueMax;
                precisionMin += valueMin;

                if (precisionMax < highMinValue)
                    precisionMax = highMinValue;
                if (precisionMin > lowMaxValue)
                    precisionMin = lowMaxValue;
                if (precisionMin <= 0)
                    precisionMin = 1;
                break;
            case 3:
                dodgeMax += valueMax;
                dodgeMin += valueMin;

                if (dodgeMax < highMinValue)
                    dodgeMax = highMinValue;
                if (dodgeMin > lowMaxValue)
                    dodgeMin = lowMaxValue;
                if (dodgeMin <= 0)
                    dodgeMin = 1;
                break;
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