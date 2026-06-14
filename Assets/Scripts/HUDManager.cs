using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("Velocímetro")]
    public Text            speedText;
    public VehicleController vehicleController;

    [Header("Pontuação e Tempo")]
    public Text scoreText;
    public Text timerText;

    [Header("Indicador de Procurado")]
    public Image[]  wantedStars;    // 5 imagens de estrela
    public Sprite   starFilled;
    public Sprite   starEmpty;
    private int     wantedLevel = 0;

    [Header("Minimapa")]
    public RawImage  minimapImage;
    public Transform playerTransform;

    private Rigidbody vehicleRb;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (vehicleController != null)
            vehicleRb = vehicleController.GetComponent<Rigidbody>();

        UpdateWantedUI();
    }

    void Update()
    {
        UpdateSpeedometer();
    }

    void UpdateSpeedometer()
    {
        if (vehicleRb == null || !vehicleController.enabled)
        {
            speedText.text = "0 km/h";
            return;
        }

        float kmh = vehicleRb.linearVelocity.magnitude * 3.6f;
        speedText.text = Mathf.RoundToInt(kmh) + " km/h";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Pontuação: " + score;
    }

    public void AddWantedLevel()
    {
        wantedLevel = Mathf.Clamp(wantedLevel + 1, 0, wantedStars.Length);
        UpdateWantedUI();
    }

    public void ClearWanted()
    {
        wantedLevel = 0;
        UpdateWantedUI();
    }

    void UpdateWantedUI()
    {
        for (int i = 0; i < wantedStars.Length; i++)
            wantedStars[i].sprite = (i < wantedLevel) ? starFilled : starEmpty;
    }
}
