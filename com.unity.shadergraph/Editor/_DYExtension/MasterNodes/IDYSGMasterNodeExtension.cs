namespace UnityEditor.ShaderGraph.DY
{
    interface IDYSGMasterNodeExtension
    {
        MasterNode masterNode { get; }

        SurfacePreset surfacePreset { get; set; }
        
        #region Options

        RenderType renderType { get; set; }

        RenderQueue renderQueue { get; set; }

        CullMode cull { get; set; }

        DY.BlendMode srcColor { get; set; }

        DY.BlendMode dstColor { get; set; }

        DY.BlendMode srcAlpha { get; set; }

        DY.BlendMode dstAlpha { get; set; }

        ZTest zTest { get; set; }

        ZWrite zWrite { get; set; }

        int sRef { get; set; }

        StencilComp sComp { get; set; }

        StencilOp sPassOp { get; set; }

        StencilOp sFailOp { get; set; }

        StencilOp sZFailOp { get; set; }

        #endregion


        string customEditor { get; set; }

        // ZWrite
        // ZTest
        // Blend Mode
        // Stencil
        
        void GetCustomPropertiesString(ShaderStringBuilder sb);
        
        // Add code after subshader, ie.. "Dependency "AddPassShader" = "Hidden/Universal Render Pipeline/Terrain/Lit（Add Pass）" 
        void GetAfterSubShaderString(ShaderStringBuilder sb);
    }
}