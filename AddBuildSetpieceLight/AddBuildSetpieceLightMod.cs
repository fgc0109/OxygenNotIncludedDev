using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace OxygenNotIncludedModDev
{
    /// <summary>
    /// 向BUILDINGS注入数据
    /// </summary>
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class Patches_GeneratedBuildings_LoadGeneratedBuildings
    {
        private static void Prefix()
        {
            Debug.Log(" === GeneratedBuildings.LoadGeneratedBuildings Prefix === " + AddBuildSetpieceLight.ID);

            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildSetpieceLight).ToUpper()}.NAME", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"CeilingLight".ToUpper()}.NAME") + "II");
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildSetpieceLight).ToUpper()}.DESC", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"CeilingLight".ToUpper()}.DESC"));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildSetpieceLight).ToUpper()}.EFFECT", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"CeilingLight".ToUpper()}.EFFECT"));

            List<string> category = (List<string>)TUNING.BUILDINGS.PLANORDER.First(plan => plan.category == PlanScreen.PlanCategory.Furniture).data;
            category.Add(AddBuildSetpieceLight.ID);
        }

        //private static void Postfix()
        //{
        //    Debug.Log(" === GeneratedBuildings.LoadGeneratedBuildings Postfix === " + AddBuildSetpieceLight.ID);

        //    object obj = Activator.CreateInstance(typeof(AddBuildSetpieceLight));
        //    BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        //}
    }

    /// <summary>
    /// 向TECH_GROUPING注入数据
    /// </summary>
    [HarmonyPatch(typeof(Db), "Initialize")]
    internal class InverseElectrolyzerTechMod
    {
        private static void Prefix()
        {
            Debug.Log(" === Db.Initialize loaded === " + AddBuildSetpieceLight.ID);

            List<string> tech = new List<string>(Database.Techs.TECH_GROUPING["InteriorDecor"]) { AddBuildSetpieceLight.ID };
            Database.Techs.TECH_GROUPING["InteriorDecor"] = tech.ToArray();
        }
    }
}