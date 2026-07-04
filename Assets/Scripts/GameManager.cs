using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerencia o estado do jogo e exibe o painel de fim de jogo.
///
/// Setup:
///  1. Crie um GameObject vazio chamado "GameManager" na cena.
///  2. Adicione este script nele.
///  3. Crie o painel de UI conforme as instruções abaixo.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ----------------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------------
    public static GameManager Instance { get; private set; }

    // ----------------------------------------------------------------
    // Inspector
    // ----------------------------------------------------------------

    [Header("UI - Painel de Fim de Jogo")]
    [Tooltip("O painel 'ACABOU' — arraste o objeto Canvas/Panel aqui.")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Nome exato da cena para reiniciar (confira em File → Build Settings).")]
    [SerializeField] private string sceneName = "SampleScene";

    // ----------------------------------------------------------------
    // Privado
    // ----------------------------------------------------------------
    private int _totalCollectibles;
    private int _collectedCount;

    // ----------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Conta quantos coletáveis existem na cena ao iniciar
        _totalCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;

        // Garante que o painel começa escondido
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // ----------------------------------------------------------------
    // API pública
    // ----------------------------------------------------------------

    /// <summary>
    /// Chamado pelo Collectible quando um item é coletado.
    /// Quando o último for coletado, exibe o painel de fim de jogo.
    /// </summary>
    public void OnItemCollected()
    {
        _collectedCount++;

        if (_collectedCount >= _totalCollectibles)
            ShowGameOver();
    }

    /// <summary>Reinicia a cena atual.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // garante que o tempo não ficou pausado
        SceneManager.LoadScene(sceneName);
    }

    // ----------------------------------------------------------------
    // Privado
    // ----------------------------------------------------------------

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Pausa o jogo enquanto o painel está visível
        Time.timeScale = 0f;

        // Reação no OLED do ESP32
        ESP32SerialReader.Instance?.SendTreasure();

        Debug.Log("[GameManager] Todos os itens coletados — fim de jogo!");
    }
}
