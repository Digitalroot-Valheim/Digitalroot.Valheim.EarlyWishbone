using BepInEx;
using BepInEx.Configuration;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Digitalroot.Valheim.EarlyWishbone
{
  [BepInPlugin(Guid, Name, Version)]
  [BepInDependency(Jotunn.Main.ModGuid, "2.20.0")]
  [NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Minor)]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public partial class Main : BaseUnityPlugin, ITraceableLogging
  {
    private Harmony _harmony;
    [UsedImplicitly] public static ConfigEntry<int> NexusId;
    public static Main Instance;

    public Main()
    {
      Instance = this;
#if DEBUG
      EnableTrace = true;
      Log.RegisterSource(Instance);
#else
      EnableTrace = false;
#endif
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
        NexusId = Config.Bind("General", "NexusID", 2840, new ConfigDescription("Nexus mod ID for updates", null, new ConfigurationManagerAttributes { Browsable = false, ReadOnly = true }));
        _harmony = Harmony.CreateAndPatchAll(typeof(Main).Assembly, Guid);
        CreatureManager.OnVanillaCreaturesAvailable += ModifyVanillaCreatures;
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void ModifyVanillaCreatures()
    {
      // Get a vanilla creature prefab and change some values
      var eikthyr = CreatureManager.Instance.GetCreaturePrefab(BossNames.Eikthyr);
      var characterDropsComponent = eikthyr.GetComponent<CharacterDrop>();
      characterDropsComponent.m_drops.Add(new CharacterDrop.Drop
      {
        m_amountMax = 1
        , m_amountMin = 0
        , m_chance = 25f
        , m_dontScale = true
        , m_levelMultiplier = false
        , m_onePerPlayer = false
        , m_prefab = PrefabManager.Instance.GetPrefab(PrefabNames.Wishbone)

      });

      // Unregister the hook, modified and cloned creatures are kept over the whole game session
      CreatureManager.OnVanillaCreaturesAvailable -= ModifyVanillaCreatures;
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion
  }
}
