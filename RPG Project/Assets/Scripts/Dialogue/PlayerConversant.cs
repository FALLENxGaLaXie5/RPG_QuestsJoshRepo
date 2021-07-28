using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] Dialogue currentDialogue;
        DialogueNode currentNode = null;
        bool isChoosing = false;

        void Awake()
        {
            currentNode = currentDialogue.GetRootNode();
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (!currentNode) return "";
            return currentNode.Text;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                return;
            }

            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            currentNode = children[Random.Range(0, children.Count())];
        }

        public bool HasNext() => currentDialogue.GetAllChildren(currentNode).Count() > 0;
    }
}