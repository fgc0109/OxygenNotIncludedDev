using System;
using TUNING;
using UnityEngine;

public class AddBuildGravitasTable : IBuildingConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string name = "AddBuildGravitasTable";
        string anim = "gravitas_table_kanim";
        int width = 3;
        int height = 1;
        int hitpoints = 10;
        float construction_time = 10f;
        float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
        string[] all_METALS = MATERIALS.ALL_METALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(name, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
        buildingDef.WorkTime = 20f;
        buildingDef.Overheatable = false;
        buildingDef.AudioCategory = "Metal";
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        go.AddOrGet<LoopingSounds>();
        go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.MessTable);
        go.AddOrGet<MessStation>();
        Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
        storage.showInUI = true;
        go.AddOrGet<AnimTileable>();
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        go.GetComponent<KAnimControllerBase>().initialAnim = "off";
        Ownable ownable = go.AddOrGet<Ownable>();
        ownable.slotID = Db.Get().AssignableSlots.MessStation.Id;
    }

    public const string ID = "AddBuildGravitasTable";
}
