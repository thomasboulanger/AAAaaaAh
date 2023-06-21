
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
    public MMoucheAMerde moucheAMerde;
    public float intervalle;
    private float _timerLimit = 1;
    private float _timer;
    private float _randomX;
    private float _randomY;
    public Transform player;
    private Rigidbody _playerRB;

    private bool _firstTimeCrossedLimitSpawning = false;
    private bool _spawnFlies = false;
    private bool _playerState = false;
    private string _test;

    void Start()
    {
        //_timerLimit = 1;
        _playerRB = player.GetComponent<Rigidbody>();
        Application.targetFrameRate = 240;
    }
    public void PlayerState(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;

        switch (GameManager.CurrentDifficulty)
        {
            case GameManager.Difficulty.Nofly:
                _spawnFlies = false;
                Physics.IgnoreLayerCollision(16, 3, true);
                Physics.IgnoreLayerCollision(16, 15, true);
                _test = "No flies";
                break;
            case GameManager.Difficulty.PeacefulFlies:
                intervalle = 0.25f;
                Physics.IgnoreLayerCollision(16, 3, true);
                Physics.IgnoreLayerCollision(16, 15, true);
                _spawnFlies = true;
                _test = "Peaceful flies";

                break;
            case GameManager.Difficulty.AgressiveFliesNoFruitLoss:
                intervalle = 1.25f;
                Physics.IgnoreLayerCollision(16, 3, false);
                Physics.IgnoreLayerCollision(0, 15, false);
                _spawnFlies = true;
                _test = "Agressive flies";

                break;
            case GameManager.Difficulty.AgressiveFliesFruitLoss:
                intervalle = 1.25f;
                Physics.IgnoreLayerCollision(16, 3, false);
                Physics.IgnoreLayerCollision(16, 15, false);
                _spawnFlies = true;
                _test = "fruit loss";

                break;
            case GameManager.Difficulty.Ganged:
                intervalle = 0.5f;
                Physics.IgnoreLayerCollision(16, 3, false);
                Physics.IgnoreLayerCollision(16, 15, false);
                _spawnFlies = true;
                _test = "Gange";

                break;
        }
        Debug.Log(_test);
        if ((GameManager.UIStateEnum) data1 is GameManager.UIStateEnum.PlayerHaveReachEndOfLevel)
        {
            _spawnFlies = false;
        }
    }
    void Update()
    {

        if (_spawnFlies == true)
        {

            transform.position = limbControllerList[j].position;
            transform.eulerAngles = limbControllerList[j].eulerAngles + new Vector3(0, 0, 0);
            _timer += Time.deltaTime;

            if (_timer > _timerLimit && player.position.x > spawnFliesBegining || Input.GetKeyDown(KeyCode.P))
            {
                _randomX = Random.Range(-_spawnPosition.x, _spawnPosition.x);
                _randomY = Random.Range(-_spawnPosition.y, _spawnPosition.y);
                //MoucheAMerde moucheAMerdePrefab = Instantiate(moucheAMerde, transform.position + spawnRadius * Vector3.Normalize(new Vector3(_randomX, _randomY, 0)), Quaternion.identity);
                MMoucheAMerde moucheAMerdePrefab = Instantiate(moucheAMerde, transform.position + spawnRadius * Vector3.Normalize(new Vector3(_randomX, _randomY, 0)), Quaternion.identity);
                moucheAMerdePrefab.body = player;
                moucheAMerdePrefab.bodyRB = _playerRB;
                _timer = 0f;
                //_timerLimit = intervalle;
                //Debug.Log(intervalle * endLevel / player.position.x + "   " + endLevel);
                _timerLimit = Random.Range(0f, intervalle * endLevel / player.position.x);
            }
        }
    }
}
