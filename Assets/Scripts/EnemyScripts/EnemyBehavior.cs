using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    public float speed;
    public Transform tower;
	
    // Use this for initialization
	void Start () {
        transform.FollowPath("EnemyWay", speed, Mr1.FollowType.Once);
    }
	
	// Update is called once per frame
	void Update () {
        transform.right = tower.position - transform.position;
        GetComponent<Rigidbody2D>().AddForce(transform.right * 0);
    }
}
