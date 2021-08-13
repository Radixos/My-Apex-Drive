using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo
{
    public LevelInfo Level  { get; private set; }
    public Player Winner { get; private set; }

    public GameInfo(LevelInfo level)
    {
        Level = level;
    }

    public void SubmitWinner(Player winner)
    {
        Winner = winner;
    }
}
