
using UnityEngine; // importa las herramientas necesarias de unity
using TMPro; // permite modificar textos de textmeshpro

public class MemoryCounter : MonoBehaviour // controla el contador de recuerdos
{
    public int totalMemories = 3; // cantidad total de recuerdos en el juego
    public int collectedMemories = 0; // cantidad de recuerdos recogidos

    private TextMeshProUGUI counterText; // referencia al texto que aparece en pantalla

    void Start() // se ejecuta al iniciar el juego
    {
        counterText = GetComponent<TextMeshProUGUI>(); // obtiene el texto del mismo objeto

        UpdateCounter(); // muestra el contador inicial
    }

    public void AddMemory() // aumenta el contador al recoger un recuerdo
    {
        if (collectedMemories < totalMemories) // evita superar el total de recuerdos
        {
            collectedMemories++; // aumenta en uno la cantidad de recuerdos
        }

        UpdateCounter(); // actualiza el texto en pantalla
    }

    void UpdateCounter() // actualiza el texto del contador
    {
        if (counterText != null) // comprueba que exista el componente de texto
        {
            counterText.text = "Recuerdos: " + collectedMemories + " / " + totalMemories; // muestra la cantidad recogida
        }
    }
}
