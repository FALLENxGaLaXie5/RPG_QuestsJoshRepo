using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;
using TMPro;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.Quest;
            title.text = quest.Title;
            objectiveContainer.DetachChildren();
            foreach (string objective in quest.Objectives)
            {
                GameObject prefab = objectiveIncompletePrefab;
                if (status.IsObjectiveComplete(objective))
                {
                    prefab = objectivePrefab;
                }
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective;
            }
        }
    }

}