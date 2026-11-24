// NextHint.cs
// Taruh di Assets/Scripts/. TIDAK perlu ditempel ke GameObject manapun.
//
// Helper bersama untuk menyusun teks "petunjuk ke langkah berikutnya"
// (tulisan buatan Anda + opsional arah & jarak ke target). Dipakai oleh
// ClueDigit, CollectibleFuse, RepairableServer, ElectricalPanel,
// UploadTerminal, dan FormulaMixerManager.

using UnityEngine;

public static class NextHint
{
    // Menghasilkan teks petunjuk. Hasilnya string kosong kalau tidak ada
    // yang perlu ditampilkan.
    public static string Compose(string hintText, Transform target, bool showDirection)
    {
        string result = "";

        if (!string.IsNullOrWhiteSpace(hintText))
        {
            result = hintText;
        }

        // Target yang sudah nonaktif (misal bahan/sekring yang sudah diambil)
        // dilewati, supaya arah tidak menunjuk ke benda yang sudah tidak ada.
        if (showDirection && target != null && target.gameObject.activeInHierarchy)
        {
            string direction = DescribeDirection(target.position);
            if (direction != null)
            {
                if (result.Length > 0) result += "\n";
                result += "Arah: " + direction;
            }
        }

        return result;
    }

    // Arah relatif terhadap posisi dan arah hadap Player saat ini.
    public static string DescribeDirection(Vector3 targetPosition)
    {
        FirstPersonController player = Object.FindFirstObjectByType<FirstPersonController>();
        if (player == null) return null;

        Vector3 local = player.transform.InverseTransformPoint(targetPosition);
        local.y = 0f;

        float distance = local.magnitude;
        float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg; // 0 = depan, positif = kanan

        string[] names =
        {
            "depan", "kanan-depan", "kanan", "kanan-belakang",
            "belakang", "kiri-belakang", "kiri", "kiri-depan"
        };

        int index = Mathf.RoundToInt(((angle + 360f) % 360f) / 45f) % 8;
        return $"{names[index]}, sekitar {Mathf.RoundToInt(distance)} meter";
    }
}
