using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float moveSpeed = 5f;             // Base movement speed
    public float fastSpeedMultiplier = 3f;  // Speed multiplier when Shift is held
    public float lookSpeed = 2f;             // Mouse sensitivity
    public float zoomSpeed = 2f;             // Zoom speed
    public float minZoom = 15f;              // Minimum field of view
    public float maxZoom = 90f;              // Maximum field of view

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Lock and hide cursor for FPS-style control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleZoom();

        // Unlock cursor when pressing Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D for left/right movement
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S for forward/backward movement
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Q)) moveY = -1f; // Move down (Q)
        if (Input.GetKey(KeyCode.E)) moveY = 1f;  // Move up (E)

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastSpeedMultiplier : 1f);
        Vector3 moveDirection = (transform.right * moveX + transform.up * moveY + transform.forward * moveZ).normalized;
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Prevents flipping upside down

        rotationY += mouseX;

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (cam != null)
        {
            cam.fieldOfView -= scroll * 10f;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }
    }
}
