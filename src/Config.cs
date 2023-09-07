using Il2Cpp;
using UnityEngine;


namespace SpiceOfLife
{
	internal class Config
	{
		internal static Dictionary<string, List<string>> replaceDict = new Dictionary<string, List<string>>();
		internal static List<string> indoorGroups = new List<string>();
		internal static Dictionary<string,int> indoorGroupValues = new Dictionary<string, int>();
		internal static string? sceneSaveData = null;
		internal static List<string> logCache = new List<string>();
		internal static Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
		internal static List<string> materialMissing = new List<string>();
		internal static string? currentScene = null;
		internal static HashSet<GameObject> sceneCache = new HashSet<GameObject>();

		internal static bool sceneProcessed = false;

		internal static string[] findObjects = Array.Empty<string>();

		internal static Dictionary<int,string> instanceToGroup = new Dictionary<int,string>();

		internal static string[] removeFromCache = {
			"shadow"
		};


		internal static void SceneChange(string? text = null)
		{
			indoorGroupValues.Clear();
			logCache.Clear();
			sceneCache.Clear();
			currentScene = GameManager.m_ActiveScene;
			sceneSaveData = text;
			instanceToGroup.Clear();

			findObjects = Array.Empty<string>();

			sceneProcessed = false;
		}


		internal static void Init()
		{
			List<string> obj_houseinteriorlights_mat = new List<string>() { "OBJ_HouseInteriorLights_A01", "OBJ_HouseInteriorLightsB_Mat", "OBJ_HouseInteriorLightsC_Mat", "OBJ_HouseInteriorLightsD_Mat" };
			replaceDict.Add("OBJ_Lamp", obj_houseinteriorlights_mat);
			indoorGroups.Add("OBJ_Lamp");

			List<string> obj_rug = new List<string>() { "OBJ_RugsA_01", "OBJ_RugsC_A1", "OBJ_RugsPatterned_Mat" };
			replaceDict.Add("OBJ_Rug", obj_rug);

			List<string> obj_sodafridge = new List<string>() { "OBJ_BrandSodaFridges_A01", "OBJ_BrandSodaFridgesB_Mat" };
			replaceDict.Add("INTERACTIVE_BrandSoda", obj_sodafridge);
			replaceDict.Add("INTERACTIVE_MountainSoda", obj_sodafridge);


			List<string> obj_storeshelf = new List<string>() { "OBJ_StoreShelves_A01", "OBJ_StoreShelves_A02", "OBJ_StoreShelves_B01" };
			replaceDict.Add("OBJ_StoreShelf", obj_storeshelf);
			indoorGroups.Add("OBJ_StoreShelf");

			List<string> obj_cartruck = new List<string>() { "OBJ_CarTruck_A01", "OBJ_CarTruck_B", "OBJ_CarTruck_C" };
			replaceDict.Add("ENCAMPMENT_CarTruck", obj_cartruck);

			//List<string> obj_dishset = new List<string>() { "OBJ_DishSet_A01", "OBJ_DishSet_B01" };
			//replaceDict.Add("obj_dishset_a01", obj_dishset);
			//replaceDict.Add("obj_dishset_b01", obj_dishset);

			//List<string> obj_metalventspipesmisc = new List<string>() { "OBJ_MetalVentsPipesMisc_A01", "OBJ_MetalVentsPipesMisc_B01", "OBJ_MetalVentsPipesMisc_C02", "OBJ_MetalVentsPipesMisc_C03" };
			//replaceDict.Add("obj_metalventspipesmisc_a01", obj_cartruck);
			//replaceDict.Add("obj_metalventspipesmisc_b01", obj_cartruck);
			//replaceDict.Add("obj_metalventspipesmisc_c02", obj_cartruck);
			//replaceDict.Add("obj_metalventspipesmisc_c03", obj_cartruck);

			//


			Main.LogInternal($"Definitions Completed ({replaceDict.Count})");
		}
		

		internal static void Debug()
		{
			foreach (KeyValuePair<string, List<string>> kvp in replaceDict)
			{
				Main.LogInternal($" - {kvp.Key} {kvp.Value.Count}");
			}
			foreach (string s in indoorGroups)
			{
				Main.LogInternal($" - {s} will be grouped");
			}
		}




	}
}
