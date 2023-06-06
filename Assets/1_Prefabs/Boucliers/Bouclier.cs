using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bouclier : MonoBehaviour
{
    public Transform[] limbControllerList = new Transform[4];
    public Transform[] bouclierList = new Transform[4];
    private int j = 0;
    public Projectile projectile;
    public Roquette roquette;
    public float intervalle;
    public float intervalleRoquette;
    private float _timerLimitRoquette;
    private float _timerLimit;
    private float _timerRoquette;
    private float _timer;
    private float _randomX;
    private float _randomXRoquette;
    private float _randomY;
    private float _randomYRoquette;
    public Transform player;
    private Rigidbody _playerRB;
    // Start is called before the first frame update
    void Start()
    {
        _timerLimit = 1;
        _timerLimitRoquette = 1;
        _playerRB = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) 
        {
            j += 1;
        }
        if (j > 3)
        {
            j = 0;
        }
        transform.position = limbControllerList[j].position;
        transform.eulerAngles = limbControllerList[j].eulerAngles + new Vector3(0,0,0);
        _timer += Time.deltaTime;
        _timerRoquette += Time.deltaTime;
        if (_timer > _timerLimit)
        {
            _randomX = Random.Range(-10f, 10f);
            _randomY = Random.Range(-10f, 10f);
            Projectile projectilePrefab = Instantiate(projectile,transform.position + 10 * Vector3.Normalize(new Vector3(_randomX, _randomY, 0)), Quaternion.identity);
            projectilePrefab.body = player;
            projectilePrefab.bodyRB = _playerRB;
            _timer = 0f;
            _timerLimit = Random.Range(0f, intervalle);

        }

        if (_timerRoquette > _timerLimitRoquette)
        {
            _randomXRoquette = Random.Range(-10f, 10f);
            _randomYRoquette = Random.Range(-10f, 10f);
            Roquette roquettePrefab = Instantiate(roquette, transform.position + 10 * Vector3.Normalize(new Vector3(_randomXRoquette, _randomYRoquette, 0)), Quaternion.identity);
            roquettePrefab.body = player;
            roquettePrefab.bodyRB = _playerRB;
            roquettePrefab.bouclierList = bouclierList;
            _timerRoquette = 0f;
            _timerLimitRoquette = Random.Range(intervalleRoquette/5, intervalleRoquette);

        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, 0.5f);
    //}
}
