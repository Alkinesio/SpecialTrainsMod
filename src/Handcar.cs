// Ignore Spelling: supertrains

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace supertrains.src
{
    public class Handcar : Block, IWrenchOrientable
    {
        private readonly string handCarCodeEntity = "handcar_entity";

        /// <summary> Convert handcar as entity instead to rotate with Wrench. </summary>
        void IWrenchOrientable.Rotate(EntityAgent byEntity, BlockSelection blockSel, int dir)
        {
            // Spawn entity transport
            // byEntity.World.SpawnEntity(GetEntityTransport());

            // Remove block
            byEntity.World.BlockAccessor.RemoveBlockEntity(blockSel.Position);
        }

        // Entity GetEntityTransport(IWorldAccessor world) => world.GetEntityType(new AssetLocation(handCarCodeEntity)).;
    }
}
