using UnityEngine;
using System.Collections;

public class PlayerPosition : MonoBehaviour {

    public string CameraName;

    private GameObject aloneMode;
    private GameObject walkMode;
    private GameObject teleportMode;
    private int mode;

    // Use this for initialization
    void Start () {
        aloneMode = GameObject.Find("AloneMode").gameObject;
        walkMode = GameObject.Find("WalkMode").gameObject;
        teleportMode = GameObject.Find("TeleportMode").gameObject;
        setMode(1);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKeyDown("1"))
        {
            setMode(1);
        }
        if (Input.GetKeyDown("2"))
        {
            setMode(2);
        }
        if (Input.GetKeyDown("3"))
        {
            setMode(3);
        }
        if (mode == 3 && Input.GetMouseButtonDown(0)) //Teleport
        {
            Camera cam = GameObject.Find(CameraName).GetComponent<Camera>();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.point.y < 0.2)
                    transform.position = hit.point;
            }
        }
    }

    void setMode(int m)
    {
        mode = m;
        switch (m)
        {
            case 1: //Alone
                walkMode.SetActive(false);
                teleportMode.SetActive(false);
                aloneMode.SetActive(true);
                break;
            case 2: //Walk
                aloneMode.SetActive(false);
                teleportMode.SetActive(false);
                walkMode.SetActive(true);
                break;
            case 3: //Teleport
                aloneMode.SetActive(false);
                walkMode.SetActive(false);
                teleportMode.SetActive(true);
                break;
            default:
                break;
        }
    }
}
