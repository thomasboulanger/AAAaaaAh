//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using TMPro;
using UnityEngine;

/// <summary>
/// Script that change the difficulty preset
/// </summary>
public class ChangeDifficulty : MonoBehaviour
{
    [SerializeField] private GameEvent onUpdateDifficulty;
    
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text description1;
    [SerializeField] private TMP_Text description2;
    [SerializeField] private TMP_Text description3;

    private int _currentState = 2;

    private void Start() => UpdateDifficulty();
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        _currentState++;
        if (_currentState > 4) //get .count of enum later
            _currentState = 0;
        
        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        GameManager.CurrentDifficulty = (GameManager.Difficulty) _currentState;
        onUpdateDifficulty.Raise(this,null,null,null);
        
        switch (GameManager.CurrentDifficulty)
        {
            case GameManager.Difficulty.Nofly:
                difficultyText.text = "Rookiest";
                description1.text = "Flies: no";
                description2.text = "Flies aggressive what fly?";
                description3.text = "Flies steal fruits what fly?";
                return;
            case GameManager.Difficulty.PeacefulFlies:
                difficultyText.text = "Rookie";
                description1.text = "Flies: yes";
                description2.text = "Flies aggressive no";
                description3.text = "Flies steal fruits no";
                return;
            case GameManager.Difficulty.AgressiveFliesNoFruitLoss:
                difficultyText.text = "Fun";
                description1.text = "Flies: yes";
                description2.text = "Flies aggressive yes";
                description3.text = "Flies steal fruits no";
                return;
            case GameManager.Difficulty.AgressiveFliesFruitLoss:
                difficultyText.text = "Now we talk";
                description1.text = "Flies: yes";
                description2.text = "Flies aggressive yes";
                description3.text = "Flies steal fruits yes";
                return;
            case GameManager.Difficulty.Ganged:
                difficultyText.text = "Well, good luck";
                description1.text = "Flies: lot";
                description2.text = "Flies aggressive yes";
                description3.text = "Flies steal fruits yes";
                return;
        }
    }
}