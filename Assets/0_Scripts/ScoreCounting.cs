using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounting : MonoBehaviour
{
    [SerializeField] private float averageTime = 480f;
    [SerializeField] private float scorePerFruit = 1000f;
    [SerializeField] private int maxFruits = 37;
    private float _scorePerSecond;
    private float _timePerfruit;
    private float _completionTime;
    private float _score;
    private float _collectedFruits;
    private float _timeDifferential;
    private bool _gameOver = false;
    private bool _scoreGenerated = false;
    private float _maxScore;
    private float _fruitScore;
    private float _timeScore;
    private float _startTime;


    [SerializeField] private GameEvent onScoreGenerated;
    public void PlayerState(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;
        if ((int)data1 != 6) return;
        _startTime = Time.time;
        Debug.Log("playerState");
       //6
        //onchangepanel
    }//début du 8

        public void PlayerReachedEnd(Component sender, object data1, object unUsed1, object unUsed2)
    {
        _collectedFruits = GetComponent<PoubelleVisualManager>().storedFruits.Count;
        Debug.Log("player reached end");

    }
    public void NoFruitsRemainig(Component sender, object data1, object unUsed1, object unUsed2)
    {
        
        _completionTime = Time.time - _startTime;
        _timeDifferential = averageTime - _completionTime;
        _timePerfruit = averageTime / maxFruits;
        _scorePerSecond = scorePerFruit / _timePerfruit;
        if (_timeDifferential < 0)
        {
            //_score = _collectedFruits * scorePerFruit / (_completionTime/averageTime);
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

        if (_score > _maxScore)
        {
            _maxScore = _score;
        }
        Debug.Log("NofruitsRemaining " + "score = " + _score);

    }



}
