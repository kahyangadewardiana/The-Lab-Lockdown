// GhostAI.cs (versi baru: 'Sudden Appear' bukan patroli terus-menerus)
// Pasang ke GameObject karakter hantu, sama seperti sebelumnya.
//
// PERILAKU BARU:
// 1. Hantu MULAI DALAM KEADAAN TERSEMBUNYI (tidak terlihat, tidak bergerak).
// 2. Setelah jeda waktu ACAK (antara Min/Max Appear Interval), dia
//    TELEPORT ke titik dekat Player (di luar pandangan kamera kalau bisa),
//    lalu MUNCUL dan MENGEJAR Player selama beberapa detik (Max Chase
//    Duration).
// 3. Kalau selama mengejar dia sampai sedekat 'Catch Distance' ke Player,
//    Player 'ketangkep' -> animasi Attack -> pindah ke JumpscareScene.
// 4. Kalau waktu kejar habis tanpa menangkap, hantu HILANG lagi (balik
//    tersembunyi), nunggu jeda acak berikutnya, ulang dari langkah 2.
//
// CARA SETUP (sama seperti sebelumnya):
// 1. GameObject karakter hantu perlu: Nav Mesh Agent + Animator + GhostAI.
// 2. Isi field 'Player' dengan drag GameObject Player.
// 3. Isi field 'Animator' dengan drag karakter itu sendiri.
// 4. Isi 'Jumpscare Scene Name' sesuai nama scene Jumpscare Anda.
// 5. NavMesh WAJIB sudah di-bake (NavMeshSurface) seperti sebelumnya,
//    karena dipakai untuk mencari titik valid di dekat Player waktu
//    hantu mau muncul.

