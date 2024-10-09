using UnityEngine;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="SceneNode" />
    /// </summary>
    public class SceneNode : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ImageTarget
        /// </summary>
        [field: SerializeField]
        public Texture ImageTarget { get; set; }

        /// <summary>
        /// Gets or sets the Width
        /// </summary>
        [field: SerializeField]
        public float Width { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the Orientation
        /// </summary>
        [field: SerializeField]
        public Orientation Orientation { get; set; }

        #endregion
    }
}
