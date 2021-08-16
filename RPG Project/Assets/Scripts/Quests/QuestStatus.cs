using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public class QuestStatus
    {
        [SerializeField] Quest quest;
        [SerializeField] List<string> completedObjectives = new List<string>();

        public QuestStatus(Quest quest) => this.quest = quest;

        public Quest Quest => quest;
        public int CompletedCount => completedObjectives.Count;

        public bool IsObjectiveComplete(string objective) => completedObjectives.Contains(objective);

        public void CompleteObjective(string objective)
        {
            if(quest.HasObjective(objective)) completedObjectives.Add(objective);

        }
    }
}