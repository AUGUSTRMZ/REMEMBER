
using UnityEngine; // importa las herramientas necesarias de unity

public class RoomTransitions : MonoBehaviour // controla los cambios entre habitaciones
{
    public Transform targetSpawn; // punto donde aparecera el jugador
    public Transform targetRoom; // habitacion de destino
    public float activationDistance = 0.65f; // distancia necesaria para activar la puerta

    private Transform player; // referencia a la posicion del jugador
    private Rigidbody2D playerRb; // referencia al rigidbody del jugador
    private BoxCollider2D doorCollider; // referencia a la zona de la puerta
    private CameraControl cameraController; // referencia al controlador de camara

    private static float nextTransitionTime = 0f; // evita cambios consecutivos demasiado rapidos

[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] // ejecuta este metodo al iniciar una partida
static void ResetTransitionTime() // reinicia el tiempo de espera de las puertas
{
    nextTransitionTime = 0f; // permite utilizar las puertas desde el inicio
}

    void Start() // metodo que se ejecuta al iniciar el juego
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // busca al jugador por su etiqueta

        if (playerObject != null) // comprueba que exista el jugador
        {
            player = playerObject.transform; // guarda su transform
            playerRb = playerObject.GetComponent<Rigidbody2D>(); // obtiene su rigidbody
        }

        doorCollider = GetComponent<BoxCollider2D>(); // obtiene el collider de la puerta

        Camera mainCamera = Camera.main; // obtiene la camara principal

        if (mainCamera != null) // comprueba que exista la camara
        {
            cameraController = mainCamera.GetComponent<CameraControl>(); // obtiene el controlador de la camara
        }

        if (player == null || doorCollider == null || cameraController == null) // revisa los componentes necesarios
        {
            Debug.LogWarning("revisa los componentes del trigger " + gameObject.name); // informa si falta alguna referencia
        }
    }

    void Update() // comprueba continuamente la cercania del jugador
    {
        if (player == null || doorCollider == null) // comprueba que existan el jugador y la puerta
        {
            return; // termina si falta alguno
        }

        if (targetSpawn == null || targetRoom == null || cameraController == null) // comprueba las referencias de destino
        {
            return; // termina si falta alguna referencia
        }

        if (Time.time < nextTransitionTime) // comprueba si existe una transicion reciente
        {
            return; // espera antes de permitir otro cambio
        }

        Vector3 closestPoint = doorCollider.bounds.ClosestPoint(player.position); // obtiene el punto mas cercano de la puerta

        float distance = Vector2.Distance(player.position, closestPoint); // calcula la distancia entre el jugador y la puerta

        if (distance <= activationDistance) // comprueba si el jugador esta suficientemente cerca
        {
            ChangeRoom(); // realiza el cambio de habitacion
        }
    }

    void ChangeRoom() // transporta al jugador y cambia la camara
    {
        nextTransitionTime = Time.time + 0.8f; // bloquea temporalmente nuevas transiciones

        if (playerRb != null) // comprueba que exista el rigidbody
        {
            playerRb.position = targetSpawn.position; // coloca al jugador en el spawn de destino
            playerRb.linearVelocity = Vector2.zero; // elimina la velocidad anterior
        }
        else // se ejecuta si el jugador no tiene rigidbody
        {
            player.position = targetSpawn.position; // mueve directamente al jugador
        }

        cameraController.ChangeRoom(targetRoom); // mueve la camara a la nueva habitacion
    }
}
