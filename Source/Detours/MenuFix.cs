using HarmonyLib;
using UnityEngine.UI;

namespace LunacidUltrawidescreenFix.Detours
{
	[HarmonyPatch]
	public class MenuFix
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Menus), nameof(Menus.LoadMenu))]
		public static void LoadMenuDetour(Menus __instance)
		{
			Plugin.LogInstance.LogError("No Canvas scaler in parent!");

			var parent = __instance.transform.parent;
			if(parent == null)
			{
				Plugin.LogInstance.LogError("No MainMenu parent!");
				return;
			}

			var canvasScaler = parent.GetComponent<CanvasScaler>();
			if(canvasScaler == null)
			{
				Plugin.LogInstance.LogError("No Canvas scaler in parent!");
				return;
			}

			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		}
	}
}
