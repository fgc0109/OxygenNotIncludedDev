using System;
using TUNING;
using UnityEngine;

public class AddBuildSaltLamp : IBuildingConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string id = "AddBuildSaltLamp";
        int width = 1;
        int height = 2;
        string anim = "saltlamp_kanim";
        int hitpoints = 10;
        float construction_time = 10f;
        float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
        string[] all_METALS = MATERIALS.ALL_METALS;
        float melting_point = 800f;
        BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, none, 0.2f);
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 8f;
        buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
        buildingDef.ViewMode = SimViewMode.Light;
        buildingDef.AudioCategory = "Metal";
        return buildingDef;
    }

    public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
    {
        LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
        lightShapePreview.lux = 1000;
        lightShapePreview.radius = 4f;
        lightShapePreview.shape = LightShape.Circle;
        lightShapePreview.offset = new CellOffset((int)def.BuildingComplete.GetComponent<Light2D>().Offset.x, (int)def.BuildingComplete.GetComponent<Light2D>().Offset.y);
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        go.AddOrGet<EnergyConsumer>();
        go.AddOrGet<LoopingSounds>();
        Light2D light2D = go.AddOrGet<Light2D>();
        light2D.overlayColour = LIGHT2D.FLOORLAMP_OVERLAYCOLOR;
        light2D.Color = LIGHT2D.FLOORLAMP_COLOR;
        light2D.Range = 4f;
        light2D.Angle = 0f;
        light2D.Direction = LIGHT2D.FLOORLAMP_DIRECTION;
        light2D.Offset = LIGHT2D.FLOORLAMP_OFFSET;
        light2D.shape = LightShape.Circle;
        light2D.drawOverlay = true;
        go.AddOrGetDef<LightController.Def>();
    }

    public const string ID = "AddBuildSaltLamp";
}
