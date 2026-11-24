// DoorInteractable.cs
// Pasang script ini ke GameObject pintu (misal "wall_door" dari scene Anda),
// atau ke "DoorHinge" kalau pivot-nya di situ.
//
// CARA SETUP DASAR (sama seperti sebelumnya):
// 1. Pastikan pintu punya "engsel" yang benar sebagai pivot rotasi.
// 2. Tambahkan Collider ke pintu kalau belum ada.
// 3. Atur "Open Angle" sesuai arah bukaan yang Anda mau.
//
// FITUR BARU -- KUNCI SAMPAI MISI SELESAI:
// 1. Centang 'Locked Until Mission Complete'.
// 2. Drag GameObject 'MissionManager' Anda ke field 'Mission Manager'.
// 3. Sebelum misi selesai: tekan E di pintu ini TIDAK akan membuka pintu
//    (cuma muncul prompt 'Terkunci...'). Begitu semua server diperbaiki,
//    pintu otomatis bisa dibuka.
//
// FITUR BARU -- AUTO-CLOSE SETELAH DILEWATI:
// 1. Buat GameObject kosong sebagai child dari 'wall_door' (BUKAN child
//    dari daun pintu yang berputar, biar posisinya tidak ikut muter),
//    beri nama 'DoorTrigger'. Posisikan PERSIS di celah/lubang pintu
//    (tempat Player harus lewat).
// 2. Add Component > Box Collider di 'DoorTrigger' itu, CENTANG 'Is Trigger'.
//    Sesuaikan ukurannya menutupi seluruh celah pintu (termasuk sedikit
//    margin di depan & belakang supaya Player pasti "lewat" collider ini).
// 3. Add Component > DoorAutoCloser (script terpisah) ke 'DoorTrigger' itu.
//    Drag pintu ini (yang punya DoorInteractable) ke field 'Door' di
//    DoorAutoCloser.
// Pintu akan otomatis bisa dibuka dari SISI MANAPUN (tidak perlu setting
// tambahan untuk itu -- karena Interact() cuma toggle buka/tutup, tidak
// peduli dari sisi mana Player berdiri).

using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [Tooltip("Berapa derajat pintu terbuka dari posisi awal")]
    public float openAngle = 90f;
    [Tooltip("Seberapa cepat animasi buka/tutup pintu")]
    public float openSpeed = 2f;

    [Header("Suara (opsional)")]
    public AudioClip openSound;
    public AudioClip closeSound;
    [Tooltip("Dimainkan waktu Player mencoba membuka pintu yang terkunci")]
    public AudioClip lockedSound;

    [Header("Kunci Manual")]
    [Tooltip("Centang untuk mengunci dari awal. Akan dibuka oleh misi " +
             "lain yang memanggil Unlock() (misal ElectricalPanel, " +
             "UploadTerminal, FormulaMixerManager).")]
    public bool isLocked = false;

    [Header("Kunci Sampai Misi Server Selesai")]
    public bool lockedUntilMissionComplete = false;
    [Tooltip("Drag GameObject MissionManager ke sini kalau 'Locked Until Mission Complete' dicentang")]
    public MissionManager missionManager;

    [Header("Buka Otomatis Saat Misi Selesai")]
    [Tooltip("Pilih misi yang membuka kunci pintu ini. Begitu misi itu selesai, " +
             "pintu otomatis terbuka kuncinya. None = tidak ada.")]
    public MissionId unlockWhenMissionComplete = MissionId.None;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void OnEnable()
    {
        MissionEvents.OnMissionCompleted += HandleMissionCompleted;

        // Kalau misinya ternyata sudah selesai sebelum object ini aktif
        if (unlockWhenMissionComplete != MissionId.None &&
            MissionEvents.IsCompleted(unlockWhenMissionComplete))
        {
            Unlock();
        }
    }

    private void OnDisable()
    {
        MissionEvents.OnMissionCompleted -= HandleMissionCompleted;
    }

    private void HandleMissionCompleted(MissionId id)
    {
        if (unlockWhenMissionComplete != MissionId.None && id == unlockWhenMissionComplete)
        {
            Unlock();
        }
    }

    private void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0f, openAngle, 0f) * closedRotation;
    }

    private void Update()
    {
        // Animasi halus menuju target rotasi (terbuka atau tertutup)
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, target, Time.deltaTime * openSpeed);
    }

    private bool IsLocked()
    {
        if (isLocked) return true;

        if (!lockedUntilMissionComplete) return false;
        if (missionManager == null) return false; // aman kalau lupa isi, anggap tidak terkunci

        return !missionManager.IsMissionComplete();
    }

    // Dipanggil oleh sistem misi lain untuk membuka/mengunci pintu ini.
    public void Unlock()
    {
        isLocked = false;
        lockedUntilMissionComplete = false;
    }

    public void Lock()
    {
        isLocked = true;
    }

    // Dipanggil otomatis oleh PlayerInteraction saat player tekan E
    public void Interact()
    {
        if (IsLocked())
        {
            AudioManager.PlayAt(lockedSound, transform.position);
            return;
        }

        isOpen = !isOpen;
        AudioManager.PlayAt(isOpen ? openSound : closeSound, transform.position);
    }

    public string GetPrompt()
    {
        if (IsLocked())
        {
            return "Terkunci -- selesaikan misi dulu";
        }
        return isOpen ? "Tekan E untuk tutup pintu" : "Tekan E untuk buka pintu";
    }

    // =====================================================
    // Dipakai oleh DoorAutoCloser.cs
    // =====================================================

    public bool IsOpen => isOpen;

    public void ForceClose()
    {
        if (isOpen) AudioManager.PlayAt(closeSound, transform.position);
        isOpen = false;
    }
}