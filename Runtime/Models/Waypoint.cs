using System;
using UnityEngine;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="Waypoint" />
    /// </summary>
    [Serializable]
    public class Waypoint
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Animation
        /// </summary>
        public AnimationClip Animation { get; set; }

        /// <summary>
        /// Gets or sets the Position
        /// </summary>
        public Vector3 Position { get; set; }

        #endregion
    }
}
