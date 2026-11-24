// UploadTerminal.cs
// Pasang ke object terminal/komputer di ruangan server-office baru.
//
// FUNGSI: Player harus berdiri dekat terminal dan MENAHAN tombol E
// sampai progress upload penuh. Kalau tombol dilepas atau Player
// menjauh, progress turun pelan-pelan. Begitu upload dimulai, hantu
// dijamin muncul dalam beberapa detik (memaksa Player memilih:
// bertahan atau kabur).
//
// CARA SETUP:
// 1. Pastikan object terminal punya Collider (Box Collider) supaya
//    tulisan prompt "Tahan E..." muncul waktu dilihat.
// 2. Add Component > UploadTerminal.
// 3. Isi 'Upload Duration' (berapa detik harus ditahan), 'Interact Range'
//    (seberapa dekat Player harus berdiri).
// 4. UI: di Canvas, klik kanan > UI > Legacy > Slider, rename
//    'UploadProgressSlider'. Di komponen Slider: UNCENTANG 'Interactable',
//    Min Value 0, Max Value 1. Hapus child 'Handle Slide Area' kalau mau
//    tampilan bar polos. Taruh di bawah-tengah layar.
//    (Opsional) klik kanan Canvas > UI > Legacy > Text, rename
//    'UploadProgressText' untuk tampilan persen.
// 5. Drag 'UploadProgressSlider' ke field 'Progress Slider' (dan
//    'UploadProgressText' ke 'Progress Text' kalau ada).
// 6. Drag karakter hantu (GameObject yang punya GhostAI) ke field 'Ghost'.
// 7. (Opsional) Drag pintu-pintu yang mau ke-unlock setelah upload
//    selesai ke list 'Doors To Unlock'.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UploadTerminal : MonoBehaviour, IInteractable
{
    [Header("Pengaturan Upload")]
    [Tooltip("Berapa detik Player harus MENAHAN E sampai upload selesai")]
    public float uploadDuration = 20f;

    [Tooltip("Seberapa dekat (meter) Player harus berdiri dari terminal")]
    public float interactRange = 3f;

    [Tooltip("Seberapa cepat progress TURUN saat E dilepas/Player menjauh, " +
             "dibanding kecepatan naiknya. 0 = progress diam, 1 = turun " +
             "secepat naiknya, 0.5 = turun setengah kecepatan naik.")]
    [Range(0f, 2f)]
    public float decayMultiplier = 0.5f;

    [Header("UI")]
    public Slider progressSlider;
    public Text progressText;

    [Header("Hantu")]
    [Tooltip("Drag karakter hantu (yang punya GhostAI) ke sini supaya dia " +
             "dijamin muncul setelah upload dimulai")]
    public GhostAI ghost;

    [Tooltip("Hantu dijamin muncul dalam berapa detik setelah upload mulai")]
    public float ghostAppearWithin = 8f;

    [Header("Setelah Selesai")]
    public List<DoorInteractable> doorsToUnlock = new List<DoorInteractable>();
    [Tooltip("Pintu GESER (yang pakai DrawerInteractable) yang ikut ke-unlock (opsional)")]
    public List<DrawerInteractable> drawersToUnlock = new List<DrawerInteractable>();

    [Header("Petunjuk Setelah Upload Selesai")]
    [TextArea(2, 4)]
    public string hintAfterComplete = "";
    [Tooltip("Opsional: tujuan berikutnya (misal pintu yang baru terbuka), untuk arah & jarak")]
    public Transform nextTarget;
    public bool showDirection = false;

    private float progress = 0f; // 0 sampai 1
    private bool complete = false;
    private bool ghostTriggeredThisRun = false;
    private Transform player;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (complete) return;

        if (player == null)
        {
            FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
            if (fpc != null) player = fpc.transform;
            if (player == null) return;
        }

        bool inRange = Vector3.Distance(player.position, transform.position) <= interactRange;

        bool holding = inRange
            && !GameState.IsUIOpen
            && Keyboard.current != null
            && Keyboard.current.eKey.isPressed;

        float rate = 1f / uploadDuration;

        if (holding)
        {
            progress += rate * Time.deltaTime;

            // Pertama kali mulai menahan: pancing hantu supaya segera muncul
            if (!ghostTriggeredThisRun && ghost != null)
            {
                ghost.ForceAppearSoon(ghostAppearWithin);
                ghostTriggeredThisRun = true;
            }
        }
        else if (progress > 0f)
        {
            progress -= rate * decayMultiplier * Time.deltaTime;

            if (progress <= 0f)
            {
                progress = 0f;
                ghostTriggeredThisRun = false; // percobaan berikutnya boleh memancing hantu lagi
            }
        }

        progress = Mathf.Clamp01(progress);
        UpdateUI();

        if (progress >= 1f)
        {
            CompleteUpload();
        }
    }

    private void CompleteUpload()
    {
        complete = true;
        progress = 1f;
        UpdateUI();
        MissionEvents.Complete(MissionId.UploadData);

        foreach (DoorInteractable door in doorsToUnlock)
        {
            if (door != null) door.Unlock();
        }

        foreach (DrawerInteractable drawer in drawersToUnlock)
        {
            if (drawer != null) drawer.Unlock();
        }

        if (ClueUIManager.Instance != null)
        {
            string message = "Upload selesai! Data berhasil dikirim.";

            string hint = NextHint.Compose(hintAfterComplete, nextTarget, showDirection);
            if (hint.Length > 0) message += "\n" + hint;

            ClueUIManager.Instance.ShowMessage(message, hint.Length > 0 ? 6f : 4f);
        }
    }

    private void UpdateUI()
    {
        bool showBar = progress > 0f && !complete;

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(showBar);
            progressSlider.value = progress;
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(showBar);
            progressText.text = $"Upload: {Mathf.RoundToInt(progress * 100f)}%";
        }
    }

    // =====================================================
    // IInteractable (cuma untuk menampilkan prompt teks; progress
    // dikontrol lewat tombol E yang DITAHAN di Update(), bukan lewat
    // Interact() satu kali tekan)
    // =====================================================

    public void Interact()
    {
        // Sengaja kosong.
    }

    public string GetPrompt()
    {
        return complete ? "Upload sudah selesai" : "Tahan E untuk upload data";
    }
}