using TUNING;
using UnityEngine;

public class AddDrywallHidePipes : IBuildingConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string id = "AddDrywallHidePipes";
        int width = 1;
        int height = 1;
        string anim = "walls_kanim";
        int hitpoints = 30;
        float construction_time = 30f;
        float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
        string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, raw_MINERALS, melting_point, build_location_rule, DECOR.NONE, none, 0.2f);
        buildingDef.Entombable = false;
        buildingDef.Floodable = false;
        buildingDef.Overheatable = false;
        buildingDef.AudioCategory = "Metal";
        buildingDef.BaseTimeUntilRepair = -1f;
        buildingDef.DefaultAnimState = "off";
        buildingDef.ObjectLayer = ObjectLayer.Backwall;
        buildingDef.SceneLayer = Grid.SceneLayer.LogicWireBridgesFront;
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        GeneratedBuildings.MakeBuildingAlwaysOperational(go);
        AnimTileable animTileable = go.AddOrGet<AnimTileable>();
        animTileable.objectLayer = ObjectLayer.Backwall;
        go.AddComponent<ZoneTileClone>();
        BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        GeneratedBuildings.RemoveLoopingSounds(go);
    }

    public const string ID = "AddDrywallHidePipes";
}
