using UnityEngine;
using System.Collections;

public class BulletMover : MonoBehaviour {

    public float speed;
    public float damage;
    public DamageTypeEnum damageType = DamageTypeEnum.physical;
    public string enemyTag = "Enemy";

    // Use this for initialization
    void Start () {
        // We setting the forward direction for the bullet
        GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(new Vector2(speed, 0));
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(enemyTag))
        {
            other.gameObject.GetComponent<EnemyBehavior>().TakeDamage(damageType, damage);
        }

        Destroy(gameObject);
    }
}
