using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Dialogue;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] Button nextButton;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;

        PlayerConversant playerConversant;

        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener(Next);

            UpdateUI();
        }
        
        void Next()
        {
            playerConversant.Next();
            UpdateUI();
        }

        void UpdateUI()
        {
            
            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                //Destroy all buttons
                foreach (Transform item in choiceRoot)
                {
                    Destroy(item.gameObject);
                }

                //Create new choice buttons
                foreach (DialogueNode choice in playerConversant.GetChoices())
                {
                    GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                    choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.Text;
                }
            }
            else
            {
                AIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }
    }

}