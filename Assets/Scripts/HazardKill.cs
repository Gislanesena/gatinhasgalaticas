using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Obstáculo letal — qualquer contato com o Player reinicia a fase imediatamente.
/// Anexe este script ao meteoro ou qualquer outro hazard que mate no toque.
/// 
/// Setup:
///  1. Adicione este script no GameObject do meteoro/obstáculo
///  2. Configure o Collider2D como Trigger
///  3. Certifique-se de que o Player tem a tag "Player"
/// </summary>
public class HazardKill : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Nome da cena para reiniciar quando o player tocar o hazard.")]
    [SerializeField] private string sceneName = "SampleScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se o player tocar este obstáculo, reinicia a fase
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[HazardKill] Player atingido por {gameObject.name} — reiniciando fase!");
            
            // Opcional: trigger de reação no OLED do ESP32
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.TriggerDamageReaction();

            // Reinicia a cena imediatamente
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}
