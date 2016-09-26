using UnityEngine;
using System.Collections;

public class VRViewCameraRotate : MonoBehaviour {

    [SerializeField] private float m_Damping = 0.5f;                                // The damping with which this gameobject follows the camera.
    private const float k_DampingCoef = -20f;                                       // This is the coefficient used to ensure smooth damping of this gameobject.

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPosition = GameObject.Find("Player").transform.position;
        Vector3 cameraPosition = transform.position;
        playerPosition.y = cameraPosition.y;
        float distance = Vector3.Distance(cameraPosition, playerPosition);
        transform.position = playerPosition;
        Quaternion headRotation = Quaternion.Slerp(transform.rotation, UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.Head),
                m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

        Vector3 eulerAngles = headRotation.eulerAngles;
        eulerAngles = new Vector3(0, eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(eulerAngles);

        Camera thisCamera = this.GetComponent<Camera>();
        Ray ray = thisCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        thisCamera.transform.Translate(ray.direction * -distance, Space.World);
    }
}
