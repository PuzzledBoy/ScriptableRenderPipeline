using System.Collections.Generic;

namespace UnityEditor.ShaderGraph.DY
{
    public enum SurfacePreset
    {
        Opaque,
        Transparent,
        Custom
    }

    public class SurfacePresetValue
    {
        public readonly RenderType renderType;
        public readonly RenderQueue renderQueue;
        public readonly CullMode cull;

        public readonly BlendMode srcColor;
        public readonly BlendMode dstColor;
        public readonly BlendMode srcAlpha;
        public readonly BlendMode dstAlpha;

        public readonly ZWrite zWrite;
        public readonly ZTest zTest;

        public readonly int sRef;
        public readonly StencilComp sComp;
        public readonly StencilOp sPassOp;
        public readonly StencilOp sFailOp;
        public readonly StencilOp sZFailOp;

        public SurfacePresetValue(RenderType renderType, RenderQueue renderQueue, CullMode cull, BlendMode srcColor, BlendMode dstColor, BlendMode srcAlpha, BlendMode dstAlpha, ZWrite zWrite, ZTest zTest, int sRef, StencilComp sComp, StencilOp sPassOp, StencilOp sFailOp, StencilOp sZFailOp)
        {
            this.renderType = renderType;
            this.renderQueue = renderQueue;
            this.cull = cull;
            this.srcColor = srcColor;
            this.dstColor = dstColor;
            this.srcAlpha = srcAlpha;
            this.dstAlpha = dstAlpha;
            this.zWrite = zWrite;
            this.zTest = zTest;
            this.sRef = sRef;
            this.sComp = sComp;
            this.sPassOp = sPassOp;
            this.sFailOp = sFailOp;
            this.sZFailOp = sZFailOp;
        }


        internal void Apply(IDYSGMasterNodeExtension extension)
        {
            extension.renderType = renderType;
            extension.renderQueue = renderQueue;
            extension.cull = cull;
            extension.srcColor = srcColor;
            extension.dstColor = dstColor;
            extension.srcAlpha = srcAlpha;
            extension.dstAlpha = dstAlpha;
            extension.zWrite = zWrite;
            extension.zTest = zTest;
            extension.sRef = sRef;
            extension.sComp = sComp;
            extension.sPassOp = sPassOp;
            extension.sFailOp = sFailOp;
            extension.sZFailOp = sZFailOp;
        }
    }

    public static class SurfacePresetUtility
    {
        public static readonly Dictionary<SurfacePreset, SurfacePresetValue> presets = new Dictionary<SurfacePreset, SurfacePresetValue>()
        {
            {
                SurfacePreset.Opaque,
                new SurfacePresetValue(
                    RenderType.Opaque, RenderQueue.Geometry, CullMode.Back,
                    BlendMode.One, BlendMode.Zero, BlendMode.One, BlendMode.Zero,
                    ZWrite.On, ZTest.LessEqual,
                    0, StencilComp.Always, StencilOp.Keep, StencilOp.Keep, StencilOp.Keep)
            },
            {
                SurfacePreset.Transparent,
                new SurfacePresetValue(
                    RenderType.Transparent, RenderQueue.Transparent, CullMode.Back,
                    BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha,
                    ZWrite.Off, ZTest.LessEqual,
                    0, StencilComp.Always, StencilOp.Keep, StencilOp.Keep, StencilOp.Keep)
            },
            {
                SurfacePreset.Custom,
                null
            }
        };

        internal static void Apply(SurfacePreset preset, IDYSGMasterNodeExtension extension)
        {
            presets[preset]?.Apply(extension);
        }

//
//        public static SurfacePreset GetPresetByRenderType(RenderType renderType)
//        {
//            if (renderType == opaque.renderType) return SurfacePreset.Opaque;
//            if (renderType == transparent.renderType) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetByRenderQueue(RenderQueue renderQueue)
//        {
//            if (renderQueue == opaque.renderQueue) return SurfacePreset.Opaque;
//            if (renderQueue == transparent.renderQueue) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetBySrcColor(BlendMode srcColor)
//        {
//            if (srcColor == opaque.srcColor) return SurfacePreset.Opaque;
//            if (srcColor == transparent.srcColor) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetByDstColor(BlendMode dstColor)
//        {
//            if (dstColor == opaque.dstColor) return SurfacePreset.Opaque;
//            if (dstColor == transparent.dstColor) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetBySrcAlpha(BlendMode srcAlpha)
//        {
//            if (srcAlpha == opaque.srcAlpha) return SurfacePreset.Opaque;
//            if (srcAlpha == transparent.srcAlpha) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetByDstAlpha(BlendMode dstAlpha)
//        {
//            if (dstAlpha == opaque.dstAlpha) return SurfacePreset.Opaque;
//            if (dstAlpha == transparent.dstAlpha) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetByCull(CullMode cull)
//        {
//            if (cull == opaque.cull) return SurfacePreset.Opaque;
//            if (cull == transparent.cull) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetByZWrite(ZWrite zWrite)
//        {
//            if (zWrite == opaque.zWrite) return SurfacePreset.Opaque;
//            if (zWrite == transparent.zWrite) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetByZTest(ZTest zTest)
//        {
//            if (zTest == opaque.zTest) return SurfacePreset.Opaque;
//            if (zTest == transparent.zTest) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetBySRef(int sRef)
//        {
//            if (sRef == opaque.sRef) return SurfacePreset.Opaque;
//            if (sRef == transparent.sRef) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetBySComp(CompareFunction sComp)
//        {
//            if (sComp == opaque.sComp) return SurfacePreset.Opaque;
//            if (sComp == transparent.sComp) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetBySPassOp(StencilOp sPassOp)
//        {
//            if (sPassOp == opaque.sPassOp) return SurfacePreset.Opaque;
//            if (sPassOp == transparent.sPassOp) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetBySFailOp(StencilOp sFailOp)
//        {
//            if (sFailOp == opaque.sFailOp) return SurfacePreset.Opaque;
//            if (sFailOp == transparent.sFailOp) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
//
//        public static SurfacePreset GetPresetBySZFailOp(StencilOp sZFailOp)
//        {
//            if (sZFailOp == opaque.sZFailOp) return SurfacePreset.Opaque;
//            if (sZFailOp == transparent.sZFailOp) return SurfacePreset.Transparent;
//            return SurfacePreset.Custom;
//        }
    }
}