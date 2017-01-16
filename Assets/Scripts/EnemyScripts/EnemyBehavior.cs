using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    public float speed = 3;
    public Transform tower;
    public float startHealth = 100;
    public string pathName = "EnemyWay";
    private float currentHealth;
	
    // Use this for initialization
	void Start () {
        currentHealth = startHealth;
        transform.FollowPath(pathName, speed, Mr1.FollowType.Once);
    }
	
	// Update is called once per frame
	void Update () {
        transform.right = tower.position - transform.position;
    }

    public void TakeDamage(DamageTypeEnum damageType, float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
