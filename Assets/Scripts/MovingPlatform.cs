using UnityEngine;

/// <summary>
/// Plataforma que sobe e desce entre dois pontos definidos no Inspector.
/// 
/// Setup:
///  1. Adicione este script na Plataforma e na Plataforma 2.
///  2. Adicione um Rigidbody2D em cada plataforma:
///       - Body Type → Kinematic
///       - Collision Detection → Continuous
///       - Freeze Rotation Z → marcado
///  3. Ajuste os valores de Move Distance e Speed no Inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movimento")]
    [Tooltip("Distância que a plataforma sobe/desce a partir da posição inicial.")]
    [SerializeField] private float moveDistance = 3f;

    [Tooltip("Velocidade do movimento (unidades/segundo).")]
    [SerializeField] private float speed = 2f;

    [Tooltip("Espera em segundos nos pontos extremos antes de inverter.")]
    [SerializeField] private float waitTime = 0.5f;

    // ----------------------------------------------------------------
    // Privado
    // ----------------------------------------------------------------

    private Rigidbody2D _rb;

    private Vector2 _pointA;   // posição mais baixa
    private Vector2 _pointB;   // posição mais alta

    private Vector2 _target;   // ponto para onde está indo agora
    private float   _waitTimer;

    // ----------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Define os dois pontos a partir da posição inicial no Editor
        _pointA = transform.position;
        _pointB = _pointA + Vector2.up * moveDistance;

        // Começa indo para cima
        _target = _pointB;
    }

    private void FixedUpdate()
    {
        // Se está esperando no ponto extremo, apenas decrementa o timer
        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.fixedDeltaTime;
            return;
        }

        // Move em direção ao alvo usando MovePosition (respeitam a física)
        Vector2 newPos = Vector2.MoveTowards(_rb.position, _target, speed * Time.fixedDeltaTime);
        _rb.MovePosition(newPos);

        // Chegou no alvo — inverte direção e inicia espera
        if (Vector2.Distance(_rb.position, _target) < 0.01f)
        {
            _target    = _target == _pointA ? _pointB : _pointA;
            _waitTimer = waitTime;
        }
    }

    // ----------------------------------------------------------------
    // Gizmos — mostra os dois pontos e o caminho no Editor
    // ----------------------------------------------------------------

    private void OnDrawGizmos()
    {
        Vector2 start = Application.isPlaying ? _pointA : (Vector2)transform.position;
        Vector2 end   = start + Vector2.up * moveDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(start, 0.15f);
        Gizmos.DrawWireSphere(end,   0.15f);
        Gizmos.DrawLine(start, end);
    }
}
