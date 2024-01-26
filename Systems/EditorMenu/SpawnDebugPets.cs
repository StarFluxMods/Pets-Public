using System.Collections.Generic;
using Kitchen;
using KitchenData;
using KitchenMods;
using Pets.Components;
using Pets.Customs.Types;
using Pets.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Pets.Systems.EditorMenu
{
    public class SpawnDebugPets : GameSystemBase
    {
        private EntityQuery _displayPets;
        
        protected override void Initialise()
        {
            return;
            base.Initialise();
            _displayPets = GetEntityQuery(typeof(CDisplayPet));
            
            foreach (Pet pet in GameData.Main.Get<Pet>())
            {
                _pets.Add(pet);
            }
        }

        private Vector3 _spawnPosition = new Vector3(0, 0, 0);
        private Vector3 facing = new Vector3(0, 0, 0);
        private List<Pet> _pets = new List<Pet>();
        private int _selectedPet = 0;
        
        protected override void OnUpdate()
        {
            return;
            using NativeArray<Entity> displayPets = _displayPets.ToEntityArray(Allocator.Temp);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _spawnPosition.x -= 1;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _spawnPosition.x += 1;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _spawnPosition.z += 1;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _spawnPosition.z -= 1;
            }
            
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                facing = _spawnPosition;
                facing.z -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                facing = _spawnPosition;
                facing.x -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                facing = _spawnPosition;
                facing.x += 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                facing = _spawnPosition;
                facing.z += 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                facing = _spawnPosition;
                facing.x -= 1;
                facing.z += 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                facing = _spawnPosition;
                facing.x += 1;
                facing.z += 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                facing = _spawnPosition;
                facing.x -= 1;
                facing.z -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                facing = _spawnPosition;
                facing.x += 1;
                facing.z -= 1;
            }
            

            for (int i = 0; i < displayPets.Length; i++)
            {
                Entity displayPet = displayPets[i];
                EntityManager.AddComponentData(displayPet, new CPosition(_spawnPosition));
                EntityManager.AddComponentData(displayPet, new CMoveToLocation
                {
                    Location = _spawnPosition,
                    DesiredFacing = facing
                });
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                for (int i = 0; i < displayPets.Length; i++)
                {
                    Entity displayPet = displayPets[i];
                    EntityManager.DestroyEntity(displayPet);
                }
                
                if (_selectedPet < _pets.Count - 1)
                {
                    _selectedPet++;
                }
                else
                {
                    _selectedPet = 0;
                }
                
                Entity pet = EntityManager.CreateEntity();
                
                EntityManager.AddComponentData(pet, new CPet
                {
                    State = PetState.Idle,
                    PetType = _pets[_selectedPet].ID
                });
                
                EntityManager.AddComponentData(pet, new CPosition(_spawnPosition));
                
                EntityManager.AddComponentData(pet, new CRequiresView
                {
                    Type = _pets[_selectedPet].ViewType,
                    PhysicsDriven = true
                });
                
                EntityManager.AddComponentData(pet, new CDoNotPersist());
                EntityManager.AddComponentData(pet, new CDisplayPet());
            }
        }
    }
}