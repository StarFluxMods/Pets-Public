using KitchenLib;
using KitchenLib.Logging;
using KitchenLib.Logging.Exceptions;
using KitchenMods;
using System.Linq;
using System.Reflection;
using Kitchen;
using Kitchen.Modules;
using KitchenLib.Event;
using KitchenLib.Interfaces;
using KitchenLib.Preferences;
using KitchenLib.Utils;
using Pets.Components;
using Pets.Customs;
using Pets.Menus;
using Pets.Views;
using UnityEngine;

namespace Pets
{
    public class Mod : BaseMod, IAutoRegisterAll
    {
        public const string MOD_GUID = "com.starfluxgames.pets";
        public const string MOD_NAME = "Pets";
        public const string MOD_VERSION = "0.1.4";
        public const string MOD_AUTHOR = "StarFluxGames";
        public const string MOD_GAMEVERSION = ">=1.1.8";
        
        public static AssetBundle Bundle;
        public static KitchenLogger Logger;
        public static PreferenceManager manager;
        
        public static float MinimumSpeedThreshold = 0.1f;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

            foreach (GridMenuNavigationConfig grid in Resources.FindObjectsOfTypeAll<GridMenuNavigationConfig>())
            {
                if (grid.name == "Root")
                {
                    GridMenuPetConfig config = new GridMenuPetConfig();
                    config.Pets.Add(0);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Goose>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Penguin>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Cat>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Chick>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Rabbit>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Squirrel>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<DogChihuahua>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Elephant>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Seal>().ID);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Panda>().ID);
                    config.Icon = Bundle.LoadAsset<Texture2D>("PawPrint");
                    grid.Links.Add(config);
                }
            }
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_GUID);
            Logger = InitLogger();
            
            manager = new PreferenceManager(MOD_GUID);
            manager.RegisterPreference(new PreferenceBool("petsHaveColliders", true));
            manager.RegisterPreference(new PreferenceInt("petInteractionMode", 0)); // 0 = Always, 1 = Night Only, 2 = Day Only
            manager.Load();
            manager.Save();
            
            ModsPreferencesMenu<MainMenuAction>.RegisterMenu(MOD_NAME, typeof(PreferenceMenu<MainMenuAction>), typeof(MainMenuAction));
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(PreferenceMenu<PauseMenuAction>), typeof(PauseMenuAction));
            
            Events.MainMenuView_SetupMenusEvent += (s, args) =>
            {
                args.addMenu.Invoke(args.instance, new object[] { typeof(PreferenceMenu<MainMenuAction>), new PreferenceMenu<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
            };
            
            Events.PlayerPauseView_SetupMenusEvent += (s, args) =>
            {
                args.addMenu.Invoke(args.instance, new object[] { typeof(PreferenceMenu<PauseMenuAction>), new PreferenceMenu<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
            };
            
            ViewUtils.RegisterView("Pets.Views.PetRequestView", typeof(SPetRequestView), typeof(PetRequestView));
        }
    }
}

