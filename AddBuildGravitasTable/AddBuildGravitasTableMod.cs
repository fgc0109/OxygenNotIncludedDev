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
            Debug.Log(" === GeneratedBuildings.LoadGeneratedBuildings Prefix === " + AddBuildGravitasTable.ID);

            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildGravitasTable).ToUpper()}.NAME", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"DiningTable".ToUpper()}.NAME") + "II");
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildGravitasTable).ToUpper()}.DESC", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"DiningTable".ToUpper()}.DESC"));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddBuildGravitasTable).ToUpper()}.EFFECT", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"DiningTable".ToUpper()}.EFFECT"));

            List<string> category = (List<string>)TUNING.BUILDINGS.PLANORDER.First(plan => plan.category == PlanScreen.PlanCategory.Furniture).data;
            category.Add(AddBuildGravitasTable.ID);
        }

        //private static void Postfix()
        //{
        //    Debug.Log(" === GeneratedBuildings.LoadGeneratedBuildings Postfix === " + AddBuildGravitasTable.ID);

        //    object obj = Activator.CreateInstance(typeof(AddBuildGravitasTable));
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
            Debug.Log(" === Db.Initialize loaded === " + AddBuildGravitasTable.ID);

            List<string> tech = new List<string>(Database.Techs.TECH_GROUPING["FineDining"]) { AddBuildGravitasTable.ID };
            Database.Techs.TECH_GROUPING["FineDining"] = tech.ToArray();
        }
    }
}