// FormulaMixerManager.cs
// Pasang ke GameObject kosong, misal "FormulaMixerManager".
//
// FUNGSI:
// 1. Melacak bahan kimia yang sudah dikumpulkan (lewat ChemicalPickup.cs).
// 2. Menampilkan panel "racik" di meja eksperimen (mirip keypad kode,
//    tapi tombolnya nama bahan kimia, bukan angka) -- HANYA bisa dibuka
//    kalau semua bahan sudah terkumpul.
// 3. Player klik tombol bahan kimia SESUAI URUTAN yang benar (urutan ini
//    Anda tentukan sendiri, lalu tulis di catatan cerita/StoryNote di lab
//    supaya pemain tahu urutannya).
// 4. Kalau urutan benar -> trigger FINALE: hantu masuk mode kejar
//    permanen, pintu keluar akhir ke-unlock.
//
// CARA SETUP UI (di dalam Canvas yang sudah ada):
// 1. Klik kanan Canvas > UI > Legacy > Panel, nama "FormulaPanel".
//    Uncentang dulu (tersembunyi di awal).
// 2. Di dalamnya: 1 Text "FormulaDisplayText" (nampilin urutan yang sudah
//    diklik, misal "Biru > Merah > _ > _").
// 3. Buat 1 Button PER bahan kimia (misal 4 tombol: "ChemButton_Biru",
//    "ChemButton_Merah", dst), teks di tombolnya sesuai nama bahannya.
// 4. Buat "ClearButton" (Hapus) dan "ConfirmButton" (Racik!).
// 5. Buat 1 Text lagi "FormulaFeedbackText" untuk pesan salah/kurang bahan.
// 6. Buat GameObject kosong "FormulaMixerManager", pasang script ini,
//    isi semua field UI.
// 7. Di 'Chemical Buttons', pasangkan tiap tombol dengan nama bahannya
//    (harus SAMA PERSIS dengan 'Chemical Name' di ChemicalPickup terkait).
// 8. Isi 'Correct Order' dengan urutan yang benar, misal ["Biru","Merah",
//    "Hijau","Kuning"].
// 9. Drag karakter hantu ke 'Ghost'. Drag pintu keluar akhir ke
//    'Doors To Unlock'.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormulaMixerManager : MonoBehaviour
{
    public static FormulaMixerManager Instance { get; private set; }

    [System.Serializable]
    public class ChemicalButton
    {
        public string chemicalName;
        public Button button;
    }

    [Header("Bahan Kimia")]
    public int totalChemicalsNeeded = 4;
    private readonly HashSet<string> collectedChemicals = new HashSet<string>();

    [Header("UI Panel Racik")]
    public GameObject panelRoot;
    public Text formulaDisplayText;
    public Text feedbackText;
    public List<ChemicalButton> chemicalButtons = new List<ChemicalButton>();
    public Button clearButton;
    public Button confirmButton;
    public Button closeButton;

    [Header("Urutan yang Benar")]
    [Tooltip("Urutan nama bahan yang benar, contoh: Biru, Merah, Hijau, Kuning")]
    public List<string> correctOrder = new List<string>();

    [Header("Finale")]
    public GhostAI ghost;
    public List<DoorInteractable> doorsToUnlock = new List<DoorInteractable>();

    private List<string> enteredOrder = new List<string>();
    private bool isComplete = false;

    private void Awake()
    {
        Instance = this;

        if (panelRoot != null) panelRoot.SetActive(false);

        foreach (var cb in chemicalButtons)
        {
            if (cb.button == null) continue;
            string name = cb.chemicalName; // copy lokal buat closure
            cb.button.onClick.AddListener(() => OnChemicalPressed(name));
        }

        if (clearButton != null) clearButton.onClick.AddListener(OnClearPressed);
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmPressed);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);
    }

    // =====================================================
    // KOLEKSI BAHAN (dipanggil dari ChemicalPickup.cs)
    // =====================================================

    public void CollectChemical(string chemicalName)
    {
        collectedChemicals.Add(chemicalName);
    }

    public bool AllChemicalsCollected()
    {
        return collectedChemicals.Count >= totalChemicalsNeeded;
    }

    public int GetRemainingCount()
    {
        return Mathf.Max(0, totalChemicalsNeeded - collectedChemicals.Count);
    }

    // =====================================================
    // PANEL RACIK (dipanggil dari ExperimentTable.cs)
    // =====================================================

    // Dipakai ScoreManager untuk menghitung skor akhir.
    public bool IsComplete() => isComplete;

    public void OpenPanel()
    {
        if (isComplete) return;

        if (panelRoot != null) panelRoot.SetActive(true);
        if (feedbackText != null) feedbackText.text = "";
        UpdateDisplay();

        GameState.IsUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ClosePanel()
    {
        if (panelRoot != null) panelRoot.SetActive(false);

        GameState.IsUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnChemicalPressed(string chemicalName)
    {
        if (enteredOrder.Count >= correctOrder.Count) return;

        enteredOrder.Add(chemicalName);
        UpdateDisplay();
    }

    private void OnClearPressed()
    {
        enteredOrder.Clear();
        if (feedbackText != null) feedbackText.text = "";
        UpdateDisplay();
    }

    private void OnConfirmPressed()
    {
        bool correct = enteredOrder.Count == correctOrder.Count;
        if (correct)
        {
            for (int i = 0; i < correctOrder.Count; i++)
            {
                if (enteredOrder[i] != correctOrder[i])
                {
                    correct = false;
                    break;
                }
            }
        }

        if (correct)
        {
            CompleteFormula();
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "Urutan salah, coba lagi!";
            enteredOrder.Clear();
            UpdateDisplay();
        }
    }

    private void CompleteFormula()
    {
        isComplete = true;
        ClosePanel();

        if (ClueUIManager.Instance != null)
        {
            ClueUIManager.Instance.ShowMessage(
                "Penawar berhasil diracik! Tapi sesuatu terasa salah...", 4f);
        }

        foreach (DoorInteractable door in doorsToUnlock)
        {
            if (door != null) door.lockedUntilMissionComplete = false;
        }

        if (ghost != null)
        {
            ghost.EnterFinaleMode();
        }
    }

    private void UpdateDisplay()
    {
        if (formulaDisplayText == null) return;

        string display = "";
        for (int i = 0; i < correctOrder.Count; i++)
        {
            display += i < enteredOrder.Count ? enteredOrder[i] : "_";
            if (i < correctOrder.Count - 1) display += " > ";
        }
        formulaDisplayText.text = display;
    }
}