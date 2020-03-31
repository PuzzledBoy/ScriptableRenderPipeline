using System;
using UnityEditor.ShaderGraph.DY;
using UnityEngine;
using UnityEngine.Rendering;
using BlendMode = UnityEditor.ShaderGraph.DY.BlendMode;
using CullMode = UnityEditor.ShaderGraph.DY.CullMode;
using StencilOp = UnityEditor.ShaderGraph.DY.StencilOp;

namespace UnityEditor.Rendering.Universal.DY
{
    //TODO 对修改过的选项，显示为粗体
    public partial class DYSGMasterNodeGUI : UnityEditor.ShaderGUI
    {
        public MaterialEditor materialEditor;

        private const string k_KeyPrefix = "UniversalRP:Material:UI_State:";
        private const string m_AlphaClipKeyword = "_AlphaClip";
        private bool m_FirstTimeApply = true;

        #region Properties

        protected float defaultSrcColor { get; set; }
        protected float defaultDstColor { get; set; }
        protected float defaultSrcAlpha { get; set; }
        protected float defaultDstAlpha { get; set; }
        protected float defaultCullMode { get; set; }
        protected float defaultZTest { get; set; }
        protected float defaultZWrite { get; set; }
        protected float defaultSRef { get; set; }
        protected float defaultSComp { get; set; }
        protected float defaultSPassOp { get; set; }
        protected float defaultSFailOp { get; set; }
        protected float defaultSZFailOp { get; set; }
        protected float defaultCastShadow { get; set; }
        protected float defaultReceiveShadow { get; set; }

        protected MaterialProperty srcColor { get; set; }
        protected MaterialProperty dstColor { get; set; }
        protected MaterialProperty srcAlpha { get; set; }
        protected MaterialProperty dstAlpha { get; set; }
        protected MaterialProperty cullMode { get; set; }
        protected MaterialProperty zTest { get; set; }
        protected MaterialProperty zWrite { get; set; }
        protected MaterialProperty sRef { get; set; }
        protected MaterialProperty sComp { get; set; }
        protected MaterialProperty sPassOp { get; set; }
        protected MaterialProperty sFailOp { get; set; }
        protected MaterialProperty sZFailOp { get; set; }
        protected MaterialProperty castShadow { get; set; }
        protected MaterialProperty receiveShadow { get; set; }

        #endregion

        #region Header Foldout States

        private SavedBool m_OptionsFoldout;
        private SavedBool m_InputsFoldout;
        private SavedBool m_FeatureFoldout;

        #endregion

        //TODO Drag Shader to Material don‘t trigger
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            // 重置参数，和Shader保持一致
            var cnt = ShaderUtil.GetPropertyCount(newShader);
            for (int i = 0; i < cnt; ++i)
            {
                var name = newShader.GetPropertyName(i);

                if (name.StartsWith(PropertiesName.prefix))
                {
                    var defaultValue = newShader.GetPropertyDefaultFloatValue(i);
                    material.SetFloat(name, defaultValue);
                }
            }

            // Clear Material Properties
        }

        //在这里设置Keyword，Pass enabled等
        protected virtual void MaterialChanged(Material material)
        {
            // Keyword.
            material.shaderKeywords = null;

            //Cast Shadow
            material.SetShaderPassEnabled("ShadowCaster", material.GetFloat(PropertiesName.castShadow) != 0.0f);

            //Receive Shadow
            CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", material.GetFloat(PropertiesName.receiveShadow) == 0.0f);
        }

//        public static void SetMaterialKeywords(Material material, Action<Material> shadingModelFunc = null, Action<Material> shaderFunc = null)
//        {
//            // Clear all keywords for fresh start
//            material.shaderKeywords = null;
//            // Setup blending - consistent across all Universal RP shaders
//            SetupMaterialBlendMode(material);
//            // Receive Shadows
//            if(material.HasProperty("_ReceiveShadows"))
//                CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", material.GetFloat("_ReceiveShadows") == 0.0f);
//            // Emission
//            if (material.HasProperty("_EmissionColor"))
//                MaterialEditor.FixupEmissiveFlag(material);
//            bool shouldEmissionBeEnabled =
//                (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
//            if (material.HasProperty("_EmissionEnabled") && !shouldEmissionBeEnabled)
//                shouldEmissionBeEnabled = material.GetFloat("_EmissionEnabled") >= 0.5f;
//            CoreUtils.SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);
//            // Normal Map
//            if(material.HasProperty("_BumpMap"))
//                CoreUtils.SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap"));
//            // Shader specific keyword functions
//            shadingModelFunc?.Invoke(material);
//            shaderFunc?.Invoke(material);
//        }
        private float GetPropertiesDefaultValue(Material material, string propertiesName)
        {
            var index = material.shader.FindPropertyIndex(propertiesName);
            return material.shader.GetPropertyDefaultFloatValue(index);
        }

