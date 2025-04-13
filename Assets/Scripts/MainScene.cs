using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace DotsFisher
{
    public class MainScene : MonoBehaviour
    {
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            new Main().Run(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts.Cancel();
        }
    }
}