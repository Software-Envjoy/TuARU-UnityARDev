#if UNITY_EDITOR
#define ANIMATION_EXPORT_SUPPORTED
#endif

#if ANIMATION_EXPORT_SUPPORTED && (UNITY_ANIMATION || !UNITY_2019_1_OR_NEWER)
#define ANIMATION_SUPPORTED
#endif

using GLTF.Schema;
using System;
using System.Linq;
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
                _extension.RootNodes = new int[rootNodes.Length];
                _extension.AnimationIndices = new int[rootNodes.Length][];
                for (int i = 0; i < rootNodes.Length; i++)
                {
                    var node = rootNodes[i];
                    _extension.RootNodes[i] = exporter.GetTransformIndex(node.transform);
#if ANIMATION_SUPPORTED
                    exporter.ExportAnimationClips(node.transform, node.Clips);
                    _extension.AnimationIndices[i] = node.Clips.Select(clip => exporter.GetAnimationId(clip, node.transform)).ToArray();
#else
                    _extension.AnimationIndices[i] = Array.Empty<int>();
#endif
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
