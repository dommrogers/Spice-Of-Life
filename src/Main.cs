using Il2Cpp;
using MelonLoader;
using UnityEngine;
using LOG = MelonLoader.MelonLogger;
using System.Text;
using MelonLoader.Utils;
using static Il2CppMS.Internal.Xml.XPath.Operator;

namespace SpiceOfLife;
internal sealed class Main : MelonMod
{

	internal static Main instance;



	public override void OnInitializeMelon()
	{
		instance = this;

		Config.Init();
		Config.Debug();

		uConsole.RegisterCommand("sol_reroll", new Action(Reroll));
		uConsole.RegisterCommand("sol_loadfromsave", new Action(LoadFromSave));
		uConsole.RegisterCommand("sol_dumpscene", new Action(DumpScene));
	}


	public override void OnUpdate()
	{
		if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.R))
		{
			Reroll();
		}
	}

	internal static void ProcessScene()
	{
		if (Config.sceneProcessed)
		{
			return;
		}

		Utils.BuildSceneCache();

		foreach (KeyValuePair<string, List<string>> p in Config.replaceDict)
		{
			List<GameObject> gos = Utils.FindSceneGameObjectsByName(p.Key, true);
			LogInternalOnce($"{p.Key} found {gos.Count}");

			foreach (GameObject go in gos)
			{
				bool prefabParent = ((go.transform.parent.gameObject.name.ToLower().Contains("prefab") || go.name.ToLower().Contains("_lod")) && go.transform.parent.gameObject.name.ToLower().StartsWith(p.Key.ToLower()));

				if (prefabParent)
				{
					GameObject gop = go.transform.parent.gameObject;
					if (gop.HasComponent<ReplaceMaterial>(true))
					{
						LogInternalOnce($"- GOP {gop.name} {gop.GetInstanceID()} children has ReplaceMaterial");
						continue;
					}
					if (gop.AddReplaceMaterial(p.Key, p.Value))
					{
						gop.AddObjectGuid();
					}
				} else {
					if (go.HasComponent<ReplaceMaterial>(true))
					{
						LogInternalOnce($"- GO {go.name} {go.GetInstanceID()} children has ReplaceMaterial");
						continue;
					}
					if (go.AddReplaceMaterial(p.Key, p.Value))
					{
						go.AddObjectGuid();
					}
				}
			}
		}
		Config.sceneProcessed = true;
	}



	internal void Reroll()
	{
		ReplaceMaterial[] rms = Component.FindObjectsOfType<ReplaceMaterial>();

		LogInternal($"ReRolling {rms.Length}");

		Config.indoorGroupValues.Clear();

		foreach (ReplaceMaterial rm in rms)
		{
			rm.ScanForRenderers();
			rm.m_MaterialsReplaced = false;

			if (Config.instanceToGroup.ContainsKey(rm.gameObject.GetInstanceID()))
			{
				rm.chooseRandom = false;
				string groupName = Config.instanceToGroup[rm.gameObject.GetInstanceID()];
				int rand = 0;
				if (!Config.indoorGroupValues.ContainsKey(groupName))
				{
					rand = new System.Random().Next(0, rm.newMaterial.Count);
					Config.indoorGroupValues.Add(groupName, rand);
				} else
				{
					rand = Config.indoorGroupValues[groupName];
				}
				rm.newMatIndex = rand;
				LogInternal($"- G {rm.gameObject.name} {rand}");

				rm.ReplaceMaterials(rm.sourceMaterial, rand);
			} else
			{
				LogInternal($"- {rm.gameObject.name}");
				rm.chooseRandom = true;
				rm.Update();
			}

			
		}
	}

	internal void LoadFromSave()
	{
		if (Config.sceneSaveData != null)
		{
			LogInternal($"LoadFromSave {Config.sceneSaveData.Length}");
			ReplaceMaterialsManager.DeserializeAll(Config.sceneSaveData);
		}
	}

	internal void DumpScene()
	{

		string dumpFolder = Path.Combine(MelonEnvironment.ModsDirectory, "sol_dump");

		if (!Directory.Exists(dumpFolder))
		{
			Directory.CreateDirectory(dumpFolder);
		}

		StringBuilder sc = new StringBuilder();
		foreach (GameObject go in Config.sceneCache)
		{
			sc.AppendLine(go.name);
		}
		string gofile = Path.Combine(dumpFolder, GameManager.m_ActiveScene + "_GO.csv");
		File.WriteAllText(gofile, sc.ToString());




		Dictionary<string, int> dump = new Dictionary<string, int>();
		MeshRenderer[] mrs = Component.FindObjectsOfType<MeshRenderer>();

		foreach (MeshRenderer mr in mrs)
		{
			GameObject go = mr.gameObject;

			if (mr.material == null)
			{
				continue;
			}

			string materialName = mr.material.name.ToLower().Replace("(instance)", "").Trim();

			if (!dump.ContainsKey(materialName))
			{
				dump.Add(materialName, 0);
			}

			dump[materialName]++;

		}

		StringBuilder sb = new StringBuilder();
		string file = Path.Combine(dumpFolder, GameManager.m_ActiveScene + ".csv");

		foreach (KeyValuePair<string, int> v in dump)
		{
			sb.AppendLine(v.Key + "," + v.Value);
		}

		File.WriteAllText(file, sb.ToString());

	}

	public static void LogInternal(string str)
	{
		LOG.Msg(System.ConsoleColor.Magenta, str);
	}
	public static void LogInternalOnce(string str)
	{
		if (Config.logCache.Contains(str))
		{
			return;
		}
		Config.logCache.Add(str);
		LOG.Msg(System.ConsoleColor.Magenta, str);
	}


}
