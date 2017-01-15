using UnityEngine;
using System.Collections;

public class RotateTowardsCursos : MonoBehaviour {

    public float rotationSpeed = 60f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Quaternion targetRotation = Quaternion.LookRotation(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, Vector3.forward);
        targetRotation.x = 0;
        targetRotation.y = 0;
        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
