using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        //_timerLimit = 1;
        _playerRB = player.GetComponent<Rigidbody>();
        Application.targetFrameRate = 240;
    }

    void Update()
    {
        if (_firstTimeCrossedLimitSpawning == true)
        {
            _firstTimeCrossedLimitSpawning = false;
            _timerLimit = 1;
        }

        transform.position = limbControllerList[j].position;
        transform.eulerAngles = limbControllerList[j].eulerAngles + new Vector3(0, 0, 0);
        _timer += Time.deltaTime;
        if (_timer > _timerLimit && player.position.x > spawnFliesBegining)
        //if (Input.GetKeyDown(KeyCode.W))
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
            _timerLimit = Random.Range(0f, intervalle * endLevel/ player.position.x);

        }
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, 0.5f);
    //}
}
