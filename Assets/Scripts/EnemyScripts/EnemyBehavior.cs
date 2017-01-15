using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    public float speed = 3;
    public Transform tower;
    private string projectileTag = "Projectile";
    public float startHealth = 100;
    private float currentHealth;
	
    // Use this for initialization
	void Start () {
        currentHealth = startHealth;
        transform.FollowPath("EnemyWay", speed, Mr1.FollowType.Once);
    }
	
	// Update is called once per frame
	void Update () {
        transform.right = tower.position - transform.position;
        GetComponent<Rigidbody2D>().AddForce(transform.right * 0);
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
