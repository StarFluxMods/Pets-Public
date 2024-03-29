using System.Collections.Generic;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Preferences;
using UnityEngine;

namespace Pets.Menus
{
    public class PreferenceMenu<T>: KLMenu<T>
    {
        public PreferenceMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }
        
        private Option<bool> petsHaveColliders = new Option<bool>(new List<bool> { true, false }, Mod.manager.GetPreference<PreferenceBool>("petsHaveColliders").Value, new List<string> { "Enabled", "Disabled" });
        private Option<int> petInteractionMode = new Option<int>(new List<int> { 0, 1, 2 }, Mod.manager.GetPreference<PreferenceInt>("petInteractionMode").Value, new List<string> { "Always", "Night Only", "Day Only" });
        
        public override void Setup(int player_id)
        {
            AddLabel("Pet Colliders");
            
            New<SpacerElement>(true);
            AddSelect(petsHaveColliders);
            
            petsHaveColliders.OnChanged += delegate (object _, bool result)
            {
                Mod.manager.GetPreference<PreferenceBool>("petsHaveColliders").Set(result);
            };
            
            New<SpacerElement>(true);
            
            AddLabel("Pet Interaction ");
            
            New<SpacerElement>(true);
            AddSelect(petInteractionMode);
            
            petInteractionMode.OnChanged += delegate (object _, int result)
            {
                Mod.manager.GetPreference<PreferenceInt>("petInteractionMode").Set(result);
            };
            
            New<SpacerElement>(true);
            New<SpacerElement>(true);
            
            AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate(int i)
            {
                Mod.manager.Save();
                RequestPreviousMenu();
            });
        }
    }
}