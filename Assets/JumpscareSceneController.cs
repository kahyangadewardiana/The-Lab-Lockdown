// JumpscareSceneController.cs
// Pasang ke GameObject kosong di dalam scene 'JumpscareScene' (misal nama
// GameObject-nya 'JumpscareController').
//
// FUNGSI: Setelah scene ini muncul, tunggu beberapa detik (sambil
// mainkan suara kalau ada), lalu otomatis pindah ke 'GameOverScene'.
//
// CARA SETUP:
// 1. Di scene 'JumpscareScene', buat GameObject kosong 'JumpscareController'.
// 2. Add Component > JumpscareSceneController.
// 3. Isi 'Duration' sesuai berapa lama layar jumpscare ditampilkan (misal 2 detik).
// 4. Isi 'Next Scene Name' dengan PERSIS nama scene Game Over Anda
//    (misal "GameOverScene" -- harus sama persis termasuk huruf besar/kecil).
// 5. (Opsional) Add Component > Audio Source di GameObject yang sama,
//    isi Audio Clip dengan suara jeritan/dentuman horor, uncentang
//    'Play On Awake'. Drag Audio Source itu ke field 'Jumpscare Audio'.

using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareSceneController : MonoBehaviour
{
    [Tooltip("Berapa lama scene jumpscare ini ditampilkan sebelum pindah ke Game Over")]
    public float duration = 2f;

    [Tooltip("Nama PERSIS scene Game Over Anda (harus terdaftar di Build Settings)")]
    public string nextSceneName = "GameOverScene";

    public AudioSource jumpscareAudio;

    private void Start()
    {
        if (jumpscareAudio != null)
        {
            jumpscareAudio.Play();
        }

        Invoke(nameof(GoToGameOver), duration);
    }

    private void GoToGameOver()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
