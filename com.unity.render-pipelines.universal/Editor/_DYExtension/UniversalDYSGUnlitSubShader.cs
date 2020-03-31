using System;
using System.Collections.Generic;
using System.Linq;
using Data.Util;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.DY;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal.DY
{
    //TODO 拆分一个DYSubShader出来
    [Serializable]
    class UniversalDYSGUnlitSubShader : IDYSGUnlitSubShader
    {
        #region Passes

        ShaderPass m_UnlitPass = new ShaderPass
        {
            // Definition
            displayName = "Pass",
            referenceName = "SHADERPASS_UNLIT",
            passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",
            useInPreview = true,

            // Port mask
            vertexPorts = new List<int>()
            {
                UnlitMasterNode.PositionSlotId,
                UnlitMasterNode.VertNormalSlotId,
                UnlitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>
            {
                UnlitMasterNode.ColorSlotId,
                UnlitMasterNode.AlphaSlotId,
                UnlitMasterNode.AlphaThresholdSlotId
            },

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 3.0",
                "multi_compile_instancing",
            },
            keywords = new KeywordDescriptor[]
            {
                s_LightmapKeyword,
                s_DirectionalLightmapCombinedKeyword,
                s_SampleGIKeyword,
            },
        };

        ShaderPass m_DepthOnlyPass = new ShaderPass()
        {
            // Definition
            displayName = "DepthOnly",
            referenceName = "SHADERPASS_DEPTHONLY",
            lightMode = "DepthOnly",
            passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",
            useInPreview = true,

            // Port mask
            vertexPorts = new List<int>()
            {
                UnlitMasterNode.PositionSlotId,
                UnlitMasterNode.VertNormalSlotId,
                UnlitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>()
            {
                UnlitMasterNode.AlphaSlotId,
                UnlitMasterNode.AlphaThresholdSlotId
            },

            // Render State Overrides
            ZWriteOverride = "ZWrite On",
            ColorMaskOverride = "ColorMask 0",

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
            },
        };

        ShaderPass m_ShadowCasterPass = new ShaderPass()
        {
            // Definition
            displayName = "ShadowCaster",
            referenceName = "SHADERPASS_SHADOWCASTER",
            lightMode = "ShadowCaster",
            passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",

            // Port mask
            vertexPorts = new List<int>()
            {
                UnlitMasterNode.PositionSlotId,
                UnlitMasterNode.VertNormalSlotId,
                UnlitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>()
            {
                UnlitMasterNode.AlphaSlotId,
                UnlitMasterNode.AlphaThresholdSlotId
            },

            // Required fields
            requiredAttributes = new List<string>()
            {
                "Attributes.normalOS",
            },

            // Render State Overrides
            ZWriteOverride = "ZWrite On",
            ZTestOverride = "ZTest LEqual",

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl",
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
            },
            keywords = new KeywordDescriptor[]
            {
                s_SmoothnessChannelKeyword,
            },
        };

        #endregion

        #region Keywords

        static KeywordDescriptor s_LightmapKeyword = new KeywordDescriptor()
        {
            displayName = "Lightmap",
            referenceName = "LIGHTMAP_ON",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_DirectionalLightmapCombinedKeyword = new KeywordDescriptor()
        {
            displayName = "Directional Lightmap Combined",
            referenceName = "DIRLIGHTMAP_COMBINED",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_SampleGIKeyword = new KeywordDescriptor()
        {
            displayName = "Sample GI",
            referenceName = "_SAMPLE_GI",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_SmoothnessChannelKeyword = new KeywordDescriptor()
        {
            displayName = "Smoothness Channel",
            referenceName = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };

        #endregion

        private static ActiveFields GetActiveFieldsFromMasterNode(DYSGUnlitMasterNode masterNode, ShaderPass pass)
        {
            var activeFields = new ActiveFields();
            var baseActiveFields = activeFields.baseInstance;

            // Graph Vertex
            if (masterNode.IsSlotConnected(DYSGUnlitMasterNode.PositionSlotId) ||
                masterNode.IsSlotConnected(DYSGUnlitMasterNode.VertNormalSlotId) ||
                masterNode.IsSlotConnected(DYSGUnlitMasterNode.VertTangentSlotId))
            {
                baseActiveFields.Add("features.graphVertex");
            }

            // Graph Pixel (always enabled)
            baseActiveFields.Add("features.graphPixel");

            if (masterNode.IsSlotConnected(DYSGUnlitMasterNode.AlphaThresholdSlotId) ||
                masterNode.GetInputSlots<Vector1MaterialSlot>().First(x => x.id == DYSGUnlitMasterNode.AlphaThresholdSlotId).value > 0.0f)
            {
                baseActiveFields.Add("AlphaClip");
            }

            //TODO Transparent Keywork
            // Keywords for transparent
            // #pragma shader_feature _SURFACE_TYPE_TRANSPARENT
//            if (masterNode.surfaceType != ShaderGraph.SurfaceType.Opaque)
//            {
//                // transparent-only defines
//                baseActiveFields.Add("SurfaceType.Transparent");
//
//                // #pragma shader_feature _ _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
//                if (masterNode.alphaMode == AlphaMode.Alpha)
//                {
//                    baseActiveFields.Add("BlendMode.Alpha");
//                }
//                else if (masterNode.alphaMode == AlphaMode.Additive)
//                {
//                    baseActiveFields.Add("BlendMode.Add");
//                }
//                else if (masterNode.alphaMode == AlphaMode.Premultiply)
//                {
//                    baseActiveFields.Add("BlendMode.Premultiply");
//                }
//            }

            return activeFields;
        }


        private static bool GenerateShaderPass(DYSGUnlitMasterNode masterNode, ShaderPass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            DYSGUniversalShaderGraphUtilities.SetPassRenderOption(ref pass);

            // apply master node options to active fields
            var activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

            // use standard shader pass generation
            return GenerationUtils.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths,
                UniversalShaderGraphResources.s_Dependencies, UniversalShaderGraphResources.s_ResourceClassName, UniversalShaderGraphResources.s_AssemblyName);
        }

        public string GetSubshader(IMasterNode masterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
        {
            //TODO Add Denps

            // Master Node data
            var unlitMasterNode = masterNode as DYSGUnlitMasterNode;
            var subShader = new ShaderGenerator();

            subShader.AddShaderChunk("SubShader", true);
            subShader.AddShaderChunk("{", true);
            subShader.Indent();
            {
                var surfaceTags = DYSGUniversalShaderGraphUtilities.BuildMaterialTags(unlitMasterNode.renderType,unlitMasterNode.renderQueue);
                var tagsBuilder = new ShaderStringBuilder(0);
                surfaceTags.GetTags(tagsBuilder, "UniversalPipeline");
                subShader.AddShaderChunk(tagsBuilder.ToString());

                GenerateShaderPass(unlitMasterNode, m_UnlitPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPass(unlitMasterNode, m_ShadowCasterPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPass(unlitMasterNode, m_DepthOnlyPass, mode, subShader, sourceAssetDependencyPaths);   
            }
            subShader.Deindent();
            subShader.AddShaderChunk("}", true);

            return subShader.GetShaderString(0);
        }

        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            return renderPipelineAsset is UniversalRenderPipelineAsset;
        }

        public int GetPreviewPassIndex()
        {
            return 0;
        }
    }
}