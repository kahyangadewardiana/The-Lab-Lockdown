// WinTrigger.cs
// Pasang ke GameObject trigger (Box Collider dengan 'Is Trigger'
// dicentang) di CELAH pintu keluar AKHIR (mirip DoorTrigger, tapi ini
// yang men-trigger KEMENANGAN, bukan auto-close).
//
// CARA SETUP:
// 1. Buat GameObject kosong di celah pintu keluar akhir, beri nama
//    'WinTrigger'.
// 2. Add Component > Box Collider, CENTANG 'Is Trigger'. Sesuaikan
//    ukurannya menutupi celah pintu.
// 3. Add Component > WinTrigger (script ini).
// 4. Isi 'Win Scene Name' dengan nama scene kemenangan Anda (buat baru,
//    mirip cara bikin JumpscareScene/GameOverScene sebelumnya).
// 5. WAJIB: tambahkan scene kemenangan itu ke Build Settings/Build
//    Profiles (File > Build Profiles > Add Open Scenes waktu scene itu
//    aktif), sama seperti JumpscareScene/GameOverScene dulu.

using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public string winSceneName = "WinScene";

    [Tooltip("Drag GameTimer dari scene Anda, supaya waktu pengerjaan " +
             "ikut tercatat untuk ditampilkan di WinScene")]
    public GameTimer gameTimer;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.GetComponentInParent<FirstPersonController>() == null) return;

        triggered = true;

        // Simpan skor & waktu SEBELUM pindah scene, supaya WinScene bisa
        // menampilkannya (ScoreManager & GameTimer akan hilang begitu
        // scene ini di-unload).
        if (ScoreManager.Instance != null)
        {
            GameScoreData.FinalScore = ScoreManager.Instance.GetScore();
            GameScoreData.FinalTotalScore = ScoreManager.Instance.GetTotalScore();
        }

        if (gameTimer != null)
        {
            GameScoreData.FinalTimeFormatted = gameTimer.GetFormattedTime();
        }

        SceneManager.LoadScene(winSceneName);
    }
}