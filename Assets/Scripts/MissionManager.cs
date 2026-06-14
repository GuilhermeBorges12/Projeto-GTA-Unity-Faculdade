using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Mission
{
    public string missionName;
    public string description;
    public int    reward;
    public float  timeLimit;
    public Transform targetLocation; // ponto no mapa que o jogador deve alcançar
}

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance; // Singleton para acesso global

    [Header("Missões")]
    public Mission[] missions;

    [Header("HUD")]
    public Text       missionNameText;
    public Text       missionDescText;
    public Text       timerText;
    public Text       scoreText;
    public GameObject missionPanel;
    public GameObject missionCompletePanel;
    public GameObject missionFailPanel;

    private int   currentMissionIndex = 0;
    private bool  missionActive       = false;
    private float timer;
    private int   totalScore          = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        missionPanel.SetActive(false);
        missionCompletePanel.SetActive(false);
        missionFailPanel.SetActive(false);
        UpdateScoreHUD();
    }

    void Update()
    {
        if (!missionActive) return;

        timer -= Time.deltaTime;
        timerText.text = "Tempo: " + Mathf.CeilToInt(timer) + "s";

        if (timer <= 0f)
            FailMission();
    }

    public void StartMission(int index)
    {
        if (missionActive || index >= missions.Length) return;

        currentMissionIndex = index;
        Mission m = missions[index];

        missionActive        = true;
        timer                = m.timeLimit;
        missionNameText.text = m.missionName;
        missionDescText.text = m.description;

        missionPanel.SetActive(true);
        Debug.Log($"[MissionManager] Missão iniciada: {m.missionName}");
    }

    public void CompleteMission()
    {
        if (!missionActive) return;

        Mission m = missions[currentMissionIndex];
        totalScore    += m.reward;
        missionActive  = false;

        missionPanel.SetActive(false);
        UpdateScoreHUD();

        StartCoroutine(ShowCompleteAndLoadNext(m));
    }

    void FailMission()
    {
        missionActive = false;
        missionPanel.SetActive(false);
        StartCoroutine(ShowFailPanel());
        Debug.Log("[MissionManager] Missão falhou — tempo esgotado.");
    }

    IEnumerator ShowCompleteAndLoadNext(Mission m)
    {
        missionCompletePanel.SetActive(true);
        Debug.Log($"[MissionManager] Completa! +{m.reward} pontos. Total: {totalScore}");
        yield return new WaitForSeconds(2f);
        missionCompletePanel.SetActive(false);

        int next = currentMissionIndex + 1;
        if (next < missions.Length)
            StartMission(next);
        else
            Debug.Log("[MissionManager] Todas as missões concluídas!");
    }

    IEnumerator ShowFailPanel()
    {
        missionFailPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        missionFailPanel.SetActive(false);
    }

    void UpdateScoreHUD()
    {
        scoreText.text = "Pontuação: " + totalScore;
    }
}
