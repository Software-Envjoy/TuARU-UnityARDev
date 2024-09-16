#if DOTWEEN
using DG.Tweening;
#endif
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Envjoy.GLTF
{
    /// <summary>
    /// Defines the <see cref="ModelNode" />
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ModelNode : MonoBehaviour
    {
        #region Fields

        public AnimationClip[] Clips;
        public Path Path;
#if DOTWEEN
        private Tween _tween;
#endif
        private PlayableGraph _playableGraph;

        #endregion

        /// <summary>
        /// The Play
        /// </summary>
        [ContextMenu("Play")]
        public void Play()
        {
            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", GetComponent<Animator>());
            var mixerPlayable = AnimationMixerPlayable.Create(_playableGraph, 2);
            playableOutput.SetSourcePlayable(mixerPlayable);

            var idlePlayable = AnimationClipPlayable.Create(_playableGraph, Path.IdleAnimation);
            _playableGraph.Connect(idlePlayable, 0, mixerPlayable, 0);

            mixerPlayable.SetInputWeight(0, 1);

            var hasWaypoints = Path.Waypoints.Length > 1;
            if (hasWaypoints)
            {
                var waypointClip = Path.Waypoints[0].Animation;
                var waypointPlayable = AnimationClipPlayable.Create(_playableGraph, waypointClip);
                _playableGraph.Connect(waypointPlayable, 0, mixerPlayable, 1);

                void OnWaypointChange(int value)
                {
                    var clip = Path.Waypoints[value].Animation;
                    if (clip == null)
                        return;

                    mixerPlayable.SetInputWeight(0, 0.5f);
                    mixerPlayable.SetInputWeight(1, 0.5f);
                    waypointPlayable.SetAnimatedProperties(clip);
                    waypointPlayable.SetTime(0);
                }
#if DOTWEEN
                _tween = CreateTween()
                        .OnWaypointChange(OnWaypointChange)
                        .Play();
#endif
            }

            _playableGraph.Play();
        }

        /// <summary>
        /// The Stop
        /// </summary>
        [ContextMenu("Stop")]
        public void Stop()
        {
            _tween?.Kill();

            if (_playableGraph.IsValid())
                _playableGraph.Stop();
        }

        /// <summary>
        /// The Awake
        /// </summary>
        private void Awake()
        {
            _playableGraph = PlayableGraph.Create();
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            Clips ??= new AnimationClip[0];
            Path ??= new Path
            {
                Waypoints = new Waypoint[1]
                {
                    new Waypoint
                    {
                        Position = transform.position,
                    }
                }
            };
        }

        /// <summary>
        /// The OnDestroy
        /// </summary>
        private void OnDestroy()
        {
#if DOTWEEN
            _tween?.Kill();
#endif

            if (_playableGraph.IsValid())
                _playableGraph.Destroy();
        }

#if DOTWEEN
        /// <summary>
        /// The CreateTween
        /// </summary>
        /// <returns>The <see cref="Tween"/></returns>
        private Tween CreateTween()
        {
            var pathType = Path.ConvertCurves
                ? PathType.CatmullRom
                : PathType.Linear;

            var path = Path.Waypoints.Select(w => w.Position)
                                     .ToArray();

            return transform.DOPath(path, Path.Duration, pathType, PathMode.Full3D, gizmoColor: Color.yellow)
                            .SetLoops(-1)
                            .SetOptions(closePath: true)
                            .SetLookAt(1);
        }
#endif
    }
}
