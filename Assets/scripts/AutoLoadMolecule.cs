using UnityEngine;
using chARpack;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// This script automatically loads a specific molecule scene on startup.
/// Attach this to a GameObject in the MainScene to enable auto-loading.
/// </summary>
public class AutoLoadMolecule : MonoBehaviour
{
    [Header("Auto Load Settings")]
    [Tooltip("Name of the molecule file to load automatically (without .xml extension)")]
    public string moleculeToLoad = "Sloan-project-settings-with-H";
    
    [Tooltip("Enable automatic loading on startup")]
    public bool enableAutoLoad = true;
    
    [Tooltip("Delay in seconds before auto-loading starts")]
    public float loadDelay = 1.0f;
    
    [Tooltip("Try to load from Resources folder first (recommended for APK builds)")]
    public bool tryResourcesFirst = true;

    private void Start()
    {
        if (enableAutoLoad && !string.IsNullOrEmpty(moleculeToLoad))
        {
            // Start the auto-load process after a short delay
            StartCoroutine(LoadMoleculeCoroutine());
        }
    }

    private IEnumerator LoadMoleculeCoroutine()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(loadDelay);
        
        // Wait for GlobalCtrl to be ready
        while (GlobalCtrl.Singleton == null)
        {
            Debug.LogWarning("[AutoLoadMolecule] GlobalCtrl not ready, waiting...");
            yield return new WaitForSeconds(0.5f);
        }

        yield return StartCoroutine(LoadMoleculeAutomatically());
    }

    private IEnumerator LoadMoleculeAutomatically()
    {
        Debug.Log($"[AutoLoadMolecule] Auto-loading molecule: {moleculeToLoad}");
        
        bool loadSuccess = false;
        
        // Strategy 1: Try loading from Resources folder first (works on all platforms)
        if (tryResourcesFirst)
        {
            Debug.Log("[AutoLoadMolecule] Trying to load from Resources folder...");
            try
            {
                // For Resources, we need to provide the path relative to Resources folder
                string resourcesPath = "SavedMolecules/" + moleculeToLoad;
                Debug.Log($"[AutoLoadMolecule] Loading from Resources path: {resourcesPath}");
                
                // Check if the resource exists
                TextAsset testAsset = Resources.Load<TextAsset>(resourcesPath);
                if (testAsset == null)
                {
                    Debug.LogWarning($"[AutoLoadMolecule] Resource not found at path: {resourcesPath}");
                    Debug.LogWarning("[AutoLoadMolecule] Make sure the file exists in Assets/Resources/SavedMolecules/");
                    Debug.LogWarning("[AutoLoadMolecule] Use Tools > Copy Molecule Files to Resources to copy files");
                }
                else
                {
                    Debug.Log($"[AutoLoadMolecule] Resource found, size: {testAsset.bytes.Length} bytes");
                    GlobalCtrl.Singleton.LoadMolecule(resourcesPath, true); // from_resources = true
                    loadSuccess = true;
                    Debug.Log("[AutoLoadMolecule] Successfully loaded from Resources folder");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[AutoLoadMolecule] Failed to load from Resources: {ex.Message}");
            }
        }
        
        // Strategy 2: Try loading from normal path if Resources failed
        if (!loadSuccess)
        {
            Debug.Log("[AutoLoadMolecule] Trying to load from SavedMolecules folder...");
            
            // For Android, we need to ensure the file exists in persistentDataPath
            if (Application.platform == RuntimePlatform.Android)
            {
                yield return StartCoroutine(EnsureFileExistsOnAndroid());
            }
            
            try
            {
                GlobalCtrl.Singleton.LoadMolecule(moleculeToLoad, false); // from_resources = false
                loadSuccess = true;
                Debug.Log("[AutoLoadMolecule] Successfully loaded from SavedMolecules folder");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AutoLoadMolecule] Failed to load from SavedMolecules: {ex.Message}");
            }
        }
        
        if (loadSuccess)
        {
            // Hide the main menu if HideMenuOnLoad is available
            if (HideMenuOnLoad.Instance != null)
            {
                HideMenuOnLoad.Instance.HideMenu();
                Debug.Log("[AutoLoadMolecule] Menu hidden after auto-load");
            }
            
            Debug.Log($"[AutoLoadMolecule] Successfully auto-loaded molecule: {moleculeToLoad}");
        }
        else
        {
            Debug.LogError($"[AutoLoadMolecule] Failed to auto-load molecule: {moleculeToLoad}");
        }
    }
    
    private IEnumerator EnsureFileExistsOnAndroid()
    {
        string fileName = moleculeToLoad + ".xml";
        string streamingAssetsPath = Application.streamingAssetsPath + "/SavedMolecules/" + fileName;
        string persistentPath = Application.persistentDataPath + "/SavedMolecules/" + fileName;
        
        // Create directory if it doesn't exist
        string persistentDir = Path.GetDirectoryName(persistentPath);
        if (!Directory.Exists(persistentDir))
        {
            Directory.CreateDirectory(persistentDir);
        }
        
        // Check if file already exists in persistent path
        if (File.Exists(persistentPath))
        {
            Debug.Log($"[AutoLoadMolecule] File already exists in persistent path: {persistentPath}");
            yield break;
        }
        
        Debug.Log($"[AutoLoadMolecule] Copying file from StreamingAssets to persistent path...");
        
        // Copy file from StreamingAssets to persistentDataPath
        UnityWebRequest request = UnityWebRequest.Get(streamingAssetsPath);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(persistentPath, request.downloadHandler.data);
            Debug.Log($"[AutoLoadMolecule] Successfully copied file to: {persistentPath}");
        }
        else
        {
            Debug.LogError($"[AutoLoadMolecule] Failed to copy file from StreamingAssets: {request.error}");
        }
    }
}
