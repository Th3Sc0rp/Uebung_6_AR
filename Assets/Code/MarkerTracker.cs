using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerTracker : MonoBehaviour
{
    public Place placeManager;
    public ARTrackedImageManager imageManager;

    private GameObject currentInstance;

    void OnEnable()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    public void SetAktiv(bool aktiv)
    {
        this.enabled = aktiv;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            SpawnPrefab(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            UpdatePrefabPosition(trackedImage);
        }

        foreach (var trackedImage in args.removed)
        {
            if (currentInstance != null)
            {
                Destroy(currentInstance);
            }
        }
    }

    void SpawnPrefab(ARTrackedImage trackedImage)
    {
        if (placeManager.prefabs.Count == 0) return;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        currentInstance = Instantiate(
            placeManager.prefabs[placeManager.CurrentIndex],
            trackedImage.transform.position,
            trackedImage.transform.rotation,
            trackedImage.transform
        );
    }

    void UpdatePrefabPosition(ARTrackedImage trackedImage)
    {
        if (currentInstance != null)
        {
            currentInstance.transform.position = trackedImage.transform.position;
            currentInstance.transform.rotation = trackedImage.transform.rotation;
        }
    }

    public void ReplaceCurrentInstance()
    {
        if (imageManager.trackables.count == 0) return;

        foreach (var image in imageManager.trackables)
        {
            SpawnPrefab(image);
            break; 
        }
    }
}
