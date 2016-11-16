using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float m_DefaultLineLength = 70f;                       // How far the line renderer will reach if a target isn't hit.
        [SerializeField] private float m_Damping = 0.5f;                                // The damping with which this gameobject follows the camera.

        public float speed = 6f;            // The speed that the player will move at.
        public string CameraName;

        private GameObject aloneMode;
        private GameObject walkMode;
        private GameObject teleportMode;
        private int mode;
        private const float k_DampingCoef = -20f;                                       // This is the coefficient used to ensure smooth damping of this gameobject.

        Vector3 movement;                   // The vector to store the direction of the player's movement.
        Animator anim;                      // Reference to the animator component.
        Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
#if !MOBILE_INPUT
        int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        float camRayLength = 100f;          // The length of the ray from the camera into the scene.
#endif

        void Awake ()
        {
#if !MOBILE_INPUT
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask ("Floor");
#endif

            // Set up references.
            anim = GetComponent <Animator> ();
            playerRigidbody = GetComponent <Rigidbody> ();

            aloneMode = GameObject.Find("AloneMode").gameObject;
            walkMode = GameObject.Find("WalkMode").gameObject;
            teleportMode = GameObject.Find("TeleportMode").gameObject;
            setMode(0);
        }

        void Update()
        {
            if (Input.GetKeyDown("0") || Input.GetKeyDown("joystick button 0")) //A
            {
                setMode(0); //Alone (Green)
            }
            if (Input.GetKeyDown("1") || Input.GetKeyDown("joystick button 1")) //B
            {
                setMode(1); //Teleport (Red)
            }
            if (Input.GetKeyDown("2") || Input.GetKeyDown("joystick button 2")) //X
            {
                setMode(2); //Walk (Blue)
            }

            if (mode == 1 && Input.GetMouseButtonDown(0)) //Teleport
            {
                Camera cam = GameObject.Find(CameraName).GetComponent<Camera>();
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Only teleport to the floor, not the structures
                    if (hit.point.y < 0.1)
                        transform.position = hit.point;
                }
            }
        }

        void FixedUpdate ()
        {
            // Store the input axes.
            float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float v = CrossPlatformInputManager.GetAxisRaw("Vertical");

            if (mode == 2 && Input.GetMouseButton(0)) //Walk
            {
                Camera cam = GameObject.Find(CameraName).GetComponent<Camera>();
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // Move in the direction from the player to the mouse cursor
                    Vector3 playerToMouse = hit.point - transform.position;
                    Vector3 playerToMouseNormalized = playerToMouse.normalized;
                    h += playerToMouseNormalized.x;
                    v += playerToMouseNormalized.z;
                }
            }

            // Smoothly interpolate this gameobject's rotation towards that of the VR headset.
            Quaternion headRotation = Quaternion.Slerp(transform.rotation, UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.Head),
                m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

            Vector3 eulerAngles = headRotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            // Move the player around the scene.
            // Set the movement vector based on the axis input.
            movement.Set(h, 0f, v);

            // Normalise the movement vector and make it proportional to the speed per second.
            movement = movement.normalized * speed * Time.deltaTime;

            //Rotate movement by the current axis rotation
            movement = transform.rotation * movement;

            // Move the player to it's current position plus the movement.
            playerRigidbody.MovePosition(transform.position + movement);

            // Animate the player.
            Animating (h, v);
        }

        void setMode(int m)
        {
            mode = m;
            switch (m)
            {
                case 0: //Alone
                    walkMode.SetActive(false);
                    teleportMode.SetActive(false);
                    aloneMode.SetActive(true);
                    break;
                case 1: //Teleport
                    aloneMode.SetActive(false);
                    walkMode.SetActive(false);
                    teleportMode.SetActive(true);
                    break;
                case 2: //Walk
                    aloneMode.SetActive(false);
                    teleportMode.SetActive(false);
                    walkMode.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        void Turning ()
        {
#if !MOBILE_INPUT
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation (newRotatation);
            }
#else

            Vector3 turnDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X") , 0f , CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir != Vector3.zero)
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
        }


        void Animating (float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            anim.SetBool ("IsWalking", walking);
        }
    }
}