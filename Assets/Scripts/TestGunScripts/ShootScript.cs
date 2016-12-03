using UnityEngine;
using System.Collections;

public class ShootScript : MonoBehaviour {

    public GameObject shot;
    public Transform shotSpawn;
    public float firerate = 1f;
    private float nextFire = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            nextFire = Time.time + firerate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        }
	}
}
