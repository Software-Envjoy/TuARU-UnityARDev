using GLTF.Schema;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGLTF.Plugins;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Scene import plugin, used to import the Envjoy scene
    /// </summary>
    public class SceneImportPlugins : GLTFImportPlugin
    {
        #region Properties

        /// <summary>
        /// Gets the display name of the plugin
        /// </summary>
        public override string DisplayName => "EVJ_scene";

        #endregion

        /// <inheritdoc/>
        public override GLTFImportPluginContext CreateInstance(GLTFImportContext context)
            => new EVJ_scene_context(context);

        /// <inheritdoc/>
        private class EVJ_scene_context : GLTFImportPluginContext
        {
            #region Fields

            private readonly GLTFImportContext _context;
            private EVJ_scene extension;

            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="EVJ_scene_context"/> class.
            /// </summary>
            /// <param name="context"> The import context </param>
            public EVJ_scene_context(GLTFImportContext context) => _context = context;

            /// <inheritdoc/>
            public override void OnAfterImportRoot(GLTFRoot gltfRoot)
            {
                if (gltfRoot.Extensions == null)
                    return;

                if (gltfRoot.Extensions.TryGetValue(EVJ_scene.EXTENSION_NAME, out var value))
                    extension = value as EVJ_scene;
            }

            /// <inheritdoc/>
            public override void OnAfterImportScene(GLTFScene scene, int sceneIndex, GameObject sceneObject)
            {
                if (extension == null)
                    return;

                if (extension.SceneIndex == sceneIndex || sceneIndex == -1)
                {
                    var sceneNode = sceneObject.AddComponent<SceneNode>();
                    var clips = new List<AnimationClip>();
                    for (int i = 0; i < extension.RootNodes.Length; i++)
                    {
                        var nodeIndex = extension.RootNodes[i];
                        var nodeObject = _context.SceneImporter.NodeCache[nodeIndex];
                        var modelNode = nodeObject.AddComponent<ModelNode>();
                        modelNode.Clips = extension.AnimationIndices[i]
                            .Select(index => _context.SceneImporter.ConstructClip2(nodeObject.transform, index))
                            .ToArray();

                        clips.AddRange(modelNode.Clips);
                    }

                    _context.SceneImporter.SetCreatedAnimationClips(clips.ToArray());
                }
            }
        }
    }
}