using Kitchen;
using KitchenMods;
using Pets.Components;
using Pets.Components.Menu;

namespace Pets.Systems.EditorMenu
{
    public class ActivatePetEditorDuringDay : ItemInteractionSystem, IModSystem
    {
        protected override bool IsPossible(ref InteractionData data)
        {
            return Require(data.Target, out CPet cPet) && cPet.Owner == data.Interactor;
        }

        protected override void Perform(ref InteractionData data)
        {
            EntityManager.AddComponentData(data.Target, new CTriggerPetEditor
            {
                IsTriggered = true,
                TriggerEntity = data.Interactor
            });
        }
    }
}
