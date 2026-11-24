// SpawnHintManager.cs
// Pasang ke GameObject kosong, misal "SpawnHintManager".
//
// FUNGSI: Menampilkan pesan petunjuk otomatis begitu game mulai (Play),
// lalu hilang sendiri setelah beberapa detik. TIDAK menghentikan
// pergerakan Player (bisa dibaca sambil tetap bisa jalan).
//
// CARA SETUP UI:
// 1. Di dalam Canvas, klik kanan > UI > Legacy > Panel, beri nama
//    "SpawnHintPanel". Posisikan di bagian ATAS atau TENGAH layar,
//    ukuran secukupnya (misal 600x120). Atur warna background agak
//    gelap transparan biar teksnya kebaca jelas.
// 2. Di dalamnya, klik kanan > UI > Legacy > Text, beri nama
//    "SpawnHintText". Atur warna terang (putih), ukuran font cukup
//    besar (misal 22-28), alignment tengah.
// 3. Klik "SpawnHintPanel", UNCENTANG supaya tersembunyi di awal --
//    script ini yang akan menyalakannya otomatis waktu Play.
// 4. Buat GameObject kosong "SpawnHintManager", pasang script ini.
// 5. Drag "SpawnHintPanel" ke field 'Hint Panel', drag "SpawnHintText"
//    ke field 'Hint Text'.
// 6. Isi field 'Message' dengan teks petunjuk Anda (sudah ada contoh
//    default, boleh diganti sesuai selera).

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SpawnHintManager : MonoBehaviour
{
    public GameObject hintPanel;
    public Text hintText;

    [TextArea(2, 4)]
    public string message = "Carilah empat digit angka tersembunyi di ruangan " +
        "untuk memasukkan kode dan membuka pintu.";

    [Tooltip("Berapa lama pesan ini ditampilkan sebelum hilang otomatis (detik)")]
    public float displayDuration = 6f;

    [Tooltip("Kalau dicentang, pesan juga bisa ditutup manual dengan tombol E")]
    public bool allowSkipWithE = true;

    private bool hasShown = false;

    // Dipakai RoomHintTrigger.cs supaya ruangan lain bisa menampilkan
    // petunjuk lewat panel yang SAMA dengan petunjuk di ruangan pertama.
    public static SpawnHintManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Tampilkan pesan baru di panel yang sama. duration <= 0 berarti
    // pakai 'Display Duration' bawaan.
    public void ShowHint(string newMessage, float duration = -1f)
    {
        if (hintText != null) hintText.text = newMessage;
        if (hintPanel != null) hintPanel.SetActive(true);

        hasShown = true;
        CancelInvoke(nameof(HideHint));
        Invoke(nameof(HideHint), duration > 0f ? duration : displayDuration);
    }

    private void Start()
    {
        if (hintPanel != null) hintPanel.SetActive(true);
        if (hintText != null) hintText.text = message;

        hasShown = true;
        Invoke(nameof(HideHint), displayDuration);
    }

    private void Update()
    {
        if (!hasShown) return;
        if (!allowSkipWithE) return;
        if (hintPanel == null || !hintPanel.activeSelf) return;

        // Skip manual pakai E, tanpa perlu Input Manager lama -- pakai
        // Input System baru supaya konsisten dengan script lain.
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            HideHint();
        }
    }

    private void HideHint()
    {
        CancelInvoke(nameof(HideHint));
        if (hintPanel != null) hintPanel.SetActive(false);
    }
}