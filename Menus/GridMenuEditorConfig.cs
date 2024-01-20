using System;
using System.Collections.Generic;
using Kitchen.Modules;
using UnityEngine;

namespace Pets.Menus
{
    public class GridMenuEditorConfig : GridMenuConfig
    {
        public override GridMenu Instantiate(Transform container, int player, bool has_back)
        {
            return new EditorGridMenu(new List<GridItemOption>(), container, player, has_back);
        }

        public virtual EditorGridMenu Instantiate(Action<int> callback, Transform container, int player, bool has_back)
        {
            List<GridItemOption> gridAppliances = new List<GridItemOption>()
            {
                new GridItemOption(0, callback),
                new GridItemOption(1, callback, Mod.Bundle.LoadAsset<Texture2D>("Stop")),
                new GridItemOption(2, callback, Mod.Bundle.LoadAsset<Texture2D>("Activity")),
                new GridItemOption(99, callback, Mod.Bundle.LoadAsset<Texture2D>("Rename"))
            };
            return new EditorGridMenu(gridAppliances, container, player, has_back);
        }
    }
}
