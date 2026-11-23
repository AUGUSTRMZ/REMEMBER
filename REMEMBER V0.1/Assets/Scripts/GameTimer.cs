
using UnityEngine; // importa las herramientas necesarias de unity
using TMPro; // permite modificar los textos de textmeshpro

public class GameTimer : MonoBehaviour // controla el temporizador y la derrota por tiempo
{
    public float totalTime = 120.0f; // duracion total de la partida en segundos

    public float remainingTime; // tiempo restante de la partida

    public GameObject panelDerrota; // referencia al panel de derrota

    private TextMeshProUGUI timerText; // referencia al texto del temporizador

    private bool timerRunning = true; // indica si el temporizador esta funcionando

    void Start() // se ejecuta al iniciar la partida
    {
        timerText = GetComponent<TextMeshProUGUI>(); // obtiene el componente de texto

        remainingTime = totalTime; // establece el tiempo inicial

        timerRunning = true; // activa el temporizador

        UpdateTimerText(); // muestra el tiempo inicial en pantalla
    }

    void Update() // se ejecuta una vez por fotograma
    {
        if (timerRunning) // comprueba que el temporizador este activo
        {
            remainingTime -= Time.deltaTime; // disminuye el tiempo restante

            if (remainingTime <= 0.0f) // comprueba si el tiempo se ha terminado
            {
                remainingTime = 0.0f; // evita mostrar tiempo negativo

                UpdateTimerText(); // muestra 00:00 en pantalla

                LoseGame(); // activa la derrota por tiempo

                return; // termina la actualizacion
            }

            UpdateTimerText(); // actualiza los minutos y segundos
        }
    }

    void UpdateTimerText() // actualiza el texto del temporizador
    {
        int secondsLeft = Mathf.CeilToInt(remainingTime); // convierte el tiempo restante a segundos enteros

        int minutes = secondsLeft / 60; // calcula los minutos restantes

        int seconds = secondsLeft % 60; // calcula los segundos restantes

        if (timerText != null) // comprueba que exista el componente de texto
        {
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00"); // muestra el tiempo en formato mm:ss
        }
    }

    void LoseGame() // ejecuta la derrota cuando se termina el tiempo
    {
        timerRunning = false; // detiene el temporizador

        if (panelDerrota != null) // comprueba que exista el panel de derrota
        {
            panelDerrota.SetActive(true); // muestra la pantalla de derrota
        }

        Time.timeScale = 0.0f; // pausa el movimiento y el tiempo del juego

        Debug.Log("derrota: se agoto el tiempo"); // muestra un mensaje en la consola
    }

    public void StopTimer() // permite detener el temporizador al ganar la partida
    {
        timerRunning = false; // detiene el conteo del tiempo
    }
}
