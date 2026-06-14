using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Áudio")]
    public AudioSource backgroundMusic;
    public Slider      volumeSlider;

    [Header("Configurações")]
    public string gameSceneName = "GameScene";

    void Start()
    {
        // Garante que apenas o painel principal esteja ativo
        ShowPanel(mainPanel);

        // Restaura volume salvo (ou usa 1.0 como padrão)
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        backgroundMusic.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Inicia a música se não estiver tocando
        if (!backgroundMusic.isPlaying)
            backgroundMusic.Play();
    }

    // ---------- Botões do menu ----------

    public void OnPlayButton()
    {
        backgroundMusic.Stop();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOptionsButton()
    {
        ShowPanel(optionsPanel);
    }

    public void OnCreditsButton()
    {
        ShowPanel(creditsPanel);
    }

    public void OnBackButton()
    {
        ShowPanel(mainPanel);
    }

    public void OnQuitButton()
    {
        Debug.Log("[MainMenu] Saindo do jogo...");
        Application.Quit();
    }

    // ---------- Opções ----------

    void OnVolumeChanged(float value)
    {
        backgroundMusic.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    // ---------- Utilitários ----------

    void ShowPanel(GameObject target)
    {
        mainPanel.SetActive(target == mainPanel);
        optionsPanel.SetActive(target == optionsPanel);
        creditsPanel.SetActive(target == creditsPanel);
    }
}
