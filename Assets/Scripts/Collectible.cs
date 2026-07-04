using UnityEngine;

/// <summary>
/// Coloca este script em qualquer item coletável da cena.
///
/// Setup:
///  1. O item precisa ter um Collider2D com "Is Trigger" marcado.
///  2. O Player precisa ter a tag "Player" (Edit → Project Settings → Tags).
///  3. Ajuste o que acontece nas regiões marcadas com "AÇÃO" abaixo.
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Efeito visual ao coletar (opcional — arraste um prefab de partícula).")]
    [SerializeField] private GameObject collectEffect;

    [Tooltip("Som ao coletar (opcional).")]
    [SerializeField] private AudioClip collectSound;

    // ----------------------------------------------------------------
    // Trigger
    // ----------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Só reage ao Player
        if (!other.CompareTag("Player")) return;

        // ── Avisa o GameManager que um item foi coletado ──────────────
        GameManager.Instance?.OnItemCollected();

        // ── AÇÃO: reação no OLED do ESP32 ────────────────────────────
        ESP32SerialReader.Instance?.SendTreasure();

        // ── AÇÃO: efeito visual ───────────────────────────────────────
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // ── AÇÃO: som ────────────────────────────────────────────────
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // ── Remove o item da cena ─────────────────────────────────────
        Destroy(gameObject);
    }
}
