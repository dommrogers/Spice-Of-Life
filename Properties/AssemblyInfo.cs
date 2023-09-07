using MelonLoader;
using System.Reflection;

[assembly: AssemblyTitle(BuildInfo.ModName)]
[assembly: AssemblyCopyright($"Created by STBlade")]

[assembly: AssemblyVersion(BuildInfo.ModVersion)]
[assembly: AssemblyFileVersion(BuildInfo.ModVersion)]
[assembly: MelonInfo(typeof(SpiceOfLife.Main), BuildInfo.ModName, BuildInfo.ModVersion, BuildInfo.ModAuthor)]

[assembly: MelonGame("Hinterland", "TheLongDark")]

internal static class BuildInfo
{
	internal const string ModName = "Spice of Life";
	internal const string ModAuthor = "STBlade";
	internal const string ModVersion = "1.0.0";
}