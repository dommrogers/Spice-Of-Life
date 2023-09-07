using HarmonyLib;
using Il2Cpp;
using System.Diagnostics;

namespace SpiceOfLife;

internal class Patches
{
	[HarmonyPatch(typeof(SaveGameSystem), nameof(SaveGameSystem.LoadSceneData))]
	private class SaveGameSystem_LoadSceneData_PRE
	{
		private static void Prefix()
		{
			Config.SceneChange();
		}

	}

	[HarmonyPatch(typeof(ReplaceMaterialsManager), nameof(ReplaceMaterialsManager.DeserializeAll))]
	private class ReplaceMaterialsManager_DeserializeAll
	{
		private static void Prefix(string text)
		{
			Stopwatch sw = Stopwatch.StartNew();

			Config.SceneChange(text);

			Main.LogInternal($"ReplaceMaterialsManager.DeserializeAll : {GameManager.m_ActiveScene}");

			Main.ProcessScene();
			sw.Stop();

			Main.LogInternal($"ReplaceMaterialsManager.DeserializeAll : {GameManager.m_ActiveScene} ... complete {sw.Elapsed.TotalSeconds}s");
		}

	}

	[HarmonyPatch(typeof(SaveGameSystem), nameof(SaveGameSystem.LoadSceneData))]
	private class SaveGameSystem_LoadSceneData
	{
		private static void Postfix()
		{
			if (!Config.sceneProcessed)
			{
				Config.SceneChange();
				Stopwatch sw = Stopwatch.StartNew();

				Main.LogInternal($"SaveGameSystem.LoadSceneData : {GameManager.m_ActiveScene}");

				Main.ProcessScene();
				sw.Stop();

				Main.LogInternal($"SaveGameSystem.LoadSceneData : {GameManager.m_ActiveScene} ... complete {sw.Elapsed.TotalSeconds}s");
			}
		}

	}


}


