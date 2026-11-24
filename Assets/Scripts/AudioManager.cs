// AudioManager.cs
// Pasang ke GameObject kosong di scene GAMEPLAY, misal "AudioManager".
//
// FUNGSI:
// 1. Memutar suara latar (ambient) dan musik tegang secara looping
//    dengan fade-in halus waktu scene mulai. Semua AudioSource dibuat
//    otomatis, Anda tidak perlu menambahkannya manual.
// 2. Saat hantu mengejar, musik tenang di-fade turun dan musik kejar
//    di-fade naik (dan sebaliknya saat hantu hilang). Dipanggil otomatis
//    oleh GhostAI.
// 3. Menyediakan fungsi bantu untuk script lain memutar efek suara:
//    AudioManager.PlayAt(clip, posisi) dan AudioManager.Play2D(clip).
//
// CARA SETUP:
// 1. Buat GameObject kosong "AudioManager", Add Component > AudioManager.
// 2. Drag file suara ke 'Ambient Loop', 'Calm Music', 'Chase Music'.
//    Kosongkan yang tidak mau dipakai.
// 3. Atur volume masing-masing sesuai selera.
//
// CATATAN: musik menu utama berada di scene MainMenu dan berhenti saat
// pindah ke gameplay. Kalau mau lagu yang sama dipakai di gameplay,
// drag file lagu itu ke 'Calm Music' (lagu akan mulai dari awal, bukan
// lanjut dari posisi terakhir di menu).

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Suara Latar dan Musik (looping)")]
    [Tooltip("Dengung/angin/ruangan sepi, terus berputar")]
    public AudioClip ambientLoop;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;

    [Tooltip("Musik tegang pelan saat tidak ada kejaran")]
    public AudioClip calmMusic;
    [Range(0f, 1f)] public float calmMusicVolume = 0.35f;

    [Tooltip("Musik saat hantu mengejar")]
    public AudioClip chaseMusic;
    [Range(0f, 1f)] public float chaseMusicVolume = 0.8f;

    [Tooltip("Kecepatan fade volume (semakin besar semakin cepat)")]
    public float fadeSpeed = 1.5f;

    [Header("Efek Suara")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource ambientSource;
    private AudioSource calmSource;
    private AudioSource chaseSource;
    private AudioSource sfxSource;
    private bool chaseMode = false;

    private void Awake()
    {
        Instance = this;

        ambientSource = CreateLoopSource(ambientLoop);
        calmSource = CreateLoopSource(calmMusic);
        chaseSource = CreateLoopSource(chaseMusic);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
    }

    // Dibuat dengan volume 0 lalu naik pelan-pelan di Update (fade-in).
    private AudioSource CreateLoopSource(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0f;

        if (clip != null) source.Play();
        return source;
    }

    private void Update()
    {
        float step = fadeSpeed * Time.deltaTime;

        ambientSource.volume = Mathf.MoveTowards(
            ambientSource.volume, ambientLoop != null ? ambientVolume : 0f, step);

        float calmTarget = (chaseMode || calmMusic == null) ? 0f : calmMusicVolume;
        calmSource.volume = Mathf.MoveTowards(calmSource.volume, calmTarget, step);

        float chaseTarget = (chaseMode && chaseMusic != null) ? chaseMusicVolume : 0f;
        chaseSource.volume = Mathf.MoveTowards(chaseSource.volume, chaseTarget, step);
    }

    // Dipanggil GhostAI: true saat hantu mulai mengejar, false saat hilang.
    public void SetChaseMode(bool on)
    {
        chaseMode = on;
    }

    // Efek suara 3D di posisi tertentu (pintu, item, dll).
    public static void PlayAt(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        float finalVolume = volume * (Instance != null ? Instance.sfxVolume : 1f);
        AudioSource.PlayClipAtPoint(clip, position, finalVolume);
    }

    // Efek suara 2D (terdengar sama di mana pun Player berdiri).
    public static void Play2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null || Instance == null) return;

        Instance.sfxSource.PlayOneShot(clip, volume * Instance.sfxVolume);
    }
}
