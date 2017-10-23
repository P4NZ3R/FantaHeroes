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
    public enum TournamentType {test,hour,fiveMin,length}

    [HideInInspector]
    public int activeTournament;
    public Tournament[] tournaments;

//    [SerializeField]
    public float avgHeroLife;
//    [SerializeField]
    public float avgHeroDeath;
//    [SerializeField]
    public float avgHeroWin;

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
        num = Input.GetKeyDown(KeyCode.Return) ? 1000 : num;

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
                return (System.DateTime.Today.Year - 2017)*365*24 + (System.DateTime.Today.DayOfYear - 297)*24 + (System.DateTime.Now.Hour - 0);
            case TournamentType.fiveMin:
                return (System.DateTime.Today.Year - 2017)*365*24*12 + (System.DateTime.Today.DayOfYear - 297)*24*12 + (System.DateTime.Now.Hour-19)*12 + Mathf.FloorToInt(System.DateTime.Now.Minute/5);
            case TournamentType.test:
                return 0;
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
        int turns = Mathf.Max(hero1.agility,hero2.agility);
        for (int i = 0; i < turns*2; i++)
        {
            int j = i % 2;
            int k = (j + 1) % 2;
            if (hero[j].agility < (i+1) / 2f)
                continue;
            if (Random.Range(0, hero[j].precision) - Random.Range(0, hero[k].dodge) > 0)
            {
                damagedealt[j] += Random.Range(0, hero[j].force);
                if (fightDebugMode)
                    Debug.Log((i+1)+" -> "+ hero[j].id+" deal damage, damage amount "+damagedealt[i%2]);
            }
            else
            {
                if (fightDebugMode)
                    Debug.Log((i+1)+" -> "+hero[j].id+" miss");
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

        if (damagedealt[0] > 10*Random.Range(hero[1].constitution/2,hero[1].constitution) - Mathf.FloorToInt(hero[1].loseCount*tournaments[activeTournament].loseCountMultiplier/hero[1].charisma))
        {
            if(debugMode)
                Debug.LogWarning("hero "+hero[1].id + " death");
            PlayerManager.singleton.OnHeroDie(hero[1]);
            heroes[hero[1].arrayPos] = new Hero(hero[1].arrayPos);
            hero[0].killCount++;
        }
        if (damagedealt[1] > 10*Random.Range(hero[0].constitution/2,hero[0].constitution) - Mathf.FloorToInt(hero[0].loseCount*tournaments[activeTournament].loseCountMultiplier/hero[0].charisma))
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
    int minValue=5;
    int maxValue=9+1;
    //attribute
    public int constitution;
    public int force;

    public int precision;
    public int dodge;

    public int charisma;
    public int agility;

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
                    UpgradeRandomStat(1);
                    UpgradeRandomStat(1);
                    UpgradeRandomStat(1);
                    UpgradeRandomStat(1);
                    UpgradeRandomStat(1);
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
        constitution = Random.Range(minValue, maxValue);
        force = Random.Range(minValue, maxValue);

        precision = Random.Range(minValue, maxValue);
        dodge = Random.Range(minValue, maxValue);

        charisma = Random.Range(minValue, maxValue);
        agility = Random.Range(minValue, maxValue);

        id = HeroesManager.singleton.heroCount++;
        arrayPos = pos;

//        constitution = constitution < minValue ? minValue : constitution;
//        force = force < minValue ? minValue : force;
//        precision = precision < minValue ? minValue : precision;
//        dodge = dodge < minValue ? minValue : dodge;

        rank = HeroesManager.Rank.D;
        UpdateRank();
        isNew = true;
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

    public float Avg()
    {
        return (constitution + force + precision + dodge + agility + charisma) / 6f;
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

    void UpgradeRandomStat(int value=1)
    {
        switch(Random.Range(0,5+1))
        {
            case 0:
                constitution += value + constitution < minValue ? minValue : value;
                break;
            case 1:
                force += value + force < minValue ? minValue : value;
                break;
            case 2:
                precision += value + precision < minValue ? minValue : value;
                break;
            case 3:
                dodge += value + dodge < minValue ? minValue : value;
                break;
            case 4:
                agility += value + agility < minValue ? minValue : value;
                break;
            case 5:
                charisma += value + charisma < minValue ? minValue : value;
                break;
        }
    }

    public string ToString(bool stamp=false)
    {
        string tmp = "id=" + id.ToString() +
                     ", win=" + winCount.ToString() +
                     ", lose=" + loseCount.ToString() +
                     ", constitution=" + constitution +
                     ", force=" + force +
                     ", precision=" + precision +
                     ", dodge=" + dodge +
                        ", charisma=" + charisma +
                        ", agility=" + agility;
        if (stamp)
            Debug.Log(tmp);
        return tmp;
    }
}