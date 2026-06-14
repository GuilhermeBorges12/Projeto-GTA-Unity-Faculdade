# 🚗 City Chaos

> Jogo de mundo aberto em terceira pessoa desenvolvido na Unity 3D, inspirado no estilo GTA. Explore a cidade, dirija veículos, complete missões e cause o caos pelas ruas!

---

## 👤 Desenvolvedor

**[Guilherme Borges Rocha]**  
Curso: [Engenharia de Software]  
RA: [24833]

---

## 📖 Descrição do Jogo

**City Chaos** é um jogo de ação em mundo aberto onde o jogador controla um personagem dentro de uma cidade 3D. O objetivo é completar missões espalhadas pelo mapa, coletar itens e acumular pontos enquanto desvia das consequências de suas ações. O jogo combina exploração, direção de veículos e mecânicas de missão para criar uma experiência dinâmica e imersiva.

---

## 🎮 Instruções de Jogabilidade

### Controles do Personagem (a pé)
| Tecla | Ação |
|-------|------|
| `W A S D` | Movimentar o personagem |
| `Shift` | Correr |
| `Espaço` | Pular |
| `E` | Interagir / Entrar em veículo |
| `Mouse` | Controlar câmera |

### Controles de Veículo
| Tecla | Ação |
|-------|------|
| `W / Seta Cima` | Acelerar |
| `S / Seta Baixo` | Frear / Ré |
| `A / D` | Virar |
| `E` | Sair do veículo |
| `Espaço` | Freio de mão |

### Objetivo
- Complete as missões indicadas no HUD para ganhar pontos
- Colete os itens espalhados pelo mapa
- Evite ser capturado ao atingir o nível máximo de procurado

---

## ⚙️ Funcionalidades Desenvolvidas

### 1. 🚗 Sistema de Controle de Veículo

Foi desenvolvido um sistema de física de veículo utilizando o componente `WheelCollider` do Unity, permitindo que o jogador entre, dirija e saia de veículos espalhados pelo mapa. O sistema simula aceleração, frenagem, curvas e freio de mão de forma realista, aumentando a imersividade da jogabilidade.

Quando o jogador pressiona `E` próximo a um veículo, o personagem é desativado e a câmera troca para o modo de direção. A física é aplicada nas rodas individualmente via `WheelCollider`, que calcula atrito e torque automaticamente.

```csharp
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Configurações")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;
    private bool isOccupied = false;

    public GameObject player;
    public Camera drivingCamera;
    public Camera playerCamera;

    void Update()
    {
        if (!isOccupied) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);

        // Sair do veículo
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitVehicle();
        }
    }

    void FixedUpdate()
    {
        if (!isOccupied) return;

        HandleMotor();
        HandleSteering();
    }

    void HandleMotor()
    {
        float torque = verticalInput * motorForce;
        rearLeftWheel.motorTorque = torque;
        rearRightWheel.motorTorque = torque;

        float brake = isBraking ? brakeForce : 0f;
        frontLeftWheel.brakeTorque = brake;
        frontRightWheel.brakeTorque = brake;
        rearLeftWheel.brakeTorque = brake;
        rearRightWheel.brakeTorque = brake;
    }

    void HandleSteering()
    {
        float steer = maxSteerAngle * horizontalInput;
        frontLeftWheel.steerAngle = steer;
        frontRightWheel.steerAngle = steer;
    }

    public void EnterVehicle(GameObject playerObj)
    {
        isOccupied = true;
        player = playerObj;
        player.SetActive(false);
        playerCamera.gameObject.SetActive(false);
        drivingCamera.gameObject.SetActive(true);
    }

    void ExitVehicle()
    {
        isOccupied = false;
        player.transform.position = transform.position + transform.right * 2f;
        player.SetActive(true);
        playerCamera.gameObject.SetActive(true);
        drivingCamera.gameObject.SetActive(false);
    }
}
```


---

