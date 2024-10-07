using GLTF.Schema;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Scene importer for Envjoy GLTF
    /// </summary>
    public class EvjSceneImporter : GLTFSceneImporter
    {
        #region Fields

        private readonly EVJ_scene _extension;
        private int _currentAnimationIndex;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EvjSceneImporter"/> class.
        /// </summary>
        /// <param name="rootNode">The rootNode<see cref="GLTFRoot"/></param>
        /// <param name="gltfStream">The gltfStream<see cref="Stream"/></param>
        /// <param name="options">The options<see cref="ImportOptions"/></param>
        public EvjSceneImporter(GLTFRoot rootNode,
                                Stream gltfStream,
                                ImportOptions options)
            : base(rootNode, gltfStream, options)
        {
            LoadUnreferencedImagesAndMaterials = true;
            IsMultithreaded = true;
            options.AnimationMethod = AnimationMethod.None;

            if (rootNode.Extensions != null && rootNode.Extensions.TryGetValue(EVJ_scene.EXTENSION_NAME, out var value))
            {
                _extension = value as EVJ_scene;
                if (rootNode.Animations != null)
                {
                    _currentAnimationIndex = 0;
                    CreatedAnimationClips = new AnimationClip[rootNode.Animations?.Count ?? 0];
                }
            }
        }

        /// <summary>
        /// The ConstructNode
        /// </summary>
        /// <param name="node">The node<see cref="Node"/></param>
        /// <param name="nodeIndex">The nodeIndex<see cref="int"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task ConstructNode(Node node, int nodeIndex, CancellationToken cancellationToken)
        {
            await base.ConstructNode(node, nodeIndex, cancellationToken);
            if (_extension == null)
                return;

            var evjNode = _extension.RootNodes.FirstOrDefault(n => n.NodeIndex == nodeIndex);
            if (evjNode == null)
                return;

            var nodeObject = NodeCache[nodeIndex];
#if ANIMATION_SUPPORTED
            var animationIndices = evjNode.AnimationIndices;
            for (int i = 0; i < animationIndices.Length; i++)
            {
                var animationIndex = animationIndices[i];
                var clip = await ConstructClip(nodeObject.transform, animationIndex, cancellationToken);
                clip.wrapMode = UnityEngine.WrapMode.Loop;
                if (!TrySetCreatedAnimationClip(clip))
                    throw new Exception("Failed to set created animation clip");
            }
#endif
        }

        /// <summary>
        /// The ConstructScene
        /// </summary>
        /// <param name="scene">The scene<see cref="GLTFScene"/></param>
        /// <param name="showSceneObj">The showSceneObj<see cref="bool"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task ConstructScene(GLTFScene scene, bool showSceneObj, CancellationToken cancellationToken)
        {
            await base.ConstructScene(scene, showSceneObj, cancellationToken);
            if (_extension != null)
                return;

            var modelNode = CreatedObject.AddComponent<ModelNode>();
            if (Root.Animations == null)
                return;

            _currentAnimationIndex = 0;
            CreatedAnimationClips = new AnimationClip[Root.Animations?.Count ?? 0];
            for (int i = 0; i < Root.Animations.Count; i++)
            {
                var clip = await ConstructClip(modelNode.transform, i, cancellationToken);
                clip.wrapMode = UnityEngine.WrapMode.Loop;
                if (!TrySetCreatedAnimationClip(clip))
                    throw new Exception("Failed to set created animation clip");
            }

            modelNode.Clips = CreatedAnimationClips;

#if ANIMATION_SUPPORTED
            var animations = CreatedObject.AddComponent<InstantiatedGLTFAnimations>();
            animations.CacheData = new(Root.Animations, AnimationCache);
#endif
        }

        /// <summary>
        /// Try to set the created animation clip in the array
        /// </summary>
        /// <param name="clip">Clip to set</param>
        /// <returns>Returns true if the clip was set, otherwise false</returns>
        private bool TrySetCreatedAnimationClip(AnimationClip clip)
        {
            var index = _currentAnimationIndex++;
            if (index < 0 || index >= CreatedAnimationClips.Length)
                return false;

            CreatedAnimationClips[index] = clip;
            return true;
        }
    }
}
