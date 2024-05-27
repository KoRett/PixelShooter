using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;

    void Start()
    {
        StartCoroutine(spawnEnemy());
    }

    IEnumerator spawnEnemy()
    {
        while(true) 
        { 
            Instantiate(enemy, transform);
            yield return new WaitForSeconds(3);
        }
    }

}
