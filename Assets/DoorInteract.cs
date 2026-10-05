using UnityEngine;
using System.Collections;

// Tempelkan script ini ke GameObject pintu (misalnya Door_Left_01 atau Door_Arch_01)
// Pintu akan berputar membuka/menutup di sekitar sumbu Y saat player mendekat dan menekan tombol E
[RequireComponent(typeof(Collider))]
public class DoorInteract : MonoBehaviour
{
    [Header("Pengaturan Pintu")]
    [Tooltip("Sudut buka pintu dalam derajat (misal 90 = buka 90 derajat)")]
    public float openAngle = 90f;

    [Tooltip("Kecepatan buka/tutup pintu")]
    public float speed = 2f;

    [Tooltip("Buka ke arah kiri (-) atau kanan (+)")]
    public bool openInverse = false;

    [Header("Interaksi")]
    [Tooltip("Tombol untuk membuka/menutup pintu")]
    public KeyCode interactKey = KeyCode.E;

    [Tooltip("Tag object player, dipakai untuk mendeteksi jarak")]
    public string playerTag = "Player";

    private bool isOpen = false;
    private bool isAnimating = false;
    private bool playerInRange = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Simpan rotasi awal (tertutup) sebagai patokan
        closedRotation = transform.localRotation;

        float angle = openInverse ? -openAngle : openAngle;
        openRotation = closedRotation * Quaternion.Euler(0f, angle, 0f);

        // Pastikan collider di GameObject ini dijadikan Trigger untuk mendeteksi player
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && !isAnimating)
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateDoor(isOpen ? closedRotation : openRotation));
        isOpen = !isOpen;
    }

    IEnumerator AnimateDoor(Quaternion targetRotation)
    {
        isAnimating = true;
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * speed);
            yield return null;
        }
        transform.localRotation = targetRotation;
        isAnimating = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
}
