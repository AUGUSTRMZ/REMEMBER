
using UnityEngine; // importa las herramientas necesarias de unity

public class PlayerMovement : MonoBehaviour // controla el movimiento y las animaciones de la protagonista
{
    public float speed = 2.0f; // velocidad de movimiento del personaje

    public float horizontalInput; // almacena el movimiento horizontal del teclado
    public float verticalInput; // almacena el movimiento vertical del teclado

    private Rigidbody2D playerRb; // referencia al rigidbody del personaje
    private Animator playerAnimator; // referencia al animator del objeto visual

    private Vector2 movement; // almacena la direccion del movimiento
    private int direction = 0; // almacena la direccion actual de la animacion

    void Start() // se ejecuta una vez al iniciar el juego
    {
        playerRb = GetComponent<Rigidbody2D>(); // obtiene el rigidbody del personaje

        playerAnimator = GetComponentInChildren<Animator>(); // obtiene el animator del objeto visual

        if (playerAnimator != null) // comprueba que exista el animator
        {
            playerAnimator.SetBool("Moving", false); // inicia con el personaje quieto

            playerAnimator.SetInteger("Direction", 0); // inicia con la direccion hacia abajo
        }
    }

    void Update() // se ejecuta una vez por cada fotograma
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // obtiene el movimiento horizontal con a y d

        verticalInput = Input.GetAxisRaw("Vertical"); // obtiene el movimiento vertical con w y s

        movement = new Vector2(horizontalInput, verticalInput); // crea el vector de movimiento

        movement = movement.normalized; // evita que el personaje avance mas rapido en diagonal

        if (verticalInput > 0) // comprueba si se presiona w
        {
            direction = 1; // selecciona la animacion de caminar hacia arriba
        }
        else if (verticalInput < 0) // comprueba si se presiona s
        {
            direction = 0; // selecciona la animacion de caminar hacia abajo
        }
        else if (horizontalInput < 0) // comprueba si se presiona a
        {
            direction = 2; // selecciona la animacion de caminar hacia la izquierda
        }
        else if (horizontalInput > 0) // comprueba si se presiona d
        {
            direction = 3; // selecciona la animacion de caminar hacia la derecha
        }

        if (playerAnimator != null) // comprueba que exista el animator
        {
            bool isMoving = movement.sqrMagnitude > 0.01f; // comprueba si el personaje esta caminando

            playerAnimator.SetBool("Moving", isMoving); // activa o desactiva las animaciones de caminar

            playerAnimator.SetInteger("Direction", direction); // comunica la direccion al animator
        }
    }

    void FixedUpdate() // se ejecuta durante las actualizaciones de fisica
    {
        Vector2 newPosition = playerRb.position + movement * speed * Time.fixedDeltaTime; // calcula la nueva posicion

        playerRb.MovePosition(newPosition); // mueve al personaje respetando las colisiones
    }
}
