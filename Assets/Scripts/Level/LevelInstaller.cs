using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Level
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private Pencil _pencil;
        [SerializeField] private AreaCollider _areaCollider;
        [SerializeField] private LassoMovementManager _movementManager;
        [SerializeField] private List<Ball> _balls;
        public override void InstallBindings()
        {
            Container.BindInstance(_balls);
            BindMovementManager();
            BindPencil();
            BindDrawArea();
            BindLevel();
        }

        private void BindMovementManager()
        {
            Container.BindInstance(_movementManager);
        }

        private void BindDrawArea()
        {
            Container.BindInstance(_areaCollider);
        }

        private void BindPencil()
        {
            Container.BindInstance(_pencil);
        }

        private void BindLevel()
        {
            Container.BindInterfacesAndSelfTo<Level>()
                .AsSingle()
                .NonLazy();
        }
    }
}
