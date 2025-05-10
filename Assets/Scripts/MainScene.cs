namespace DotsFisher
{
    using Cysharp.Threading.Tasks;
    using DotsFisher.Mono;
    using System.Threading;
    using UnityEngine;

    public partial class MainScene : MonoBehaviour
    {
        [SerializeField] private FishDebugDrawer _fishDebugDrawer;
        [SerializeField] private BulletDebugDrawer _bulletDebugDrawer;
        [SerializeField] private AABBDebugDrawer _aabbDebugDrawer;

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            new Main().Run(
                _fishDebugDrawer,
                _bulletDebugDrawer,
                _aabbDebugDrawer,
                _cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts.Cancel();
        }
    }
}