public class KillObjective : Quest.QuestObjective
{
    public string EnemyType;

    public override string GetDescription()
    {
        return $"Kill {EnemyType}";
    }

    public override void Initialize()
    {
        base.Initialize();
        if(EventManager.Instance) EventManager.Instance.AddListener<EnemyKillGameEvent>(OnBuilding);
    }

    private void OnBuilding(EnemyKillGameEvent eventInfo)
    {
        if (eventInfo.enemyType == "Bee")
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
