using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CutoutMaskUI : Image 
{
    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        var mat = base.GetModifiedMaterial(baseMaterial);
        if (mat != null)
        {
            // Invert the mask: draw where stencil != reference
            mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
        }
        return mat;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // Force a proper rebind after scene loads/enables
        SetMaterialDirty();
        SetVerticesDirty();
    }
}