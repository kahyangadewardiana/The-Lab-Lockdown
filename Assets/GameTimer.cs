// GameTimer.cs
// Pasang ke GameObject kosong, misal "GameTimer" (boleh sejajar
// MissionManager).
//
// FUNGSI: Menghitung waktu berjalan (stopwatch) sejak scene ini mulai,
// ditampilkan di layar format MM:SS.
//
// CARA SETUP:
// 1. Di dalam Canvas, klik kanan > UI > Legacy > Text, beri nama
//    'TimerText'. Posisikan di pojok layar (misal kanan atas), atur
//    warna & ukuran font sesuai selera.
// 2. Buat GameObject kosong 'GameTimer', Add Component > GameTimer.
// 3. Drag 'TimerText' ke field 'Timer Text'.

using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text timerText;

    [Tooltip("Kalau dicentang, waktu BERHENTI menghitung waktu panel UI " +
             "(soal/kode/dll) sedang terbuka. Kalau tidak, waktu terus jalan.")]
    public bool pauseDuringUI = false;

    private float elapsedTime = 0f;

    private void Update()
    {
        bool shouldCount = !pauseDuringUI || !GameState.IsUIOpen;

        if (shouldCount)
        {
            elapsedTime += Time.deltaTime;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Dipakai kalau nanti mau ambil waktu akhir buat ditampilkan di
    // Game Over (misal "Kamu bertahan selama 04:32")
    public float GetElapsedSeconds()
    {
        return elapsedTime;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
