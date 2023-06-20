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

    // Update is called once per frame
    void Update()
    {
        if (_gameOver == true && _scoreGenerated == false)
        {
            _scoreGenerated = true;
            _completionTime = Time.time;
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

            _fruitScore = _collectedFruits * scorePerFruit;

            if (_score > _maxScore)
            {
                _maxScore = _score;
            }

        }

        if (_scoreGenerated == true )
        {
            Debug.Log(_score);
        }
    }
}
