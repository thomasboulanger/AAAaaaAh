using TMPro;
using UnityEngine;

public class ChangeScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text fruitScore;
    [SerializeField] private TMP_Text timeScore;
    [SerializeField] private TMP_Text totalScore;

    private void Start()
    {
        fruitScore.text = ScoreCounting._fruitScore.ToString();
        timeScore.text = ScoreCounting._timeScore.ToString();
        totalScore.text = ScoreCounting._score.ToString();
    }
}
