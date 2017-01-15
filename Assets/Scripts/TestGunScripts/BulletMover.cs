using UnityEngine;
using System.Collections;

public class BulletMover : MonoBehaviour {

    public float speed;
    public float damage;
    public DamageTypeEnum damageType = DamageTypeEnum.physical;
    public string enemyTag = "Enemy";

    // Use this for initialization
    void Start () {
        Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = target - myPos;
        direction.Normalize();
        GetComponent<Rigidbody2D>().velocity = direction * speed;
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
