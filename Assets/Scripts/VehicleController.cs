using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Wheel Transforms (visuais)")]
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    [Header("Configurações do Veículo")]
    public float motorForce    = 1500f;
    public float brakeForce    = 3000f;
    public float maxSteerAngle = 30f;
    public float maxSpeed      = 120f;

    [Header("Referências")]
    public GameObject player;
    public Camera     drivingCamera;
    public Camera     playerCamera;
    public Transform  exitPoint;

    private Rigidbody  rb;
    private float      horizontalInput;
    private float      verticalInput;
    private bool       isBraking;
    private bool       isOccupied = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // estabilidade
    }

    void Update()
    {
        if (!isOccupied) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput   = Input.GetAxis("Vertical");
        isBraking       = Input.GetKey(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.E))
            ExitVehicle();
    }

    void FixedUpdate()
    {
        if (!isOccupied) return;
        HandleMotor();
        HandleSteering();
        UpdateWheelVisuals();
        ClampSpeed();
    }

    void HandleMotor()
    {
        // Limita aceleração se já estiver na velocidade máxima
        float speed  = rb.linearVelocity.magnitude * 3.6f; // m/s para km/h
        float torque = (speed < maxSpeed) ? verticalInput * motorForce : 0f;

        rearLeftWheel.motorTorque  = torque;
        rearRightWheel.motorTorque = torque;

        float brake = isBraking ? brakeForce : 0f;
        frontLeftWheel.brakeTorque  = brake;
        frontRightWheel.brakeTorque = brake;
        rearLeftWheel.brakeTorque   = brake;
        rearRightWheel.brakeTorque  = brake;
    }

    void HandleSteering()
    {
        float steer = maxSteerAngle * horizontalInput;
        frontLeftWheel.steerAngle  = steer;
        frontRightWheel.steerAngle = steer;
    }

    void ClampSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed / 3.6f)
            rb.linearVelocity = rb.linearVelocity.normalized * (maxSpeed / 3.6f);
    }

    // Sincroniza a mesh visual das rodas com o WheelCollider
    void UpdateWheelVisuals()
    {
        UpdateSingleWheel(frontLeftWheel,  frontLeftTransform);
        UpdateSingleWheel(frontRightWheel, frontRightTransform);
        UpdateSingleWheel(rearLeftWheel,   rearLeftTransform);
        UpdateSingleWheel(rearRightWheel,  rearRightTransform);
    }

    void UpdateSingleWheel(WheelCollider col, Transform trans)
    {
        Vector3    pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        trans.position = pos;
        trans.rotation = rot;
    }

    public void EnterVehicle(GameObject playerObj)
    {
        isOccupied = true;
        player     = playerObj;
        player.SetActive(false);
        playerCamera.gameObject.SetActive(false);
        drivingCamera.gameObject.SetActive(true);
    }

    void ExitVehicle()
    {
        isOccupied = false;
        player.transform.position = exitPoint != null
            ? exitPoint.position
            : transform.position + transform.right * 2f;

        player.SetActive(true);
        playerCamera.gameObject.SetActive(true);
        drivingCamera.gameObject.SetActive(false);
    }
}
