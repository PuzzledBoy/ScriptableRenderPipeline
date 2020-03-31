using System;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph.DY
{
    class DYSGMasterSettingsView : VisualElement
    {
        protected IDYSGMasterNodeExtension extension;

        public DYSGMasterSettingsView(IDYSGMasterNodeExtension extension)
        {
            this.extension = extension;
            OnCreateUIElements();
        }

        protected virtual void OnCreateUIElements()
        {
            var ps = new PropertySheet();
            CreateOptionUI(ps);
            Add(ps);
        }

        protected void CreateOptionUI(VisualElement ps)
        {
            ps.Add(new PropertyRow(new Label("Surface Preset")), row =>
            {
                row.Add(new EnumField(DY.SurfacePreset.Opaque), field =>
                {
                    field.value = this.extension.surfacePreset;
                    field.RegisterValueChangedCallback(ChangeSurfacePreset);
                });
            });


            //Base
            ps.Add(new PropertyRow(new Label("Render Type")), (row) =>
            {
                row.Add(new EnumField(DY.RenderType.Background), (field) =>
                {
                    field.value = this.extension.renderType;
                    field.RegisterValueChangedCallback(ChangeRenderType);
                });
            });
            ps.Add(new PropertyRow(new Label("Render Queue")), (row) =>
            {
                row.Add(new EnumField(DY.RenderQueue.Background), (field) =>
                {
                    field.value = this.extension.renderQueue;
                    field.RegisterValueChangedCallback(ChangeRenderQueue);
                });
            });
            ps.Add(new PropertyRow(new Label("Cull Mode")), (row) =>
            {
                row.Add(new EnumField(DY.CullMode.Back), (field) =>
                {
                    field.value = this.extension.cull;
                    field.RegisterValueChangedCallback(ChangeCullMode);
                });
            });

            // Blend
            PropertySheet blendPs = new PropertySheet(new Label("Blend"));

            blendPs.Add(new PropertyRow(new Label("SrcColor")), (row) =>
            {
                row.Add(new EnumField(DY.BlendMode.One), (field) =>
                {
                    field.value = this.extension.srcColor;
                    field.RegisterValueChangedCallback(ChangeSrcColor);
                });
            });

            blendPs.Add(new PropertyRow(new Label("DstColor")), (row) =>
            {
                row.Add(new EnumField(DY.BlendMode.One), (field) =>
                {
                    field.value = this.extension.dstColor;
                    field.RegisterValueChangedCallback(ChangeDstColor);
                });
            });
            blendPs.Add(new PropertyRow(new Label("SrcAlpha")), (row) =>
            {
                row.Add(new EnumField(DY.BlendMode.One), (field) =>
                {
                    field.value = this.extension.srcAlpha;
                    field.RegisterValueChangedCallback(ChangeSrcAlpha);
                });
            });
            blendPs.Add(new PropertyRow(new Label("DstAlpha")), (row) =>
            {
                row.Add(new EnumField(DY.BlendMode.One), (field) =>
                {
                    field.value = this.extension.dstAlpha;
                    field.RegisterValueChangedCallback(ChangeDstAlpha);
                });
            });

            ps.Add(blendPs);

            //Depth
            var depthPs = new PropertySheet(new Label("Depth"));

            depthPs.Add(new PropertyRow(new Label("ZWrite")), row =>
            {
                row.Add(new EnumField(DY.ZWrite.Off), field =>
                {
                    field.value = this.extension.zWrite;
                    field.RegisterValueChangedCallback(ChangeZWrite);
                });
            });
            depthPs.Add(new PropertyRow(new Label("ZTest")), row =>
            {
                row.Add(new EnumField(DY.ZTest.Always), field =>
                {
                    field.value = this.extension.zTest;
                    field.RegisterValueChangedCallback(ChangeZTest);
                });
            });
            ps.Add(depthPs);

            //Stencil
            var sPs = new PropertySheet(new Label("Stencil"));
            sPs.Add(new PropertyRow(new Label("Ref")), row =>
            {
                row.Add(new IntegerField(), field =>
                {
                    field.value = this.extension.sRef;
                    field.RegisterValueChangedCallback(ChangeSRef);
                });
            });
            sPs.Add(new PropertyRow(new Label("Comp")), row =>
            {
                row.Add(new EnumField(DY.StencilComp.Always), field =>
                {
                    field.value = this.extension.sComp;
                    field.RegisterValueChangedCallback(ChangeSComp);
                });
            });
            sPs.Add(new PropertyRow(new Label("Pass Op")), row =>
            {
                row.Add(new EnumField(DY.StencilOp.Invert), field =>
                {
                    field.value = this.extension.sPassOp;
                    field.RegisterValueChangedCallback(ChangeSPassOp);
                });
            });
            sPs.Add(new PropertyRow(new Label("Fail Op")), row =>
            {
                row.Add(new EnumField(DY.StencilOp.Invert), field =>
                {
                    field.value = this.extension.sFailOp;
                    field.RegisterValueChangedCallback(ChangeSFailOp);
                });
            });
            sPs.Add(new PropertyRow(new Label("ZFail Op")), row =>
            {
                row.Add(new EnumField(DY.StencilOp.Invert), field =>
                {
                    field.value = this.extension.sZFailOp;
                    field.RegisterValueChangedCallback(ChangeSZFailOp);
                });
            });

            ps.Add(sPs);

            //Editor
            //TODO 空行
            ps.Add(new PropertyRow(new Label("Custom Editor")), (row) =>
            {
                row.Add(new TextField(), (field) =>
                {
                    field.value = this.extension.customEditor;
                    field.RegisterValueChangedCallback(ChangeCustomEditor);
                });
            });
        }

        #region ChangeEvents

        void ChangeSurfacePreset(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.surfacePreset, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Surface Preset Change");

            extension.surfacePreset = (DY.SurfacePreset) evt.newValue;
            //Apply
            SurfacePresetUtility.Apply((DY.SurfacePreset) evt.newValue, extension);
//            Add(ps);
//            MarkDirtyRepaint();
            //TODO Refresh , 设置之后并没有自动刷新，要关掉面板重开之后才行
        }

        void ChangeRenderType(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.renderType, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Render Type Change");
            extension.renderType = (DY.RenderType) evt.newValue;
        }

        void ChangeRenderQueue(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.renderQueue, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Render Queue Change");
            extension.renderQueue = (DY.RenderQueue) evt.newValue;
        }

        void ChangeCullMode(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.cull, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Cull Mode Change");
            extension.cull = (DY.CullMode) evt.newValue;
        }

        void ChangeSrcColor(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.srcColor, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Src Color Change");
            extension.srcColor = (DY.BlendMode) evt.newValue;
        }

        void ChangeDstColor(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.dstColor, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Dst Color Change");
            extension.dstColor = (DY.BlendMode) evt.newValue;
        }

        void ChangeSrcAlpha(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.srcAlpha, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Src Alpha Change");
            extension.srcAlpha = (DY.BlendMode) evt.newValue;
        }

        void ChangeDstAlpha(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.dstAlpha, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Dst Alpha Change");
            extension.dstAlpha = (DY.BlendMode) evt.newValue;
        }

        void ChangeZWrite(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.zWrite, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Z Write Change");
            extension.zWrite = (DY.ZWrite) evt.newValue;
        }

        void ChangeZTest(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.zTest, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Z Test Change");
            extension.zTest = (DY.ZTest) evt.newValue;
        }

        void ChangeSRef(ChangeEvent<int> evt)
        {
            if (Equals(extension.sRef, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Stencil Ref Change");
            extension.sRef = (int) evt.newValue;
        }

        void ChangeSComp(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.sComp, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Stencil Comp Change");
            extension.sComp = (DY.StencilComp) evt.newValue;
        }

        void ChangeSPassOp(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.sPassOp, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Stencil Pass Op Change");
            extension.sPassOp = (DY.StencilOp) evt.newValue;
        }

        void ChangeSFailOp(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.sFailOp, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Stencil Fail Op Change");
            extension.sFailOp = (DY.StencilOp) evt.newValue;
        }

        void ChangeSZFailOp(ChangeEvent<Enum> evt)
        {
            if (Equals(extension.sZFailOp, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Stencil Z Fail Op Change");
            extension.sZFailOp = (DY.StencilOp) evt.newValue;
        }

        void ChangeCustomEditor(ChangeEvent<string> evt)
        {
            if (Equals(extension.customEditor, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Custom Editor Change");
            extension.customEditor = (string) evt.newValue;
        }

        #endregion
    }
}