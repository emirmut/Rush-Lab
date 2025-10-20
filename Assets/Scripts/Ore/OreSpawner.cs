using System.Collections;
using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    [SerializeField] private Ore[] _orePrefabs;
    [SerializeField] private float _spawnInterval;

    private void Start()
    {
        StartCoroutine(SpawnOres());
    }

    private IEnumerator SpawnOres()
    {
        int random = Random.Range(0, _orePrefabs.Length);
        Ore spawnedOre = Instantiate(_orePrefabs[random], transform.position, Quaternion.identity);
        spawnedOre.transform.SetParent(transform);
        spawnedOre.gameObject.tag = "Ore";

        yield return new WaitForSeconds(_spawnInterval);
        StartCoroutine(SpawnOres());
    }
}
