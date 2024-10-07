using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="QueueAnimationClipPlayable" />
    /// </summary>
    public class QueueAnimationClipPlayable : PlayableBehaviour
    {
        #region Fields

        private int _currentClipIndex;
        private Playable _mixer;

        #endregion

        /// <summary>
        /// The Initialize
        /// </summary>
        /// <param name="clipsToPlay">The clipsToPlay<see cref="AnimationClip[]"/></param>
        /// <param name="owner">The owner<see cref="Playable"/></param>
        /// <param name="graph">The graph<see cref="PlayableGraph"/></param>
        public void Initialize(AnimationClip[] clipsToPlay, Playable owner, PlayableGraph graph)
        {
            _mixer = AnimationMixerPlayable.Create(graph, clipsToPlay.Length);

            owner.SetInputCount(1);
            graph.Connect(_mixer, 0, owner, 0);
            owner.SetInputWeight(0, 1);

            for (int clipIndex = 0; clipIndex < _mixer.GetInputCount(); ++clipIndex)
            {
                graph.Connect(AnimationClipPlayable.Create(graph, clipsToPlay[clipIndex]), 0, _mixer, clipIndex);
                _mixer.SetInputWeight(clipIndex, 1f);
            }
        }

        /// <summary>
        /// The SetIndex
        /// </summary>
        /// <param name="value">The value<see cref="int"/></param>
        public void SetIndex(int value)
        {
            value = Mathf.Clamp(value, 0, _mixer.GetInputCount() - 1);
            _currentClipIndex = value;

            for (int clipIndex = 0; clipIndex < _mixer.GetInputCount(); ++clipIndex)
                _mixer.SetInputWeight(clipIndex, clipIndex == _currentClipIndex ? 1 : 0);

            _mixer.GetInput(_currentClipIndex).SetTime(0);
        }
    }
}
