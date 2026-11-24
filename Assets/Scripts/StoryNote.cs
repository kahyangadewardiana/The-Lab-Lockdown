// StoryNote.cs
// Pasang ke object yang mau jadi "catatan cerita" (foto, surat, diary).
//
// Kalau di scene ada NoteViewerManager, catatan tampil sebagai panel
// besar berisi GAMBAR + teks yang tidak hilang sendiri dan ditutup lewat
// tombol Close atau Esc. Kalau NoteViewerManager tidak ada, otomatis
// jatuh ke popup teks biasa (ClueUIManager) seperti sebelumnya.
//
// CARA SETUP:
// 1. Pastikan object punya Collider (Box Collider).
// 2. Add Component > StoryNote.
// 3. Isi 'Title' (opsional), 'Story Text', dan drag gambar ke 'Image'
//    (gambar harus di-import sebagai Sprite 2D and UI, lihat petunjuk
//    di NoteViewerManager.cs). Kosongkan 'Image' kalau tidak mau gambar.

using UnityEngine;

public class StoryNote : MonoBehaviour, IInteractable
{
    [Tooltip("Judul catatan (opsional)")]
    public string title = "";

    [TextArea(3, 8)]
    public string storyText = "Tulis potongan cerita di sini...";

    [Tooltip("Gambar catatan (foto, kertas, dll). Boleh kosong. " +
             "File gambar harus di-import sebagai 'Sprite (2D and UI)'.")]
    public Sprite image;

    [Tooltip("Hanya dipakai kalau NoteViewerManager tidak ada (popup teks biasa)")]
    public float displayDuration = 6f;

    [Header("Suara (opsional)")]
    [Tooltip("Misal suara kertas dibuka")]
    public AudioClip openSound;

    public void Interact()
    {
        AudioManager.PlayAt(openSound, transform.position);

        if (NoteViewerManager.Instance != null)
        {
            NoteViewerManager.Instance.Open(title, storyText, image);
            return;
        }

        if (ClueUIManager.Instance != null)
        {
            ClueUIManager.Instance.ShowMessage(storyText, displayDuration);
        }
    }

    public string GetPrompt()
    {
        return "Tekan E untuk baca";
    }
}