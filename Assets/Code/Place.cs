using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Place : MonoBehaviour
{
    public enum Platziermodus { Manuell, Marker }
    public Platziermodus aktuellerModus = Platziermodus.Manuell;

    public List<GameObject> prefabs;
    public MarkerTracker markerTracker;

    private ARRaycastManager raycastManager;
    private static readonly List<ARRaycastHit> hits = new();
    private int currentIndex = 0;
    private List<GameObject> platzierteObjekte = new();

    public int CurrentIndex => currentIndex;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        foreach (var prefab in prefabs)
        {
            Debug.Log("Awake() Prefab in Liste: " + prefab?.name);
        }

        
        if (markerTracker != null)
        {
            markerTracker.SetAktiv(false);
        }
    }

    void Update()
    {
        if (aktuellerModus != Platziermodus.Manuell)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 click = Mouse.current.position.ReadValue();

            if (raycastManager.Raycast(click, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                if (prefabs != null && prefabs.Count > currentIndex && prefabs[currentIndex] != null)
                {
                    GameObject obj = Instantiate(prefabs[currentIndex], hitPose.position, hitPose.rotation);
                    platzierteObjekte.Add(obj);
                    Debug.Log("Prefab instanziiert: " + prefabs[currentIndex].name);
                }
                else
                {
                    Debug.LogWarning("Kein gültiges Prefab vorhanden!");
                }
            }
        }
    }

    public void NextPrefab()
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("Prefab-Liste ist leer – kein Wechsel möglich.");
            return;
        }

        currentIndex = (currentIndex + 1) % prefabs.Count;
        Debug.Log($"NextPrefab gedrückt: Index {currentIndex}, Prefab: {prefabs[currentIndex].name}");

        if (aktuellerModus == Platziermodus.Marker && markerTracker != null)
            markerTracker.ReplaceCurrentInstance();
    }

    public void PreviousPrefab()
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("Prefab-Liste ist leer – kein Wechsel möglich.");
            return;
        }

        currentIndex = (currentIndex - 1 + prefabs.Count) % prefabs.Count;
        Debug.Log($"PreviousPrefab gedrückt: Index {currentIndex}, Prefab: {prefabs[currentIndex].name}");

        if (aktuellerModus == Platziermodus.Marker && markerTracker != null)
            markerTracker.ReplaceCurrentInstance();
    }

    public void ToggleMode()
    {
        if (aktuellerModus == Platziermodus.Manuell)
        {
            aktuellerModus = Platziermodus.Marker;
            Debug.Log("Modus: Marker Tracking");

            
            foreach (var obj in platzierteObjekte)
            {
                if (obj != null)
                    Destroy(obj);
            }

            platzierteObjekte.Clear();

            if (markerTracker != null)
                markerTracker.SetAktiv(true);
        }
        else
        {
            aktuellerModus = Platziermodus.Manuell;
            Debug.Log("Modus: Manuelles Platzieren");

            if (markerTracker != null)
                markerTracker.SetAktiv(false);
        }
    }
}
