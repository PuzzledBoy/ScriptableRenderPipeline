using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.DY;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.Rendering.Universal.DY
{
    // 用于解决默认Master Node无法指定ZWrite ZTest的问题
    static class DYSGUniversalShaderGraphUtilities
    {
        public static SurfaceMaterialTags BuildMaterialTags(RenderType renderType, RenderQueue renderQueue)
        {
            var tags = new SurfaceMaterialTags();
            tags.renderType = (SurfaceMaterialTags.RenderType)Enum.Parse(typeof(SurfaceMaterialTags.RenderType), renderType.ToString());
            tags.renderQueue = (SurfaceMaterialTags.RenderQueue) Enum.Parse(typeof(SurfaceMaterialTags.RenderQueue), renderQueue.ToString());
            return tags;
        }
        
        public static void SetPassRenderOption(ref ShaderPass pass)
        {
            if (string.IsNullOrEmpty(pass.CullOverride))
            {
                pass.CullOverride = $"Cull [{PropertiesName.cull}]";
            }

            if (string.IsNullOrEmpty(pass.BlendOverride))
            {
                pass.BlendOverride = $"Blend [{PropertiesName.srcColor}] [{PropertiesName.dstColor}], [{PropertiesName.srcAlpha}] [{PropertiesName.dstAlpha}]";
            }
            
            if (string.IsNullOrEmpty(pass.ZWriteOverride))
            {
                pass.ZWriteOverride = $"ZWrite [{PropertiesName.zWrite}]";
            }

            if (string.IsNullOrEmpty(pass.ZTestOverride))
            {
                pass.ZTestOverride = $"ZTest [{PropertiesName.zTest}]";
            }

            if (pass.StencilOverride == null)
            {
                pass.StencilOverride = new List<string>(){$"Ref [{PropertiesName.sRef}]",$"Comp [{PropertiesName.sComp}]",$"Pass [{PropertiesName.sPassOp}]",$"Fail [{PropertiesName.sFailOp}]",$"ZFail [{PropertiesName.sZFailOp}]"};
            }
        }

        public static void SetRenderState(SurfaceType surfaceType, AlphaMode alphaMode, SurfaceMaterialOptions.CullMode cullMode, SurfaceMaterialOptions.ZWrite zWrite, SurfaceMaterialOptions.ZTest zTest, ref ShaderPass pass)
        {
            // Get default render state from Master Node
            var options = GetMaterialOptions(surfaceType, alphaMode, cullMode, zWrite, zTest);

            // Update render state on ShaderPass if there is no active override
            if (string.IsNullOrEmpty(pass.ZWriteOverride))
            {
                pass.ZWriteOverride = "ZWrite " + options.zWrite.ToString();
            }

            if (string.IsNullOrEmpty(pass.ZTestOverride))
            {
                pass.ZTestOverride = "ZTest " + options.zTest.ToString();
            }

            if (string.IsNullOrEmpty(pass.CullOverride))
            {
                pass.CullOverride = "Cull " + options.cullMode.ToString();
            }

            if (string.IsNullOrEmpty(pass.BlendOverride))
            {
                pass.BlendOverride = string.Format("Blend {0} {1}, {2} {3}", options.srcBlend, options.dstBlend, options.alphaSrcBlend, options.alphaDstBlend);
            }
        }

        public static SurfaceMaterialOptions GetMaterialOptions(SurfaceType surfaceType, AlphaMode alphaMode, SurfaceMaterialOptions.CullMode cullMode, SurfaceMaterialOptions.ZWrite zWrite, SurfaceMaterialOptions.ZTest zTest, bool offscreenTransparent = false)
        {
            var materialOptions = new SurfaceMaterialOptions();
            switch (surfaceType)
            {
                case SurfaceType.Opaque:
                    materialOptions.srcBlend = SurfaceMaterialOptions.BlendMode.One;
                    materialOptions.dstBlend = SurfaceMaterialOptions.BlendMode.Zero;
                    materialOptions.cullMode = cullMode;
                    materialOptions.zTest = zTest;
                    materialOptions.zWrite = zWrite;
                    break;
                case SurfaceType.Transparent:
                    switch (alphaMode)
                    {
                        case AlphaMode.Alpha:
                            materialOptions.srcBlend = SurfaceMaterialOptions.BlendMode.SrcAlpha;
                            materialOptions.dstBlend = SurfaceMaterialOptions.BlendMode.OneMinusSrcAlpha;
                            if (offscreenTransparent)
                            {
                                materialOptions.alphaSrcBlend = SurfaceMaterialOptions.BlendMode.Zero;
                            }
                            else
                            {
                                materialOptions.alphaSrcBlend = SurfaceMaterialOptions.BlendMode.One;
                            }

                            materialOptions.alphaDstBlend = SurfaceMaterialOptions.BlendMode.OneMinusSrcAlpha;
                            materialOptions.cullMode = cullMode;
                            materialOptions.zTest = zTest;
                            materialOptions.zWrite = zWrite;
                            break;
                        case AlphaMode.Premultiply:
                            materialOptions.srcBlend = SurfaceMaterialOptions.BlendMode.One;
                            materialOptions.dstBlend = SurfaceMaterialOptions.BlendMode.OneMinusSrcAlpha;
                            if (offscreenTransparent)
                            {
                                materialOptions.alphaSrcBlend = SurfaceMaterialOptions.BlendMode.Zero;
                            }
                            else
                            {
                                materialOptions.alphaSrcBlend = SurfaceMaterialOptions.BlendMode.One;
                            }

                            materialOptions.alphaDstBlend = SurfaceMaterialOptions.BlendMode.OneMinusSrcAlpha;
                            materialOptions.cullMode = cullMode;
                            materialOptions.zTest = zTest;
                            materialOptions.zWrite = zWrite;
                            break;
                        case AlphaMode.Additive:
                            materialOptions.srcBlend = SurfaceMaterialOptions.BlendMode.One;
                            materialOptions.dstBlend = SurfaceMaterialOptions.BlendMode.One;
                            if (offscreenTransparent)
                            {
                                materialOptions.alphaSrcBlend = SurfaceMaterialOptions.BlendMode.Zero;
                            }
                            else
                            {
                                materialOptions.alphaSrcBlend = SurfaceMaterialOptions.BlendMode.One;
                            }

                            materialOptions.alphaDstBlend = SurfaceMaterialOptions.BlendMode.One;
                            materialOptions.cullMode = cullMode;
                            materialOptions.zTest = zTest;
                            materialOptions.zWrite = zWrite;
                            break;
                        case AlphaMode.Multiply:
                            materialOptions.srcBlend = SurfaceMaterialOptions.BlendMode.DstColor;
                            materialOptions.dstBlend = SurfaceMaterialOptions.BlendMode.Zero;
                            materialOptions.cullMode = cullMode;
                            materialOptions.zTest = zTest;
                            materialOptions.zWrite = zWrite;
                            break;
                    }

                    break;
            }

            return materialOptions;
        }
    }
}