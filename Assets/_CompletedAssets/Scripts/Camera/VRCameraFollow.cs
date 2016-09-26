using UnityEngine;
using System.Collections;

public class VRCameraFollow : MonoBehaviour {

    public Transform target;            // The position that that camera will be following.

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.parent.position = target.position;
    }
}
