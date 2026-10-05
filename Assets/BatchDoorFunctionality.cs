// BatchDoorFunctionality.cs
// Taruh di folder: Assets/Editor/
//
// FUNGSI: Dari sekian banyak pintu koridor yang sudah dibuat interaktif
// (lewat Batch Door Setup sebelumnya), tool ini MEMATIKAN fungsi buka-
// tutup di SEMUA pintu KECUALI yang Anda pilih sendiri untuk tetap aktif.
// Pintu yang dimatikan akan jadi dekorasi biasa (tidak ada prompt 'Tekan
// E', tidak bisa dibuka sama sekali), tapi tetap kelihatan normal secara
// visual.
//
// CARA PAKAI:
// 1. Buka menu Unity: Tools > Batch Door Functionality
// 2. Drag GameObject induk yang berisi semua pintu (misal 'corridor_doors',
//    yang dulu dipakai di Batch Door Setup) ke field 'Parent Containing Doors'
// 3. Di field 'Doors To Keep Functional', drag pintu-pintu (yang bertanda
//    '_Hinge') yang MAU TETAP BISA DIBUKA -- boleh select banyak sekaligus
//    di Hierarchy lalu drag bareng ke List ini.
// 4. Klik 'Apply: Disable All Except Listed'
// 5. Semua pintu SELAIN yang ada di list itu otomatis jadi non-interaktif.
//
// Aman dijalankan berkali-kali kalau mau ubah pilihan lagi -- klik
// 'Re-enable All First' dulu kalau mau reset semua pintu jadi aktif lagi
// sebelum memilih ulang.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatchDoorFunctionality : EditorWindow
{
    private Transform parentContainingDoors;
    private List<Transform> doorsToKeepFunctional = new List<Transform>();
    private Vector2 scrollPos;
    private List<string> log = new List<string>();

    [MenuItem("Tools/Batch Door Functionality")]
    public static void ShowWindow()
    {
        GetWindow<BatchDoorFunctionality>("Batch Door Functionality");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Door Functionality", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Matikan fungsi buka-tutup di semua pintu KECUALI yang dipilih " +
            "di list 'Doors To Keep Functional'.", MessageType.Info);

        parentContainingDoors = (Transform)EditorGUILayout.ObjectField(
            "Parent Containing Doors", parentContainingDoors, typeof(Transform), true);

        EditorGUILayout.Space();
        GUILayout.Label("Doors To Keep Functional", EditorStyles.boldLabel);

        // Daftar manual (drag & drop 1-per-1), lebih stabil di EditorWindow
        // dibanding pakai SerializedObject/PropertyField untuk List biasa.
        for (int i = 0; i < doorsToKeepFunctional.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            doorsToKeepFunctional[i] = (Transform)EditorGUILayout.ObjectField(
                doorsToKeepFunctional[i], typeof(Transform), true);

            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                doorsToKeepFunctional.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                break; // hindari error index setelah list berubah di tengah loop
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Tambah Slot Pintu"))
        {
            doorsToKeepFunctional.Add(null);
        }

        if (GUILayout.Button("+ Tambah Semua yang Sedang Di-select di Hierarchy"))
        {
            foreach (Transform t in Selection.transforms)
            {
                if (!doorsToKeepFunctional.Contains(t))
                {
                    doorsToKeepFunctional.Add(t);
                }
            }
        }

        EditorGUILayout.Space();

        GUI.enabled = parentContainingDoors != null;
        if (GUILayout.Button("Apply: Disable All Except Listed", GUILayout.Height(30)))
        {
            ApplyFunctionality();
        }
        if (GUILayout.Button("Re-enable All First (reset)", GUILayout.Height(25)))
        {
            ReenableAll();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        GUILayout.Label("Log", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        foreach (var line in log)
        {
            EditorGUILayout.LabelField(line, EditorStyles.wordWrappedMiniLabel);
        }
        EditorGUILayout.EndScrollView();
    }

    private void ApplyFunctionality()
    {
        log.Clear();
        int kept = 0, disabled = 0;

        foreach (Transform child in parentContainingDoors)
        {
            if (child == null || !child.name.EndsWith("_Hinge")) continue;

            bool shouldKeep = doorsToKeepFunctional.Contains(child);

            DoorInteractable interactable = child.GetComponent<DoorInteractable>();
            Collider doorCollider = child.GetComponentInChildren<Collider>();

            if (shouldKeep)
            {
                if (interactable != null) interactable.enabled = true;
                if (doorCollider != null) doorCollider.enabled = true;
                kept++;
                log.Add($"AKTIF: '{child.name}'");
            }
            else
            {
                if (interactable != null) interactable.enabled = false;
                if (doorCollider != null) doorCollider.enabled = false;
                disabled++;
            }
        }

        log.Add($"--- Selesai. Tetap aktif: {kept}, dimatikan: {disabled}. ---");
        Debug.Log($"Batch Door Functionality selesai. Aktif: {kept}, dimatikan: {disabled}.");
    }

    private void ReenableAll()
    {
        int count = 0;
        foreach (Transform child in parentContainingDoors)
        {
            if (child == null || !child.name.EndsWith("_Hinge")) continue;

            DoorInteractable interactable = child.GetComponent<DoorInteractable>();
            Collider doorCollider = child.GetComponentInChildren<Collider>();

            if (interactable != null) interactable.enabled = true;
            if (doorCollider != null) doorCollider.enabled = true;
            count++;
        }
        log.Clear();
        log.Add($"Semua {count} pintu diaktifkan ulang.");
    }
}