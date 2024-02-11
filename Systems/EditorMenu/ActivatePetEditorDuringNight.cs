using Kitchen;
using KitchenLib.Preferences;
using KitchenMods;
using Pets.Components;
using Pets.Components.Menu;

namespace Pets.Systems.EditorMenu
{
    public class ActivatePetEditorDuringNight : ApplianceInteractionSystem, IModSystem
    {
        protected override bool IsPossible(ref InteractionData data)
        {
            return Require(data.Target, out CPet cPet) && cPet.Owner == data.Interactor;
        }

        protected override void Perform(ref InteractionData data)
        {
            int petInteractionMode = Mod.manager.GetPreference<PreferenceInt>("petInteractionMode").Value;
            if (petInteractionMode == 0 || petInteractionMode == 1)
            {
                EntityManager.AddComponentData(data.Target, new CTriggerPetEditor
                {
                    IsTriggered = true,
                    TriggerEntity = data.Interactor
                });
            }
        }
    }
}
