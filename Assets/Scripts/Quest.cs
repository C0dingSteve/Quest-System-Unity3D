using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Quest : ScriptableObject
{
    [System.Serializable]
    public struct Info
    {
        public string title;
        public Sprite icon;
        public string description;
        public enum Location { Erangel, Miramar, Vikendi, Sanhok, Karakin }
        public Location location;
    }

    [Header("Info")] public Info information;

    [System.Serializable]
    public struct Stat
    {
        public int gold;
        public int xp;
    }
    [Header("Stats")] public Stat statBonus = new Stat() { gold = 10, xp = 10 };
    
    public bool Completed { get; protected set; }
    public QuestCompletedEvent questCompleted;

    // Rewards

    [System.Serializable]
    public struct Reward
    {
        public enum RewardType { Chest, Scroll, Gem }
        public Sprite icon;
        public RewardType type;
        public int quantity;
    }
    [Header("Rewards")] public List<Reward> rewards;

    // OBJECTIVE CLASS
    public abstract class QuestObjective : ScriptableObject
    {
        protected string description;
        public int CurrentAmount { get; protected set; }
        public int requiredAmount = 1;

        public bool Completed { get; protected set; }
        [HideInInspector] public UnityEvent objectiveCompleted;

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual void Initialize()
        {
            Completed = false;
            objectiveCompleted = new UnityEvent();
        }

        protected void Evaluate()
        {
            if (CurrentAmount >= requiredAmount)
            {
                Complete();
            }
        }

        private void Complete()
        {
            Completed = true;
            objectiveCompleted.Invoke();
            objectiveCompleted.RemoveAllListeners();
        }

        public void Skip()
        {
            //charge the player some gold for skipping the quest
            Complete();
        }
    }

    public List<QuestObjective> objectives;
    
    public void InitObjectives()
    {
        Completed = false;
        questCompleted = new QuestCompletedEvent();

        foreach (var objective in objectives)
        {
            objective.Initialize();
            objective.objectiveCompleted.AddListener(delegate { CheckObjectives(); });
        }
    }
    private void CheckObjectives()
    {
        Completed = objectives.All(g => g.Completed);
        if (Completed)
        {
            //give statBonus
            questCompleted.Invoke(this);
            questCompleted.RemoveAllListeners();
        }
    }
}
public class QuestCompletedEvent : UnityEvent<Quest> { }