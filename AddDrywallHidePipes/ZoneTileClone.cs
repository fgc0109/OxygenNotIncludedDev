using System;
using ProcGen;

internal class ZoneTileClone : KMonoBehaviour
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        int cell = Grid.PosToCell(this);
        for (int i = 0; i < this.width; i++)
        {
            for (int j = 0; j < this.height; j++)
            {
                int cell2 = Grid.OffsetCell(cell, i, j);
                SimMessages.ModifyCellWorldZone(cell2, 0);
            }
        }
    }

    protected override void OnCleanUp()
    {
        base.OnCleanUp();
        int cell = Grid.PosToCell(this);
        for (int i = 0; i < this.width; i++)
        {
            for (int j = 0; j < this.height; j++)
            {
                int cell2 = Grid.OffsetCell(cell, i, j);
                SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(cell2);
                byte zone_id = (subWorldZoneType != SubWorld.ZoneType.Space) ? ((byte)subWorldZoneType) : byte.MaxValue;
                SimMessages.ModifyCellWorldZone(cell2, zone_id);
            }
        }
    }

    public int width = 1;

    public int height = 1;
}