        private void RevertOption()
        {
            //TODO RenderType and RenderQueue
            srcColor.floatValue = defaultSrcColor;
            dstColor.floatValue = defaultDstColor;
            srcAlpha.floatValue = defaultSrcAlpha;
            dstAlpha.floatValue = defaultDstAlpha;

            cullMode.floatValue = defaultCullMode;
            zTest.floatValue = defaultZTest;
            zWrite.floatValue = defaultZWrite;

            sRef.floatValue = defaultSRef;
            sComp.floatValue = defaultSComp;
            sPassOp.floatValue = defaultSPassOp;
            sFailOp.floatValue = defaultSFailOp;
            sZFailOp.floatValue = defaultSZFailOp;
        }
        
        private void FindPropertiesDefaultValue(Material material)
        {
            defaultSrcColor = GetPropertiesDefaultValue(material, PropertiesName.srcColor);
            defaultDstColor = GetPropertiesDefaultValue(material, PropertiesName.dstColor);
            defaultSrcAlpha = GetPropertiesDefaultValue(material, PropertiesName.srcAlpha);
            defaultDstAlpha = GetPropertiesDefaultValue(material, PropertiesName.dstAlpha);

            defaultCullMode = GetPropertiesDefaultValue(material, PropertiesName.cull);
            defaultZTest = GetPropertiesDefaultValue(material, PropertiesName.zTest);
            defaultZWrite = GetPropertiesDefaultValue(material, PropertiesName.zWrite);

            defaultSRef = GetPropertiesDefaultValue(material, PropertiesName.sRef);
            defaultSComp = GetPropertiesDefaultValue(material, PropertiesName.sComp);
            defaultSPassOp = GetPropertiesDefaultValue(material, PropertiesName.sPassOp);
            defaultSFailOp = GetPropertiesDefaultValue(material, PropertiesName.sFailOp);
            defaultSZFailOp = GetPropertiesDefaultValue(material, PropertiesName.sZFailOp);

            defaultCastShadow = GetPropertiesDefaultValue(material, PropertiesName.castShadow);
            defaultReceiveShadow = GetPropertiesDefaultValue(material, PropertiesName.receiveShadow);
        }

        protected virtual void FindProperties(MaterialProperty[] properties)
        {
            srcColor = FindProperty(PropertiesName.srcColor, properties);
            dstColor = FindProperty(PropertiesName.dstColor, properties);
            srcAlpha = FindProperty(PropertiesName.srcAlpha, properties);
            dstAlpha = FindProperty(PropertiesName.dstAlpha, properties);

            cullMode = FindProperty(PropertiesName.cull, properties);
            zTest = FindProperty(PropertiesName.zTest, properties);
            zWrite = FindProperty(PropertiesName.zWrite, properties);

            sRef = FindProperty(PropertiesName.sRef, properties);
            sComp = FindProperty(PropertiesName.sComp, properties);
            sPassOp = FindProperty(PropertiesName.sPassOp, properties);
            sFailOp = FindProperty(PropertiesName.sFailOp, properties);
            sZFailOp = FindProperty(PropertiesName.sZFailOp, properties);

            castShadow = FindProperty(PropertiesName.castShadow, properties);
            receiveShadow = FindProperty(PropertiesName.receiveShadow, properties);
        }

