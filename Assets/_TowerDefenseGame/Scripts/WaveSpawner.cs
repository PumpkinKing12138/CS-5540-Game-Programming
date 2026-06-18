using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemyPrefabs;
        public int enemyCount = 5;
        public float spawnInterval = 2f;
    }

    public Wave[] waves;
    public float timeBetween = 5;
    public TMP_Text waveText;
    public int curIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.DeleteKey("LastWave");
        curIndex = PlayerPrefs.GetInt("LastWave", 0);
        StartCoroutine(ReleaseWaves());
    }

    IEnumerator ReleaseWaves()
    {
        while (curIndex < waves.Length)
        {
            UpdateWaveText();
            yield return new WaitForSeconds(timeBetween);
            yield return StartCoroutine(SpawnWave(waves[curIndex]));
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
            ++curIndex;
            PlayerPrefs.SetInt("LastWave", curIndex);
            PlayerPrefs.Save();
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; ++i)
        {
            int enemyIndex = Random.Range(0, wave.enemyPrefabs.Length);
            GameObject enemyPrefab = wave.enemyPrefabs[enemyIndex];
            SpawnEnemy(enemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }

    bool AllDestroyed()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    void UpdateWaveText()
    {
        if (waveText)
        {
            waveText.text = (curIndex + 1).ToString() + " / " + waves.Length.ToString();
        }
    }
}
