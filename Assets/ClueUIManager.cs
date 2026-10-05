// ClueUIManager.cs
// Pasang ke GameObject kosong, misal "ClueUIManager".
//
// FUNGSI: Menampilkan popup kecil berisi angka petunjuk saat player
// interact dengan salah satu ClueDigit. TIDAK menghentikan pergerakan
// Player (beda dengan panel keypad di server) -- cuma info sekilas yang
// otomatis hilang sendiri setelah beberapa detik.
//
// CARA SETUP UI:
// 1. Di dalam Canvas yang sudah ada, klik kanan > UI > Legacy > Panel,
//    beri nama "CluePopupPanel". Kecilkan ukurannya (misal 400x100),
//    posisikan di bagian BAWAH layar (bukan tengah, biar tidak menghalangi
//    pandangan seperti panel keypad nanti).
// 2. Di dalam CluePopupPanel, klik kanan > UI > Legacy > Text, beri nama
//    "CluePopupText".
// 3. Klik CluePopupPanel, UNCENTANG supaya tersembunyi di awal.
// 4. Buat GameObject kosong "ClueUIManager", pasang script ini, isi field
//    'Popup Panel' dan 'Popup Text' dengan drag dari Hierarchy.

using UnityEngine;
using UnityEngine.UI;

public class ClueUIManager : MonoBehaviour
{
    public static ClueUIManager Instance { get; private set; }

    public GameObject popupPanel;
    public Text popupText;
    public float displayDuration = 3f;

    private void Awake()
    {
        Instance = this;
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void ShowClue(int digit, string flavorText)
    {
        ShowMessage($"{flavorText}\nAngka: {digit}", 3f);
    }

    // Dipakai untuk pesan bebas (misal catatan cerita, StoryNote.cs),
    // bukan cuma angka petunjuk kode.
    public void ShowMessage(string message, float duration = -1f)
    {
        if (popupText != null)
        {
            popupText.text = message;
        }

        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            CancelInvoke(nameof(HidePopup));
            Invoke(nameof(HidePopup), duration > 0 ? duration : displayDuration);
        }
    }

    private void HidePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }
}