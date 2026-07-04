using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Alien que patrulha horizontalmente entre dois pontos.
/// Vira o sprite ao inverter direção.
/// Se o Player tocar no alien, reinicia a fase.
/// Se um projétil atingir o alien, ele é destruído.
/// 
/// Setup:
///  1. Adicione este script no alien
///  2. Configure Rigidbody2D (Kinematic) e BoxCollider2D (Trigger)
///  3. Defina a tag "Enemy" no alien
///  4. Ajuste patrolDistance e speed no Inspector
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrulha")]
    [Tooltip("Distância total da patrulha (o alien vai metade para cada lado).")]
    [SerializeField] private float patrolDistance = 4f;

    [Tooltip("Velocidade de movimento.")]
    [SerializeField] private float speed = 2f;

    [Header("Config")]
    [Tooltip("Nome da cena para reiniciar quando o player tocar o alien.")]
    [SerializeField] private string sceneName = "SampleScene";

    private Vector3 _startPosition;
    private float _direction = 1f; // 1 = direita, -1 = esquerda
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Move o alien
        float movement = _direction * speed * Time.fixedDeltaTime;
        transform.position += new Vector3(movement, 0f, 0f);

        // Checa se chegou no limite da patrulha
        float distanceFromStart = transform.position.x - _startPosition.x;

        if (Mathf.Abs(distanceFromStart) >= patrolDistance / 2f)
        {
            // Inverte direção
            _direction *= -1f;

            // Flip do sprite
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * _direction;
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se o player tocar o alien, reinicia a fase
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[EnemyPatrol] Player atingido por {gameObject.name} — reiniciando fase!");
            
            // Opcional: trigger de reação no OLED do ESP32
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.TriggerDamageReaction();

            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        // Se um projétil atingir o alien, destrói o alien
        if (other.CompareTag("Projectile"))
        {
            Debug.Log($"[EnemyPatrol] {gameObject.name} destruído por projétil!");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualiza a área de patrulha no editor
        Vector3 center = Application.isPlaying ? _startPosition : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            center + Vector3.left * (patrolDistance / 2f),
            center + Vector3.right * (patrolDistance / 2f)
        );
    }
}
