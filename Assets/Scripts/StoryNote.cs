// StoryNote.cs
// Pasang ke object yang mau jadi "catatan cerita" tersembunyi (foto,
// surat, diary, dll). Pakai ClueUIManager yang sama dengan ClueDigit,
// jadi tidak perlu bikin UI baru lagi.
//
// CARA SETUP:
// 1. Pilih 3-4 object di ruangan baru untuk jadi catatan cerita.
// 2. Untuk masing-masing: Add Component > Box Collider (kalau belum ada).
// 3. Add Component > StoryNote.
// 4. Isi field 'Story Text' dengan potongan cerita masing-masing
//    (buat beda-beda tiap object supaya jadi cerita yang nyambung
//    kalau dibaca semua).

using UnityEngine;

public class StoryNote : MonoBehaviour, IInteractable
{
    [TextArea(3, 6)]
    public string storyText = "Tulis potongan cerita di sini...";

    [Tooltip("Berapa lama catatan ini ditampilkan (detik). " +
             "Lebih lama dari clue angka karena teksnya lebih panjang.")]
    public float displayDuration = 6f;

    public void Interact()
    {
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
