using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text locationText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject objectivePrefab;
    [SerializeField] private Transform objectivePanel;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardPanel;

    public void Initialize(Quest quest)
    {
        titleText.text = quest.information.title;
        locationText.text = Enum.GetName(typeof(Quest.Info.Location), quest.information.location);
        descriptionText.text = quest.information.description;

        // Handle Objectives
        foreach(Transform child in objectivePanel)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var objective in quest.objectives)
        {
            GameObject objectiveObj = Instantiate(objectivePrefab, objectivePanel);
            objectiveObj.transform.Find("Title").GetComponent<Text>().text = objective.GetDescription();

            GameObject countObj = objectiveObj.transform.Find("Count").gameObject;
            GameObject skipObj = objectiveObj.transform.Find("Skip").gameObject;

            if (objective.Completed)
            {
                countObj.SetActive(false);
                skipObj.SetActive(false);
                objectiveObj.transform.Find("Done").gameObject.SetActive(true);
            }
            else
            {
                countObj.GetComponent<Text>().text = objective.CurrentAmount + "/" + objective.requiredAmount;
                
                skipObj.GetComponent<Button>().onClick.AddListener(delegate
                {
                    objective.Skip();
                    
                    countObj.SetActive(false);
                    skipObj.SetActive(false);
                    objectiveObj.transform.Find("Done").gameObject.SetActive(true);
                });
            }
        }
        
        //Handle Rewards
        foreach(Transform child in rewardPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var reward in quest.rewards)
        {
            GameObject rewardObj = Instantiate(rewardPrefab, rewardPanel);
            rewardObj.transform.Find("Icon").GetComponent<Image>().sprite = reward.icon;
            rewardObj.transform.Find("Amount").GetComponent<Text>().text = reward.quantity.ToString();
        }
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < objectivePanel.childCount; i++)
        {
            Destroy(objectivePanel.GetChild(i).gameObject);
        }
    }
}
