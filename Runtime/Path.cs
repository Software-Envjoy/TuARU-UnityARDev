using System;
using UnityEngine;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="Path" />
    /// </summary>
    [Serializable]
    public class Path
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
        /// Gets or sets the IdleAnimation
        /// </summary>
        public AnimationClip IdleAnimation { get; set; }

        /// <summary>
        /// Gets or sets the Waypoints
        /// </summary>
        public Waypoint[] Waypoints { get; set; }

        #endregion
    }
}
