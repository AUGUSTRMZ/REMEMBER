
using UnityEngine; // importa las herramientas necesarias de unity

public class VictoryExit : MonoBehaviour // controla la salida y la victoria del juego
{
    public GameObject panelVictoria; // referencia al panel que aparece cuando ganamos

    public float activationDistance = 0.5f; // distancia necesaria para activar la salida

    private GameObject player; // referencia a la protagonista
    private Collider2D exitCollider; // referencia al collider de la salida
    private MemoryCounter memoryCounter; // referencia al contador de recuerdos
    private GameTimer gameTimer; // referencia al temporizador

    private bool gameWon = false; // evita ejecutar la victoria varias veces

    void Start() // se ejecuta al iniciar la partida
    {
        player = GameObject.FindGameObjectWithTag("Player"); // busca a la protagonista

        exitCollider = GetComponent<Collider2D>(); // obtiene el collider de la salida

        memoryCounter = FindAnyObjectByType<MemoryCounter>(); // busca el contador de recuerdos

        gameTimer = FindAnyObjectByType<GameTimer>(); // busca el temporizador
    }

    void Update() // comprueba constantemente si el jugador puede escapar
    {
        if (gameWon) // comprueba si la partida ya fue ganada
        {
            return; // evita repetir la victoria
        }

        if (player == null || exitCollider == null || memoryCounter == null || panelVictoria == null) // comprueba que existan las referencias
        {
            return; // evita errores si falta algun objeto
        }

        if (memoryCounter.collectedMemories < memoryCounter.totalMemories) // comprueba si faltan recuerdos
        {
            return; // impide escapar sin los tres recuerdos
        }

        Vector2 playerPosition = player.transform.position; // obtiene la posicion de la protagonista

        Vector2 closestPoint = exitCollider.bounds.ClosestPoint(playerPosition); // encuentra el punto mas cercano de la salida

        float distance = Vector2.Distance(playerPosition, closestPoint); // calcula la distancia a la salida

        if (distance <= activationDistance) // comprueba si la protagonista esta cerca de la salida
        {
            WinGame(); // activa la victoria
        }
    }

    void WinGame() // ejecuta las acciones necesarias para ganar
    {
        gameWon = true; // marca la partida como ganada

        if (gameTimer != null) // comprueba que exista el temporizador
        {
            gameTimer.StopTimer(); // detiene el reloj
        }

        panelVictoria.SetActive(true); // muestra la pantalla de victoria

        Time.timeScale = 0.0f; // pausa el movimiento y el tiempo del juego

        Debug.Log("victoria: la protagonista logro escapar"); // muestra un mensaje en consola
    }
}
