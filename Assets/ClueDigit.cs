// ClueDigit.cs
// Pasang script ini ke 3-4 object "petunjuk" yang disebar di ruangan
// (misal sticky note, monitor, papan tulis, kertas di meja, dll).
// Tiap object akan menyimpan 1 ANGKA yang jadi bagian dari kode server.
//
// CARA SETUP:
// 1. Pilih 4 object di ruangan (boleh dari furniture office yang sudah
//    ada, misal 'DocumentHolder', 'Mug', 'Clock', 'Whiteboard', dll --
//    yang penting ada Collider di object itu).
// 2. Untuk masing-masing: Add Component > Box Collider (kalau belum ada),
//    lalu Add Component > ClueDigit.
// 3. Isi 'Digit Value' dengan angka 0-9 yang berbeda-beda untuk tiap object
//    (misal object 1 = 7, object 2 = 2, object 3 = 8, object 4 = 5, jadi
//    total kodenya nanti "7285").
// 4. Isi 'Flavor Text' opsional, deskripsi singkat biar terasa immersive,
//    misal "Angka ini dicoret di sudut kertas".
// 5. PENTING: kode gabungan dari semua ClueDigit ini (sesuai urutan Anda
//    tentukan sendiri) harus sama dengan 'Correct Code' yang diisi di
//    CodeKeypadManager.cs nanti.

using UnityEngine;

public class ClueDigit : MonoBehaviour, IInteractable
{
    [Header("Kode Petunjuk")]
    [Range(0, 9)]
    public int digitValue = 0;

    [TextArea(1, 3)]
    public string flavorText = "Ada angka tercoret di sini.";

    public void Interact()
    {
        if (ClueUIManager.Instance != null)
        {
            ClueUIManager.Instance.ShowClue(digitValue, flavorText);
        }
    }

    public string GetPrompt()
    {
        return "Tekan E untuk periksa";
    }
}
