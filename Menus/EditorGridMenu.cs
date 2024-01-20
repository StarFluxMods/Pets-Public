using System;
using System.Collections.Generic;
using Kitchen.Modules;
using UnityEngine;

namespace Pets.Menus
{
    public class EditorGridMenu : GridMenu<GridItemOption>
    {
        public EditorGridMenu(List<GridItemOption> items, Transform container, int player, bool has_back) : base(items, container, player, has_back)
        {
        }

        protected override int ColumnLength => 1;

        protected override void SetupElement(GridItemOption item, GridMenuElement element)
        {
            element.Set(item);
        }

        protected override void OnSelect(GridItemOption item)
        {
            item.DoCallback();
        }
    }

    [Serializable]
    public struct GridItemOption : IGridItem
    {
        public readonly int ActionID;
        public Texture2D icon;
        private Action<int> SelectCallback;

        public GridItemOption(int ActionID, Action<int> callback, Texture2D icon = null)
        {
            this.ActionID = ActionID;
            this.icon = icon;
            SelectCallback = callback;
        }

        public int SnapshotKey => ActionID;

        public Texture2D GetSnapshot()
        {
            if (ActionID == 0)
            {
                return Mod.Bundle.LoadAsset<Texture2D>("MenuBack");
            }

            return icon;
        }

        public void DoCallback()
        {
            SelectCallback?.Invoke(ActionID);
        }
    }
}
