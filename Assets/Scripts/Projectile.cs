using UnityEngine;

/// <summary>
/// Projétil disparado automaticamente pela gatinha.
/// Se move horizontalmente na direção configurada.
/// Destrói inimigos ao acertar e se autodestrói ao sair da tela ou após um tempo limite.
/// 
/// Setup:
///  1. Crie um sprite pequeno para o projétil (pode ser um quadrado pixel art)
///  2. Adicione este script no prefab do projétil
///  3. Configure BoxCollider2D (Trigger) e Rigidbody2D (Kinematic)
///  4. Defina a tag "Projectile" no projétil
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Velocidade do projétil.")]
    [SerializeField] private float speed = 10f;

    [Tooltip("Tempo máximo de vida do projétil (segundos).")]
    [SerializeField] private float lifetime = 3f;

    private Rigidbody2D _rb;
    private float _direction = 1f; // 1 = direita, -1 = esquerda

    /// <summary>
    /// Define a direção do projétil antes de instanciá-lo.
    /// Chame isso imediatamente após Instantiate().
    /// </summary>
    public void SetDirection(float direction)
    {
        _direction = Mathf.Sign(direction);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        // Define a velocidade do projétil
        _rb.linearVelocity = new Vector2(_direction * speed, 0f);

        // Autodestrói após o tempo de vida
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se acertar um inimigo, destrói o inimigo e o projétil
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"[Projectile] Acertou {other.gameObject.name}!");
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        // Se acertar uma parede ou plataforma, destrói o projétil
        // (opcional — você pode comentar isso se quiser que o projétil atravesse tudo)
        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        // Destrói o projétil quando sair da câmera
        // Isso evita que projéteis fiquem voando infinitamente fora da tela
        Destroy(gameObject);
    }
}
