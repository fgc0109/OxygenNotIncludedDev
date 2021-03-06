﻿using System;
using TUNING;
using UnityEngine;

public class AddBuildSetpieceLight : IBuildingConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string name = "AddBuildSetpieceLight";
        string anim = "setpiece_light_kanim";
        int width = 1;
        int height = 1;
        int hitpoints = 10;
        float construction_time = 10f;
        float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
        string[] all_METALS = MATERIALS.ALL_METALS;
        float melting_point = 800f;
        BuildLocationRule build_location_rule = BuildLocationRule.OnCeiling;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(name, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, none, 0.2f);
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 10f;
        buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
        buildingDef.ViewMode = SimViewMode.Light;
        buildingDef.AudioCategory = "Metal";
        return buildingDef;
    }

    public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
    {
        LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
        lightShapePreview.lux = 1800;
        lightShapePreview.radius = 8f;
        lightShapePreview.shape = LightShape.Cone;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        go.AddOrGet<LoopingSounds>();
        Light2D light2D = go.AddOrGet<Light2D>();
        light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
        light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
        light2D.Range = 8f;
        light2D.Angle = 2.6f;
        light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
        light2D.Offset = LIGHT2D.CEILINGLIGHT_OFFSET;
        light2D.shape = LightShape.Cone;
        light2D.drawOverlay = true;
        light2D.Lux = 1800;
        go.AddOrGetDef<LightController.Def>();
    }

    public const string ID = "AddBuildSetpieceLight";
}
