using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Core.UI.Tooltips;
using RPG.Quests;
using RPG.UI.Quests;

public class QuestTooltipSpawner : TooltipSpawner
{
    public override bool CanCreateTooltip() => true;

    public override void UpdateTooltip(GameObject tooltip)
    {
        QuestStatus status = GetComponent<QuestItemUI>().QuestStatus;
        tooltip.GetComponent<QuestTooltipUI>().Setup(status);
    }
}
