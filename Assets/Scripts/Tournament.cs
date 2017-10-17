using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Tournament : ScriptableObject 
{
    public HeroesManager.TournamentType tournamentType;
    public int numHeroes;
    public float loseCountMultiplier = 0.5f;
    public int winLoseForRankUp = 5;
	
}
