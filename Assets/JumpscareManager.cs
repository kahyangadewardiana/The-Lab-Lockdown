// JumpscareManager.cs
// Pasang ke GameObject kosong, misal "JumpscareManager".
//
// FUNGSI: Waktu Player ketangkap hantu, tampilkan efek jumpscare
// (layar penuh + suara) selama beberapa detik, BARU pindah ke Game Over.
//
// CARA SETUP UI:
// 1. Di dalam Canvas yang sudah ada, klik kanan > UI > Legacy > Image,
//    beri nama "JumpscarePanel". Atur Rect Transform-nya supaya
//    menutupi SELURUH layar (anchor preset: stretch-stretch, semua
//    margin 0).
// 2. Di komponen 'Image' JumpscarePanel, ganti 'Source Image' jadi
//    texture wajah hantu Anda kalau ada (misal dari Body.png/Eye.png
//    Smiley), ATAU biarkan kosong dan ganti 'Color' jadi merah tua/hitam
//    pekat untuk efek 'flash' sederhana.
// 3. Klik JumpscarePanel, UNCENTANG supaya tersembunyi di awal.
// 4. (Opsional) Add Component > Audio Source ke JumpscarePanel atau ke
//    GameObject JumpscareManager sendiri, isi 'AudioClip' dengan suara
//    jeritan/dentuman horor, uncentang 'Play On Awake'.
// 5. Buat GameObject kosong "JumpscareManager", pasang script ini, isi
//    field 'Jumpscare Panel' dan (opsional) 'Jumpscare Audio'.

using UnityEngine;

public class JumpscareManager : MonoBehaviour
{
    public static JumpscareManager Instance { get; private set; }

    [Header("UI & Audio")]
    public GameObject jumpscarePanel;
    public AudioSource jumpscareAudio;

    [Header("Timing")]
    [Tooltip("Berapa lama layar jumpscare tampil sebelum pindah ke Game Over")]
    public float jumpscareDuration = 2f;

    private void Awake()
    {
        Instance = this;
        if (jumpscarePanel != null) jumpscarePanel.SetActive(false);
    }

    public void TriggerJumpscare()
    {
        // Hentikan kontrol Player sepenuhnya selama jumpscare
        GameState.IsUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (jumpscarePanel != null) jumpscarePanel.SetActive(true);
        if (jumpscareAudio != null) jumpscareAudio.Play();

        Invoke(nameof(FinishJumpscare), jumpscareDuration);
    }

    private void FinishJumpscare()
    {
        if (jumpscarePanel != null) jumpscarePanel.SetActive(false);

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver();
        }
    }
}
