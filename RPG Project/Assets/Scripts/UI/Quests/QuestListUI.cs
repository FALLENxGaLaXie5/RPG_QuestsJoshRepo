using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestItemUI questPrefab;

        QuestList questList = null;

        void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            if(questList) questList.onUpdate += Redraw;

            Redraw();
        }

        void Redraw()
        {
            transform.DetachChildren();
            foreach (QuestStatus status in questList.Statuses)
            {
                QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
                uiInstance.Setup(status);
            }
        }

    }
}