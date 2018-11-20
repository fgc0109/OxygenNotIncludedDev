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
            Debug.Log(" === GeneratedBuildings.LoadGeneratedBuildings Prefix === " + AddDrywallHidePipes.ID);

            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddDrywallHidePipes).ToUpper()}.NAME", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"ExteriorWall".ToUpper()}.NAME") + "II");
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddDrywallHidePipes).ToUpper()}.DESC", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"ExteriorWall".ToUpper()}.DESC"));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{nameof(AddDrywallHidePipes).ToUpper()}.EFFECT", Strings.Get($"STRINGS.BUILDINGS.PREFABS.{"ExteriorWall".ToUpper()}.EFFECT"));

            List<string> category = (List<string>)TUNING.BUILDINGS.PLANORDER.First(plan => plan.category == PlanScreen.PlanCategory.Utilities).data;
            category.Add(AddDrywallHidePipes.ID);
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
    internal class InverseRefinedObjectsTechMod
    {
        private static void Prefix()
        {
            Debug.Log(" === Db.Initialize loaded === " + AddDrywallHidePipes.ID);

            List<string> tech = new List<string>(Database.Techs.TECH_GROUPING["RefinedObjects"]) { AddDrywallHidePipes.ID };
            Database.Techs.TECH_GROUPING["RefinedObjects"] = tech.ToArray();
        }
    }

    [HarmonyPatch(typeof(KSerialization.Manager), "GetType", new Type[] { typeof(string) })]
    public static class AddDrywallHidePipes_ObjectsSerializationPatch
    {
        public static void Postfix(string type_name, ref Type result)
        {
            if (type_name == "AddDrywallHidePipes.ZoneTileClone")
            {
                result = typeof(ZoneTileClone);
            }
        }
    }
}