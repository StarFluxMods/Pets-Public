using System.Collections.Generic;
using Controllers;
using Kitchen;
using Kitchen.Modules;
using KitchenMods;
using MessagePack;
using Pets.Components;
using Pets.Components.Menu;
using Pets.Menus;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Pets.Views
{
    public class PetEditorView : ResponsiveObjectView<PetEditorView.ViewData, PetEditorView.ResponseData>, IInputConsumer
    {
        public class UpdateView : ResponsiveViewSystemBase<ViewData, ResponseData>, IModSystem
        {
            EntityQuery Views;
            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CPetEditorInfo), typeof(CLinkedView));
            }

            protected override void OnUpdate()
            {
                using NativeArray<Entity> entities = Views.ToEntityArray(Allocator.Temp);
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using NativeArray<CPetEditorInfo> infos = Views.ToComponentDataArray<CPetEditorInfo>(Allocator.Temp);

                foreach (Entity entity in entities)
                {
                    if (!Require(entity, out CLinkedView cLinkedView)) continue;
                    if (!Require(entity, out CPetEditorInfo cPetEditorInfo)) continue;
                    
                    SendUpdate(cLinkedView.Identifier, new ViewData
                    {
                        Player = cPetEditorInfo.Player.PlayerID
                    });
                    
                    ResponseData result = default(ResponseData);

                    if (ApplyUpdates(cLinkedView, (data) => { result = data; }, only_final_update: true))
                    {
                        cPetEditorInfo.IsComplete = result.IsComplete;
                        Set(entity, cPetEditorInfo);
                        
                        Entity stateRequest = EntityManager.CreateEntity();
                        EntityManager.AddComponentData(stateRequest, new CRequestStateChange
                        {
                            PlayerID = result.PlayerID,
                            StateID = result.Option
                        });
                    }
                }
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : IViewData, IViewResponseData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)]
            public int Player;

            public bool IsChangedFrom(ViewData check)
            {
                return Player != check.Player;
            }
        }

        [MessagePackObject(false)]
        public struct ResponseData : IResponseData, IViewResponseData
        {
            [Key(0)]
            public bool IsComplete;
            [Key(1)]
            public int Option;
            [Key(2)]
            public int PlayerID;
        }

        private struct MenuStackElement
        {
            public GridMenuEditorConfig Config;

            public int Index;
        }

        public GridMenuEditorConfig rootMenuConfig;
        public Transform container;
        private EditorGridMenu _gridMenu;
        private int _playerID;
        private InputLock.Lock _lock;
        private bool _isComplete;
        private readonly Stack<MenuStackElement> _menuStack = new Stack<MenuStackElement>();
        private int _option;

        private void CloseMenu()
        {
            if (_menuStack.Count > 1)
            {
                int index = _menuStack.Pop().Index;
                MenuStackElement menuStackElement = _menuStack.Pop();
                SetNewMenu(menuStackElement.Config, index, menuStackElement.Index);
            }
            else
            {
                Remove();
            }
        }

        private void SetNewMenu(GridMenuEditorConfig menu, int newIndex, int previousIndex)
        {
            _gridMenu?.Destroy();
            _gridMenu = menu.Instantiate(delegate (int result)
            {
                _option = result;
                CloseMenu();
            }, container, _playerID, _menuStack.Count > 0);
            _gridMenu.OnRequestMenu += delegate (GridMenuConfig c)
            {
                if (c is GridMenuEditorConfig cApp)
                    SetNewMenu(cApp, 0, _gridMenu?.SelectedIndex() ?? 0);
            };
            _gridMenu.OnGoBack += CloseMenu;
            _gridMenu.SelectByIndex(newIndex);
            _menuStack.Push(new MenuStackElement
            {
                Config = menu,
                Index = previousIndex
            });
        }

        protected override void UpdateData(ViewData data)
        {
            if (InputSourceIdentifier.DefaultInputSource == null) return;
            if (!Players.Main.Get(data.Player).IsLocalUser)
            {
                gameObject.SetActive(value: false);
                return;
            }
            gameObject.SetActive(value: true);
            _option = 0;
            InitialiseForPlayer(data.Player);
        }

        private void InitialiseForPlayer(int player)
        {
            LocalInputSourceConsumers.Register(this);
            if (_lock.Type != 0)
                InputSourceIdentifier.DefaultInputSource.ReleaseLock(_playerID, _lock);
            _playerID = player;
            SetNewMenu(rootMenuConfig, 0, 0);
            _lock = InputSourceIdentifier.DefaultInputSource.SetInputLock(_playerID, PlayerLockState.NonPause);
        }

        public override void Remove()
        {
            _isComplete = true;
            InputSourceIdentifier.DefaultInputSource.ReleaseLock(_playerID, _lock);
            base.Remove();
        }

        private void OnDestroy()
        {
            LocalInputSourceConsumers.Remove(this);
        }

        public InputConsumerState TakeInput(int playerID, InputState state)
        {
            if (_playerID == 0 || playerID != _playerID) return InputConsumerState.NotConsumed;
            if (state.MenuTrigger == ButtonState.Pressed)
            {
                _isComplete = true;
                InputSourceIdentifier.DefaultInputSource.ReleaseLock(_playerID, _lock);
                return InputConsumerState.Terminated;
            }
            if (_gridMenu != null && !_gridMenu.HandleInteraction(state) && state.MenuCancel == ButtonState.Pressed)
            {
                CloseMenu();
            }
            return !_isComplete ? InputConsumerState.Consumed : InputConsumerState.Terminated;
        }

        public override bool HasStateUpdate(out IResponseData state)
        {
            state = null;
            if (_isComplete)
            {
                state = new ResponseData
                {
                    IsComplete = _isComplete,
                    Option = _option,
                    PlayerID = _playerID
                };
            }
            return _isComplete;
        }
    }
}
