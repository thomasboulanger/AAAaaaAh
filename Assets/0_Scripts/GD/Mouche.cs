using UnityEngine;

public class Mouche : MonoBehaviour
{
    public Transform[] limbControllerList = new Transform[4];
    public float spawnRadius = 10f;
    public MMoucheAMerde moucheAMerde;
    public float intervalle;
    public Transform player;

    [SerializeField] private float endLevel = 120f;
    [SerializeField] private float spawnFliesBegining = 30f;

    private float _playerAdvancement;
    private float _timerLimit = 1;
    private float _timer;
    private float _randomX;
    private float _randomY;
    private Vector2 _spawnPosition = new(5f, 5f);
    private Rigidbody _playerRb;
    private bool _firstTimeCrossedLimitSpawning;
    private bool _spawnFlies;
    private bool _playerState;

    void Start()
    {
        _playerRb = player.GetComponent<Rigidbody>();
        Application.targetFrameRate = 240;
    }

    public void PlayerState(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;

        switch (GameManager.CurrentDifficulty)
        {
            case GameManager.Difficulty.Nofly: //No flies
                _spawnFlies = false;
                Physics.IgnoreLayerCollision(16, 3, true);
                Physics.IgnoreLayerCollision(16, 15, true);
                break;
            case GameManager.Difficulty.PeacefulFlies: //Peaceful flies
                intervalle = 0.25f;
                Physics.IgnoreLayerCollision(16, 3, true);
                Physics.IgnoreLayerCollision(16, 15, true);
                _spawnFlies = true;
                break;
            case GameManager.Difficulty.AgressiveFliesNoFruitLoss: //Agressive flies
                intervalle = 2f;
                Physics.IgnoreLayerCollision(16, 3, false);
                Physics.IgnoreLayerCollision(0, 15, false);
                _spawnFlies = true;
                break;
            case GameManager.Difficulty.AgressiveFliesFruitLoss: //Fruit loss
                intervalle = 2.5f;
                Physics.IgnoreLayerCollision(16, 3, false);
                Physics.IgnoreLayerCollision(16, 15, false);
                _spawnFlies = true;
                break;
            case GameManager.Difficulty.Ganged: //Gange level
                intervalle = 0.5f;
                Physics.IgnoreLayerCollision(16, 3, false);
                Physics.IgnoreLayerCollision(16, 15, false);
                _spawnFlies = true;
                break;
        }
    }

    void Update()
    {
        if (GameManager.UICanvaState is GameManager.UIStateEnum.PlayerHaveReachEndOfLevel && _spawnFlies)
        {
            GameObject[] flies = GameObject.FindGameObjectsWithTag("Flies");
            foreach (GameObject element in flies) Destroy(element);
            _spawnFlies = false;
        }

        if (!_spawnFlies) return;

        transform.position = limbControllerList[0].position;
        transform.eulerAngles = limbControllerList[0].eulerAngles + new Vector3(0, 0, 0);
        _timer += Time.deltaTime;

        if ((!(_timer > _timerLimit) || !(player.position.x > spawnFliesBegining)) &&
            !Input.GetKeyDown(KeyCode.P)) return;

        _randomX = Random.Range(-_spawnPosition.x, _spawnPosition.x);
        _randomY = Random.Range(-_spawnPosition.y, _spawnPosition.y);
        MMoucheAMerde moucheAMerdePrefab = Instantiate(moucheAMerde,
            transform.position + spawnRadius * Vector3.Normalize(new Vector3(_randomX, _randomY, 0)),
            Quaternion.identity);
        moucheAMerdePrefab.body = player;
        moucheAMerdePrefab.bodyRB = _playerRb;
        _timer = 0f;
        _timerLimit = Random.Range(0f, intervalle * endLevel / player.position.x);
    }
}