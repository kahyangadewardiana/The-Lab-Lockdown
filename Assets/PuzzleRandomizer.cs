// PuzzleRandomizer.cs
// Pasang ke GameObject kosong, misal "PuzzleRandomizer" (boleh sejajar
// dengan MissionManager).
//
// FUNGSI: Setiap kali game di-Play, script ini otomatis:
// 1. Bikin kode digit ACAK (misal 4 digit: "3971")
// 2. Bagikan tiap digit acak itu ke masing-masing ClueDigit di ruangan
// 3. Kasih tahu CodeKeypadManager kode yang benar (sesuai hasil acak tadi)
// Jadi TIDAK PERLU lagi isi manual 'Digit Value' di tiap ClueDigit atau
// 'Correct Code' di CodeKeypadManager -- semua diisi otomatis oleh script
// ini setiap kali Play.
//
// CARA SETUP:
// 1. Buat GameObject kosong "PuzzleRandomizer", pasang script ini.
// 2. Di Inspector, field 'Clue Digits In Order': ubah Size sesuai jumlah
//    ClueDigit yang Anda punya (misal 4), lalu drag ke-4 object ClueDigit
//    itu ke slot yang muncul, SESUAI URUTAN posisi digit di kode nanti
//    (misal: slot 0 = digit pertama kode, slot 1 = digit kedua, dst --
//    urutan ini bebas Anda tentukan sendiri, cuma untuk acuan internal).
// 3. (Opsional) Centang 'Allow Duplicate Digits' kalau boleh ada angka
//    yang sama muncul dobel di kode (misal '3311'). Uncentang kalau mau
//    semua digit dalam 1 kode harus beda-beda (misal '3971', tidak ada
//    yang sama).
// 4. Klik Play berkali-kali untuk coba -- setiap kali Play, kodenya akan
//    beda (cek lewat Console, ada log kode barunya untuk keperluan testing
//    -- nanti kalau sudah yakin sistemnya benar, boleh hapus baris
//    Debug.Log itu supaya pemain tidak curang lihat Console).

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleRandomizer : MonoBehaviour
{
    [Tooltip("Drag semua ClueDigit di ruangan, urutan bebas (cuma acuan internal)")]
    public List<ClueDigit> clueDigitsInOrder = new List<ClueDigit>();

    [Tooltip("Kalau dicentang, boleh ada digit yang sama dobel dalam 1 kode")]
    public bool allowDuplicateDigits = true;

    private void Start()
    {
        if (clueDigitsInOrder.Count == 0)
        {
            Debug.LogWarning("PuzzleRandomizer: 'Clue Digits In Order' masih kosong, " +
                "tidak ada yang di-random.");
            return;
        }

        List<int> randomDigits = GenerateRandomDigits(clueDigitsInOrder.Count);

        string generatedCode = "";
        for (int i = 0; i < clueDigitsInOrder.Count; i++)
        {
            clueDigitsInOrder[i].digitValue = randomDigits[i];
            generatedCode += randomDigits[i].ToString();
        }

        if (CodeKeypadManager.Instance != null)
        {
            CodeKeypadManager.Instance.correctCode = generatedCode;
        }
        else
        {
            Debug.LogWarning("PuzzleRandomizer: CodeKeypadManager belum ada di scene!");
        }

        // Log ini buat testing/debug saja. Hapus atau comment baris ini
        // kalau sudah yakin sistemnya jalan, supaya pemain tidak bisa
        // curang lihat kode lewat Console.
        Debug.Log($"[DEBUG] Kode acak baru untuk sesi ini: {generatedCode}");
    }

    private List<int> GenerateRandomDigits(int count)
    {
        if (allowDuplicateDigits)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < count; i++)
            {
                result.Add(Random.Range(0, 10));
            }
            return result;
        }
        else
        {
            // Tanpa duplikat: acak urutan angka 0-9, ambil sejumlah 'count'
            List<int> pool = Enumerable.Range(0, 10).ToList();
            List<int> result = new List<int>();

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int idx = Random.Range(0, pool.Count);
                result.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
            return result;
        }
    }
}