### 2. 🎯 Sistema de Missões com Objetivos no HUD

Foi desenvolvido um sistema de missões que exibe objetivos no HUD do jogador em tempo real. Cada missão possui um gatilho de área (`Trigger`) no mundo 3D — quando o jogador entra na zona, a missão é ativada e um timer começa a contar. Ao completar o objetivo (coletar itens ou chegar ao destino), a pontuação é atualizada e a próxima missão é carregada automaticamente.

O sistema usa `ScriptableObjects` para armazenar os dados de cada missão de forma modular, facilitando a adição de novas missões sem alterar o código principal.

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Mission
{
    public string missionName;
    public string description;
    public int reward;
    public float timeLimit;
}

public class MissionManager : MonoBehaviour
{
    [Header("Missões")]
    public Mission[] missions;
    private int currentMissionIndex = 0;
    private bool missionActive = false;
    private float timer;

    [Header("HUD")]
    public Text missionNameText;
    public Text missionDescText;
    public Text timerText;
    public Text scoreText;
    public GameObject missionPanel;

    private int totalScore = 0;

    void Start()
    {
        missionPanel.SetActive(false);
        UpdateScoreHUD();
    }

    void Update()
    {
        if (!missionActive) return;

        timer -= Time.deltaTime;
        timerText.text = "Tempo: " + Mathf.CeilToInt(timer) + "s";

        if (timer <= 0)
        {
            FailMission();
        }
    }

    public void StartMission(int index)
    {
        if (missionActive || index >= missions.Length) return;

        currentMissionIndex = index;
        Mission m = missions[index];

        missionActive = true;
        timer = m.timeLimit;

        missionNameText.text = m.missionName;
        missionDescText.text = m.description;
        missionPanel.SetActive(true);

        Debug.Log($"Missão iniciada: {m.missionName}");
    }

    public void CompleteMission()
    {
        if (!missionActive) return;

        Mission m = missions[currentMissionIndex];
        totalScore += m.reward;
        missionActive = false;

        UpdateScoreHUD();
        missionPanel.SetActive(false);

        Debug.Log($"Missão concluída! +{m.reward} pontos");

        // Carrega próxima missão após 2 segundos
        StartCoroutine(LoadNextMission());
    }

    void FailMission()
    {
        missionActive = false;
        missionPanel.SetActive(false);
        missionNameText.text = "Missão falhou!";
        Debug.Log("Missão falhou - tempo esgotado");
    }

    IEnumerator LoadNextMission()
    {
        yield return new WaitForSeconds(2f);
        int next = currentMissionIndex + 1;
        if (next < missions.Length)
        {
            StartMission(next);
        }
        else
        {
            Debug.Log("Todas as missões completadas!");
        }
    }

    void UpdateScoreHUD()
    {
        scoreText.text = "Pontuação: " + totalScore;
    }

    // Chamado pelo trigger de zona no mundo
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CompleteMission();
        }
    }
}
```


---

## 🛠️ Tecnologias Utilizadas

- **Engine:** Unity 3D (versão 2022.3 LTS)
- **Linguagem:** C#
- **Controle de versão:** Git + GitHub
- **Assets:** Unity Asset Store (gratuitos)

---

## 🚀 Como Executar o Projeto

1. Clone o repositório:
   ```bash
   git clone https://github.com/SEU_USUARIO/city-chaos.git
   ```
2. Abra o Unity Hub e adicione o projeto clonado
3. Aguarde a importação dos pacotes
4. Abra a cena `Assets/Scenes/MainMenu.unity`
5. Pressione **Play** no Unity Editor

---

## 📁 Estrutura do Projeto

```
Assets/
├── Scenes/
│   ├── MainMenu.unity
│   └── GameScene.unity
├── Scripts/
│   ├── VehicleController.cs
│   ├── MissionManager.cs
│   ├── PlayerController.cs
│   └── MenuManager.cs
├── Prefabs/
├── Audio/
└── UI/
```
