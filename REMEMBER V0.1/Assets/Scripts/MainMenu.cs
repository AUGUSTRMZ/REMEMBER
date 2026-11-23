
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // nombre de la escena donde comienza el videojuego
    public string gameSceneName;

    // inicia una nueva partida
    public void NewGame()
    {
        // comprueba que se haya configurado la escena
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("falta configurar Game Scene Name");
            return;
        }

        // restaura el tiempo normal del juego
        Time.timeScale = 1f;

        // carga la escena principal
        SceneManager.LoadScene(gameSceneName);
    }

    // cierra el videojuego
    public void ExitGame()
    {
        Debug.Log("cerrando REMEMBER");

        // cierra la aplicacion compilada
        Application.Quit();

        // permite probar el boton desde el editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
