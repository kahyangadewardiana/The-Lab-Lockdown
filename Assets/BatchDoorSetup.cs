// BatchDoorSetup.cs
// Taruh file ini di folder: Assets/Editor/
// (folder harus bernama "Editor" persis)
//
// FUNGSI: Otomatis mengubah banyak object pintu polos (hasil split dari
// Blender) menjadi pintu interaktif sekaligus — bikin GameObject 'Hinge'
// kosong untuk tiap pintu di posisi tepi yang benar, jadikan pintu sebagai
// child-nya, tambahkan Box Collider, dan tambahkan script DoorInteractable.
// Tidak perlu setup manual satu-satu untuk puluhan pintu.
//
// CARA PAKAI:
// 1. Import 'corridor_doors.fbx' ke Unity seperti biasa, drag ke Hierarchy.
// 2. Kalau semua pintu itu jadi child dari 1 GameObject induk (misal
//    'corridor_doors'), itu bagus — itu yang nanti dijadikan 'Parent
//    Containing Doors' di window ini.
// 3. Buka menu Unity: Tools > Batch Door Setup
// 4. Drag GameObject induk tadi ke field 'Parent Containing Doors'
// 5. Atur 'Hinge Side' sesuai arah engsel yang diinginkan (Left/Right,
//    berdasarkan sumbu lokal X pintu)
// 6. Klik 'Setup All Doors as Interactable'
// 7. Klik Play, coba salah satu pintu, tekan E.
//
// CATATAN: Script ini otomatis SKIP object yang sudah pernah diproses
// (sudah punya DoorInteractable), supaya aman dijalankan berkali-kali
// kalau ada pintu baru ditambahkan belakangan.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatchDoorSetup : EditorWindow
{
    private Transform parentContainingDoors;
    private enum HingeSide { Left, Right }
    private HingeSide hingeSide = HingeSide.Left;
    private float openAngle = 90f;
    private float openSpeed = 2f;
    private bool addBoxCollider = true;

    private Vector2 scrollPos;
    private List<string> log = new List<string>();

    [MenuItem("Tools/Batch Door Setup")]
    public static void ShowWindow()
    {
        GetWindow<BatchDoorSetup>("Batch Door Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Door Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Drag GameObject induk yang berisi banyak pintu (child-child-nya), " +
            "lalu klik tombol di bawah untuk setup semuanya sekaligus.",
            MessageType.Info);

        parentContainingDoors = (Transform)EditorGUILayout.ObjectField(
            "Parent Containing Doors", parentContainingDoors, typeof(Transform), true);

        hingeSide = (HingeSide)EditorGUILayout.EnumPopup("Hinge Side (local X)", hingeSide);
        openAngle = EditorGUILayout.FloatField("Open Angle", openAngle);
        openSpeed = EditorGUILayout.FloatField("Open Speed", openSpeed);
        addBoxCollider = EditorGUILayout.Toggle("Add Box Collider", addBoxCollider);

        EditorGUILayout.Space();

        GUI.enabled = parentContainingDoors != null;
        if (GUILayout.Button("Setup All Doors as Interactable", GUILayout.Height(30)))
        {
            SetupAllDoors();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        GUILayout.Label("Log", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));
        foreach (var line in log)
        {
            EditorGUILayout.LabelField(line, EditorStyles.wordWrappedMiniLabel);
        }
        EditorGUILayout.EndScrollView();
    }

    private void SetupAllDoors()
    {
        log.Clear();

        // Ambil snapshot daftar child dulu, karena kita akan mengubah
        // hierarki (reparenting) sambil looping.
        List<Transform> doorCandidates = new List<Transform>();
        foreach (Transform child in parentContainingDoors)
        {
            doorCandidates.Add(child);
        }

        int processed = 0, skipped = 0;

        foreach (Transform door in doorCandidates)
        {
            if (door == null) continue;

            // Skip kalau ini sebenarnya Hinge hasil proses sebelumnya
            // (biar aman dijalankan berkali-kali)
            if (door.name.EndsWith("_Hinge"))
            {
                continue;
            }

            // Skip kalau sudah pernah diproses (sudah jadi child Hinge)
            if (door.parent != null && door.parent.name.EndsWith("_Hinge"))
            {
                skipped++;
                log.Add($"Skip '{door.name}': sudah diproses sebelumnya.");
                continue;
            }

            MeshRenderer renderer = door.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                skipped++;
                log.Add($"Skip '{door.name}': tidak ada MeshRenderer (bukan mesh pintu).");
                continue;
            }

            // Hitung posisi tepi (hinge) berdasarkan local bounds mesh
            MeshFilter meshFilter = door.GetComponent<MeshFilter>();
            Bounds localBounds = meshFilter != null && meshFilter.sharedMesh != null
                ? meshFilter.sharedMesh.bounds
                : renderer.bounds;

            float edgeLocalX = hingeSide == HingeSide.Left
                ? localBounds.min.x
                : localBounds.max.x;

            // Titik tepi dalam local space pintu, lalu ubah ke world space
            Vector3 edgeLocalPoint = new Vector3(
                edgeLocalX, localBounds.center.y, localBounds.center.z);
            Vector3 edgeWorldPoint = door.TransformPoint(edgeLocalPoint);

            // Buat GameObject Hinge kosong di posisi tepi tadi
            GameObject hingeObj = new GameObject(door.name + "_Hinge");
            hingeObj.transform.SetParent(parentContainingDoors, false);
            hingeObj.transform.position = edgeWorldPoint;
            hingeObj.transform.rotation = door.rotation;

            // Jadikan pintu sebagai child Hinge, world position tetap sama
            door.SetParent(hingeObj.transform, true);

            // Tambahkan Box Collider kalau diminta dan belum ada
            if (addBoxCollider && door.GetComponent<Collider>() == null)
            {
                door.gameObject.AddComponent<BoxCollider>();
            }

            // Tambahkan DoorInteractable ke Hinge
            DoorInteractable interactable = hingeObj.AddComponent<DoorInteractable>();
            interactable.openAngle = openAngle;
            interactable.openSpeed = openSpeed;

            processed++;
            log.Add($"OK: '{door.name}' -> hinge dibuat, collider & script ditambahkan.");
        }

        log.Add($"--- Selesai. Diproses: {processed}, dilewati: {skipped}. ---");
        Debug.Log($"Batch Door Setup selesai. Diproses: {processed}, dilewati: {skipped}.");
    }
}
