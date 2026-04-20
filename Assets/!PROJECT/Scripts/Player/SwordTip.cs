using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class SwordTip : MonoBehaviour
{
    [SerializeField] private Sword _sword;
    private void OnTriggerEnter(Collider other)
    {
        _sword.CheckCollision(other).Forget();
    }
}
