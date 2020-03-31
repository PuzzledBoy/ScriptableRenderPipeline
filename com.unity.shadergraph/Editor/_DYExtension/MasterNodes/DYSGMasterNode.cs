using System;
using UnityEditor.Graphing;
using UnityEngine;

namespace UnityEditor.ShaderGraph.DY
{
    [Serializable]
    abstract class DYSGMasterNode<T> : MasterNode<T>, IDYSGMasterNodeExtension where T : class, ISubShader
    {
        [SerializeField] private DY.SurfacePreset m_SurfacePreset = SurfacePreset.Opaque;

        [SerializeField] private RenderType m_RenderType = RenderType.Opaque;
        [SerializeField] private RenderQueue m_RenderQueue = RenderQueue.Geometry;
        [SerializeField] private CullMode m_Cull = CullMode.Back;
        [SerializeField] private DY.BlendMode m_SrcColor = DY.BlendMode.One;
        [SerializeField] private DY.BlendMode m_DstColor = DY.BlendMode.Zero;
        [SerializeField] private DY.BlendMode m_SrcAlpha = DY.BlendMode.One;
        [SerializeField] private DY.BlendMode m_DstAlpha = DY.BlendMode.Zero;
        [SerializeField] private ZTest m_ZTest = ZTest.LessEqual;
        [SerializeField] private ZWrite m_ZWrite = ZWrite.On;
        [SerializeField] private int m_Ref = 0;
        [SerializeField] private StencilComp m_Comp = StencilComp.Always;
        [SerializeField] private StencilOp m_PassOp = StencilOp.Keep;
        [SerializeField] private StencilOp m_FailOp = StencilOp.Keep;
        [SerializeField] private StencilOp m_ZFailOp = StencilOp.Keep;

        [SerializeField] private string m_CustomEditor;

        public MasterNode masterNode => this;

        #region Properties

        public SurfacePreset surfacePreset
        {
            get { return m_SurfacePreset; }
            set
            {
                if (m_SurfacePreset == value)
                    return;

                m_SurfacePreset = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        public RenderType renderType
        {
            get { return m_RenderType; }
            set
            {
                if (m_RenderType == value)
                    return;

                m_RenderType = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public RenderQueue renderQueue
        {
            get { return m_RenderQueue; }
            set
            {
                if (m_RenderQueue == value)
                    return;

                m_RenderQueue = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public CullMode cull
        {
            get { return m_Cull; }
            set
            {
                if (m_Cull == value)
                    return;

                m_Cull = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public DY.BlendMode srcColor
        {
            get { return m_SrcColor; }
            set
            {
                if (m_SrcColor == value) return;
                m_SrcColor = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public DY.BlendMode dstColor
        {
            get { return m_DstColor; }
            set
            {
                if (m_DstColor == value) return;
                m_DstColor = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public DY.BlendMode srcAlpha
        {
            get { return m_SrcAlpha; }
            set
            {
                if (m_SrcAlpha == value) return;
                m_SrcAlpha = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public DY.BlendMode dstAlpha
        {
            get { return m_DstAlpha; }
            set
            {
                if (m_DstAlpha == value) return;
                m_DstAlpha = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public ZTest zTest
        {
            get { return m_ZTest; }
            set
            {
                if (m_ZTest == value) return;
                m_ZTest = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public ZWrite zWrite
        {
            get { return m_ZWrite; }
            set
            {
                if (m_ZWrite == value) return;
                m_ZWrite = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public int sRef
        {
            get { return m_Ref; }
            set
            {
                if (m_Ref == value) return;
                m_Ref = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public StencilComp sComp
        {
            get { return m_Comp; }
            set
            {
                if (m_Comp == value) return;
                m_Comp = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public StencilOp sPassOp
        {
            get { return m_PassOp; }
            set
            {
                if (m_PassOp == value) return;
                m_PassOp = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public StencilOp sFailOp
        {
            get { return m_FailOp; }
            set
            {
                if (m_FailOp == value) return;
                m_FailOp = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public StencilOp sZFailOp
        {
            get { return m_ZFailOp; }
            set
            {
                if (m_ZFailOp == value) return;
                m_ZFailOp = value;
                Dirty(ModificationScope.Graph);
            }
        }

        #endregion

        public virtual string customEditor
        {
            get { return m_CustomEditor; }
            set
            {
                if (m_CustomEditor == value)
                    return;

                m_CustomEditor = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public virtual void GetCustomPropertiesString(ShaderStringBuilder sb)
        {
            //@ 重要，修改后，请更改DyMasterNodeGUI.optionPropertiesCount，与这里的行数一致

//            sb.AppendLine($"[HideInInspector] {PropertiesName.blendPreset}(\"__src\", Float) = {(int) }");
            sb.AppendLine($"[HideInInspector] {PropertiesName.srcColor}(\"__src\", Float) = {(int) srcColor}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.dstColor}(\"__dst\", Float) = {(int) dstColor}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.srcAlpha}(\"__src_alpha\", Float) = {(int) srcAlpha}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.dstAlpha}(\"__dst_alpha\", Float) = {(int) dstAlpha}");

            sb.AppendLine($"[HideInInspector] {PropertiesName.cull}(\"__cull\", Float) = {(int) cull}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.zWrite}(\"__z_write\", Float) = {(int) zWrite}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.zTest}(\"__z_test\", Float) = {(int) zTest}");

            sb.AppendLine($"[HideInInspector] {PropertiesName.sRef}(\"__ref\", Float) = {sRef}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.sComp}(\"__comp\", Float) = {(int) sComp}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.sPassOp}(\"__pass_op\", Float) = {(int) sPassOp}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.sFailOp}(\"__fail_op\", Float) = {(int) sFailOp}");
            sb.AppendLine($"[HideInInspector] {PropertiesName.sZFailOp}(\"__z_fail_op\", Float) = {(int) sZFailOp}");

            sb.AppendLine($"[HideInInspector] {PropertiesName.castShadow}(\"__cast_shadow\", Float) = 1");
            sb.AppendLine($"[HideInInspector] {PropertiesName.receiveShadow}(\"__receive_shadow\", Float) = 1");
        }
        
        public virtual void GetAfterSubShaderString(ShaderStringBuilder sb)
        {
        }
    }
}