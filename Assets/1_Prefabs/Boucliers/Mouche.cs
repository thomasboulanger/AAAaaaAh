using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.ShaderData;

public class Mouche : MonoBehaviour
{
    [SerializeField] private float endLevel = 120f;
    [SerializeField] private float spawnFliesBegining = 30f;
    private float playerAdvancement;

    public Transform[] limbControllerList = new Transform[4];
    public Transform[] bouclierList = new Transform[4];
    public float spawnRadius = 10f;
    private Vector2 _spawnPosition = new Vector2(5f, 5f);
    private int j = 0;
    //public MoucheAMerde moucheAMerde;
    public MMoucheAMerde moucheAMerde;
    public float intervalle;
    private float _timerLimit;
    private float _timer;
    private float _randomX;
    private float _randomY;
    public Transform player;
    private Rigidbody _playerRB;

    private bool _firstTimeCrossedLimitSpawning = false;
    private bool _spawnFlies = false;
    private bool _playerState = false;
    private string _test;
    // Start is called before the first frame update
    void Start()
    {
        //_timerLimit = 1;
        _playerRB = player.GetComponent<Rigidbody>();
        Application.targetFrameRate = 240;
    }
    public void PlayerState(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;
        if ((int)data1 != 6)
        {
            Debug.Log(data1);
            return;
        }
        _playerState = true;

        if (_playerState == true)
        {
            switch (GameManager.CurrentDifficulty)
            {
                case GameManager.Difficulty.Nofly:
                    _spawnFlies = false;
                    _test = "No flies";
                    break;
                case GameManager.Difficulty.PeacefulFlies:
                    intervalle = 0.25f;
                    Physics.IgnoreLayerCollision(0, 0, true);
                    _spawnFlies = true;
                    _test = "Peaceful flies";

                    break;
                case GameManager.Difficulty.AgressiveFliesNoFruitLoss:
                    intervalle = 1.25f;
                    Physics.IgnoreLayerCollision(0, 0, false);
                    _spawnFlies = true;
                    _test = "Agressive flies";

                    break;
                case GameManager.Difficulty.AgressiveFliesFruitLoss:
                    intervalle = 1.25f;
                    Physics.IgnoreLayerCollision(0, 0, false);
                    _spawnFlies = true;
                    _test = "fruit loss";

                    break;
                case GameManager.Difficulty.Ganged:
                    intervalle = 0.5f;
                    Physics.IgnoreLayerCollision(0, 0, false);
                    _spawnFlies = true;
                    _test = "Gange";

                    break;
            }
            Debug.Log(_test);
        }
    }
    void Update()
    {

        if (_spawnFlies == true)
        {
            if (_firstTimeCrossedLimitSpawning == true)
            {
                _firstTimeCrossedLimitSpawning = false;
                _timerLimit = 1;
            }

            transform.position = limbControllerList[j].position;
            transform.eulerAngles = limbControllerList[j].eulerAngles + new Vector3(0, 0, 0);
            _timer += Time.deltaTime;
            //if (_timer > _timerLimit && player.position.x > spawnFliesBegining)
            if (Input.GetKeyDown(KeyCode.P))
            {
                _randomX = Random.Range(-_spawnPosition.x, _spawnPosition.x);
                _randomY = Random.Range(-_spawnPosition.y, _spawnPosition.y);
                //MoucheAMerde moucheAMerdePrefab = Instantiate(moucheAMerde, transform.position + spawnRadius * Vector3.Normalize(new Vector3(_randomX, _randomY, 0)), Quaternion.identity);
                MMoucheAMerde moucheAMerdePrefab = Instantiate(moucheAMerde, transform.position + spawnRadius * Vector3.Normalize(new Vector3(_randomX, _randomY, 0)), Quaternion.identity);
                moucheAMerdePrefab.body = player;
                moucheAMerdePrefab.bodyRB = _playerRB;
                _timer = 0f;
                //_timerLimit = intervalle;
                Debug.Log(intervalle * endLevel / player.position.x + "   " + endLevel);
                _timerLimit = Random.Range(0f, intervalle * endLevel / player.position.x);

            }
        }
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, 0.5f);
    //}
}
