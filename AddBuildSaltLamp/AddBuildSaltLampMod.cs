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
            Debug.Log(" === GeneratedBuildings.LoadGeneratedBuildings Prefix === " + AddBuildSaltLamp.ID);

            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildSaltLamp).ToUpper()}.NAME", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"FloorLamp".ToUpper()}.NAME") + "II");
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildSaltLamp).ToUpper()}.DESC", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"FloorLamp".ToUpper()}.DESC"));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildSaltLamp).ToUpper()}.EFFECT", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"FloorLamp".ToUpper()}.EFFECT"));

            List<string> category = (List<string>)TUNING.BUILDINGS.PLANORDER.First(plan => plan.category == PlanScreen.PlanCategory.Furniture).data;
            category.Add(AddBuildSaltLamp.ID);
        }

        //private static void Postfix()
        //{
        //    Debug.Log(" === GeneratedBuildings.LoadGeneratedBuildings Postfix === " + AddBuildSaltLamp.ID);

        //    object obj = Activator.CreateInstance(typeof(AddBuildSaltLamp));
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
            Debug.Log(" === Db.Initialize loaded === " + AddBuildSaltLamp.ID);

            List<string> tech = new List<string>(Database.Techs.TECH_GROUPING["FineDining"]) { AddBuildSaltLamp.ID };
            Database.Techs.TECH_GROUPING["FineDining"] = tech.ToArray();
        }
    }
}