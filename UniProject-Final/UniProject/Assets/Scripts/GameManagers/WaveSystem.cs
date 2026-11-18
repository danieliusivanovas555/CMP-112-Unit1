
//Dan Ivanovas
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    //Waypoint path for spawned enemies
    public WaypointPath path;

    //Different stages of a wave
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform[] enemy;
        public int[] numberOfEnemy;
        public int count;
        public float rate;

        public bool isRandom = false;
    }

    public Wave[] waves;

    //EndlessStage
    public float enemyIncreasePerRound = 0.07f;
    public float spawnRateIncreasePerRound = 0.025f;
    public float maxSpawnRateMultiplier = 3f;

    //Shared wave number between all spawners
    private static int nextWave = 0;

    public int NextWave
    {
        get { return nextWave + 1; }
    }

    //Time before the next wave starts
    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    public float WaveCountdown
    {
        get { return waveCountdown; }
    }

    //How often to check for remaining enemies
    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    public SpawnState State
    {
        get { return state; }
    }

    void Awake()
    {
        nextWave = 0;
    }

    void Start()
    {
        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        //Update the wave UI
        UI.waveValue = nextWave;

        if (waves.Length == 0)
        {
            return;
        }

        //Wait until all enemies are gone
        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }

            return;
        }

        //Start the next wave after the countdown
        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                if (nextWave < waves.Length)
                {
                    //Use the manually created waves first
                    StartCoroutine(SpawnWave(waves[nextWave]));
                }
                else
                {
                    //Automatically generate endless waves
                    Wave endlessWave = GenerateEndlessWave();

                    StartCoroutine(SpawnWave(endlessWave));
                }
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        //Find every wave spawner
        WaveSpawner[] spawners = FindObjectsByType<WaveSpawner>(FindObjectsSortMode.None);

        // Wait until all spawners finish spawning
        foreach (WaveSpawner spawner in spawners)
        {
            if (spawner.state != SpawnState.WAITING)
            {
                return;
            }
        }

        Debug.Log("Wave Completed!");

        //Reward the player for completing the wave (WOOHOO)
        CurrencyManager currencyManager = FindFirstObjectByType<CurrencyManager>();

        if (currencyManager != null)
        {
            currencyManager.AddCurrency(currencyManager.roundCompletionReward);
        }

        //Increase the round without resetting back to 1
        nextWave++;

        //Reset all spawners for the next wave
        foreach (WaveSpawner spawner in spawners)
        {
            spawner.state = SpawnState.COUNTING;
            spawner.waveCountdown = spawner.timeBetweenWaves;
        }
    }

    bool EnemyIsAlive()
    {
        //Check for enemies every second
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;

            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    //Automatically create a harder wave once the normal waves finish
    Wave GenerateEndlessWave()
    {
        //Use the final manual wave as the starting difficulty
        Wave lastWave = waves[waves.Length - 1];

        //Count how many endless rounds have passed
        int endlessRound = nextWave - waves.Length + 1;

        //Get the total enemies from the last manual wave
        int baseCount = GetWaveEnemyCount(lastWave);

        //Gradually increase the enemy count
        float countMultiplier = 1f + endlessRound * enemyIncreasePerRound;

        int newCount = Mathf.Max(1, Mathf.CeilToInt(baseCount * countMultiplier));

        //Gradually increase the spawn rate
        float rateMultiplier = 1f + endlessRound * spawnRateIncreasePerRound;

        rateMultiplier = Mathf.Min(rateMultiplier, maxSpawnRateMultiplier);

        float newRate = Mathf.Max(0.01f, lastWave.rate) * rateMultiplier;

        //Create the new wave automatically
        Wave newWave = new Wave();

        newWave.name = "Endless Round " + endlessRound;
        newWave.count = newCount;
        newWave.rate = newRate;
        newWave.isRandom = true;

        return newWave;
    }

    //Find how many enemies the final manual wave contains
    int GetWaveEnemyCount(Wave wave)
    {
        if (wave.isRandom)
        {
            return Mathf.Max(1, wave.count);
        }

        int total = 0;

        if (wave.numberOfEnemy != null)
        {
            foreach (int amount in wave.numberOfEnemy)
            {
                total += amount;
            }
        }

        return Mathf.Max(1, total);
    }

    //Using a coroutine since the game is on a cycle
    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.name);

        state = SpawnState.SPAWNING;

        // Check if this is an endless round
        bool isEndless = nextWave >= waves.Length;

        if (isEndless)
        {
            //Spawn enemies using the generated round settings
            for (int i = 0; i < _wave.count; i++)
            {
                SpawnEndlessEnemy();

                yield return new WaitForSeconds(1f / _wave.rate);
            }
        }
        else if (_wave.isRandom)
        {
            //Spawn random enemies using the wave count
            for (int i = 0; i < _wave.count; i++)
            {
                SpawnEnemy(_wave.enemy);

                yield return new WaitForSeconds(1f / _wave.rate);
            }
        }
        else
        {
            //Spawn each enemy type in order
            for (int i = 0; i < _wave.enemy.Length; i++)
            {
                Transform[] selectedEnemy = { _wave.enemy[i] };

                //Spawn the set amount of each enemy
                for (int j = 0; j < _wave.numberOfEnemy[i]; j++)
                {
                    SpawnEnemy(selectedEnemy);

                    yield return new WaitForSeconds(1f / _wave.rate);
                }
            }
        }

        // Wait for remaining enemies to be defeated
        state = SpawnState.WAITING;
    }

    // Choose enemies from the last few manually created waves
    void SpawnEndlessEnemy()
    {
        int endlessRound = nextWave - waves.Length + 1;

        // Only use the last 3 waves for enemy selection
        int earliestWave = Mathf.Max(0, waves.Length - 3);

        int selectedWave = Random.Range(earliestWave, waves.Length);

        // Slowly increase the chance of selecting the last wave
        float strongerEnemyChance = Mathf.Clamp01(0.20f + endlessRound * 0.02f);

        if (Random.value < strongerEnemyChance)
        {
            selectedWave = waves.Length - 1;
        }

        // Use the enemy prefabs from the selected wave
        SpawnEnemy(waves[selectedWave].enemy);
    }

    void SpawnEnemy(Transform[] _enemy)
    {
        //Stop if there are no enemies assigned
        if (_enemy == null || _enemy.Length == 0)
        {
            return;
        }

        //Select an enemy from the array
        int randomSpawn = Random.Range(0, _enemy.Length);

        Transform randomEnemy = _enemy[randomSpawn];

        //Spawn the selected enemy prefab
        Transform spawnedEnemy = Instantiate(randomEnemy, transform.position, transform.rotation);

        //Assign the waypoint path to the enemy
        EnemyWaypointMovement movement = spawnedEnemy.GetComponent<EnemyWaypointMovement>();

        if (movement != null)
        {
            movement.path = path;
        }
    }
}
