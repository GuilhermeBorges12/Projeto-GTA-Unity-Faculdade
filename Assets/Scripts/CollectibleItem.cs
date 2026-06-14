using UnityEngine;

// Coloque este script em qualquer item colecionável do mapa (dinheiro, estrela, etc.)
public class CollectibleItem : MonoBehaviour
{
    [Header("Configurações")]
    public int   pointValue   = 50;
    public float rotateSpeed  = 90f;   // graus por segundo
    public float bobAmplitude = 0.3f;  // altura do balanço
    public float bobFrequency = 2f;

    [Header("Efeitos")]
    public GameObject collectEffect; // partícula instanciada ao coletar
    public AudioClip  collectSound;

    private Vector3    startPosition;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        audioSource   = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Rotação contínua
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // Efeito de balanço (bob)
        float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Efeito visual
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // Som
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // Notifica o MissionManager se houver missão ativa
        if (MissionManager.Instance != null)
            MissionManager.Instance.CompleteMission();

        // Atualiza HUD
        if (HUDManager.Instance != null)
            HUDManager.Instance.UpdateScore(pointValue);

        Destroy(gameObject);
    }
}