using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    private enum GhostState { Hidden, Chasing }

    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Muncul Mendadak")]
    [Tooltip("Jeda waktu PALING CEPAT sebelum hantu muncul lagi (detik)")]
    public float minAppearInterval = 20f;
    [Tooltip("Jeda waktu PALING LAMA sebelum hantu muncul lagi (detik)")]
    public float maxAppearInterval = 50f;
    [Tooltip("Jarak PALING DEKAT dari Player waktu hantu teleport muncul")]
    public float appearDistanceMin = 5f;
    [Tooltip("Jarak PALING JAUH dari Player waktu hantu teleport muncul")]
    public float appearDistanceMax = 8f;
    [Tooltip("Berapa lama hantu mengejar sebelum menyerah & hilang lagi kalau tidak berhasil menangkap")]
    public float maxChaseDuration = 6f;

    [Header("Peringatan Sebelum Muncul")]
    [Tooltip("Berapa detik SEBELUM hantu muncul, suara/efek peringatan dimainkan")]
    public float warningLeadTime = 2.5f;
    [Tooltip("Suara peringatan (misal bisikan/growl pelan), dimainkan sebelum hantu benar-benar muncul")]
    public AudioSource warningSound;

    [Tooltip("Tulisan peringatan yang muncul di layar sebelum hantu muncul. " +
             "Butuh GameObject 'GhostWarningUI' di scene. Kosongkan kalau tidak mau ada tulisan.")]
    public string warningMessage = "Ada sesuatu yang mendekat...";

    [Tooltip("Kalau dicentang, tiap kali hantu muncul tampil tulisan arahnya " +
             "terhadap Player (misal 'kiri-belakang, sekitar 6 meter'). " +
             "Butuh GameObject 'GhostWarningUI' di scene.")]
    public bool showDirectionOnAppear = true;
    [Tooltip("Awalan tulisan arah. Hasil akhir: '<awalan> <arah>!'")]
    public string appearMessage = "HANTU MUNCUL di";
    [Tooltip("Berapa lama tulisan arah ditampilkan (detik)")]
    public float appearHintDuration = 3f;

    [Header("Kejar")]
    public float chaseSpeed = 5f;
    public float catchDistance = 1.5f;

    [Header("Scene Jumpscare")]
    [Tooltip("Nama PERSIS scene Jumpscare Anda (harus terdaftar di Build Settings)")]
    public string jumpscareSceneName = "JumpscareScene";

    [Header("Animator Parameters")]
    public string speedParamName = "Speed";
    public string attackParamName = "Attack";

    [Header("Audio (opsional)")]
    [Tooltip("Suara waktu hantu tiba-tiba muncul, misal jeritan/dentuman")]
    public AudioSource appearSound;

    [Tooltip("Suara yang terus berputar selama hantu mengejar (misal napas berat/geraman). " +
             "Pakai Audio Source terpisah dari 'Appear Sound'.")]
    public AudioSource chaseLoopSound;

    private NavMeshAgent agent;
    private Renderer[] renderers;
    private GhostState state = GhostState.Hidden;
    private float stateTimer;
    private float nextAppearTime;
    private bool warningPlayed = false;
    private bool hasCaughtPlayer = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        agent.speed = chaseSpeed;
        SetHidden();
        ScheduleNextAppearance();
    }

    private void Update()
    {
        if (hasCaughtPlayer) return;
        if (player == null) return;

        switch (state)
        {
            case GhostState.Hidden:
                UpdateHidden();
                break;
            case GhostState.Chasing:
                UpdateChasing();
                break;
        }
    }

    // =====================================================
    // HIDDEN (menunggu giliran muncul)
    // =====================================================

    private void SetHidden()
    {
        state = GhostState.Hidden;
        SetVisible(false);

        if (chaseLoopSound != null) chaseLoopSound.Stop();
        if (AudioManager.Instance != null) AudioManager.Instance.SetChaseMode(false);

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    private void ScheduleNextAppearance()
    {
        float wait = Random.Range(minAppearInterval, maxAppearInterval);
        nextAppearTime = Time.time + wait;
        warningPlayed = false;
    }

    // Dipanggil dari script lain (misal UploadTerminal) untuk MEMPERCEPAT
    // kemunculan hantu berikutnya, supaya dia dijamin muncul dalam waktu
    // 'maxSecondsFromNow' detik dari sekarang (kalau jadwal aslinya lebih
    // lama dari itu). Tidak berpengaruh kalau hantu sedang mengejar/sudah
    // menangkap Player.
    public void ForceAppearSoon(float maxSecondsFromNow)
    {
        if (state != GhostState.Hidden || hasCaughtPlayer) return;

        float target = Time.time + maxSecondsFromNow;
        if (nextAppearTime > target)
        {
            nextAppearTime = target;
            warningPlayed = false; // supaya suara peringatan ikut dimainkan ulang
        }
    }

    private void UpdateHidden()
    {
        // Mainkan peringatan sedikit sebelum waktunya muncul
        if (!warningPlayed && Time.time >= nextAppearTime - warningLeadTime)
        {
            warningPlayed = true;
            if (warningSound != null) warningSound.Play();

            if (!string.IsNullOrWhiteSpace(warningMessage) && GhostWarningUI.Instance != null)
            {
                // Tampil sampai hantu benar-benar muncul
                float remaining = Mathf.Max(0.5f, nextAppearTime - Time.time);
                GhostWarningUI.Instance.Show(warningMessage, remaining);
            }
        }

        if (Time.time >= nextAppearTime)
        {
            TryAppearNearPlayer();
        }
    }

    private void TryAppearNearPlayer()
    {
        // Cari titik acak di sekitar Player, coba beberapa kali sampai
        // dapat titik yang valid di NavMesh.
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized *
                Random.Range(appearDistanceMin, appearDistanceMax);
            Vector3 candidate = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                AppearAt(hit.position);
                return;
            }
        }
        // Kalau 10 percobaan gagal semua (jarang terjadi), coba lagi nanti
        ScheduleNextAppearance();
    }

    private void AppearAt(Vector3 position)
    {
        agent.Warp(position); // pindahkan hantu ke titik itu, langsung sinkron dengan NavMesh
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        SetVisible(true);
        if (appearSound != null) appearSound.Play();

        if (GhostWarningUI.Instance != null)
        {
            GhostWarningUI.Instance.Hide();

            if (showDirectionOnAppear)
            {
                string direction = NextHint.DescribeDirection(transform.position);
                if (direction != null)
                {
                    GhostWarningUI.Instance.Show($"{appearMessage} {direction}!", appearHintDuration);
                }
            }
        }

        if (chaseLoopSound != null)
        {
            chaseLoopSound.loop = true;
            chaseLoopSound.Play();
        }
        if (AudioManager.Instance != null) AudioManager.Instance.SetChaseMode(true);

        agent.isStopped = false;
        agent.speed = chaseSpeed;

        state = GhostState.Chasing;
        stateTimer = 0f;
    }

    // =====================================================
    // CHASING (mengejar setelah muncul)
    // =====================================================

    private void UpdateChasing()
    {
        stateTimer += Time.deltaTime;

        agent.SetDestination(player.position);
        UpdateAnimator();

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= catchDistance)
        {
            CatchPlayer();
            return;
        }

        // Di mode 'finale', hantu TIDAK PERNAH menyerah -- terus mengejar
        // selamanya sampai berhasil menangkap Player.
        if (!finaleMode && stateTimer >= maxChaseDuration)
        {
            GiveUpAndHide();
        }
    }

    private void GiveUpAndHide()
    {
        SetHidden();
        ScheduleNextAppearance();
    }

    // =====================================================
    // MODE FINALE (dipanggil dari script lain, misal FormulaMixerManager,
    // untuk memicu kejaran permanen di ruangan terakhir)
    // =====================================================

    private bool finaleMode = false;

    public void EnterFinaleMode()
    {
        if (hasCaughtPlayer) return;

        finaleMode = true;
        CancelInvoke(); // batalkan jadwal muncul/sembunyi normal yang lama

        if (state == GhostState.Hidden)
        {
            TryAppearNearPlayer();
        }
    }

    private void CatchPlayer()
    {
        hasCaughtPlayer = true;
        agent.isStopped = true;

        if (chaseLoopSound != null) chaseLoopSound.Stop();
        animator?.SetFloat(speedParamName, 0f);
        animator?.SetTrigger(attackParamName);

        Invoke(nameof(GoToJumpscareScene), 0.8f);
    }

    private void GoToJumpscareScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(jumpscareSceneName);
    }

    // =====================================================
    // UTIL
    // =====================================================

    private void SetVisible(bool visible)
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat(speedParamName, currentSpeed);
    }

    // Visualisasi range di Scene view (untuk debug)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
}