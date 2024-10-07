using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Envjoy.GLTF
{

    /// <summary>
    /// Defines the <see cref="LoopAnimationClipPlayable" />
    /// </summary>
    public class LoopAnimationClipPlayable : PlayableBehaviour
    {
        #region Fields

        private AnimationClipPlayable _animation;
        private float _length;
        private float _currentTime;

        #endregion

        /// <summary>
        /// The Initialize
        /// </summary>
        /// <param name="clip">The clip<see cref="AnimationClip"/></param>
        /// <param name="owner">The owner<see cref="Playable"/></param>
        /// <param name="graph">The graph<see cref="PlayableGraph"/></param>
        public void Initialize(AnimationClip clip, Playable owner, PlayableGraph graph)
        {
            _animation = AnimationClipPlayable.Create(graph, clip);

            owner.SetInputCount(1);
            graph.Connect(_animation, 0, owner, 0);
            owner.SetInputWeight(0, 1);

            _currentTime = 0;
            _length = clip.length;
        }

        /// <summary>
        /// The PrepareFrame
        /// </summary>
        /// <param name="owner">The owner<see cref="Playable"/></param>
        /// <param name="info">The info<see cref="FrameData"/></param>
        public override void PrepareFrame(Playable owner, FrameData info)
        {
            _currentTime = (_currentTime + info.deltaTime) % _length;
            _animation.SetTime(_currentTime);
        }
    }
}
