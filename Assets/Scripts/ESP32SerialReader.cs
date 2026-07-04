using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent;

/// <summary>
/// Comunicação bidirecional entre Unity e ESP32 via USB serial.
///
/// ESP32 → Unity (botões):
///   "L1" / "L0"  →  esquerda pressionado / solto
///   "R1" / "R0"  →  direita pressionado  / solto
///   "D1" / "D0"  →  baixo pressionado    / solto  (reservado)
///   "U"          →  pulo (borda de pressionar)
///
/// Unity → ESP32 (reações no OLED):
///   'I'  →  carinha neutra   (O _ O)
///   'T'  →  carinha feliz    (^ u ^)
///   'D'  →  carinha assust.  (> _ <)
/// </summary>
public class ESP32SerialReader : MonoBehaviour
{
    // ----------------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------------
    public static ESP32SerialReader Instance { get; private set; }

    // ----------------------------------------------------------------
    // Inspector
    // ----------------------------------------------------------------
    [Header("Configuração Serial")]
    [Tooltip("Mac: /dev/tty.usbserial-XXXX  |  Windows: COM3")]
    [SerializeField] private string portName = "/dev/tty.usbserial-0001";

    [Tooltip("Deve bater com Serial.begin() no ESP32 → 115200")]
    [SerializeField] private int baudRate = 115200;

    // ----------------------------------------------------------------
    // Estado público dos botões
    // ----------------------------------------------------------------

    /// <summary>True enquanto o botão Esquerda está pressionado.</summary>
    public bool ButtonLeft  { get; private set; }

    /// <summary>True enquanto o botão Direita está pressionado.</summary>
    public bool ButtonRight { get; private set; }

    /// <summary>True enquanto o botão Baixo está pressionado.</summary>
    public bool ButtonDown  { get; private set; }

    /// <summary>True por exatamente um frame quando Up (pulo) é pressionado.</summary>
    public bool JumpPressed { get; private set; }

    // Flag interna: setada pela thread de leitura, consumida no Update
    private bool _jumpPending;

    // ----------------------------------------------------------------
    // Privado
    // ----------------------------------------------------------------
    private SerialPort _serial;
    private Thread     _readThread;
    private bool       _running;

    private readonly ConcurrentQueue<string> _messageQueue
        = new ConcurrentQueue<string>();

    // ----------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() => OpenSerial();

    private void Update()
    {
        // JumpPressed é válido por apenas um frame
        JumpPressed = false;

        // Processa mensagens recebidas desde o último frame
        while (_messageQueue.TryDequeue(out string msg))
            ParseIncoming(msg.Trim());

        // Aplica pulo na main thread
        if (_jumpPending)
        {
            JumpPressed  = true;
            _jumpPending = false;
        }
    }

    private void OnDestroy()         => CloseSerial();
    private void OnApplicationQuit() => CloseSerial();

    // ----------------------------------------------------------------
    // API pública — envia reações para o OLED do ESP32
    // ----------------------------------------------------------------

    /// <summary>Carinha neutra (O _ O)</summary>
    public void SendIdle()     => WriteChar('I');

    /// <summary>Carinha feliz (^ u ^) — achou tesouro</summary>
    public void SendTreasure() => WriteChar('T');

    /// <summary>Carinha assustada (> _ <) — levou dano</summary>
    public void SendDamage()   => WriteChar('D');

    // ----------------------------------------------------------------
    // Serial
    // ----------------------------------------------------------------

    private void OpenSerial()
    {
        try
        {
            _serial = new SerialPort(portName, baudRate)
            {
                ReadTimeout  = 100,
                WriteTimeout = 100,
                DtrEnable    = true
            };
            _serial.Open();

            _running    = true;
            _readThread = new Thread(ReadLoop) { IsBackground = true };
            _readThread.Start();

            Debug.Log($"[ESP32] Conectado em {portName} @ {baudRate} baud.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[ESP32] Falha ao abrir '{portName}': {e.Message}\n" +
                             "Verifique a porta no Inspector e se o ESP32 está conectado.");
        }
    }

    private void CloseSerial()
    {
        _running = false;
        _readThread?.Join(500);
        if (_serial != null && _serial.IsOpen)
            _serial.Close();
    }

    // ----------------------------------------------------------------
    // Thread de leitura (não roda na main thread)
    // ----------------------------------------------------------------

    private void ReadLoop()
    {
        while (_running)
        {
            try
            {
                if (_serial.IsOpen)
                {
                    string line = _serial.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                        _messageQueue.Enqueue(line);
                }
            }
            catch (System.TimeoutException) { /* normal */ }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[ESP32] Erro de leitura: {e.Message}");
                _running = false;
            }
        }
    }

    // ----------------------------------------------------------------
    // Parser
    // ----------------------------------------------------------------

    private void ParseIncoming(string msg)
    {
        if (msg.Length == 0) return;

        switch (msg)
        {
            case "L1": ButtonLeft  = true;         break;
            case "L0": ButtonLeft  = false;        break;
            case "R1": ButtonRight = true;         break;
            case "R0": ButtonRight = false;        break;
            case "D1": ButtonDown  = true;         break;
            case "D0": ButtonDown  = false;        break;
            case "U":  _jumpPending = true; Debug.Log("[ESP32] Pulo recebido!"); break;
            default:
                Debug.Log($"[ESP32] Mensagem desconhecida: '{msg}'");
                break;
        }
    }

    // ----------------------------------------------------------------
    // Escrita para o ESP32
    // ----------------------------------------------------------------

    private void WriteChar(char c)
    {
        try
        {
            if (_serial != null && _serial.IsOpen)
                _serial.Write(c.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[ESP32] Falha ao enviar '{c}': {e.Message}");
        }
    }
}
