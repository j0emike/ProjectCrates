using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Spawner Settings")]
    public GameObject[] boxPrefabs;
    public int totalBoxesToSpawn = 6;
    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;
    public float spawnSafetyRadius = 1.0f; // Para evitar que se encimen
    public LayerMask interactableLayer;    //

    [Header("Game Stats")]
    public int currentScore = 0;
    private int boxesRemaining;

    [Header("Timer Settings")]
    public float timeRemaining = 60f; 
    private bool timerIsRunning = false;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject victoryMenuUI;
    public GameObject gameOverMenuUI;

    private bool isPaused = false;
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        SpawnBoxes();
        Application.targetFrameRate = 60;
        timerIsRunning = true;
    }

    void Update()
    {
        if (isGameOver) return;

        //Temporizador
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                // Actualizamos la UI
                if (UIManager.instance != null)
                    UIManager.instance.UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                LoseGame();
            }
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    void LoseGame()
    {
        isGameOver = true;
        gameOverMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Perdiste.");
    }

    void WinGame()
    {
        timerIsRunning = false; 
        isGameOver = true;
        victoryMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void SpawnBoxes()
    {
        if (boxPrefabs == null || boxPrefabs.Length == 0) return;

        boxesRemaining = totalBoxesToSpawn;

        for (int i = 0; i < totalBoxesToSpawn; i++)
        {
            Vector3 finalPos = Vector3.zero;
            bool foundPosition = false;
            int attempts = 0;
            int maxAttempts = 100; 

            while (!foundPosition && attempts < maxAttempts)
            {
                attempts++;

                // Generar posicion aleatoria dentro del rango
                float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
                float randomZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);

                // Usamos la Y de spawnAreaMin como base
                Vector3 testPos = new Vector3(randomX, spawnAreaMin.y, randomZ);

                //Checamos que no se encimen
                if (!Physics.CheckSphere(testPos, spawnSafetyRadius, interactableLayer))
                {
                    finalPos = testPos;
                    foundPosition = true;
                }
            }

            if (foundPosition)
            {
                GameObject prefabToSpawn = boxPrefabs[Random.Range(0, boxPrefabs.Length)];
                Instantiate(prefabToSpawn, finalPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"Caja {i}: No cabe.");
            }
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        boxesRemaining--;

        if (UIManager.instance != null)
            UIManager.instance.UpdateScoreDisplay(currentScore);

        if (boxesRemaining <= 0)
        {
            WinGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
{
    Debug.Log("Saliendo del juego");
    
    // Si estamos en el Editor de Unity, detenemos el Play Mode
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // Si es el juego final instalado, se cierra la aplicación
        Application.Quit();
    #endif
}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellowGreen;
        Vector3 center = (spawnAreaMin + spawnAreaMax) / 2;
        Vector3 size = new Vector3(
            Mathf.Abs(spawnAreaMax.x - spawnAreaMin.x),
            0.1f,
            Mathf.Abs(spawnAreaMax.z - spawnAreaMin.z)
        );
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(spawnAreaMin, spawnSafetyRadius);
    }
}