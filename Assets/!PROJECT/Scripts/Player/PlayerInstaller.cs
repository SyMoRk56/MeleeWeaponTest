using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] PlayerManager _playerManager;
        public override void InstallBindings()
        {
            Container.BindInstance(_playerManager).AsSingle().NonLazy();
        }
    }

}
