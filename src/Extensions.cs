using Il2Cpp;
using Il2CppTLD.PDID;
using UnityEngine;

namespace SpiceOfLife
{
	internal static class Extensions
	{

		internal static bool HasComponent<T>(this GameObject go, bool checkChildren = false, bool checkParent = false) where T : Component
		{

			if (checkChildren && checkParent)
			{
				return go.transform.parent.gameObject.GetComponentsInChildren<T>() != null;
			}

			if (checkChildren)
			{
				return go.GetComponentInChildren<T>() != null;
			}
			if (checkParent)
			{
				return go.transform.parent.gameObject.GetComponent<T>() != null;
			}


			if (!checkChildren && !checkParent)
			{
				return go.GetComponent<T>() != null;
			}
			return false;
		}

		internal static bool AddObjectGuid(this GameObject go)
		{
			if (go.HasComponent<ObjectGuid>())
			{
				return false;
			}

			ObjectGuid guid = go.AddComponent<ObjectGuid>();
			guid.GenerateUniqueGuid();
			Main.LogInternal($"- GUID added to {go.name}");
			return true;

		}

		internal static bool AddReplaceMaterial(this GameObject go, string groupName, List<string> materials)
		{
			if (go.HasComponent<ReplaceMaterial>())
			{
				return false;
			}

			Material? sourceMaterial = Utils.FindSourceMaterial(go);

			if (sourceMaterial == null)
			{
				Main.LogInternal($"- !!SourceMaterial not found {go.name}");
				return false;
			}

			// handle grouping
			if (Config.indoorGroups.Contains(groupName))
			{
				ReplaceMaterial rm = go.AddComponent<ReplaceMaterial>();
				rm.includeChildren = true;
				rm.chooseRandom = false;
				rm.sourceMaterial = sourceMaterial;
				rm.newMaterial = new Il2CppSystem.Collections.Generic.List<Material>();
				foreach (string mat in materials)
				{
					Material? material = Utils.FindMaterialWithName(mat);
					if (material != null && !rm.newMaterial.Contains(material))
					{
						rm.newMaterial.Add(material);
					}
				}

				if (Config.indoorGroupValues.ContainsKey(groupName))
				{
					rm.newMatIndex = Config.indoorGroupValues[groupName];
				}

				if (!Config.indoorGroupValues.ContainsKey(groupName))
				{
					int rand = new System.Random().Next(0, rm.newMaterial.Count);
					rm.newMatIndex = rand;
					Config.indoorGroupValues.Add(groupName, rand);
				}

				Main.LogInternal($"- ReplaceMaterial GROUP added to {go.name}");
				Config.instanceToGroup.Add(go.GetInstanceID(),groupName);
				return true;
			}

			// everything else
			if (!Config.indoorGroups.Contains(groupName))
			{
				ReplaceMaterial rm = go.AddComponent<ReplaceMaterial>();
				rm.includeChildren = true;
				rm.chooseRandom = true;
				rm.sourceMaterial = sourceMaterial;
				rm.newMaterial = new Il2CppSystem.Collections.Generic.List<Material>();
				foreach (string mat in materials)
				{
					Material? material = Utils.FindMaterialWithName(mat);
					if (material != null && !rm.newMaterial.Contains(material))
					{
						rm.newMaterial.Add(material);
					}
				}
				Main.LogInternal($"- ReplaceMaterial added to {go.name}");
				return true;
			}
			return false;
		}



		public static void GenerateUniqueGuid(this ObjectGuid guid)
		{
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(GameManager.m_ActiveScene + guid.transform.position.ToString() + guid.transform.rotation.ToString() + guid.gameObject.name);
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				string newMD5 = Convert.ToHexString(hashBytes).ToLower();
				string newGuid = Utils.MD5ToGuid(newMD5);

				PdidTable.RuntimeRegister(guid, newGuid);
			}
		}




	}
}
