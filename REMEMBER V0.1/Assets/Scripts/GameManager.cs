
using UnityEngine; // importa las herramientas necesarias de unity
using UnityEngine.SceneManagement; // permite cargar y reiniciar escenas

public class GameManager : MonoBehaviour // administra las acciones generales del juego
{
    public void RestartGame() // reinicia la partida desde el principio
    {
        Time.timeScale = 1.0f; // devuelve la velocidad normal al juego

        Scene currentScene = SceneManager.GetActiveScene(); // obtiene la escena actual

        SceneManager.LoadScene(currentScene.buildIndex); // vuelve a cargar la misma escena
    }
}
