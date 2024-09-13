using UnityEngine;

namespace UnityGLTF
{
    /// <summary>
    /// Defines the <see cref="GLTFSceneImporter" />
    /// </summary>
    public partial class GLTFSceneImporter
    {
#if UNITY_ANIMATION || !UNITY_2019_1_OR_NEWER

        /// <summary>
        /// The ConstructClip2
        /// </summary>
        /// <param name="root">The root<see cref="Transform"/></param>
        /// <param name="animationId">The animationId<see cref="int"/></param>
        /// <returns>The <see cref="AnimationClip"/></returns>
        public AnimationClip ConstructClip2(Transform root, int animationId)
            => ConstructClip(root, animationId, default).GetAwaiter().GetResult();

        /// <summary>
        /// The SetCreatedAnimationClips
        /// </summary>
        /// <param name="clips">The clips<see cref="AnimationClip[]"/></param>
        public void SetCreatedAnimationClips(AnimationClip[] clips)
            => CreatedAnimationClips = clips;
#endif
    }
}
