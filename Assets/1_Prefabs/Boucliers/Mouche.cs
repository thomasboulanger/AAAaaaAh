using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Mouche : MonoBehaviour
{
    public Transform[] limbControllerList = new Transform[4];
    public Transform[] bouclierList = new Transform[4];
    private int j = 0;
    public MoucheAMerde moucheAMerde;
    public float intervalle;
    private float _timerLimit;
    private float _timer;
    private float _randomX;
    private float _randomY;
    public Transform player;
    private Rigidbody _playerRB;
    // Start is called before the first frame update
    void Start()
    {
        _timerLimit = 1;
        _playerRB = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        transform.position = limbControllerList[j].position;
        transform.eulerAngles = limbControllerList[j].eulerAngles + new Vector3(0, 0, 0);
        _timer += Time.deltaTime;
        //if (_timer > _timerLimit)
        if (Input.GetKeyDown(KeyCode.W))
        {
            _randomX = Random.Range(-10f, 10f);
            _randomY = Random.Range(-10f, 10f);
            MoucheAMerde moucheAMerdePrefab = Instantiate(moucheAMerde, transform.position + 10 * Vector3.Normalize(new Vector3(_randomX, _randomY, 0)), Quaternion.identity);
            moucheAMerdePrefab.body = player;
            moucheAMerdePrefab.bodyRB = _playerRB;
            _timer = 0f;
            _timerLimit = intervalle;
            //_timerLimit = Random.Range(0f, intervalle);

        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, 0.5f);
    //}
}
