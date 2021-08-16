using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] bool isPlayerSpeaking = false;
        [SerializeField] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] string onEnterAction;
        [SerializeField] string onExitAction;

        #region Data Accessors
        public string Text => text;

        public List<string> Children => children;


        public Rect Rect => rect;

        public bool IsPlayerSpeaking => isPlayerSpeaking;

        public string OnEnterAction => onEnterAction;

        public string OnExitAction => onExitAction;


        public void SetText(string text)
        {
            if (text != this.text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                this.text = text;
                EditorUtility.SetDirty(this);
            }
        }

        
        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add a Child");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove a Child");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        

#if UNITY_EDITOR
        public void SetPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = position;
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool isPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            this.isPlayerSpeaking = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }


#endif

        #endregion
    }
}