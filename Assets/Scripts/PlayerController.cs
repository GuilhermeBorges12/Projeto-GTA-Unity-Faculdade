using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed       = 5f;
    public float runSpeed        = 10f;
    public float jumpForce       = 8f;
    public float gravity         = -20f;
    public float vehicleEntryRadius = 3f;

    [Header("Câmera")]
    public Transform cameraTransform;
    public float     mouseSensitivity = 2f;
    public float     minPitch         = -80f;
    public float     maxPitch         =  80f;

    [Header("Estado")]
    public bool isInVehicle = false;

    private CharacterController controller;
    private Vector3              velocity;
    private float                xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        if (isInVehicle) return;

        HandleMouseLook();
        HandleMovement();
        HandleVehicleEntry();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation  = Mathf.Clamp(xRotation, minPitch, maxPitch);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        bool  running = Input.GetKey(KeyCode.LeftShift);
        float speed   = running ? runSpeed : walkSpeed;

        float    h    = Input.GetAxis("Horizontal");
        float    v    = Input.GetAxis("Vertical");
        Vector3  move = transform.right * h + transform.forward * v;

        controller.Move(move * speed * Time.deltaTime);

        // Pulo — só no chão
        if (controller.isGrounded)
        {
            if (velocity.y < 0f) velocity.y = -2f; // mantém no chão

            if (Input.GetKeyDown(KeyCode.Space))
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleVehicleEntry()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        Collider[] nearby = Physics.OverlapSphere(transform.position, vehicleEntryRadius);
        foreach (var col in nearby)
        {
            VehicleController car = col.GetComponent<VehicleController>();
            if (car != null)
            {
                isInVehicle = true;
                car.EnterVehicle(gameObject);
                break;
            }
        }
    }

    // Chamado pelo VehicleController ao sair do carro
    public void OnExitVehicle()
    {
        isInVehicle = false;
    }
}
