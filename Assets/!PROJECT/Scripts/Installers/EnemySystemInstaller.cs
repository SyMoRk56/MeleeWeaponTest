using com.marufhow.meshslicer.core;
using UnityEngine;
using Zenject;

public class EnemySystemInstaller : MonoInstaller
{
    [SerializeField] private MHCutter _cutter;

    public override void InstallBindings()
    {
        Container.BindInstance(_cutter).AsSingle().NonLazy();
    }
}
