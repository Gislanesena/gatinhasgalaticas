using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Coloque este script no objeto do final da fase.
/// Quando o Player tocar nele, exibe o painel de fim de fase.
///
/// Setup:
///  1. O objeto precisa ter um Collider2D com "Is Trigger" marcado.
///  2. O Player precisa ter a tag "Player".
///  3. Crie o painel de UI e arraste no campo Game Over Panel.
///  4. Coloque o nome da cena no campo Scene Name.
/// </summary>
public class LevelEnd : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Painel 'ACABOU' que aparece ao chegar no final.")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Reiniciar")]
    [Tooltip("Nome exato da cena (confira em File → Build Settings).")]
    [SerializeField] private string sceneName = "SampleScene";

    // ----------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------

    private void Start()
    {
        // Garante que o painel começa escondido
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // ----------------------------------------------------------------
    // Trigger
    // ----------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[LevelEnd] Trigger ativado por: {other.gameObject.name} | tag: {other.tag}");

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[LevelEnd] Ignorado — não é o Player.");
            return;
        }

        Debug.Log("[LevelEnd] Player chegou no final! Mostrando painel...");

        // Mostra o painel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        else
            Debug.LogWarning("[LevelEnd] Game Over Panel não está configurado no Inspector!");

        // Pausa o jogo
        Time.timeScale = 0f;

        // Reação no OLED do ESP32
        ESP32SerialReader.Instance?.SendTreasure();
    }

    // ----------------------------------------------------------------
    // API pública — chame pelo botão "Jogar Novamente" no Inspector
    // ----------------------------------------------------------------

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
