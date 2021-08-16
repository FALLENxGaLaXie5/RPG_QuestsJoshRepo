using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI progress;

        QuestStatus status;
        public QuestStatus QuestStatus => status;

        public void Setup(QuestStatus status)
        {
            this.status = status;
            title.text = status.Quest.Title;
            progress.text = status.CompletedCount + "/" + status.Quest.ObjectiveCount;
        }
        
    }
}