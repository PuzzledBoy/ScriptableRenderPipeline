using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.Graphing;
using UnityEngine;

namespace UnityEditor.ShaderGraph.DY
{
    public class UpgradeProcessor : MonoBehaviour
    {
        private enum Scope
        {
            Select,
            Assets,
            Packages,
            WholeProject
        }

        private class UpgradeParams
        {
            public readonly string assetPath;
            public readonly GraphObject graphObj;

            public UpgradeParams(string assetPath, GraphObject graphObj)
            {
                this.assetPath = assetPath;
                this.graphObj = graphObj;
            }
        }
        
        delegate bool UpgradeCallBack(UpgradeParams param);

        static void Upgrade(Scope scope, UpgradeCallBack callBack)
        {
            Debug.Log($"Upgrade Start! Scope : {scope}");
            string[] matchAssetPaths;
            if (scope == Scope.Select)
                matchAssetPaths = Selection.objects
                    .Where(obj => obj != null && obj is Shader && Path.GetExtension(AssetDatabase.GetAssetPath(obj)) == ".shadergraph")
                    .Select(AssetDatabase.GetAssetPath)
                    .ToArray();
            else
            {
                string searchFilter = "";
                if (scope == Scope.Assets)
                {
                    searchFilter = "Assets";
                }
                else if (scope == Scope.Packages)
                {
                    searchFilter = "Packages";
                }
                else if (scope == Scope.WholeProject)
                {
                    searchFilter = "";
                }

                matchAssetPaths = AssetDatabase.FindAssets("t:shader", new[] {searchFilter})
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(path => Path.GetExtension(path) == ".shadergraph")
                    .ToArray();
            }

            if (matchAssetPaths.Length <= 0) Debug.Log("No Shader Graph to upgrade.");

            //TODO Show ProcessBar
            for (int i = 0; i < matchAssetPaths.Length; ++i)
            {
                var assetPath = matchAssetPaths[i];

                var textGraph = File.ReadAllText(assetPath, Encoding.UTF8);
                var graphObj = ScriptableObject.CreateInstance<GraphObject>();

                graphObj.hideFlags = HideFlags.HideAndDontSave;
                graphObj.graph = JsonUtility.FromJson<GraphData>(textGraph);
                graphObj.graph.OnEnable();
                graphObj.graph.ValidateGraph();

                if (callBack(new UpgradeParams(assetPath,graphObj)))
                {
                    Debug.Log($"Upgrade Succeed : {assetPath}");

                    // Save To File
                    if (FileUtilities.WriteShaderGraphToDisk(assetPath, graphObj.graph))
                        AssetDatabase.ImportAsset(assetPath);

                    // TODO Refresh Window if need.
//            foreach (var w in Resources.FindObjectsOfTypeAll<MaterialGraphEditWindow>())
//            {
//                if (w.selectedGuid == guid)
//                {
//                    w.Close();
//                }
//            }
                }

                DestroyImmediate(graphObj);
            }
        }

        private static bool OnUpgradeUnlit(UpgradeParams param)
        {
            if (!(param.graphObj.graph.outputNode is UnlitMasterNode)) return false;

            var masterNode = (UnlitMasterNode) param.graphObj.graph.outputNode;

            // New Master Node
            var newMasterNode = new DYSGUnlitMasterNode();
            param.graphObj.graph.AddNode(newMasterNode);

            // Transfer DrawState,groupId
            newMasterNode.drawState = masterNode.drawState;
            newMasterNode.groupGuid = masterNode.groupGuid;

            // Transfer Edges
            var inputSlots = masterNode.GetInputSlots<MaterialSlot>().ToArray();
            foreach (var inputSlot in inputSlots)
            {
                var slotId = inputSlot.id;
                SlotReference slotRef = masterNode.GetSlotReference(slotId);
                SlotReference newSlotRef;
                try
                {
                    newSlotRef = newMasterNode.GetSlotReference(slotId); // Throw Exception if no slot.
                }
                catch (Exception e)
                {
                    // 没有对应的Slot，则不替换连接
                    continue;
                }

                var edges = param.graphObj.graph.GetEdges(slotRef).ToArray();
                foreach (var e in edges)
                {
                    param.graphObj.graph.Connect(e.outputSlot, newSlotRef);
                }
            }

            // Transfer Properties
            UpgradeUnlitSettings(masterNode, newMasterNode);

            // Replace Master Node
            param.graphObj.graph.activeOutputNodeGuid = newMasterNode.guid;
            param.graphObj.graph.RemoveNode(masterNode);

            return true;
        }

