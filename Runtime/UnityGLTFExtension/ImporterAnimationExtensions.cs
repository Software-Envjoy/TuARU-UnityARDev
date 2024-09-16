using UnityEngine;

namespace UnityGLTF
{
    /// <summary>
    /// Defines the <see cref="GLTFSceneImporter" />
    /// </summary>
    public partial class GLTFSceneImporter
    {
#if UNITY_ANIMATION || !UNITY_2019_1_OR_NEWER

        #region Fields

        private int _currentAnimationIndex;

        #endregion

        /// <summary>
        /// The InitializeCreatedAnimationClips
        /// </summary>
        public void InitializeCreatedAnimationClips()
        {
            CreatedAnimationClips = new AnimationClip[Root?.Animations?.Count ?? 0];
            _currentAnimationIndex = 0;
        }

        /// <summary>
        /// The TrySetCreatedAnimationClip
        /// </summary>
        /// <param name="clip">The clip<see cref="AnimationClip"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool TrySetCreatedAnimationClip(AnimationClip clip)
        {
            var index = _currentAnimationIndex++;
            if (index < 0 || index >= CreatedAnimationClips.Length)
                return false;

            CreatedAnimationClips[index] = clip;
            return true;
        }
    }
#endif
}
