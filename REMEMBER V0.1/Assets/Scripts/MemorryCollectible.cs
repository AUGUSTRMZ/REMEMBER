
using UnityEngine; // importa las herramientas necesarias de unity

public class MemoryCollectible : MonoBehaviour // controla la recoleccion de un recuerdo
{
    public string memoryName = "Recuerdo"; // almacena el nombre del recuerdo

    public AudioClip memoryAudio; // almacena la pista de audio del recuerdo

    private bool collected = false; // evita que un recuerdo sea recogido varias veces

    void OnTriggerEnter2D(Collider2D other) // detecta cuando otro objeto entra en el trigger
    {
        if (collected) // comprueba si el recuerdo ya fue recogido
        {
            return; // termina el metodo para evitar repetir la recoleccion
        }

        if (other.CompareTag("Player")) // comprueba si el objeto que entro es la protagonista
        {
            collected = true; // marca el recuerdo como recogido

            Debug.Log("recuerdo recogido: " + memoryName); // muestra el nombre en la consola

            MemoryCounter counter = FindFirstObjectByType<MemoryCounter>(); // busca el contador de recuerdos en la escena

            if (counter != null) // comprueba que encontro el contador
            {
                counter.AddMemory(); // aumenta en uno la cantidad de recuerdos
            }
            else // se ejecuta si no encuentra el contador
            {
                Debug.LogWarning("no se encontro el contador de recuerdos"); // muestra una advertencia
            }

            if (memoryAudio != null) // comprueba si existe una pista de audio
            {
                Vector3 audioPosition = transform.position; // obtiene la posicion del recuerdo

                if (Camera.main != null) // comprueba que exista la camara principal
                {
                    audioPosition = Camera.main.transform.position; // utiliza la posicion de la camara
                }

                AudioSource.PlayClipAtPoint(memoryAudio, audioPosition); // reproduce el audio aunque desaparezca el recuerdo
            }

            gameObject.SetActive(false); // desactiva el recuerdo despues de recogerlo
        }
    }
}
