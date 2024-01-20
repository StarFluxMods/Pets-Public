using KitchenLib;
using KitchenLib.Logging;
using KitchenLib.Logging.Exceptions;
using KitchenMods;
using System.Linq;
using System.Reflection;
using Kitchen.Modules;
using KitchenLib.Interfaces;
using KitchenLib.Utils;
using Pets.Components;
using Pets.Customs;
using Pets.Customs.Types;
using Pets.Menus;
using Pets.Views;
using UnityEngine;

namespace Pets
{
    public class Mod : BaseMod, IAutoRegisterAll
    {
        public const string MOD_GUID = "com.starfluxgames.pets";
        public const string MOD_NAME = "Pets";
        public const string MOD_VERSION = "0.1.1";
        public const string MOD_AUTHOR = "StarFluxGames";
        public const string MOD_GAMEVERSION = ">=1.1.8";
        
        public static AssetBundle Bundle;
        public static KitchenLogger Logger;
        
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
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Goose>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Penguin>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Cat>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Chick>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Rabbit>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Squirrel>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<DogChihuahua>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Elephant>().GameDataObject as Pet);
                    config.Pets.Add(GDOUtils.GetCustomGameDataObject<Seal>().GameDataObject as Pet);
                    config.Icon = Bundle.LoadAsset<Texture2D>("PawPrint");
                    grid.Links.Add(config);
                }
            }
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_GUID);
            Logger = InitLogger();
            
            ViewUtils.RegisterView("Pets.Views.PetRequestView", typeof(SPetRequestView), typeof(PetRequestView));
        }
    }
}

