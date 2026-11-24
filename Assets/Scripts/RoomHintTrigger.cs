// RoomHintTrigger.cs
// Pasang ke GameObject kosong di PINTU MASUK tiap ruangan baru.
// Begitu Player masuk ke area trigger-nya, muncul petunjuk misi di
// panel yang sama dengan petunjuk di ruangan pertama (SpawnHintManager).
//
// CARA SETUP:
// 1. Buat GameObject kosong di dekat pintu masuk ruangan (bagian DALAM
//    ruangan, tepat setelah pintu), rename misal 'HintTrigger_Lab'.
//    Jangan jadikan child daun pintu yang bergerak.
// 2. Add Component > RoomHintTrigger. Box Collider akan otomatis ikut
//    ditambahkan dan 'Is Trigger' otomatis tercentang.
// 3. Atur ukuran Box Collider supaya menutupi lebar jalan masuk, agar
//    Player pasti melewatinya.
// 4. Isi 'Message' dengan petunjuk misi ruangan itu.
// 5. Pastikan di scene ada 'SpawnHintManager' (yang dipakai petunjuk
//    ruangan pertama), karena panelnya dipakai bersama.
// 6. Ulangi untuk setiap ruangan, beda GameObject beda pesan.

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomHintTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    public string message = "Tulis petunjuk misi ruangan ini di sini.";

    [Tooltip("Berapa lama petunjuk ditampilkan (detik)")]
    public float duration = 7f;

    [Tooltip("Kalau dicentang, petunjuk cuma muncul SEKALI (waktu pertama masuk). " +
             "Kalau tidak, muncul lagi setiap Player masuk ke area ini.")]
    public bool showOnlyOnce = true;

    private bool alreadyShown = false;

    // Dipanggil otomatis oleh Unity saat komponen pertama kali ditambahkan
    private void Reset()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(3f, 3f, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (showOnlyOnce && alreadyShown) return;
        if (other.GetComponentInParent<FirstPersonController>() == null) return;

        if (SpawnHintManager.Instance == null)
        {
            Debug.LogWarning("RoomHintTrigger: SpawnHintManager belum ada di scene!");
            return;
        }

        SpawnHintManager.Instance.ShowHint(message, duration);
        alreadyShown = true;
    }

    // Kotak biru muda di Scene view supaya area trigger kelihatan
    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
