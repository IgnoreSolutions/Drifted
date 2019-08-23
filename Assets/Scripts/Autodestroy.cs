using UnityEngine;
using System.Collections;

namespace Drifted.UI
{
    [DisallowMultipleComponent]
    public class Autodestroy : MonoBehaviour
    {
        public void KillIn(float time_s) => Invoke("DestroySelf", time_s);
        public void DestroySelf() => Destroy(transform.gameObject);
    }

}