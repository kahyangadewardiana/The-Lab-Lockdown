// GameOverSceneController.cs
// Pasang ke GameObject kosong di dalam scene 'GameOverScene' (misal nama
// GameObject-nya 'GameOverController').
//
// FUNGSI: Mengatur tombol 'Yes' (main lagi, load ulang scene gameplay
// utama) dan 'No' (keluar dari game).
//
// CARA SETUP:
// 1. Di scene 'GameOverScene', buat GameObject kosong 'GameOverController'.
// 2. Add Component > GameOverSceneController.
// 3. Isi 'Gameplay Scene Name' dengan PERSIS nama scene utama game Anda
//    (misal "SampleScene").
// 4. Drag tombol 'Yes' Anda ke field 'Yes Button', drag tombol 'No' ke
//    field 'No Button'.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverSceneController : MonoBehaviour
{
    [Tooltip("Nama PERSIS scene gameplay utama Anda (harus terdaftar di Build Settings)")]
    public string gameplaySceneName = "SampleScene";

    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        // Pastikan waktu berjalan normal lagi (jaga-jaga kalau sebelumnya
        // sempat di-pause via Time.timeScale = 0)
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (yesButton != null) yesButton.onClick.AddListener(OnYesPressed);
        if (noButton != null) noButton.onClick.AddListener(OnNoPressed);
    }

    private void OnYesPressed()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void OnNoPressed()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
