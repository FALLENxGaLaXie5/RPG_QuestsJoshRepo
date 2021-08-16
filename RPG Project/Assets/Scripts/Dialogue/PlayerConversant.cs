using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        bool isChoosing = false;

        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentConversant = null;
            currentNode = null;
            isChoosing = false;
            onConversationUpdated?.Invoke(); 
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing() => isChoosing;

        public string GetText()
        {
            if (!currentNode) return "";
            return currentNode.Text;
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing) return playerName;
            else return currentConversant.AIName;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated?.Invoke();
                return;
            }

            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            TriggerExitAction();
            currentNode = children[UnityEngine.Random.Range(0, children.Count())];
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
        }

        public bool HasNext() => currentDialogue.GetAllChildren(currentNode).Count() > 0;

        void TriggerEnterAction()
        {
            if (!currentNode) return;
            TriggerAction(currentNode.OnEnterAction);
        }

        void TriggerExitAction()
        {
            if (!currentNode) return;
            TriggerAction(currentNode.OnExitAction);
        }

        void TriggerAction(string action)
        {
            if (action == "" || !currentConversant) return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}