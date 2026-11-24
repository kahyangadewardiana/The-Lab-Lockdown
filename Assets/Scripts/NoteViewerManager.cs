// NoteViewerManager.cs
// Pasang ke GameObject kosong di scene gameplay, misal "NoteViewerManager".
//
// FUNGSI: Menampilkan catatan cerita sebagai panel besar berisi GAMBAR +
// teks. Panel TIDAK hilang sendiri, baru tertutup kalau Player klik
// tombol Close atau menekan Esc. Selama panel terbuka, gerakan Player
// berhenti dan kursor muncul (sama seperti panel keypad).
//
// CARA SETUP UI (di dalam Canvas yang sudah ada):
// 1. Klik kanan Canvas > UI > Legacy > Panel, nama "NotePanel". Atur
//    supaya menutupi sebagian besar layar (misal anchor stretch dengan
//    margin 100), warna gelap agak transparan.
// 2. Di dalam NotePanel: klik kanan > UI > Legacy > Image, nama
//    "NoteImage". Taruh di sisi kiri/atas panel, ukuran secukupnya.
//    (Gambar yang dipakai nanti diisi lewat script StoryNote.)
// 3. Di dalam NotePanel: Text "NoteTitleText" (judul, opsional) dan
//    Text "NoteBodyText" (isi catatan). Atur warna terang dan font size.
// 4. Di dalam NotePanel: Button "CloseButton", teks "Tutup" (atau "X"),
//    taruh di pojok panel.
// 5. Klik NotePanel, UNCENTANG supaya tersembunyi di awal.
// 6. Buat GameObject kosong "NoteViewerManager", pasang script ini, lalu
//    drag Panel Root, Note Image, Title Text, Body Text, Close Button.
//
// PENTING: gambar yang mau dipakai harus di-import sebagai Sprite. Klik
// file gambarnya di Project window, di Inspector ubah 'Texture Type'
// menjadi 'Sprite (2D and UI)', lalu klik Apply.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NoteViewerManager : MonoBehaviour
{
    public static NoteViewerManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject panelRoot;
    public Image noteImage;
    public Text titleText;
    public Text bodyText;
    public Button closeButton;

    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;

        if (panelRoot != null) panelRoot.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    public void Open(string title, string body, Sprite image)
    {
        if (isOpen) return;

        if (titleText != null)
        {
            titleText.text = title;
            titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
        }

        if (bodyText != null) bodyText.text = body;

        if (noteImage != null)
        {
            if (image != null)
            {
                noteImage.sprite = image;
                noteImage.preserveAspect = true;
                noteImage.gameObject.SetActive(true);
            }
            else
            {
                noteImage.gameObject.SetActive(false);
            }
        }

        if (panelRoot != null) panelRoot.SetActive(true);

        isOpen = true;
        GameState.IsUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        if (panelRoot != null) panelRoot.SetActive(false);

        GameState.IsUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Esc dicek di LateUpdate (bukan Update) dengan sengaja: script lain
    // (FirstPersonController) juga membaca Esc di Update dan baru berhenti
    // merespon kalau GameState.IsUIOpen masih true. Dengan menutup di
    // LateUpdate, urutannya aman dan kursor tidak ikut terbuka sendiri.
    private void LateUpdate()
    {
        if (!isOpen) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }
}
