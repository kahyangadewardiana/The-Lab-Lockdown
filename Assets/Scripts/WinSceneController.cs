// WinSceneController.cs
// Pasang ke GameObject kosong di dalam scene 'WinScene'.
//
// CARA SETUP UI (di dalam Canvas scene WinScene):
// 1. Buat Text "GameWinText", isi "GAME WIN" (atau "KAMU BERHASIL KABUR!"),
//    font besar, warna mencolok.
// 2. Buat Text "TimeText", contoh isi awal "Waktu: 00:00" (nanti diisi
//    otomatis oleh script).
// 3. Buat Text "ScoreText", contoh isi awal "Skor: 0/0" (diisi otomatis).
// 4. Buat Button "RestartButton", teks child-nya "Main Lagi".
// 5. Buat Button "MainMenuButton", teks child-nya "Menu Utama".
// 6. Buat GameObject kosong "WinSceneController", pasang script ini.
// 7. Isi field 'Gameplay Scene Name' sesuai nama scene utama Anda.
// 8. Isi field 'Main Menu Scene Name' sesuai nama scene Main Menu Anda
//    yang sudah ada.
// 9. Drag Text & Button di atas ke field yang sesuai di Inspector.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinSceneController : MonoBehaviour
{
    [Header("Nama Scene")]
    public string gameplaySceneName = "SampleScene";
    public string mainMenuSceneName = "MainMenu";

    [Header("Suara (opsional)")]
    [Tooltip("Dimainkan sekali begitu WinScene muncul (misal fanfare kemenangan)")]
    public AudioClip winSound;
    [Range(0f, 1f)] public float winSoundVolume = 1f;

    [Tooltip("Musik yang berputar terus selama di layar menang (opsional)")]
    public AudioClip winMusic;
    [Range(0f, 1f)] public float winMusicVolume = 0.6f;

    [Header("UI References")]
    public Text gameWinText;
    public Text timeText;
    public Text scoreText;
    public Button restartButton;
    public Button mainMenuButton;

    private void Awake()
    {
        // Jaga-jaga kalau sebelumnya sempat ke-pause
        Time.timeScale = 1f;
        GameState.IsUIOpen = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayWinAudio();

        if (gameWinText != null)
        {
            gameWinText.text = "GAME WIN";
        }

        if (timeText != null)
        {
            timeText.text = $"Waktu Pengerjaan: {GameScoreData.FinalTimeFormatted}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Skor: {GameScoreData.FinalScore}/{GameScoreData.FinalTotalScore}";
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }

    private void PlayWinAudio()
    {
        if (winSound != null)
        {
            AudioSource sfx = gameObject.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
            sfx.spatialBlend = 0f;
            sfx.PlayOneShot(winSound, winSoundVolume);
        }

        if (winMusic != null)
        {
            AudioSource music = gameObject.AddComponent<AudioSource>();
            music.clip = winMusic;
            music.loop = true;
            music.volume = winMusicVolume;
            music.playOnAwake = false;
            music.spatialBlend = 0f;
            music.Play();
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}