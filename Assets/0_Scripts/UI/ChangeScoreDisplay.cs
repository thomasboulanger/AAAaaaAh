using TMPro;
using UnityEngine;

public class ChangeScoreDisplay : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private TMP_Text fruitScore;   
    [SerializeField] private TMP_Text timeScore;
    [SerializeField] private TMP_Text totalScore;

    private float _fruitScore;
    private float _timeScore;
    private float _score;

    public void onDisplayScore(Component sender, object fruitScore, object timeScore, object score)
    {
        _fruitScore = (float)fruitScore;
        _timeScore = (float)timeScore;
        _score = (float)score;

        parent.SetActive(true);
        this.fruitScore.text = ((int)_fruitScore).ToString();
        this.timeScore.text = ((int)_timeScore).ToString();
        totalScore.text = ((int)_score).ToString();
    }
}
