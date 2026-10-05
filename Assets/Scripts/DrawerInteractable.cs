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

    [Header("Kunci Sampai Misi Selesai (opsional)")]
    public bool lockedUntilMissionComplete = false;
    public MissionManager missionManager;

    private bool isOpen = false;
    private Vector3 closedLocalPosition;
    private Vector3 openLocalPosition;

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
        if (!lockedUntilMissionComplete) return false;
        if (missionManager == null) return false;
        return !missionManager.IsMissionComplete();
    }

    public void Interact()
    {
        if (IsLocked()) return;
        isOpen = !isOpen;
    }

    public string GetPrompt()
    {
        if (IsLocked()) return "Terkunci -- selesaikan misi dulu";
        return isOpen ? "Tekan E untuk tutup laci" : "Tekan E untuk buka laci";
    }

    public bool IsOpen => isOpen;

    public void ForceClose()
    {
        isOpen = false;
    }
}
