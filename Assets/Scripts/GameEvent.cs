using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    public string eventDescription;
}

public class EnemyKillGameEvent : GameEvent
{
    public string enemyType;

    public EnemyKillGameEvent(string type)
    {
        enemyType = type;
    }
}
