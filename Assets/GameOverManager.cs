// GameOverManager.cs
// Pasang ke GameObject kosong, misal "GameOverManager".
//
// CARA SETUP UI:
// 1. Di dalam Canvas, klik kanan > UI > Legacy > Panel, beri nama
//    "GameOverPanel". Atur menutupi seluruh layar (anchor stretch-stretch),
//    warna hitam pekat atau merah gelap.
// 2. Di dalamnya, klik kanan > UI > Legacy > Text, beri nama
//    "GameOverText", isi teksnya "GAME OVER", ukuran font besar (misal 60),
//    warna merah/putih, posisikan di tengah atas.
// 3. Di dalamnya juga, klik kanan > UI > Legacy > Button, beri nama
//    "RestartButton", ganti teks child-nya jadi "Main Lagi".
// 4. Klik GameOverPanel, UNCENTANG supaya tersembunyi di awal.
// 5. Buat GameObject kosong "GameOverManager", pasang script ini, isi
//    field 'Game Over Panel' dan 'Restart Button'.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    public GameObject gameOverPanel;
    public Button restartButton;

    private void Awake()
    {
        Instance = this;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        GameState.IsUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // benar-benar hentikan waktu/fisika juga
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // kembalikan waktu normal dulu sebelum reload
        GameState.IsUIOpen = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
