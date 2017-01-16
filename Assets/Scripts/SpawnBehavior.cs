using UnityEngine;
using System.Collections;

public class SpawnBehavior : MonoBehaviour
{

    private static Object enemyPrefab;
    private int amount = 6;

    // Use this for initialization
    void Start()
    {
        enemyPrefab = Resources.Load("Prefabs/Enemy");
        Invoke("SpawnEnemy", 2);
    }

    void SpawnEnemy()
    {
        if (amount > 0)
        {
            string pathName = amount % 2 == 0 ? "EnemyWay" : "EnemyWay2";
            // This will spawn enemies at two different spawns - start points of two paths.
            GameObject newEnemy = Instantiate(enemyPrefab, Mr1.WaypointManager.instance.GetPathData(pathName).startPoint, Quaternion.identity) as GameObject;
            // Now we need to assign to Enemies correct paths to follow.
            EnemyBehavior enemyScript = newEnemy.GetComponent<EnemyBehavior>();
            enemyScript.pathName = pathName;
            // We also should define the tower object, so enemies will know where to look.
            enemyScript.tower = GameObject.FindGameObjectWithTag("Player").transform;
        }

        amount--;
        Invoke("SpawnEnemy", 2);
    }
}