using Il2Cpp;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Specialized;
using System.Collections;

namespace SpiceOfLife
{
	internal class Utils
	{


		internal static void BuildSceneCache()
		{
			List<string> toFind = new List<string>();
			foreach (KeyValuePair<string,List<string>> kvp in Config.replaceDict)
			{
				toFind.Add(kvp.Key.ToLower());
			}
			Config.findObjects = toFind.ToArray();


			if (Config.sceneCache.Count == 0)
			{
				Config.sceneCache = UnityEngine.Object.FindObjectsOfType<GameObject>().Where(IgnoreFromCache).ToHashSet();
				Main.LogInternal($"Scene Cache Built {Config.sceneCache.Count}");

				foreach (string str in Config.removeFromCache)
				{
					Config.sceneCache.RemoveWhere(obj => obj.name.ToLower().Contains(str.ToLower()));
					//Config.sceneCache.RemoveWhere(RemoveFromCache);
				}

				Main.LogInternal($"Scene Cache Trimmed {Config.sceneCache.Count}");
			}
		}

		private static bool IgnoreFromCache(GameObject go)
		{
			return Config.findObjects.Any(go.name.ToLower().StartsWith);
		}

		internal static List<GameObject> FindSceneGameObjectsByName(string name, bool startswith = false, bool contains = false)
		{

			if (startswith)
			{
				return Config.sceneCache.Where(obj => obj.name.ToLower().StartsWith(name.ToLower())).Cast<GameObject>().ToList();
			}
			if (contains)
			{
				return Config.sceneCache.Where(obj => obj.name.ToLower().Contains(name.ToLower())).Cast<GameObject>().ToList();
			}

			if (!startswith && !contains)
			{
				return Config.sceneCache.Where(obj => obj.name.ToLower() == (name.ToLower())).Cast<GameObject>().ToList();
			}
			return new List<GameObject>();
		}



		internal static Material? FindMaterialWithName(string name)
		{

			Material? material = null;
			string foundIn = "NOT_FOUND";

			if (Config.materialCache.ContainsKey(name))
			{
				material = Config.materialCache[name];
				if (material != null)
				{
					foundIn = "Cache";
				}
			}

			//if (material == null)
			//{
			//	material = Resources.FindObjectsOfTypeAll<Material>().Where(obj => obj.name.ToLower().Contains(name.ToLower())).Cast<Material>().FirstOrDefault();
			//	if (material != null)
			//	{
			//		foundIn = "R.FindObjectsOfTypeAll";
			//	}
			//}
			//if (material == null)
			//{
			//	material = UnityEngine.Object.FindObjectsOfTypeIncludingAssets(Il2CppSystem.Type.GetType("Material")).Where(obj => obj.name.ToLower().Contains(name.ToLower())).Cast<Material>().FirstOrDefault();
			//	if (material != null)
			//	{
			//		foundIn = "O.FindObjectsOfTypeIncludingAssets";
			//	}
			//}
			if (material == null)
			{
				material = Addressables.LoadAssetAsync<Material>(name).WaitForCompletion();
				if (material != null)
				{
					foundIn = "A.LoadAssetAsync";
				}
			}

			if (material != null && !Config.materialCache.ContainsKey(name))
			{
				material.enableInstancing = true;
				Config.materialCache.Add(name, material);
			}

			Main.LogInternalOnce($"- Material {name} found {foundIn}");

			return material;

		}

		internal static string MD5ToGuid(string md5)
		{
			return String.Concat(md5.Substring(0, 8), "-", md5.Substring(8, 4), "-", md5.Substring(12, 4), "-", md5.Substring(16, 4), "-", md5.Substring(20));
		}


		internal static Material? FindSourceMaterial(GameObject go)
		{
			Material sourceMaterial = null;

			List<MeshRenderer> mrs = go.GetComponentsInChildren<MeshRenderer>().ToList();
			//Main.LogInternal($"MeshRenderers found {mrs.Count}");
			OrderedDictionary mdict = new OrderedDictionary();
			Dictionary<string, Material> matDict = new Dictionary<string, Material>();
			foreach (MeshRenderer mr in mrs)
			{
				if (!mdict.Contains(mr.material.name))
				{
					mdict.Add(mr.material.name, 0);
					matDict.Add(mr.material.name, mr.material);
				}
				mdict[mr.material.name] = (int)mdict[mr.material.name] + 1;
			}
			IDictionaryEnumerator myEnumerator = mdict.GetEnumerator();
			int c = 0;
			string sourceMaterialName = null;
			while (myEnumerator.MoveNext())
			{
				string matname = (string)myEnumerator.Key;
				int count = (int)myEnumerator.Value;
				//Main.LogInternal($"{matname} {count}");
				if (count > c)
				{
					c = count;
					sourceMaterialName = matname;
				}
			}

			if (sourceMaterialName != null && matDict.ContainsKey(sourceMaterialName))
			{
				sourceMaterial = matDict[sourceMaterialName];
			}


			if (sourceMaterial != null)
			{
				string sourceMaterialNameClean = sourceMaterialName.Replace("(instance)", "").Trim();
				if (!Config.materialCache.ContainsKey(sourceMaterialNameClean))
				{
					Config.materialCache.Add(sourceMaterialNameClean, sourceMaterial);
				}
				//Main.LogInternal($"SourceMaterial found {sourceMaterialNameClean}");
			}

			return sourceMaterial;
		}


	}
}
