﻿using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="EVJ_node" />
    /// </summary>
    [SerializeField]
    public class EVJ_node
    {
        #region Properties

        /// <summary>
        /// Gets or sets the NodeIndex
        /// </summary>
        public int NodeIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the AnimationIndices
        /// </summary>
        public int[] AnimationIndices { get; set; }

        /// <summary>
        /// Gets or sets the Path
        /// </summary>
        public EVJ_path Path { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="EVJ_path" />
    /// </summary>
    [SerializeField]
    public class EVJ_path
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether ClosePath
        /// </summary>
        public bool ClosePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ConvertCurves
        /// </summary>
        public bool ConvertCurves { get; set; }

        /// <summary>
        /// Gets or sets the Duration
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets the IdleAnimationIndex
        /// </summary>
        public int IdleAnimationIndex { get; set; }

        /// <summary>
        /// Gets or sets the Waypoints
        /// </summary>
        public EVJ_waypoint[] Waypoints { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="EVJ_waypoint" />
    /// </summary>
    [SerializeField]
    public class EVJ_waypoint
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Position
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the AnimationIndex
        /// </summary>
        public int AnimationIndex { get; set; }

        #endregion
    }

    /// <summary>
    /// Extension for the Envjoy scene data, used to store all the necessary information
    /// for Envjoy scenes functionality
    /// </summary>
    [Serializable]
    public class EVJ_scene : IExtension
    {
        #region Constants

        public const string EXTENSION_NAME = nameof(EVJ_scene);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SceneIndex
        /// </summary>
        public int SceneIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the ImageTarget
        /// </summary>
        public int ImageTarget { get; set; } = -1;

        /// <summary>
        /// Gets or sets the RootNodes
        /// </summary>
        public EVJ_node[] RootNodes { get; set; }

        #endregion

        /// <inheritdoc/>
        public IExtension Clone(GLTFRoot root) => new EVJ_scene
        {
            SceneIndex = SceneIndex,
            ImageTarget = ImageTarget,
            RootNodes = RootNodes,
        };

        /// <inheritdoc/>
        public JProperty Serialize()
            => new(EXTENSION_NAME, JObject.FromObject(this));
    }

    /// <summary>
    /// Defines the <see cref="EVJ_scene_Factory" />
    /// </summary>
    public class EVJ_scene_Factory : ExtensionFactory
    {
        #region Constants

        public const string EXTENSION_NAME = EVJ_scene.EXTENSION_NAME;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EVJ_scene_Factory"/> class.
        /// </summary>
        public EVJ_scene_Factory()
            => ExtensionName = EXTENSION_NAME;

        /// <summary>
        /// The Deserialize
        /// </summary>
        /// <param name="root">The root<see cref="GLTFRoot"/></param>
        /// <param name="extensionToken">The extensionToken<see cref="JProperty"/></param>
        /// <returns>The <see cref="IExtension"/></returns>
        public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
        {
            if (extensionToken != null)
                return null;

            JToken evjSceneToken = extensionToken.Value[EXTENSION_NAME];
            return evjSceneToken != null
                ? evjSceneToken.ToObject<EVJ_scene>()
                : null;
        }

        /// <summary>
        /// The Register
        /// </summary>
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Register()
        {
            if (!GLTFProperty.TryRegisterExtension(new EVJ_scene_Factory()))
                throw new Exception("Failed to register EVJ_scene extension");
        }
    }
}
