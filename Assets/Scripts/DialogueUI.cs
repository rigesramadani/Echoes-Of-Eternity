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
    private bool dialogueActive;
    
    private void Update() {
        if (dialogueActive) {
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
        dialogueActive = true;
        this.dialogue = dialogue;
        this.characterName.text = characterName;
        DSDialogueSO currentDialogue = dialogue.GetDialogue();
        dialogueText.text = currentDialogue.dialogueText;
        
        for (int i = 0; i < choicesButtons.Count; i++) {
            if (i < currentDialogue.choices.Count) {
                choicesButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI choiceText = choicesButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                choiceText.text = (i + 1) + ".  " + currentDialogue.choices[i].text;
            } else {
                choicesButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public bool GetHasDialogueStarted() {
        return dialogueActive;
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
        DSDialogueSO nextDialogue = selectedChoice.nextDialogue;
        DSDialogueContainerSO dialogueContainer = dialogue.GetDialogueContainer();
        List<DSDialogueGroupSO> groups = dialogueContainer.GetGroups();

        if (nextDialogue == null) {
            dialogueActive = false;

            foreach (DSDialogueGroupSO group in groups) {
                List<DSDialogueSO> dialogues = dialogueContainer.GetGroupDialogues(group);
                foreach (DSDialogueSO dialogue in dialogues) {
                    if (dialogue.isStartingDialogue) {
                        this.dialogue.SetDSDialogueSO(dialogue);
                        this.dialogue.SetDSDialogueGroupSO(group);
                        return;
                    }
                }
            }
        }

        foreach (DSDialogueGroupSO group in groups) {
            List<DSDialogueSO> dialogues = dialogueContainer.GetGroupDialogues(group);
            if (dialogues.Contains(nextDialogue)) {
                dialogue.SetDSDialogueSO(nextDialogue);
                dialogue.SetDSDialogueGroupSO(group);
                StartDialogue(group.groupName, dialogue);
                return;
            }
        }
    }
}
