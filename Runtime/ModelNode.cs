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
        public Hotspot[] Hotspots;

#if DOTWEEN
        private Tween _tween;
#endif
        private PlayableGraph _playableGraph;

        private Vector3 _position;
        private Quaternion _rotation;
        private bool _paused;

        #endregion

        /// <summary>
        /// The Play
        /// </summary>
        [ContextMenu("Play")]
        public void Play()
        {
            _position = transform.position;
            _rotation = transform.rotation;

            _playableGraph = PlayableGraph.Create();

            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", GetComponent<Animator>());
            var mixerPlayable = AnimationMixerPlayable.Create(_playableGraph, 2);
            mixerPlayable.SetInputWeight(0, 0.5f);
            mixerPlayable.SetInputWeight(1, 0.5f);

            playableOutput.SetSourcePlayable(mixerPlayable);

            if (Path.IdleAnimation != null)
            {
                var idlePlayable = ScriptPlayable<LoopAnimationClipPlayable>.Create(_playableGraph);
                var idleBehaviour = idlePlayable.GetBehaviour();
                idleBehaviour.Initialize(Path.IdleAnimation, idlePlayable, _playableGraph);

                _playableGraph.Connect(idlePlayable, 0, mixerPlayable, 0);
            }

            var hasWaypoints = Path.Waypoints.Length > 1;
            if (hasWaypoints)
            {
                var clips = Path.Waypoints.Select(w => w.Animation).ToArray();
                var waypointPlayable = ScriptPlayable<QueueAnimationClipPlayable>.Create(_playableGraph);
                var waypointBehaviour = waypointPlayable.GetBehaviour();
                waypointBehaviour.Initialize(clips, waypointPlayable, _playableGraph);

                _playableGraph.Connect(waypointPlayable, 0, mixerPlayable, 1);
#if DOTWEEN
                _tween = CreateTween()
                        .OnWaypointChange(waypointBehaviour.SetIndex)
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
            _paused = false;
#if DOTWEEN
            _tween?.Kill();
#endif

            if (_playableGraph.IsValid())
                _playableGraph.Destroy();

            transform.SetPositionAndRotation(_position, _rotation);
        }

        /// <summary>
        /// The Pause
        /// </summary>
        [ContextMenu("Pause")]
        public void Pause()
        {
            _paused = !_paused;
            if (_paused)
            {
#if DOTWEEN
                _tween?.Pause();
#endif

                if (_playableGraph.IsValid())
                    _playableGraph.GetRootPlayable(0).Pause();
            }
            else
            {
#if DOTWEEN
                _tween?.Play();
#endif

                if (_playableGraph.IsValid())
                    _playableGraph.GetRootPlayable(0).Play();
            }
        }

        /// <summary>
        /// The Awake
        /// </summary>
        private void Awake()
        {
            Clips ??= new AnimationClip[0];
            Path ??= new Path
            {
                Duration = 1,
                Waypoints = new Waypoint[1]
                {
                    new Waypoint
                    {
                        Position = transform.position,
                    }
                }
            };
            Hotspots ??= new Hotspot[0];
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

            return transform.DOLocalPath(path, Path.Duration, pathType, PathMode.Full3D, gizmoColor: Color.yellow)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1)
                            .SetOptions(closePath: Path.ClosePath)
                            .SetLookAt(0.1f, false);
        }
#endif
    }
}
