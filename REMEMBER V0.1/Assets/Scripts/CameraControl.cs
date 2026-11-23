
using UnityEngine; // importa las herramientas necesarias de unity

public class CameraControl : MonoBehaviour // clase encargada de controlar la camara
{
    public Transform currentRoom; // almacena la habitacion actual
    public float cameraZ = -10.0f; // profundidad de la camara

    void Start() // metodo que se ejecuta al iniciar el juego
    {
        ChangeRoom(currentRoom); // coloca la camara en la habitacion inicial
    }

    public void ChangeRoom(Transform newRoom) // cambia la camara a otra habitacion
    {
        if (newRoom == null) // comprueba que exista la habitacion
        {
            return; // termina el metodo si no existe
        }

        currentRoom = newRoom; // actualiza la habitacion actual

        SpriteRenderer roomSprite = currentRoom.GetComponentInChildren<SpriteRenderer>(); // busca el dibujo de la habitacion

        if (roomSprite == null) // comprueba que exista un sprite
        {
            Debug.LogWarning("la habitacion no tiene un sprite renderer"); // muestra una advertencia
            return; // termina si no encuentra el dibujo
        }

        Vector3 roomCenter = roomSprite.bounds.center; // obtiene el centro real del dibujo

        Vector3 newPosition = new Vector3(
            roomCenter.x, // obtiene la coordenada horizontal del dibujo
            roomCenter.y, // obtiene la coordenada vertical del dibujo
            cameraZ // conserva la profundidad de la camara
        );

        transform.position = newPosition; // mueve la camara al centro del dibujo
    }
}
