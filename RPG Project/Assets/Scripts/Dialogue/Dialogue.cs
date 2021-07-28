using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] Vector2 newNodeOffset = new Vector2(50, 0);
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();


        void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode child in GetAllChildren(parentNode))
            {
                if (child.IsPlayerSpeaking) yield return child;                
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode child in GetAllChildren(parentNode))
            {
                if (!child.IsPlayerSpeaking) yield return child;
            }
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.Children)
            {
                if(nodeLookup.ContainsKey(childID)) yield return nodeLookup[childID];
            }            
        }

        

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }        

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
        
        DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            //protect againast if parent is null
            if (parent)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking);
                newNode.SetPosition(parent.Rect.position + newNodeOffset);
            }

            return newNode;
        }

        void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public bool ContainsChild(DialogueNode parentNode, DialogueNode nodeToCheck)
        {
            foreach (string id in parentNode.Children)
            {
                if (id == nodeToCheck.name) return true;
            }
            return false;
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count <= 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }


            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}
