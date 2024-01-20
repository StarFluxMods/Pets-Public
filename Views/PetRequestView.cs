using System;
using Kitchen;
using KitchenMods;
using MessagePack;
using Pets.Components;
using Pets.Systems;
using Unity.Collections;
using Unity.Entities;

namespace Pets.Views
{
    public class PetRequestView : UpdatableObjectView<PetRequestView.ViewData>, ISpecificViewResponse
	{
		public class UpdateView : ResponsiveViewSystemBase<ViewData, ResponseData>, IModSystem
		{
			EntityQuery Query;
			protected override void Initialise()
			{
				base.Initialise();
				Query = GetEntityQuery(typeof(CLinkedView), typeof(SPetRequestView));
			}

			protected override void OnUpdate()
			{
				using NativeArray<CLinkedView> linkedViews = Query.ToComponentDataArray<CLinkedView>(Allocator.Temp);

				foreach (CLinkedView view in linkedViews)
				{
					SendUpdate(view.Identifier, new ViewData());
					if (ApplyUpdates(view.Identifier, PerformUpdateWithResponse, only_final_update: false)) { }
				}
			}

			private void PerformUpdateWithResponse(ResponseData data)
			{
				PetSelector.Main.CreatePlayerRequest(data.PlayerID, data.PetID);
			}
		}

		[MessagePackObject(false)]
		public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
		{
			public IUpdatableObject GetRelevantSubview(IObjectView view)
			{
				return view.GetSubView<PetRequestView>();
			}

			public bool IsChangedFrom(ViewData check)
			{
				return true;
			}
		}

		[MessagePackObject(false)]
		public class ResponseData : IResponseData, IViewResponseData
		{
			[Key(0)]
			public int PlayerID;
			[Key(1)]
			public int PetID;
		}

		private Action<IResponseData, Type> Callback;

		public static int PlayerID = 0;
		public static int PetID = 0;

		public ResponseData Cache;
		
		protected override void UpdateData(ViewData data)
		{
			Cache ??= new ResponseData();

			if (Cache.PlayerID != PlayerID || Cache.PetID != PetID)
			{
				Cache.PlayerID = PlayerID;
				Cache.PetID = PetID;
				if (Callback != null) 
					Callback.Invoke(Cache, typeof(ResponseData));
			}
		}

		public void SetCallback(Action<IResponseData, Type> callback)
		{
			Callback = callback;
		}
	}
}
