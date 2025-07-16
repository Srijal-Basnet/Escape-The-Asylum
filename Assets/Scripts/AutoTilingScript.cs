using UnityEngine;

public class AutoTileWalls : MonoBehaviour
{
    // Scaling factor to control texture size
    public Vector2 textureScaleMultiplier = new Vector2(1, 1);

    void Start()
    {
        // Get the Renderer component of the wall
        Renderer renderer = GetComponent<Renderer>();

        // Ensure the renderer and material exist
        if (renderer != null && renderer.material != null)
        {
            // Get the scale of the wall
            Vector3 scale = transform.localScale;

            // Adjust the material's tiling based on the scale and multiplier
            renderer.material.mainTextureScale = new Vector2(
                scale.x / textureScaleMultiplier.x,
                scale.z / textureScaleMultiplier.y
            );
        }
        else
        {
            Debug.LogWarning($"Renderer or Material missing on {gameObject.name}");
        }
    }
}


