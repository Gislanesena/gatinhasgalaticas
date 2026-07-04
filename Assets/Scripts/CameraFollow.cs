using UnityEngine;

/// <summary>
/// Faz a câmera seguir o player suavemente apenas no eixo X (side-scroller).
/// A câmera não segue o Y para manter a visão estável durante pulos.
/// 
/// Setup:
///  1. Adicione este script na Main Camera
///  2. Arraste o GameObject do Player para o campo "target" no Inspector
///  3. Ajuste smoothSpeed e offset conforme necessário
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Transform do player que a câmera deve seguir.")]
    [SerializeField] private Transform target;

    [Tooltip("Velocidade de suavização do movimento (maior = mais rápido).")]
    [SerializeField] private float smoothSpeed = 0.125f;

    [Tooltip("Offset da câmera em relação ao player.")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Limites (Opcional)")]
    [Tooltip("Se marcado, limita o movimento horizontal da câmera.")]
    [SerializeField] private bool useBounds = false;

    [Tooltip("Limite esquerdo da câmera (se useBounds estiver ativo).")]
    [SerializeField] private float minX = -10f;

    [Tooltip("Limite direito da câmera (se useBounds estiver ativo).")]
    [SerializeField] private float maxX = 10f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calcula a posição desejada seguindo apenas o X do player
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            transform.position.y, // mantém Y fixo
            offset.z
        );

        // Aplica limites se configurado
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        // Suaviza o movimento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