        protected virtual void OnOpenGUI(Material material)
        {
            // Foldout states
            
            var headerStateKey = k_KeyPrefix + material.shader.name; // Create key string for editor prefs
            m_OptionsFoldout = new SavedBool($"{headerStateKey}.SurfaceOptionsFoldout", true);
            m_InputsFoldout = new SavedBool($"{headerStateKey}.SurfaceInputsFoldout", true);
            m_FeatureFoldout = new SavedBool($"{headerStateKey}.AdvancedFoldout", false);

            //TODO 在开着面板的情况下，Shader Properties 更新
            //Default Value主要用在对修改的属性显示为粗体，只和当前面板显示的Material Properties有关，所以在这里初始化
            FindPropertiesDefaultValue(material);

            foreach (var obj in materialEditor.targets)
                MaterialChanged((Material) obj);
        }
        

        protected virtual void OnPropertiesGUI(Material material, MaterialProperty[] properties)
        {
            EditorGUI.BeginChangeCheck();

            // Input
            m_InputsFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(m_InputsFoldout.value, Styles.Inputs);
            if (m_InputsFoldout.value)
            {
                DrawInputProperties(material, properties);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            // Options
            m_OptionsFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(m_OptionsFoldout.value, Styles.Options);
            if (m_OptionsFoldout.value)
            {
                DrawOptions(material);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            // Features
            m_FeatureFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(m_FeatureFoldout.value, Styles.Features);
            if (m_FeatureFoldout.value)
            {
                DrawFeatures(material);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in materialEditor.targets)
                    MaterialChanged((Material) obj);
            }
        }

        protected virtual void DrawInputProperties(Material material, MaterialProperty[] properties)
        {
            foreach (var prop in properties)
            {
                if ((uint) (prop.flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) <= 0U)
                {
                    //TODO TextureField 畸形
                    materialEditor.ShaderProperty(EditorGUILayout.GetControlRect(true, materialEditor.GetPropertyHeight(prop, prop.displayName), EditorStyles.layerMaskField), prop, prop.displayName);
                }
            }
        }

        protected virtual void DrawOptions(Material material)
        {
            //Revert To Shader

//            var origFontStyle = EditorStyles.label.fontStyle;
//            EditorStyles.label.fontStyle = FontStyle.Bold;
//            EditorGUILayout.Toggle("Is active?", false, EditorStyles.toggle);
//            EditorStyles.label.fontStyle = origFontStyle;

            //TODO 更友好的UI
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = HasMultipleMixedRenderTypeValues();
            //TODO 是否改成properties
            var type = EditorGUILayout.EnumPopup(Styles.RenderType, (RenderType) Enum.Parse(typeof(RenderType), material.GetTag("RenderType", true), true));

            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.RenderType.text);
                material.SetOverrideTag("RenderType", type.ToString());
            }

            EditorGUI.showMixedValue = false;

            materialEditor.RenderQueueField();
            DoEnumPopUp<CullMode>(cullMode, Styles.CullMode,defaultCullMode);

            DrawTitle("Blend");
            DoEnumPopUp<BlendMode>(srcColor, Styles.srcColor,defaultSrcColor);
            DoEnumPopUp<BlendMode>(dstColor, Styles.dstColor,defaultDstColor);
            DoEnumPopUp<BlendMode>(srcAlpha, Styles.srcAlpha,defaultSrcAlpha);
            DoEnumPopUp<BlendMode>(dstAlpha, Styles.dstAlpha,defaultDstAlpha);

            DrawTitle("Depth");
            DoEnumPopUp<ZTest>(zTest, Styles.ZTest,defaultZTest);
            DoEnumPopUp<ZWrite>(zWrite, Styles.ZWrite,defaultZWrite);

