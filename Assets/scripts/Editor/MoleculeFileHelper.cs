using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor utility to help copy molecule files from StreamingAssets to Resources folder
/// for APK builds. This script only works in the Unity Editor.
/// </summary>
public class MoleculeFileHelper
{
    [MenuItem("Tools/Copy Molecule Files to Resources")]
    public static void CopyMoleculeFilesToResources()
    {
        string streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets", "SavedMolecules");
        string resourcesPath = Path.Combine(Application.dataPath, "Resources", "SavedMolecules");
        
        if (!Directory.Exists(streamingAssetsPath))
        {
            EditorUtility.DisplayDialog("Error", "StreamingAssets/SavedMolecules folder not found!", "OK");
            return;
        }
        
        // Create Resources/SavedMolecules directory if it doesn't exist
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }
        
        // Copy all XML files
        string[] xmlFiles = Directory.GetFiles(streamingAssetsPath, "*.xml");
        int copiedCount = 0;
        
        foreach (string xmlFile in xmlFiles)
        {
            string fileName = Path.GetFileName(xmlFile);
            string destPath = Path.Combine(resourcesPath, fileName);
            
            try
            {
                File.Copy(xmlFile, destPath, true);
                copiedCount++;
                Debug.Log($"Copied: {fileName} to Resources/SavedMolecules/");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to copy {fileName}: {ex.Message}");
            }
        }
        
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Copy Complete", 
            $"Successfully copied {copiedCount} molecule files to Resources/SavedMolecules/\n\n" +
            "These files will now be included in APK builds.", "OK");
    }
    
    [MenuItem("Tools/Copy Specific Molecule to Resources")]
    public static void CopySpecificMoleculeToResources()
    {
        string moleculeName = "Sloan-project-settings-with-H"; // Default molecule name
        
        // Show input dialog
        string inputName = EditorUtility.OpenFilePanel("Select Molecule File", 
            Path.Combine(Application.dataPath, "StreamingAssets", "SavedMolecules"), "xml");
        
        if (string.IsNullOrEmpty(inputName))
            return;
            
        string fileName = Path.GetFileNameWithoutExtension(inputName);
        string resourcesPath = Path.Combine(Application.dataPath, "Resources", "SavedMolecules");
        
        // Create Resources/SavedMolecules directory if it doesn't exist
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }
        
        string destPath = Path.Combine(resourcesPath, Path.GetFileName(inputName));
        
        try
        {
            File.Copy(inputName, destPath, true);
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Copy Complete", 
                $"Successfully copied {Path.GetFileName(inputName)} to Resources/SavedMolecules/\n\n" +
                $"You can now use '{fileName}' in AutoLoadMolecule script.", "OK");
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to copy file: {ex.Message}", "OK");
        }
    }
}
