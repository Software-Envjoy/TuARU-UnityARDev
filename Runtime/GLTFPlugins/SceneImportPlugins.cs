﻿using GLTF.Schema;
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

        /// <summary>
        /// Gets the description of the plugin
        /// </summary>
        public override string Description => "Import Envjoy sceneNode data, " +
            "if not present, the sceneNode will be imported like a Envjoy model node";

        #endregion

        /// <inheritdoc/>
        public override GLTFImportPluginContext CreateInstance(GLTFImportContext context)
            => new EVJ_scene_context(context);

        /// <inheritdoc/>
        private class EVJ_scene_context : GLTFImportPluginContext
        {
            #region Fields

            private readonly GLTFImportContext _context;
            private EVJ_scene _extension;

            #endregion

            //private EVJ_scene extension;

            /// <summary>
            /// Initializes a new instance of the <see cref="EVJ_scene_context"/> class.
            /// </summary>
            /// <param name="context">The import context</param>
            public EVJ_scene_context(GLTFImportContext context) => _context = context;

            /// <inheritdoc/>
            public override void OnAfterImportRoot(GLTFRoot gltfRoot)
            {
                if (gltfRoot.Extensions == null)
                    return;

                if (gltfRoot.Extensions.TryGetValue(EVJ_scene.EXTENSION_NAME, out var value))
                    _extension = value as EVJ_scene;
            }

            /// <inheritdoc/>
            public override void OnAfterImportNode(Node node, int nodeIndex, GameObject nodeObject)
            {
                if (_extension == null)
                    return;

                var evjNode = _extension.RootNodes.FirstOrDefault(n => n.NodeIndex == nodeIndex);
                if (evjNode == null)
                    return;

                var modelNode = nodeObject.AddComponent<ModelNode>();
                var animationIndices = evjNode.AnimationIndices;
                modelNode.Clips = new AnimationClip[animationIndices.Length];
                for (int i = 0; i < animationIndices.Length; i++)
                {
                    var animationIndex = animationIndices[i];
                    modelNode.Clips[i] = _context.SceneImporter.AnimationCache[animationIndex].LoadedAnimationClip;
                }

                var path = evjNode.Path;
                modelNode.Path = new Path
                {
                    ClosePath = path.ClosePath,
                    ConvertCurves = path.ConvertCurves,
                    Duration = path.Duration,
                };

                var idleAnimationIndex = evjNode.Path.IdleAnimationIndex;
                if (idleAnimationIndex != -1)
                    modelNode.Path.IdleAnimation = _context.SceneImporter.AnimationCache[idleAnimationIndex].LoadedAnimationClip;

                modelNode.Path.Waypoints = new Waypoint[path.Waypoints.Length];
                for (int i = 0; i < path.Waypoints.Length; i++)
                {
                    var waypoint = path.Waypoints[i];
                    modelNode.Path.Waypoints[i] = new Waypoint
                    {
                        Position = waypoint.Position
                    };

                    if (waypoint.AnimationIndex != -1)
                        modelNode.Path.Waypoints[i].Animation = _context.SceneImporter.AnimationCache[waypoint.AnimationIndex].LoadedAnimationClip;
                }
            }

            /// <inheritdoc/>
            public override void OnAfterImportScene(GLTFScene scene, int sceneIndex, GameObject sceneObject)
            {
                if (_extension == null)
                    return;

                if (_extension.SceneIndex != sceneIndex && sceneIndex != -1)
                    return;

                var sceneNode = sceneObject.AddComponent<SceneNode>();
                if (_extension.ImageTarget != -1)
                    sceneNode.ImageTarget = _context.SceneImporter.TextureCache[_extension.ImageTarget].Texture;
            }
        }
    }
}
