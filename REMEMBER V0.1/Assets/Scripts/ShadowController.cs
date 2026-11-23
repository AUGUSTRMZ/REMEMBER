
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShadowController : MonoBehaviour
{
    // configuracion de la persecucion

    public float chaseSpeed = 2.3f;
    public float chaseDuration = 10f;
    public float chaseAudioDuration = 9f;
    public float captureDistance = 0.65f;


    // puntos de aparicion de las habitaciones

    public Transform salaSpawn;
    public Transform dormitorioSpawn;
    public Transform cocinaSpawn;
    public Transform porticoSpawn;
    public Transform patioSpawn;


    // sonidos de la sombra

    public AudioClip spawnSound;
    public AudioClip chaseMusic;
    public AudioClip disappearSound;
    public AudioClip deathScream;


    // pantalla que aparece cuando muere la protagonista

    public GameObject panelMuerte;
    public float deathScreenDuration = 3f;


    // componentes de la sombra

    private Rigidbody2D shadowRb;
    private Animator shadowAnimator;
    private SpriteRenderer shadowRenderer;
    private Collider2D shadowCollider;


    // referencias del juego

    private GameObject player;
    private GameTimer gameTimer;


    // reproductores de audio

    private AudioSource spawnSource;
    private AudioSource chaseSource;
    private AudioSource disappearSource;
    private AudioSource deathSource;


    // configuracion de las cuatro apariciones

    private Transform[] spawnPoints;

    private float[] appearanceTimes =
    {
        15f,
        45f,
        75f,
        105f
    };

    private int nextAppearance = 0;
    private float chaseElapsed = 0f;


    // estados de la sombra

    private bool isChasing = false;
    private bool playerCaught = false;
    private bool chaseAudioStopped = false;

    private Transform currentSpawn;


    void Start()
    {
        // obtiene los componentes de la sombra

        shadowRb = GetComponent<Rigidbody2D>();
        shadowAnimator = GetComponent<Animator>();
        shadowRenderer = GetComponent<SpriteRenderer>();
        shadowCollider = GetComponent<Collider2D>();


        // busca la protagonista y el temporizador

        player = GameObject.FindGameObjectWithTag("Player");
        gameTimer = FindAnyObjectByType<GameTimer>();


        // almacena los puntos de aparicion

        spawnPoints = new Transform[]
        {
            salaSpawn,
            dormitorioSpawn,
            cocinaSpawn,
            porticoSpawn,
            patioSpawn
        };


        // crea cuatro reproductores de audio independientes

        spawnSource = CreateAudioSource();
        chaseSource = CreateAudioSource();
        disappearSource = CreateAudioSource();
        deathSource = CreateAudioSource();


        // permite repetir la musica de persecucion

        chaseSource.loop = true;


        // da prioridad al sonido del grito

        deathSource.priority = 0;


        // oculta el panel de muerte al comenzar

        if (panelMuerte != null)
        {
            panelMuerte.SetActive(false);
        }


        // comienza con la sombra oculta

        HideShadow();
    }


    AudioSource CreateAudioSource()
    {
        // agrega un reproductor de audio al objeto

        AudioSource source = gameObject.AddComponent<AudioSource>();

        // evita que el sonido comience automaticamente

        source.playOnAwake = false;

        // utiliza audio 2d para escucharlo desde cualquier punto

        source.spatialBlend = 0f;

        // configura el volumen

        source.volume = 1f;

        // evita que los sonidos normales se repitan

        source.loop = false;

        return source;
    }


    void Update()
    {
        // no hace nada si la protagonista ya murio

        if (playerCaught)
        {
            return;
        }


        // comprueba si el juego esta pausado

        if (Time.timeScale == 0f)
        {
            if (isChasing)
            {
                isChasing = false;
                chaseSource.Stop();
                HideShadow();
            }

            return;
        }


        // verifica que existan las referencias necesarias

        if (player == null || gameTimer == null)
        {
            return;
        }


        // evita nuevas apariciones si se termino el tiempo

        if (gameTimer.remainingTime <= 0f)
        {
            return;
        }


        // calcula cuantos segundos han pasado desde que inicio el juego

        float elapsedTime =
            gameTimer.totalTime - gameTimer.remainingTime;


        // revisa si debe aparecer la sombra

        if (!isChasing && nextAppearance < appearanceTimes.Length)
        {
            if (elapsedTime >= appearanceTimes[nextAppearance])
            {
                // registra la aparicion para no repetirla

                nextAppearance++;

                // comienza la persecucion

                StartChase();
            }
        }


        // actualiza una persecucion activa

        if (isChasing)
        {
            // aumenta el contador de la persecucion

            chaseElapsed += Time.deltaTime;


            // identifica la habitacion donde esta la protagonista

            Transform newRoomSpawn = FindCurrentRoomSpawn();


            // lleva la sombra a la nueva habitacion si el jugador cambio

            if (newRoomSpawn != null && newRoomSpawn != currentSpawn)
            {
                currentSpawn = newRoomSpawn;

                shadowRb.position = currentSpawn.position;
            }


            // actualiza la animacion segun la direccion

            UpdateDirection();


            // detiene la musica despues de nueve segundos

            if (chaseElapsed >= chaseAudioDuration &&
                !chaseAudioStopped)
            {
                chaseSource.Stop();

                chaseAudioStopped = true;
            }


            // termina la persecucion despues de diez segundos

            if (chaseElapsed >= chaseDuration)
            {
                EndChase();
            }
        }
    }


    void FixedUpdate()
    {
        // comprueba si la sombra puede moverse

        if (!isChasing || playerCaught || player == null)
        {
            return;
        }


        // obtiene la posicion de la protagonista

        Vector2 targetPosition = player.transform.position;


        // calcula la siguiente posicion de la sombra

        Vector2 newPosition = Vector2.MoveTowards(
            shadowRb.position,
            targetPosition,
            chaseSpeed * Time.fixedDeltaTime
        );


        // mueve la sombra hacia la protagonista

        shadowRb.MovePosition(newPosition);


        // calcula la distancia entre ambos personajes

        float distance = Vector2.Distance(
            newPosition,
            targetPosition
        );


        // comprueba si la sombra alcanzo a la protagonista

        if (distance <= captureDistance)
        {
            CatchPlayerNow();
        }
    }


    void StartChase()
    {
        // busca el punto de aparicion de la habitacion actual

        currentSpawn = FindCurrentRoomSpawn();


        // evita aparecer si no encuentra un punto valido

        if (currentSpawn == null)
        {
            Debug.LogWarning(
                "no se encontro un punto de aparicion para la sombra"
            );

            return;
        }


        // coloca la sombra en el punto correspondiente

        shadowRb.position = currentSpawn.position;


        // reinicia la informacion de persecucion

        chaseElapsed = 0f;
        chaseAudioStopped = false;
        isChasing = true;


        // hace visible la sombra y activa su detector

        shadowRenderer.enabled = true;
        shadowCollider.enabled = true;


        // reproduce el sonido de aparicion

        if (spawnSound != null)
        {
            spawnSource.PlayOneShot(spawnSound);
        }


        // reproduce la musica de persecucion

        if (chaseMusic != null)
        {
            chaseSource.clip = chaseMusic;
            chaseSource.Play();
        }


        Debug.Log("la sombra ha aparecido");
    }


    void EndChase()
    {
        // evita terminar dos veces la misma persecucion

        if (!isChasing || playerCaught)
        {
            return;
        }


        // desactiva la persecucion

        isChasing = false;


        // detiene la musica

        chaseSource.Stop();


        // oculta la sombra

        HideShadow();


        // reproduce el sonido de desaparicion

        if (disappearSound != null)
        {
            disappearSource.PlayOneShot(disappearSound);
        }


        Debug.Log("la sombra ha desaparecido");
    }


    void HideShadow()
    {
        // oculta la imagen sin desactivar el objeto principal

        if (shadowRenderer != null)
        {
            shadowRenderer.enabled = false;
        }


        // desactiva el detector de contacto

        if (shadowCollider != null)
        {
            shadowCollider.enabled = false;
        }
    }


    Transform FindCurrentRoomSpawn()
    {
        // evita buscar si no existe la protagonista

        if (player == null)
        {
            return null;
        }


        Transform closestSpawn = null;
        float closestDistance = float.MaxValue;


        // revisa los cinco puntos de aparicion

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null)
            {
                continue;
            }


            // calcula la distancia entre el jugador y el punto

            float distance = Vector2.Distance(
                player.transform.position,
                spawn.position
            );


            // guarda el punto mas cercano

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSpawn = spawn;
            }
        }


        return closestSpawn;
    }


    void UpdateDirection()
    {
        // verifica que existan el animator y la protagonista

        if (shadowAnimator == null || player == null)
        {
            return;
        }


        // calcula la direccion hacia la protagonista

        Vector2 direction =
            player.transform.position - transform.position;


        // utiliza abajo como direccion inicial

        int animationDirection = 0;


        // comprueba si predomina el movimiento horizontal

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < 0f)
            {
                // izquierda

                animationDirection = 2;
            }
            else
            {
                // derecha

                animationDirection = 3;
            }
        }
        else if (direction.y > 0f)
        {
            // arriba

            animationDirection = 1;
        }


        // cambia la animacion de la sombra

        shadowAnimator.SetInteger(
            "Direction",
            animationDirection
        );
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // comprueba si existe una persecucion activa

        if (!isChasing || playerCaught)
        {
            return;
        }


        // comprueba si el collider pertenece a la protagonista

        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            CatchPlayerNow();
        }
    }


    void CatchPlayerNow()
    {
        // evita procesar varias capturas al mismo tiempo

        if (playerCaught || !isChasing)
        {
            return;
        }


        // registra que la protagonista fue atrapada

        playerCaught = true;

        // detiene la persecucion

        isChasing = false;


        Debug.Log("la sombra atrapo a la protagonista");


        // comienza la secuencia de muerte

        StartCoroutine(CatchPlayer());
    }


    IEnumerator CatchPlayer()
    {
        // detiene los sonidos de la persecucion

        chaseSource.Stop();
        spawnSource.Stop();
        disappearSource.Stop();


        // oculta la sombra

        HideShadow();


        // detiene el temporizador

        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }


        // pausa el movimiento del juego

        Time.timeScale = 0f;


        // establece una duracion provisional del grito

        float screamDuration = 0f;


        // reproduce el grito femenino completo

        if (deathScream != null)
        {
            deathSource.clip = deathScream;

            deathSource.Play();

            screamDuration = deathScream.length;
        }
        else
        {
            Debug.LogWarning(
                "falta asignar Death Scream en el inspector"
            );
        }


        // espera a que termine el grito sin depender del tiempo pausado

        if (screamDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                screamDuration
            );
        }


        // muestra la pantalla de muerte

        if (panelMuerte != null)
        {
            panelMuerte.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "no se asigno un panel"
            );
        }


        // conserva la pantalla visible durante tres segundos

        yield return new WaitForSecondsRealtime(
            deathScreenDuration
        );


        // restaura el tiempo normal

        Time.timeScale = 1f;


        // busca el administrador de la partida

        GameManager manager =
            FindAnyObjectByType<GameManager>();


        // reinicia la escena

        if (manager != null)
        {
            manager.RestartGame();
        }
        else
        {
            Scene currentScene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}
