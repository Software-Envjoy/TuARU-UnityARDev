using UnityEngine;

namespace Envjoy.GLTF
{
    public class SceneNode : MonoBehaviour
    {
        [field: SerializeField]
        public Texture ImageTarget { get; set; }

        [field: SerializeField]
        public float Width { get; set; } = 1.0f;

        [field: SerializeField]
        public Orientation Orientation { get; set; }
    }
}