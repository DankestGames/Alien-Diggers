using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    public float speed;
    public Transform tower;
	
    // Use this for initialization
	void Start () {
        //rotate to look at the player
        transform.right = tower.position - transform.position;
        GetComponent<Rigidbody2D>().AddForce(transform.right * speed);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
