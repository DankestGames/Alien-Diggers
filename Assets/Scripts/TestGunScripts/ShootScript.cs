using UnityEngine;
using System.Collections;

public class ShootScript : MonoBehaviour {

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate = 1f;
    private float nextFire = 0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        }
	}
}
