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
        private Block blockTransport = null;
        private Entity entityTransport = null;
        private readonly string handCarCodeBlock = "handcar";

        /// <summary>Called before to try and place the transport (to check conditions).</summary>
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            if (blockSel == null)
                return;

            // TODO: Check that is placed on 2x1 simple rails blocks (and not only 1x1)

            // Get placing player
            IPlayer byPlayer = byEntity.World.PlayerByUid((byEntity as EntityPlayer).PlayerUID);

            // TODO: check that is greater or equal to zero at least
            // Set orientation (flip if horizontal/vertical)
            orientation = Block.SuggestedHVOrientation(byPlayer, blockSel)[0];

            if (api is ICoreClientAPI capi)
            {
                capi.SendChatMessage("Ok!");
            }

            // Set asset location from parent based on orientation
            base.SetAssetLocation(new AssetLocation(Code.Domain, handCarCodeBlock + (orientation.IsAxisNS ? "-ns" : "-we")));

            // Call parent's method (here spawn the transport)
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);

            // Get rails block and Its position
            SimpleRailsBlock rails = GetDownsideRails();
            BlockPos railsPos = GetDownsideRailsPos();

            // Check for 1x1 rails block
            if (rails == null)
                return;

            // Check for 2x1 rails block
            // if ()

            // Get transport
            blockTransport = GetTransport();

            // Check for transport
            if (blockTransport == null)
                return;

            // Define collision and selection boxes
            Cuboidf[] collisionBoxes = new Cuboidf[] {
                new Cuboidf(x1: 0, y1: 0, z1: orientation.IsAxisWE ? 0 : -1, x2: orientation.IsAxisWE ? 2 : 1, y2: 0.25f, z2: 1)
            };
            Cuboidf[] selectionBoxes = new Cuboidf[] {
                new Cuboidf(x1: 0, y1: 0, z1: orientation.IsAxisWE ? 0 : -1, x2: orientation.IsAxisWE ? 2 : 1, y2: 0.25f, z2: 1)
            };

            // Set collision and selection boxes
            blockTransport.CollisionBoxes = collisionBoxes;
            blockTransport.SelectionBoxes = selectionBoxes;
        }

    }
}