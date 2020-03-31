using System;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph.DY
{
    class DYSGPBRSettingView : DYSGMasterSettingsView
    {
        public DYSGPBRSettingView(IDYSGMasterNodeExtension extension) : base(extension)
        {
        }

        protected override void OnCreateUIElements()
        {
            var ps = new PropertySheet();
            ps.Add(new PropertyRow(new Label("Workflow")), (row) =>
            {
                row.Add(new EnumField(DYSGPBRMasterNode.Model.Metallic), (field) =>
                {
                    field.value = ((DYSGPBRMasterNode)extension.masterNode).model;
                    field.RegisterValueChangedCallback(ChangeWorkFlow);
                });
            });
            CreateOptionUI(ps);
            Add(ps);
        }
        
        void ChangeWorkFlow(ChangeEvent<Enum> evt)
        {
            if (Equals(((DYSGPBRMasterNode)extension.masterNode).model, evt.newValue))
                return;

            extension.masterNode.owner.owner.RegisterCompleteObjectUndo("Work Flow Change");
            ((DYSGPBRMasterNode)extension.masterNode).model = (DYSGPBRMasterNode.Model)evt.newValue;
        }
    }
}