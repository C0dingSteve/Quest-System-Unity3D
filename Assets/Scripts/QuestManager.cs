using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Transform questsContent;
    [SerializeField] private GameObject questHolder;

    public List<Quest> currentQuests;

    private void Awake()
    {
        foreach (var quest in currentQuests)
        {
            quest.InitObjectives();
            quest.questCompleted.AddListener(OnQuestCompleted);
            
            GameObject questObj = Instantiate(questPrefab, questsContent);

            //questObj.transform.Find("Icon").GetComponent<Image>().sprite = quest.information.icon;
            questObj.transform.Find("Title").GetComponent<Text>().text = quest.information.title;
            questObj.transform.Find("RewardSection").Find("CoinAmount").GetComponent<Text>().text = quest.statBonus.gold.ToString();
            questObj.transform.Find("RewardSection").Find("XPAmount").GetComponent<Text>().text = quest.statBonus.xp.ToString();
            //questObj.GetComponent<Button>().onClick.AddListener(delegate
            questObj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                questHolder.GetComponent<QuestWindow>().Initialize(quest);
                questHolder.SetActive(true);
            });
        }
        //Initialize QuestWindow with first quest
        questHolder.GetComponent<QuestWindow>().Initialize(currentQuests[0]);
        questHolder.SetActive(true);
    }

    public void Build(string buildingName)
    {
        EventManager.Instance.QueueEvent(new EnemyKillGameEvent(buildingName));
    }

    private void OnQuestCompleted(Quest quest)
    {
        questsContent.GetChild(currentQuests.IndexOf(quest)).Find("GoButton").gameObject.SetActive(true);
    }
}
