using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] string aiName;
        [SerializeField] Dialogue dialogue;

        public string AIName => aiName;


        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!dialogue) return false;

            if (Input.GetMouseButtonDown(0))
            {
                callingController.transform.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
            }
            return true;
        }
    }

}
