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
    public Vector3 spawnAreaMin; //Esquinas del Spawner
    public Vector3 spawnAreaMax; 

    [Header("Game Stats")]
    public int currentScore = 0;
    private int boxesRemaining;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject victoryMenuUI; 

    private bool isPaused = false;
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        SpawnBoxes();
    }

    void Update()
    {
        if (isGameOver) return;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    void SpawnBoxes()
    {
        boxesRemaining = totalBoxesToSpawn;

        for (int i = 0; i < totalBoxesToSpawn; i++)
        {
            GameObject prefabToSpawn = boxPrefabs[Random.Range(0, boxPrefabs.Length)];

            Vector3 randomPos = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                spawnAreaMin.y,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
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

    void WinGame()
    {
        isGameOver = true;
        victoryMenuUI.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
    Debug.Log("Saliendo del juego...");
    
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
            0.5f,
            Mathf.Abs(spawnAreaMax.z - spawnAreaMin.z)
        );
        Gizmos.DrawWireCube(center, size);
    }
}