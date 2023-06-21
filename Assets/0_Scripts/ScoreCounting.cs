using UnityEngine;

public class ScoreCounting : MonoBehaviour
{
    [SerializeField] private GameEvent onDisplayScore;
    [SerializeField] private GameEvent onScoreGenerated;
    
    [SerializeField] private float averageTime = 480f;
    [SerializeField] private float scorePerFruit = 1000f;
    [SerializeField] private int maxFruits = 37;
    private float _scorePerSecond;
    private float _timePerfruit;
    private float _completionTime;
    public static float _score;
    private float _collectedFruits;
    private float _timeDifferential;
    private bool _gameOver = false;
    private bool _scoreGenerated = false;
    private float _maxScore;
    public static float _fruitScore;
    public static float _timeScore;
    private float _startTime;
    private bool _scoreWasGenerated;

    

    public void PlayerState(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;
        if ((int) data1 != 6) return;
        _startTime = Time.time;
    }

    public void PlayerReachedEnd(Component sender, object data1, object unUsed1, object unUsed2) =>
        _collectedFruits = GetComponent<PoubelleVisualManager>().storedFruits.Count;
    
    public void NoFruitsRemainig(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (_scoreWasGenerated) return;
        _completionTime = Time.time - _startTime;
        _timeDifferential = averageTime - _completionTime;
        _timePerfruit = averageTime / maxFruits;
        _scorePerSecond = scorePerFruit / _timePerfruit;
        if (_timeDifferential < 0)
        {
            _score = _collectedFruits * scorePerFruit;
            _timeScore = 0;
        }
        else
        {
            _score = _collectedFruits * scorePerFruit + _timeDifferential * _scorePerSecond;
            _timeScore = _timeDifferential * _scorePerSecond;
        }

        onScoreGenerated.Raise(this, _score, null, null);
        _fruitScore = _collectedFruits * scorePerFruit;

        if (_collectedFruits == 0)
        {
            _fruitScore = 0;
            _timeScore = 0;
            _score = 0;
        }

        if (_score > _maxScore)
            _maxScore = _score;

        //Debug.Log("NofruitsRemaining " + "score = " + _score + "             nbr de fruits : " + _collectedFruits);
        onDisplayScore.Raise(this, _fruitScore,_timeScore,_score);

        _scoreWasGenerated = true;
    }
}