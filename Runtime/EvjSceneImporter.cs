using GLTF.Schema;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityGLTF;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="EvjSceneImporter" />
    /// </summary>
    public class EvjSceneImporter : GLTFSceneImporter
    {
        #region Fields

        private readonly EVJ_scene _extension;

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
            options.AnimationMethod = AnimationMethod.None;

            if (rootNode.Extensions != null && rootNode.Extensions.TryGetValue(EVJ_scene.EXTENSION_NAME, out var value))
            {
                _extension = value as EVJ_scene;
                if (rootNode.Animations != null)
                    InitializeCreatedAnimationClips();
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
            var animationIndices = evjNode.AnimationIndices;
            for (int i = 0; i < animationIndices.Length; i++)
            {
                var animationIndex = animationIndices[i];
                var clip = await ConstructClip(nodeObject.transform, animationIndex, cancellationToken);
                clip.wrapMode = UnityEngine.WrapMode.Loop;
                if (!TrySetCreatedAnimationClip(clip))
                    throw new Exception("Failed to set created animation clip");
            }
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

            InitializeCreatedAnimationClips();
            for (int i = 0; i < Root.Animations.Count; i++)
            {
                var clip = await ConstructClip(modelNode.transform, i, cancellationToken);
                clip.wrapMode = UnityEngine.WrapMode.Loop;
                if (!TrySetCreatedAnimationClip(clip))
                    throw new Exception("Failed to set created animation clip");
            }

            modelNode.Clips = CreatedAnimationClips;
        }
    }
}
