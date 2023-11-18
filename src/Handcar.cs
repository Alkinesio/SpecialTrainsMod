// Ignore Spelling: Sel

using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using SuperTrains.Utilities;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using Vintagestory.API.Client;
using System.Diagnostics;
using System;

using Entities = SuperTrains.Utilities.Entities;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Config;

namespace SuperTrains
{

    public sealed class Handcar : SimpleTransport
    {
        /// <summary> Entity name for handcar. </summary>
        private readonly string handcarEntityName = "handcarEntity";

        private BlockFacing orientation, direction;

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {

            // Set orientation (flip if horizontal/vertical)
            orientation = SuggestedHVOrientation(byPlayer, blockSel)[0];

            // Set direction
            direction = BlockFacing.HorizontalFromAngle(GameMath.Mod(byPlayer.Entity.Pos.Yaw, (float)Math.PI * 2f));

            // Check for placeable block
            if (!this.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return false;
            }

            // Define direction type
            string directionType = "e";

            // Set direction type on different value
            if (direction == BlockFacing.NORTH)
            {
                directionType = "n";
            }
            else if (direction == BlockFacing.WEST)
            {
                directionType = "w";
            }
            else if (direction == BlockFacing.SOUTH)
            {
                directionType = "s";
            }

            // Set code based on orientation
            Code = CodeWithVariant("type", directionType);

            // Set asset location from parent based on orientation
            SetAssociatedTransport(new AssetLocation(Code.Domain, $"{handcarEntityName}-{directionType}"));

            // Call parent's method (block transport will be spawned here)
            if (!base.DoPlaceBlock($"{FirstCodePart()}-{directionType}", world, byPlayer, blockSel, itemstack))
            {
                return false;
            }

            return true;
        }

        public override bool CanPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref string failureCode)
        {
            // Check parent's conditions
            if (!base.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return false;
            }

            // Check for flat bottom rail blocks
            for (int i = 0; i < 2; i++)
            {
                // Define offset axes
                int x, y, z;

                // Set offset's axes
                x = orientation.IsAxisWE ? (direction == BlockFacing.EAST ? i : -i) : 0;
                y = 0;
                z = orientation.IsAxisNS ? (direction == BlockFacing.SOUTH ? i : -i) : 0;

                // Set offset
                BlockPos offset = new(x, y, z);

                // Set block instance
                Block bottomBlock = Blocks.GetBlockToDown(world, blockSel.Position + offset);

                // Check
                if (!Rails.IsFlatRailBlock(bottomBlock)
                    ||
                    !Rails.FlatRailBlockWith(bottomBlock, orientation.IsAxisNS ? "ns" : "we")
                    )
                {
                    // Alert client
                    (api as ICoreClientAPI)?.TriggerIngameError(this, "notonrails", "Handcar can be placed only on 2 x 1 flat rails!");

                    return false;
                }
            }

            return true;
        }
    }
}