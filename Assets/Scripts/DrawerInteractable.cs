// DrawerInteractable.cs
// Pasang ke object LACI (yang buka-tutupnya dengan cara GESER, bukan
// muter seperti pintu). Untuk lemari yang pintunya MUTER seperti pintu
// biasa, pakai DoorInteractable.cs yang sudah ada (bukan script ini).
//
// CARA SETUP:
// 1. Pastikan object laci punya Collider.
// 2. Add Component > DrawerInteractable.
// 3. Isi 'Open Offset' -- arah & jarak geser laci waktu terbuka, dalam
//    LOCAL SPACE object ini. Contoh: kalau laci maju ke arah depannya
//    sendiri (sumbu Z lokal) sejauh 0.4 meter waktu dibuka, isi
//    (0, 0, 0.4). Coba klik Play dulu untuk lihat arahnya benar atau
//    kebalik -- kalau kebalik/salah arah, tinggal balik tandanya
//    (misal dari 0.4 jadi -0.4).
// 4. (Opsional) Centang 'Locked Until Mission Complete' + isi
//    'Mission Manager' kalau laci ini mau dikunci sampai misi tertentu
//    selesai (sama seperti DoorInteractable).

using UnityEngine;

public class DrawerInteractable : MonoBehaviour, IInteractable
{
    [Header("Drawer Settings")]
    [Tooltip("Pergeseran posisi (local space) waktu laci terbuka, " +
             "relatif dari posisi tertutupnya")]
    public Vector3 openOffset = new Vector3(0f, 0f, 0.4f);
    [Tooltip("Seberapa cepat animasi geser buka/tutup")]
    public float openSpeed = 3f;

    [Header("Suara (opsional)")]
    public AudioClip openSound;
    public AudioClip closeSound;
    [Tooltip("Dimainkan waktu Player mencoba membuka yang terkunci")]
    public AudioClip lockedSound;

    [Header("Kunci Manual")]
    [Tooltip("Centang untuk mengunci dari awal. Akan dibuka oleh misi " +
             "lain yang memanggil Unlock() (misal ElectricalPanel, " +
             "UploadTerminal, FormulaMixerManager).")]
    public bool isLocked = false;

    [Header("Kunci Sampai Misi Server Selesai (opsional)")]
    public bool lockedUntilMissionComplete = false;
    public MissionManager missionManager;

    [Header("Buka Otomatis Saat Misi Selesai")]
    [Tooltip("Pilih misi yang membuka kunci pintu ini. Begitu misi itu selesai, " +
             "pintu otomatis terbuka kuncinya. None = tidak ada.")]
    public MissionId unlockWhenMissionComplete = MissionId.None;

    private bool isOpen = false;
    private Vector3 closedLocalPosition;
    private Vector3 openLocalPosition;

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
        closedLocalPosition = transform.localPosition;
        openLocalPosition = closedLocalPosition + openOffset;
    }

    private void Update()
    {
        Vector3 target = isOpen ? openLocalPosition : closedLocalPosition;
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, target, Time.deltaTime * openSpeed);
    }

    private bool IsLocked()
    {
        if (isLocked) return true;

        if (!lockedUntilMissionComplete) return false;
        if (missionManager == null) return false;
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
        if (IsLocked()) return "Terkunci -- selesaikan misi dulu";
        return isOpen ? "Tekan E untuk tutup laci" : "Tekan E untuk buka laci";
    }

    public bool IsOpen => isOpen;

    public void ForceClose()
    {
        if (isOpen) AudioManager.PlayAt(closeSound, transform.position);
        isOpen = false;
    }
}