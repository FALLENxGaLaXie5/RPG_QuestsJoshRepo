using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG Project/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] List<string> objectives = new List<string>();

        public string Title => name;
        public int ObjectiveCount => objectives.Count;
        public IEnumerable<string> Objectives => objectives;

        public bool HasObjective(string objective) => objectives.Contains(objective);
    }

}