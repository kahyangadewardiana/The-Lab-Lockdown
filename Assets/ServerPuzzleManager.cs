// ServerPuzzleManager.cs
// Pasang script ini ke sebuah GameObject kosong di scene, misal
// "ServerPuzzleManager" (boleh taruh di dalam Canvas atau sejajar).
//
// FUNGSI: Mengatur panel UI soal logika -- menampilkan pertanyaan,
// menerima jawaban ketikan player, mengecek benar/salah, dan memberi
// tahu RepairableServer yang memicu soal itu kalau jawabannya benar.
//
// CARA SETUP UI (WAJIB dibuat manual dulu di Hierarchy):
// 1. Klik kanan Hierarchy > UI > Canvas (kalau belum ada Canvas di scene).
// 2. Klik kanan Canvas > UI > Panel, beri nama "PuzzlePanel". Ini jadi
//    kotak dialog soal. Atur warna background agak gelap transparan.
// 3. Di dalam PuzzlePanel, klik kanan > UI > Text (bukan TextMeshPro),
//    beri nama "QuestionText". Ini nanti menampilkan pertanyaannya.
// 4. Di dalam PuzzlePanel, klik kanan > UI > Input Field, beri nama
//    "AnswerInput". Ini tempat player mengetik jawaban.
// 5. Di dalam PuzzlePanel, klik kanan > UI > Button, beri nama
//    "SubmitButton", ganti teks tombolnya jadi "Jawab".
// 6. Di dalam PuzzlePanel, klik kanan > UI > Text lagi, beri nama
//    "FeedbackText". Ini menampilkan "Salah, coba lagi!" kalau jawaban
//    keliru. Boleh dikosongkan teksnya di awal.
// 7. Setelah semua dibuat, klik "PuzzlePanel" itu sendiri, di Inspector
//    UNCENTANG kotak centang di sebelah namanya (biar panel ini
//    tersembunyi di awal, baru muncul saat dipanggil).
// 8. Buat GameObject kosong baru (boleh di luar Canvas), beri nama
//    "ServerPuzzleManager", pasang script ini ke situ.
// 9. Di Inspector script ini, isi semua field (Panel Root, Question Text,
//    Answer Input, Submit Button, Feedback Text) dengan drag dari
//    Hierarchy sesuai nama yang dibuat tadi.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ServerPuzzleManager : MonoBehaviour
{
    public static ServerPuzzleManager Instance { get; private set; }

    [Header("UI References (drag dari Hierarchy)")]
    public GameObject panelRoot;
    public Text questionText;
    [Tooltip("Text biasa (BUKAN Input Field) yang menampilkan jawaban yang sedang diketik")]
    public Text answerDisplayText;
    public Button submitButton;
    public Text feedbackText;

    [Header("Soal-soal Logika")]
    [Tooltip("Kosongkan untuk pakai daftar soal default bawaan script ini")]
    public List<LogicQuestion> customQuestions = new List<LogicQuestion>();

    private List<LogicQuestion> activeQuestionPool;
    private LogicQuestion currentQuestion;
    private RepairableServer currentRequester;
    private string typedAnswer = "";

    private void Awake()
    {
        Instance = this;

        activeQuestionPool = customQuestions.Count > 0
            ? customQuestions
            : GetDefaultQuestions();

        if (panelRoot != null) panelRoot.SetActive(false);

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitPressed);
        }
    }

    private void Update()
    {
        if (panelRoot == null || !panelRoot.activeSelf) return;

        // Backspace: hapus huruf terakhir
        if (Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            if (typedAnswer.Length > 0)
            {
                typedAnswer = typedAnswer.Substring(0, typedAnswer.Length - 1);
                UpdateAnswerDisplay();
            }
        }

        // Enter: submit langsung (selain klik tombol)
        if (Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame ||
             Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            OnSubmitPressed();
        }
    }

    private void OnEnable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += OnTextInput;
        }
    }

    private void OnDisable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= OnTextInput;
        }
    }

    private void OnTextInput(char c)
    {
        // Cuma proses ketikan kalau panel soal sedang terbuka
        if (panelRoot == null || !panelRoot.activeSelf) return;

        // Abaikan karakter kontrol (enter, backspace, dll -- sudah ditangani terpisah)
        if (char.IsControl(c)) return;

        typedAnswer += c;
        UpdateAnswerDisplay();
    }

    private void UpdateAnswerDisplay()
    {
        if (answerDisplayText != null)
        {
            // Tambahkan garis kedip sederhana di akhir biar kelihatan seperti caret
            answerDisplayText.text = typedAnswer + "_";
        }
    }

    // =====================================================
    // DIPANGGIL OLEH RepairableServer.cs
    // =====================================================

    public void OpenPuzzle(RepairableServer requester)
    {
        currentRequester = requester;
        currentQuestion = activeQuestionPool[Random.Range(0, activeQuestionPool.Count)];

        typedAnswer = "";

        if (questionText != null) questionText.text = currentQuestion.questionText;
        if (feedbackText != null) feedbackText.text = "";
        UpdateAnswerDisplay();

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

    private void OnSubmitPressed()
    {
        if (currentRequester == null) return;
        if (panelRoot == null || !panelRoot.activeSelf) return;

        bool correct = currentQuestion.CheckAnswer(typedAnswer);

        if (correct)
        {
            currentRequester.OnRepaired();
            ClosePuzzle();
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Salah, coba lagi!";
            }
            typedAnswer = "";
            UpdateAnswerDisplay();
        }
    }

    // Soal default kalau 'Custom Questions' dikosongkan di Inspector
    private List<LogicQuestion> GetDefaultQuestions()
    {
        return new List<LogicQuestion>
        {
            new LogicQuestion {
                questionText = "Jika 2 mesin butuh 2 menit untuk membuat 2 komponen, " +
                    "berapa menit yang dibutuhkan 100 mesin untuk membuat 100 komponen?",
                correctAnswer = "2"
            },
            new LogicQuestion {
                questionText = "Deret angka: 2, 4, 8, 16, ... Angka selanjutnya adalah?",
                correctAnswer = "32"
            },
            new LogicQuestion {
                questionText = "Sebuah kode terdiri dari huruf pertama tiap kata: " +
                    "'Server Utama Sedang Rusak Total'. Kode itu adalah?",
                correctAnswer = "SUSRT"
            },
            new LogicQuestion {
                questionText = "Jika hari ini Senin, hari apa 100 hari dari sekarang?",
                correctAnswer = "Rabu"
            },
            new LogicQuestion {
                questionText = "Berapa hasil dari (7 x 8) - (6 x 5)?",
                correctAnswer = "26"
            },
        };
    }
}

