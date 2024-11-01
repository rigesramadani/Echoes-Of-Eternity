using System.Collections.Generic;
using DS;
using DS.Data;
using DS.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {
    [SerializeField] private List<Button> choicesButtons;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private GameObject panel;
    private DSDialogue dialogue;
    private bool hasDialogueStarted;
    
    private void Update() {
        if (hasDialogueStarted) {
            Show();
            CheckDialogueInput();
        } else {
            Hide();
        }
    }
    
    private void Show() {
        panel.gameObject.SetActive(true);
    }
    
    private void Hide() {
        panel.gameObject.SetActive(false);
    }

    public void StartDialogue(string characterName, DSDialogue dialogue) {
        hasDialogueStarted = true;
        this.dialogue = dialogue;
        this.characterName.text = characterName;
        DSDialogueSO currentDialogue = dialogue.GetDialogue();
        dialogueText.text = currentDialogue.dialogueText;
        
        for (int i = 0; i < choicesButtons.Count; i++) {
            if (i < currentDialogue.choices.Count) {
                choicesButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI choiceText = choicesButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                choiceText.text = currentDialogue.choices[i].text;
            } else {
                choicesButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public bool GetHasDialogueStarted() {
        return hasDialogueStarted;
    }
    
    private void CheckDialogueInput() {
        for (int i = 0; i < dialogue.GetDialogue().choices.Count; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                OnChoiceSelected(i);
                return;
            }
        }
    }
    
    private void OnChoiceSelected(int choiceIndex) {
        DSDialogueChoiceData selectedChoice = dialogue.GetDialogue().choices[choiceIndex];
    }
}
