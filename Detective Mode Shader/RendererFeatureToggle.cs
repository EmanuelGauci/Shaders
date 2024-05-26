using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class RendererFeatureToggle : MonoBehaviour {
    public ScriptableRendererFeature rendererFeature;

    public bool activateFeature = true;
    private GameObject[] glowObjects;
    public Material separateMaterial; // Assign the separate material in the Unity Editor

    // Store the original materials of glowObjects
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

    private void Start() {
        if (rendererFeature == null) {
            Debug.LogError("RendererFeature is not assigned!");
            enabled = false; // Disable the script if RendererFeature is not assigned
            return;
        }

        // Find all game objects with the "GlowTag" tag
        glowObjects = GameObject.FindGameObjectsWithTag("GlowTag");

        // Store the original materials
        foreach (GameObject glowObject in glowObjects) {
            Renderer renderer = glowObject.GetComponent<Renderer>();
            if (renderer != null) {
                originalMaterials[glowObject] = renderer.materials;
            }
        }
    }

    private void Update() {
        if (rendererFeature != null && rendererFeature.isActive != activateFeature) {
            rendererFeature.SetActive(activateFeature);
        }

        if (activateFeature && rendererFeature != null && rendererFeature.isActive && glowObjects != null) {
            // Pass the glowObjects array to the DetectiveModeFeature
            DetectiveModeFeature detectiveFeature = rendererFeature as DetectiveModeFeature;
            if (detectiveFeature != null) {
                detectiveFeature.SetGlowObjects(glowObjects);

                // Replace the material of glowObjects with separateMaterial
                foreach (GameObject glowObject in glowObjects) {
                    Renderer renderer = glowObject.GetComponent<Renderer>();
                    if (renderer != null) {
                        Material[] materials = new Material[renderer.sharedMaterials.Length];
                        for (int i = 0; i < materials.Length; i++) {
                            materials[i] = separateMaterial;
                        }
                        renderer.materials = materials;
                    }
                }
            }
        } else if (!activateFeature) {
            // Restore the original materials when activateFeature is false
            foreach (KeyValuePair<GameObject, Material[]> pair in originalMaterials) {
                Renderer renderer = pair.Key.GetComponent<Renderer>();
                if (renderer != null) {
                    renderer.materials = pair.Value;
                }
            }
        }
    }
}
