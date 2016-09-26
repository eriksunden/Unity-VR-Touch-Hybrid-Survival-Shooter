using UnityEngine;
using System.Collections;

public class VRCamera : MonoBehaviour {

    // Use this for initialization
    IEnumerator Start () {
        yield return new WaitForEndOfFrame();
        UnityEngine.VR.VRSettings.showDeviceView = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