            DrawTitle("Stencil");
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = sRef.hasMixedValue;
            //TODO IntField support modify editor
            var newSRef = EditorGUILayout.IntField(Styles.sRef, (int) sRef.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.sRef.text);
                newSRef = Mathf.Clamp(newSRef, 0, 255);
                sRef.floatValue = newSRef;
            }

            EditorGUI.showMixedValue = false;

            DoEnumPopUp<StencilComp>(sComp, Styles.sComp);
            DoEnumPopUp<StencilOp>(sPassOp, Styles.sPassOp);
            DoEnumPopUp<StencilOp>(sFailOp, Styles.sFailOp);
            DoEnumPopUp<StencilOp>(sZFailOp, Styles.sZFailOp);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Revert To Shader"))
            {
                materialEditor.RegisterPropertyChangeUndo("Revert");
                RevertOption();
            }
        }

        protected virtual void DrawFeatures(Material material)
        {
            DoToggle(castShadow, Styles.castShadow);
            DoToggle(receiveShadow, Styles.receiveShadow);

            materialEditor.DoubleSidedGIField();
            materialEditor.EnableInstancingField();
            //TODO 和当前ShaderGraph的AlphaClip使用总感觉有点奇怪
//            // Alpha Clip
//            var alphaClip = material.IsKeywordEnabled(m_AlphaClipKeyword);
//            alphaClip = EditorGUILayout.Toggle(Styles.alphaClip, alphaClip);
//            if (alphaClip) material.EnableKeyword(m_AlphaClipKeyword);
//            else material.DisableKeyword(m_AlphaClipKeyword);
        }


        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
        {
            if (materialEditorIn == null)
                throw new ArgumentNullException("materialEditor");

            FindProperties(properties); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly

            materialEditor = materialEditorIn;
            Material material = materialEditor.target as Material;

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a universal shader.
            if (m_FirstTimeApply)
            {
                OnOpenGUI(material);
                m_FirstTimeApply = false;
            }

            OnPropertiesGUI(material, properties);
        }

        private bool HasMultipleMixedRenderTypeValues()
        {
            string renderType = (materialEditor.targets[0] as Material).GetTag("RenderType", true);
            for (int index = 1; index < materialEditor.targets.Length; ++index)
            {
                if (renderType != (materialEditor.targets[index] as Material).GetTag("RenderType", true))
                    return true;
            }

            return false;
        }

//        private bool HasMultipleMixedQueueValues()
//        {
//            int rawRenderQueue = (this.targets[0] as Material).rawRenderQueue;
//            for (int index = 1; index < this.targets.Length; ++index)
//            {
//                if (rawRenderQueue != (this.targets[index] as Material).rawRenderQueue)
//                    return true;
//            }
//            return false;
//        }


        protected class Styles
        {
            // Catergories
            public static readonly GUIContent Options =
                new GUIContent("Options", "Controls how Universal RP renders the Material on a screen.");

            public static readonly GUIContent Inputs = new GUIContent("Inputs",
                "These settings describe the look and feel of the surface itself.");

            public static readonly GUIContent Features = new GUIContent("Features",
                "These settings affect behind-the-scenes rendering and underlying calculations.");

            // Options
            public static readonly GUIContent RenderType = new GUIContent("Render Type");
            public static readonly GUIContent RenderQueue = new GUIContent("Render Queue");
            public static readonly GUIContent CullMode = new GUIContent("Cull Mode");
            public static readonly GUIContent ZTest = new GUIContent("Z Test");
            public static readonly GUIContent ZWrite = new GUIContent("Z Write");

            public static readonly GUIContent srcColor = new GUIContent("Src Color");
            public static readonly GUIContent dstColor = new GUIContent("Dst Color");
            public static readonly GUIContent srcAlpha = new GUIContent("Src Alpha");
            public static readonly GUIContent dstAlpha = new GUIContent("Dst Alpha");

            public static readonly GUIContent sRef = new GUIContent("Ref");
            public static readonly GUIContent sComp = new GUIContent("Comp");
            public static readonly GUIContent sPassOp = new GUIContent("Pass Op");
            public static readonly GUIContent sFailOp = new GUIContent("Fail Op");
            public static readonly GUIContent sZFailOp = new GUIContent("ZFail Op");

            // Features
            public static readonly GUIContent castShadow = new GUIContent("Cast Shadow");
            public static readonly GUIContent receiveShadow = new GUIContent("Receive Shadow");
            public static readonly GUIContent alphaClip = new GUIContent("Alpha Clip");
            public static readonly GUIContent alphaClipThreshold = new GUIContent("Threshold");
        }
    }
}