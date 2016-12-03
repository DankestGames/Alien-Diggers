using UnityEngine;
using System.Collections;

public class BulletMover : MonoBehaviour {

    public float speed;
	// Use this for initialization
	void Start () {
        Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = target - myPos;
        direction.Normalize();
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 1); //надо посмотреть, через что считать время можно
    }
}
