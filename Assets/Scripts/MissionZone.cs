using UnityEngine;

// Coloque este script em um GameObject com Collider (Is Trigger = true)
// para definir zonas de início e conclusão de missões no mapa.
public class MissionZone : MonoBehaviour
{
    public enum ZoneType { StartMission, CompleteMission }

    [Header("Configuração da Zona")]
    public ZoneType zoneType      = ZoneType.StartMission;
    public int      missionIndex  = 0;

    [Header("Visual (opcional)")]
    public Renderer zoneRenderer;
    public Color    activeColor   = new Color(0f, 1f, 0f, 0.3f);
    public Color    inactiveColor = new Color(1f, 1f, 0f, 0.3f);

    void Start()
    {
        if (zoneRenderer != null)
            zoneRenderer.material.color = inactiveColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType.StartMission)
        {
            MissionManager.Instance.StartMission(missionIndex);
            if (zoneRenderer != null)
                zoneRenderer.material.color = activeColor;
        }
        else if (zoneType == ZoneType.CompleteMission)
        {
            MissionManager.Instance.CompleteMission();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneRenderer != null)
            zoneRenderer.material.color = inactiveColor;
    }

    // Desenha o raio da zona no editor para facilitar o level design
    void OnDrawGizmos()
    {
        Gizmos.color = (zoneType == ZoneType.StartMission)
            ? new Color(1f, 1f, 0f, 0.4f)
            : new Color(0f, 1f, 0f, 0.4f);

        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
    }
}
