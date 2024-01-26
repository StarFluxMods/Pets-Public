using System;
using System.Collections.Generic;
using Controllers;
using Kitchen;
using KitchenData;
using KitchenLib.Preferences;
using KitchenMods;
using MessagePack;
using Pets.Components;
using Pets.Enums;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Pets.Views
{
    public class PetView : UpdatableObjectView<PetView.ViewData>, ISpecificViewResponse
    {
        public NavMeshAgent agent;
        public Animator animator;
        public VisualEffect vfx;
        public GameObject warningIcon;
        public TextMeshPro label;
        public List<Collider> Colliders;
        
        public override void SetPosition(UpdateViewPositionData pos)
        {
            if (!pos.Force && !((transform.localPosition - pos.Position).Chebyshev() > 0.5f)) return;
            base.SetPosition(pos);
            agent.Warp(pos.Position);
        }
        
        protected override void UpdatePosition() { }
        
        public class UpdateView : ResponsiveViewSystemBase<ViewData, ResponseData>, IModSystem
        {
            private EntityQuery _pets;

            protected override void Initialise()
            {
                base.Initialise();
                _pets = GetEntityQuery(typeof(CLinkedView), typeof(CPet));
            }

            protected override void OnUpdate()
            {
                NativeArray<Entity> pets = _pets.ToEntityArray(Allocator.Temp);

                foreach (Entity pet in pets)
                {
                    if (!Require(pet, out CLinkedView cLinkedView) || !Require(pet, out CPet cPet) || !Require(pet, out CPosition cPosition)) continue;

                    Vector3 TargetPosition = Vector3.zero;
                    Vector3 PreferedFacingDirection = Vector3.zero;
                    float StopDistance = 0;
                    bool ShouldMove = false;
                    int RequestingInputSource = 0;
                    
                    if (Require(pet, out CMoveToLocation cMoveToLocation))
                    {
                        TargetPosition = cMoveToLocation.Location;
                        PreferedFacingDirection = cMoveToLocation.DesiredFacing;
                        StopDistance = cMoveToLocation.StoppingDistance;
                        ShouldMove = true;
                    }
                    
                    
                    if (Require(pet, out CRequestNameChange cRequestNameChange) && !cRequestNameChange.IsTriggered)
                    {
                        RequestingInputSource = cRequestNameChange.Source;
                        cRequestNameChange.IsTriggered = true;
                        EntityManager.AddComponentData(pet, cRequestNameChange);
                    }
                    

                    SendUpdate(cLinkedView, new ViewData
                    {
                        TargetPosition = TargetPosition,
                        StopDistance = StopDistance,
                        IsMoving = ShouldMove,
                        PreferedFacingDirection = PreferedFacingDirection,
                        State = cPet.State,
                        RequestingInputSource = RequestingInputSource,
                        PetName = cPet.PetName.Value,
                        EnableColliders = Mod.manager.GetPreference<PreferenceBool>("petsHaveColliders").Value
                    });
                    if (ApplyUpdates(cLinkedView, (data) =>
                        {
                            EntityManager.AddComponentData(pet, new CCurrentSpeed
                            {
                                speed = data.Speed
                            });
                            
                            if (data.UpdateName)
                            {
                                if (!Require(pet, out CPet cPet)) return;
                                cPet.PetName = data.PetName;
                                if (Require(pet, out CDefaultState cDefaultState))
                                    cPet.State = cDefaultState.State;
                                
                                EntityManager.AddComponentData(pet, cPet);
                            }
                            
                            if (Has<CRequestNameChange>(pet))
                            {
                                EntityManager.RemoveComponent<CRequestNameChange>(pet);
                            }
                        }, only_final_update: false)) { }
                }

                pets.Dispose();
            }
        }
        
        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            public IUpdatableObject GetRelevantSubview(IObjectView view)
            {
                return view.GetSubView<PetView>();
            }

            public bool IsChangedFrom(ViewData check)
            {
                return true;
            }

            [Key(0)] public bool IsMoving;
            [Key(1)] public Vector3 TargetPosition;
            [Key(2)] public float StopDistance;
            [Key(3)] public Vector3 PreferedFacingDirection;
            [Key(4)] public PetState State;
            [Key(5)] public int RequestingInputSource;
            [Key(6)] public string PetName;
            [Key(7)] public bool EnableColliders;
        }

        [MessagePackObject(false)]
        public class ResponseData : IResponseData, IViewResponseData
        {
            [Key(0)] public float Speed;
            [Key(1)] public string PetName;
            [Key(2)] public bool UpdateName;
        }

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Eating = Animator.StringToHash("Eating");
        private static readonly int Sleeping = Animator.StringToHash("Sleeping");

        private ViewData Data;
        
        protected override void UpdateData(ViewData viewData)
        {
            Data = viewData;
            
            if (Data == null) return;
            
            if (Data.RequestingInputSource == InputSourceIdentifier.Identifier)
            {
                TextInputView.RequestTextInput("Rename Pet", "", 24, HandleNewName);
            }
        }

        private void HandleNewName(TextInputView.TextInputState state, string result)
        {
            if (state != TextInputView.TextInputState.TextEntryComplete) return;
            
            Cache ??= new ResponseData();
            Cache.PetName = result;
            Cache.UpdateName = true;
            if (Callback != null)
                Callback?.Invoke(Cache, typeof(ResponseData));
        }

        private Action<IResponseData, Type> Callback;
        public void SetCallback(Action<IResponseData, Type> callback)
        {
            Callback = callback;
        }

        public ResponseData Cache;
        
        private void Update()
        {
            if (Data == null) return;

            Cache ??= new ResponseData();
            
            if (Data.PetName == Cache.PetName)
                Cache.UpdateName = false;

            if (agent != null)
            {
                if (Cache.Speed != agent.velocity.magnitude)
                {
                    Cache.Speed = agent.velocity.magnitude;
                    if (Callback != null)
                        Callback.Invoke(Cache, typeof(ResponseData));
                }
            
                if (Data.TargetPosition != Vector3.zero && Data.IsMoving)
                {
                    agent.stoppingDistance = Data.StopDistance;
                    agent.SetDestination(Data.TargetPosition);
                }
                
                if (agent.remainingDistance <= agent.stoppingDistance && Data.PreferedFacingDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(Data.PreferedFacingDirection - transform.position, Vector3.up);
                }
            }

            if (animator != null && agent != null)
            {
                animator.SetFloat(Speed, agent.velocity.magnitude);
                animator.SetBool(Eating, Data.State == PetState.Eat && agent.velocity.magnitude < Mod.MinimumSpeedThreshold);
                animator.SetBool(Sleeping, Data.State == PetState.Sleep && agent.velocity.magnitude < Mod.MinimumSpeedThreshold);
            }

            if (vfx != null)
                vfx.enabled = Data.State == PetState.Sleep && agent.velocity.magnitude < Mod.MinimumSpeedThreshold;
            
            if (warningIcon != null) 
                warningIcon.SetActive(Data.State is PetState.Error);

            if (label != null)
            {
                label.font = GameData.Main.GlobalLocalisation.Fonts[KitchenData.Font.Default];
                label.text = Data.PetName;
            }

            if (Colliders != null)
            {
                foreach (Collider collider in Colliders)
                {
                    collider.enabled = Data.EnableColliders;
                }
            }
        }
    }
}