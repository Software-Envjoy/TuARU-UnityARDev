#if UNITY_EDITOR
#define ANIMATION_EXPORT_SUPPORTED
#endif

#if ANIMATION_EXPORT_SUPPORTED && (UNITY_ANIMATION || !UNITY_2019_1_OR_NEWER)
#define ANIMATION_SUPPORTED
#endif

using GLTF.Schema;
using System;
using UnityGLTF;
using UnityGLTF.Plugins;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Scene export plugin, used to export the Envjoy scene
    /// </summary>
    public class SceneExportPlugin : GLTFExportPlugin
    {
        #region Properties

        /// <summary>
        /// Gets the display name of the plugin
        /// </summary>
        public override string DisplayName => "EVJ_scene";

        /// <summary>
        /// Gets the description of the plugin
        /// </summary>
        public override string Description => "Export Envjoy scene data, if available";

        #endregion

        /// <inheritdoc/>
        public override GLTFExportPluginContext CreateInstance(ExportContext context)
            => new EVJ_scene_context();

        /// <inheritdoc/>
        private class EVJ_scene_context : GLTFExportPluginContext
        {
            #region Fields

            private EVJ_scene _extension;

            #endregion

            /// <inheritdoc/>
            public override void AfterSceneExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot)
            {
                if (exporter.RootTransforms == null)
                    return;

                if (exporter.RootTransforms.Count == 0)
                    return;

                var root = exporter.RootTransforms[0].parent;
                if (root == null || !root.TryGetComponent(out SceneNode sceneNode))
                    return;

                var rootNodes = root.GetComponentsInChildren<ModelNode>();

                _extension.SceneIndex = gltfRoot.Scene.Id;
                _extension.RootNodes = new EVJ_node[rootNodes.Length];
                for (int i = 0; i < rootNodes.Length; i++)
                {
                    var modelNode = rootNodes[i];
                    var node = new EVJ_node
                    {
                        NodeIndex = exporter.GetTransformIndex(modelNode.transform),
                    };

#if ANIMATION_SUPPORTED
                    node.Path = new EVJ_path
                    {
                        ClosePath = modelNode.Path.ClosePath,
                        ConvertCurves = modelNode.Path.ConvertCurves,
                        Duration = modelNode.Path.Duration,
                    };

                    exporter.ExportAnimationClips(modelNode.transform, modelNode.Clips);

                    node.AnimationIndices = new int[modelNode.Clips.Length];
                    for (int j = 0; j < modelNode.Clips.Length; j++)
                        node.AnimationIndices[j] = exporter.GetAnimationId(modelNode.Clips[j], modelNode.transform);

                    node.Path.IdleAnimationIndex = exporter.GetAnimationId(modelNode.Path.IdleAnimation, modelNode.transform);
                    node.Path.Waypoints = new EVJ_waypoint[modelNode.Path.Waypoints.Length];
                    for (int j = 0; j < modelNode.Path.Waypoints.Length; j++)
                    {
                        var waypoint = modelNode.Path.Waypoints[j];
                        node.Path.Waypoints[j] = new EVJ_waypoint
                        {
                            Position = waypoint.Position,
                            AnimationIndex = exporter.GetAnimationId(waypoint.Animation, modelNode.transform)
                        };
                    }
#endif
                    _extension.RootNodes[i] = node;
                }

                exporter.DeclareExtensionUsage(EVJ_scene.EXTENSION_NAME, true);
                gltfRoot.AddExtension(EVJ_scene.EXTENSION_NAME, _extension);
                gltfRoot.Asset.Copyright = $"Envjoy {DateTime.Now.Year}";
            }

            /// <inheritdoc/>
            public override void BeforeSceneExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot)
            {
                if (exporter.RootTransforms == null)
                    return;

                if (exporter.RootTransforms.Count == 0)
                    return;

                var root = exporter.RootTransforms[0].parent;
                if (root == null || !root.TryGetComponent(out SceneNode sceneNode))
                    return;

                _extension = new EVJ_scene();

                if (sceneNode.ImageTarget != null)
                {
                    var textureId = exporter.ExportTexture(sceneNode.ImageTarget, default);
                    _extension.ImageTarget = textureId.Id;
                }
            }
        }
    }
}
