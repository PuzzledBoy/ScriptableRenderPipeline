using System;

namespace UnityEditor.ShaderGraph.DY
{
    public static class PropertiesName
    {
        public const string prefix = "DYSG";

        public const string blendPreset = prefix + "_BlendPreset";
        public const string srcColor = prefix + "_SrcColor";
        public const string dstColor = prefix + "_DstColor";
        public const string srcAlpha = prefix + "_SrcAlpha";
        public const string dstAlpha = prefix + "_DstAlpha";

        public const string cull = prefix + "_Cull";
        public const string zWrite = prefix + "_ZWrite";
        public const string zTest = prefix + "_ZTest";

        public const string sRef = prefix + "_SRef";
        public const string sComp = prefix + "_SComp";
        public const string sPassOp = prefix + "_SPassOp";
        public const string sFailOp = prefix + "_SFailOp";
        public const string sZFailOp = prefix + "_SZFailOp";

        public const string castShadow = prefix + "_CastShadow";
        public const string receiveShadow = prefix + "_ReceiveShadow";
        
    }


    //TODO 有一些枚举的值和UnityEngine.Rendering中的不一样，需要研究一下哪边才是对的
    // 注意这里的巨坑，SurfaceMaterialOptions，或Shaderlab文档，或API文档中相关枚举的定义都不一样。到底哪个才是shader解析时正确读的值？
    // 目前来说，CompareFunction，StencilOp都可以确认是按照API文档中来的，见https://forum.unity.com/threads/stencil-op-comparison-values.362425/；
    // 但Cull，ZWrite，ZTest API文档没有定义，因此保持和SurfaceMaterialOptions中的定义相同；BlendMode两边都有而且数值不一样，暂时以API为准。待一个测试或确认
    public enum RenderType
    {
        Opaque,
        Transparent,
        TransparentCutout,
        Background,
        Overlay
    }

    public enum RenderQueue
    {
        Background,
        Geometry,
        AlphaTest,
        Transparent,
        Overlay
    }

    public enum BlendPreset
    {
        Additive,
        Multiply,
        PreMultiply,
        Alpha,
    }
    
    public enum BlendMode
    {
        Zero,
        One,
        DstColor,
        SrcColor,
        OneMinusDstColor,
        SrcAlpha,
        OneMinusSrcColor,
        DstAlpha,
        OneMinusDstAlpha,
        SrcAlphaSaturate,
        OneMinusSrcAlpha
    }

    public enum CullMode
    {
        Off,
        Front,
        Back
    }

    public enum ZTest
    {
        [Obsolete] Disabled, // 等于Always
        Never,
        Less,
        Equal,
        LessEqual,
        Greater,
        NotEqual,
        GreaterEqual,
        Always
    }

    public enum ZWrite
    {
        Off,
        On
    }

    public enum StencilComp
    {
        [Obsolete] Disabled, // 等于Always
        Never,
        Less,
        Equal,
        LessEqual,
        Greater,
        NotEqual,
        GreaterEqual,
        Always
    }

    public enum StencilOp
    {
        Keep,
        Zero,
        Replace,
        IncrSat,
        DecrSat,
        Invert,
        IncrWrap,
        DecrWrap
    }
}