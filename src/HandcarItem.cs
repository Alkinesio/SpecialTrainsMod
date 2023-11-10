// Ignore Spelling: Sel

using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using SuperTrains.Utilities;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using Vintagestory.API.Client;
using System.Diagnostics;
using System;

namespace SuperTrains
{

    public sealed class HandcarItem : SimpleTransport
    {
        private BlockFacing orientation = null;
        private Entity transport = null;
        private readonly string handCarCodeEntity = "handcar_entity";

        /// <summary>Called before to try and place the transport (to check conditions).</summary>
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            if (blockSel == null)
                return;

            Debug.WriteLine("Handcar start!");

            // TODO: Check that is placed on 2x1 simple rails blocks (and not only 1x1)

            // Get placing player
            IPlayer byPlayer = byEntity.World.PlayerByUid((byEntity as EntityPlayer).PlayerUID);

            // Get world by entity
            IWorldAccessor world = byEntity.World;

            // TODO: check that is greater or equal to zero at least
            // Set orientation (flip if horizontal/vertical)
            orientation = Block.SuggestedHVOrientation(byPlayer, blockSel)[0];

            // Set asset location from parent based on orientation
            base.SetAssetLocation(new AssetLocation(Code.Domain, handCarCodeEntity + (orientation.IsAxisNS ? "-ns" : "-we")));

            // Call parent's method (here spawn the transport)
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);

            // Get placing block position
            BlockPos position = blockSel.Position;
            
            // Define collision and selection boxes
            Cuboidf[] collisionBoxes = new Cuboidf[] {
                new Cuboidf(x1: 0, y1: 0, z1: orientation.IsAxisWE ? 0 : -1, x2: orientation.IsAxisWE ? 2 : 1, y2: 0.25f, z2: 1)
            };
            Cuboidf[] selectionBoxes = new Cuboidf[] {
                new Cuboidf(x1: 0, y1: 0, z1: orientation.IsAxisWE ? 0 : -1, x2: orientation.IsAxisWE ? 2 : 1, y2: 0.25f, z2: 1)
            };

            // Set collision and selection boxes
            world.BlockAccessor.GetBlock(position).CollisionBoxes = collisionBoxes;
            world.BlockAccessor.GetBlock(position).SelectionBoxes = selectionBoxes;

            // Get rails block and Its position
            SimpleRailsBlock rails = GetDownsideRails();
            BlockPos railsPos = GetDownsideRailsPos();

            // Check for rails
            if (rails == null)
                return;

            Debug.WriteLine("Rails block is not null, nice!");

            // Get transport
            transport = GetTransport();

            // Check for transport
            if (transport == null)
                return;

            Debug.WriteLine("Transport is not null, nice!");

            // Set initial position for the placing block
            float railsHeight = rails.GetCollisionBoxes(world.BlockAccessor, railsPos)[0].Height;
            float handcarHeight = 0;
            if (transport.CollisionBox != null)
                handcarHeight = transport.CollisionBox.Height;
            transport.Pos = new EntityPos(position.X, railsPos.Y + railsHeight / 2 + handcarHeight / 2, blockSel.Position.Z);

            Debug.WriteLine("Conditions for handcar correctly initialized!");
        }

    }
}