        // Blend Mode 参考 SetRenderState 函数
        private static void UpgradeUnlitSettings(UnlitMasterNode masterNode, DYSGUnlitMasterNode newMasterNode)
        {
            // Precision
            newMasterNode.precision = masterNode.precision;

            // Surface
            if (masterNode.surfaceType == SurfaceType.Opaque)
            {
                newMasterNode.renderType = RenderType.Opaque;
                newMasterNode.renderQueue = RenderQueue.Geometry;
                newMasterNode.zTest = ZTest.LessEqual;
                newMasterNode.zWrite = ZWrite.On;
                newMasterNode.srcColor = DY.BlendMode.One;
                newMasterNode.dstColor = DY.BlendMode.Zero;
                newMasterNode.srcAlpha = DY.BlendMode.One;
                newMasterNode.dstAlpha = DY.BlendMode.Zero;
            }
            else if (masterNode.surfaceType == SurfaceType.Transparent)
            {
                newMasterNode.renderType = RenderType.Transparent;
                newMasterNode.renderQueue = RenderQueue.Transparent;
                newMasterNode.zTest = ZTest.LessEqual;
                newMasterNode.zWrite = ZWrite.Off;
                switch (masterNode.alphaMode)
                {
                    case AlphaMode.Alpha:
                        newMasterNode.srcColor = BlendMode.SrcAlpha;
                        newMasterNode.dstColor = BlendMode.OneMinusSrcAlpha;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.OneMinusSrcAlpha;
                        break;
                    case AlphaMode.Premultiply:
                        newMasterNode.srcColor = BlendMode.One;
                        newMasterNode.dstColor = BlendMode.OneMinusSrcAlpha;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.OneMinusSrcAlpha;
                        break;
                    case AlphaMode.Additive:
                        newMasterNode.srcColor = BlendMode.One;
                        newMasterNode.dstColor = BlendMode.One;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.One;
                        break;
                    case AlphaMode.Multiply:
                        newMasterNode.srcColor = BlendMode.DstColor;
                        newMasterNode.dstColor = BlendMode.Zero;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.Zero;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // Cull
            if (masterNode.twoSided.isOn) newMasterNode.cull = CullMode.Off;
            else if (!masterNode.twoSided.isOn) newMasterNode.cull = CullMode.Back;
        }

        private static bool OnUpgradePBR(UpgradeParams param)
        {
            if (!(param.graphObj.graph.outputNode is PBRMasterNode)) return false;

            var masterNode = (PBRMasterNode) param.graphObj.graph.outputNode;

            // New Master Node
            var newMasterNode = new DYSGPBRMasterNode();
            param.graphObj.graph.AddNode(newMasterNode);

            // Transfer DrawState,groupId
            newMasterNode.drawState = masterNode.drawState;
            newMasterNode.groupGuid = masterNode.groupGuid;

            // Transfer Edges
            var inputSlots = masterNode.GetInputSlots<MaterialSlot>().ToArray();
            foreach (var inputSlot in inputSlots)
            {
                var slotId = inputSlot.id;
                SlotReference slotRef = masterNode.GetSlotReference(slotId);
                SlotReference newSlotRef;
                try
                {
                    newSlotRef = newMasterNode.GetSlotReference(slotId); // Throw Exception if no slot.
                }
                catch (Exception e)
                {
                    // 没有对应的Slot，则不替换连接
                    continue;
                }

                var edges = param.graphObj.graph.GetEdges(slotRef).ToArray();
                foreach (var e in edges)
                {
                    param.graphObj.graph.Connect(e.outputSlot, newSlotRef);
                }
            }

            // Transfer Properties
            UpgradePBRSettings(masterNode, newMasterNode);

            // Replace Master Node
            param.graphObj.graph.activeOutputNodeGuid = newMasterNode.guid;
            param.graphObj.graph.RemoveNode(masterNode);

            return true;
        }

        private static void UpgradePBRSettings(PBRMasterNode masterNode, DYSGPBRMasterNode newMasterNode)
        {
            // Precision
            newMasterNode.precision = masterNode.precision;

            // PBR : WorkFlow
            newMasterNode.model = (DYSGPBRMasterNode.Model) Enum.Parse(typeof(DYSGPBRMasterNode.Model), masterNode.model.ToString());

            // Surface
            if (masterNode.surfaceType == SurfaceType.Opaque)
            {
                newMasterNode.renderType = RenderType.Opaque;
                newMasterNode.renderQueue = RenderQueue.Geometry;
                newMasterNode.zTest = ZTest.LessEqual;
                newMasterNode.zWrite = ZWrite.On;
                newMasterNode.srcColor = DY.BlendMode.One;
                newMasterNode.dstColor = DY.BlendMode.Zero;
                newMasterNode.srcAlpha = DY.BlendMode.One;
                newMasterNode.dstAlpha = DY.BlendMode.Zero;
            }
            else if (masterNode.surfaceType == SurfaceType.Transparent)
            {
                newMasterNode.renderType = RenderType.Transparent;
                newMasterNode.renderQueue = RenderQueue.Transparent;
                newMasterNode.zTest = ZTest.LessEqual;
                newMasterNode.zWrite = ZWrite.Off;
                switch (masterNode.alphaMode)
                {
                    case AlphaMode.Alpha:
                        newMasterNode.srcColor = BlendMode.SrcAlpha;
                        newMasterNode.dstColor = BlendMode.OneMinusSrcAlpha;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.OneMinusSrcAlpha;
                        break;
                    case AlphaMode.Premultiply:
                        newMasterNode.srcColor = BlendMode.One;
                        newMasterNode.dstColor = BlendMode.OneMinusSrcAlpha;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.OneMinusSrcAlpha;
                        break;
                    case AlphaMode.Additive:
                        newMasterNode.srcColor = BlendMode.One;
                        newMasterNode.dstColor = BlendMode.One;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.One;
                        break;
                    case AlphaMode.Multiply:
                        newMasterNode.srcColor = BlendMode.DstColor;
                        newMasterNode.dstColor = BlendMode.Zero;
                        newMasterNode.srcAlpha = BlendMode.One;
                        newMasterNode.dstAlpha = BlendMode.Zero;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // Cull
            if (masterNode.twoSided.isOn) newMasterNode.cull = CullMode.Off;
            else if (!masterNode.twoSided.isOn) newMasterNode.cull = CullMode.Back;
        }

        [MenuItem("DyTools/DYShaderGraph/Upgrade Unlit Master Selection")]
        public static void UpgradeUnlitMasterSelection()
        {
            Upgrade(Scope.Select, OnUpgradeUnlit);
        }

        [MenuItem("DyTools/DYShaderGraph/Upgrade Unlit Master AssetFolder")]
        public static void UpgradeUnlitMasterAssetFolder()
        {
            Upgrade(Scope.Assets, OnUpgradeUnlit);
        }

        [MenuItem("DyTools/DYShaderGraph/Upgrade PBR Master Selection")]
        public static void UpgradePBRMasterSelection()
        {
            Upgrade(Scope.Select, OnUpgradePBR);
        }

        [MenuItem("DyTools/DYShaderGraph/Upgrade PBR Master AssetFolder")]
        public static void UpgradePBRMasterAssetFolder()
        {
            Upgrade(Scope.Assets, OnUpgradePBR);
        }
    }
}