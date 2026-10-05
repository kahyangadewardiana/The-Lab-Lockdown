// CodeKeypadManager.cs
// Pasang ke GameObject kosong, misal "CodeKeypadManager".
//
// FUNGSI: Panel keypad angka 0-9 (semua lewat tombol/klik, TIDAK ada
// ketik keyboard sama sekali -- supaya tidak kena masalah kompatibilitas
// Input System seperti sebelumnya). Player klik digit satu-satu, klik
// Confirm untuk cek kodenya benar atau salah.
//
// CARA SETUP UI (di dalam Canvas yang sudah ada):
// 1. Klik kanan Canvas > UI > Legacy > Panel, beri nama "KeypadPanel".
//    Uncentang dulu supaya tersembunyi di awal (aktifkan nanti lewat
//    Inspector setelah semua diisi).
// 2. Di dalam KeypadPanel, klik kanan > UI > Legacy > Text, beri nama
//    "CodeDisplayText" -- ini menampilkan digit yang sudah diklik,
//    misal "7 2 _ _".
// 3. Di dalam KeypadPanel, buat 10 tombol (klik kanan > UI > Legacy >
//    Button) untuk angka 0-9. Beri nama masing-masing "DigitButton_0"
//    sampai "DigitButton_9", ganti teks di dalamnya sesuai angkanya.
//    Susun jadi grid rapi (3x3 + 1, atau 2x5).
// 4. Buat 1 tombol lagi "ClearButton" (teks "Hapus") dan 1 tombol
//    "ConfirmButton" (teks "Konfirmasi").
// 5. Buat 1 Text lagi "KeypadFeedbackText" untuk pesan "Kode salah!".
// 6. Buat GameObject kosong "CodeKeypadManager", pasang script ini.
// 7. Di Inspector script ini: isi 'Panel Root' = KeypadPanel,
//    'Code Display Text' = CodeDisplayText, 'Feedback Text' =
//    KeypadFeedbackText, 'Clear Button' = ClearButton, 'Confirm Button'
//    = ConfirmButton.
// 8. Di field 'Digit Buttons' (List), ubah Size jadi 10, lalu drag
//    DigitButton_0 ke slot index 0, DigitButton_1 ke slot index 1, dst
//    (URUTAN HARUS SESUAI ANGKANYA -- index 0 = tombol angka 0, dst).
// 9. Isi 'Correct Code' dengan kode gabungan dari semua ClueDigit yang
//    sudah Anda sebar (misal "7285").

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeKeypadManager : MonoBehaviour
{
    public static CodeKeypadManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject panelRoot;
    public Text codeDisplayText;
    public Text feedbackText;
    public Button clearButton;
    public Button confirmButton;
    [Tooltip("Tombol buat menutup panel TANPA harus jawab benar dulu " +
             "(misal mau cek petunjuk lagi kalau lupa)")]
    public Button closeButton;

    [Tooltip("Urutan HARUS sesuai angkanya: index 0 = tombol angka 0, dst.")]
    public List<Button> digitButtons = new List<Button>();

    [Header("Kode Jawaban")]
    [Tooltip("Kode yang benar, gabungan dari semua ClueDigit di ruangan. " +
             "Contoh: \"7285\"")]
    public string correctCode = "0000";

    private string enteredCode = "";
    private RepairableServer currentRequester;

    private void Awake()
    {
        Instance = this;

        if (panelRoot != null) panelRoot.SetActive(false);

        for (int i = 0; i < digitButtons.Count; i++)
        {
            int digit = i; // penting: copy lokal supaya closure benar
            if (digitButtons[i] != null)
            {
                digitButtons[i].onClick.AddListener(() => OnDigitPressed(digit));
            }
        }

        if (clearButton != null) clearButton.onClick.AddListener(OnClearPressed);
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmPressed);
        if (closeButton != null) closeButton.onClick.AddListener(OnClosePressed);
    }

    // =====================================================
    // DIPANGGIL OLEH RepairableServer.cs
    // =====================================================

    public void OpenKeypad(RepairableServer requester)
    {
        currentRequester = requester;
        // CATATAN: enteredCode SENGAJA tidak direset di sini, supaya kalau
        // Player tutup panel ini (buat cek petunjuk lagi) lalu buka lagi,
        // angka yang sudah diketik sebelumnya masih ada, tidak hilang.

        if (feedbackText != null) feedbackText.text = "";
        UpdateDisplay();

        if (panelRoot != null) panelRoot.SetActive(true);

        GameState.IsUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ClosePuzzle()
    {
        if (panelRoot != null) panelRoot.SetActive(false);

        GameState.IsUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentRequester = null;
    }

    // =====================================================
    // TOMBOL-TOMBOL
    // =====================================================

    private void OnDigitPressed(int digit)
    {
        if (enteredCode.Length >= correctCode.Length) return; // sudah penuh

        enteredCode += digit.ToString();
        UpdateDisplay();
    }

    private void OnClearPressed()
    {
        enteredCode = "";
        if (feedbackText != null) feedbackText.text = "";
        UpdateDisplay();
    }

    private void OnConfirmPressed()
    {
        if (currentRequester == null) return;

        if (enteredCode == correctCode)
        {
            currentRequester.OnRepaired();
            ClosePuzzle();
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "Kode salah, coba lagi!";
            enteredCode = "";
            UpdateDisplay();
        }
    }

    private void OnClosePressed()
    {
        // Tutup panel TANPA mengecek/menghapus kode yang sudah diketik.
        // Player bisa buka lagi nanti dan lanjut dari angka yang sama.
        ClosePuzzle();
    }

    private void UpdateDisplay()
    {
        if (codeDisplayText == null) return;

        // Tampilkan digit yang sudah diklik + garis bawah untuk sisanya
        // Contoh hasil: "7 2 _ _"
        string display = "";
        for (int i = 0; i < correctCode.Length; i++)
        {
            display += i < enteredCode.Length ? enteredCode[i].ToString() : "_";
            display += " ";
        }
        codeDisplayText.text = display.Trim();
    }
}