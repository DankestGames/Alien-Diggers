using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {
        DestroyObject(other.gameObject);
    }
}
