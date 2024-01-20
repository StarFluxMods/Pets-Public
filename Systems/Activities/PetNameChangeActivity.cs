using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Enums;
using Unity.Entities;

namespace Pets.Systems.Activities
{
    public class PetNameChangeActivity : PetActivitySystem, IModSystem
    {
        protected override PetState StateForUpdate => PetState.NameChange;

        protected override bool IsPossible(ActivityData data)
        {
            return Require(data.Pet, out CPet cPet) && cPet.Owner != Entity.Null && Has<CPlayer>(cPet.Owner) && !Has<CRequestNameChange>(data.Pet);
        }
        
        protected override bool Perform(ActivityData data)
        {
            if (!Require(data.Pet, out CPet cPet)) return false;
            if (!Require(cPet.Owner, out CPlayer cPlayer)) return false;

            EntityManager.AddComponentData(data.Pet, new CRequestNameChange
            {
                Source = cPlayer.InputSource,
                IsTriggered = false
            });
            return true;
        }
    }
}