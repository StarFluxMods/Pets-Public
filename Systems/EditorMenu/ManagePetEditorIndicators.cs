using Kitchen;
using KitchenLib.Utils;
using KitchenMods;
using Pets.Components;
using Pets.Components.Menu;
using Unity.Entities;

namespace Pets.Systems.EditorMenu
{
    public class ManagePetEditorIndicators : IndicatorManager, IModSystem
    {
        protected override ViewType ViewType => (ViewType)VariousUtils.GetID("com.starfluxgames.pets.PetEditorView");

        protected override EntityQuery GetSearchQuery()
        {
            return GetEntityQuery(typeof(CPet), typeof(CPosition));
        }

        protected override bool ShouldHaveIndicator(Entity candidate)
        {
            if (Require(candidate, out CHasIndicator comp))
                return Require(comp.Indicator, out CPetEditorInfo cPetEditorInfo) && !cPetEditorInfo.IsComplete;
            
            if (!Has<CPosition>(candidate)) return false;
            if (!Require(candidate, out CTriggerPetEditor trigger)) return false;
            if (!Has<CPlayer>(trigger.TriggerEntity)) return false;
            if (!trigger.IsTriggered) return false;
            
            trigger.IsTriggered = false;
            Set(candidate, trigger);
            return true;

        }

        protected override Entity CreateIndicator(Entity source)
        {
            if (!Require(source, out CPosition position)) return default(Entity);
            if (!Require(source, out CPet cPet)) return default(Entity);
            if (!Require(source, out CTriggerPetEditor trigger)) return default(Entity);
            if (!Require<CPlayer>(trigger.TriggerEntity, out CPlayer player)) return default(Entity);
            
            Entity entity = base.CreateIndicator(source);
            EntityManager.AddComponentData(entity, new CPosition(position));
            EntityManager.AddComponentData(entity, new CPetEditorInfo
            {
                Player = player
            });
            return entity;

        }
    }
}
