// Ignore Spelling: Sel

using SuperTrains.Utilities;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

using Assets = SuperTrains.Utilities.AssetLocations;
using Entities = SuperTrains.Utilities.Entities;

namespace SuperTrains
{
    public abstract class SimpleTransport : Block, IWrenchOrientable
    {
        /// <summary> Required bottom rails. </summary>
        private SimpleRailsBlock bottomRails;
        /// <summary> Position for required bottom rails. </summary>
        private BlockPos bottomRailsPosition;
        /// <summary> (Optional) Associated type entity for the transport. </summary>
        private AssetLocation associatedTransport;
        /// <summary>
        /// Entity transport obtained from <see cref="associatedTransport">associated transport</see> asset.
        /// </summary>
        private Entity entityTransport;

        public SimpleRailsBlock GetDownsideRails() => bottomRails;
        public BlockPos GetDownsideRailsPosition() => bottomRailsPosition;
        public AssetLocation GetAssociatedTransport() => associatedTransport;
        public Entity GetEntityTransport() => entityTransport;

        public void SetAssociatedTransport(AssetLocation associatedTransport)
        {
            this.associatedTransport = associatedTransport;
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            IPlayer player = null;
            if (byEntity is EntityPlayer)
            {
                player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
            }
            if (player == null)
            {
                return;
            }
            if (!byEntity.World.Claims.TryAccess(player, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
            {
                return;
            }
            Block block = this.api.World.GetBlock(new AssetLocation(this.Code.Domain + this.Code.Path));
            if (block == null || block is not SimpleTransport)
            {
                (api as ICoreClientAPI)?.TriggerIngameError(this, "notsuchtransport", $"{this.Code.Domain + this.Code.Path} not found as simple transport!");
                return;
            }
            BlockPos blockPos = blockSel.Position.AddCopy(blockSel.Face);
            string text = "";
            BlockSelection blockSelection = blockSel.Clone();
            blockSelection.Position.Add(blockSel.Face, 1);
            if (((block as SimpleTransport).TryPlaceBlock(this.api.World, player, slot.Itemstack, blockSelection, ref text)))
            {
                if (player.WorldData.CurrentGameMode != EnumGameMode.Creative)
                {
                    slot.TakeOut(1);
                    slot.MarkDirty();
                }
                this.api.World.PlaySoundAt(block.Sounds.Place, (double)blockPos.X + 0.5, (double)blockPos.Y, (double)blockPos.Z + 0.5, player, true, 32f, 1f);
                handling = EnumHandHandling.PreventDefault;
                return;
            }
        }

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            // Check placeable block
            if (!this.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return false;
            }

            return base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
        }

        /// <summary> DoPlaceBlock but with a custom path that would be useful to change variant before to place it. </summary>
        public bool DoPlaceBlock(string customPath, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
        => world.GetBlock(CodeWithPath(customPath)).DoPlaceBlock(world, byPlayer, blockSel, byItemStack);


        public override bool CanPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref string failureCode)
        {
            // Call parent's method
            if (!base.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                (api as ICoreClientAPI)?.TriggerIngameError(this, "nope", "NOPE!");

                return false;
            }

            // Check for bottom rails
            if (!CheckBottomRails(world, blockSel))
            {
                (api as ICoreClientAPI)?.TriggerIngameError(this, "notonrails", $"Could not place {Code} because not on rails!");

                return false;
            }

            // Alert for missing associated entity transport
            if (associatedTransport == null)
            {
                world.Logger.Warning("Associated entity transport for {0} transport block is missing!", Code);
            }

            return true;
        }

        public bool CheckBottomRails(IWorldAccessor world, BlockSelection blockSel)
        {
            // Get coordinates as block position
            BlockPos position = blockSel.Position - new BlockPos(0, 1, 0);

            // Must place on simple rails
            if (!Rails.IsThereRailBlock(world, position))
            {
                return false;
            }

            // Set required rails parameters
            bottomRails = Rails.GetRailBlock(world, position);
            bottomRailsPosition = position - new BlockPos(Blocks.FaceToCoordinates(BlockFacing.DOWN));

            return true;
        }

        /// <summary> Generate associated transport on wrench interaction if possible. </summary>
        void IWrenchOrientable.Rotate(EntityAgent byEntity, BlockSelection blockSel, int dir)
        {
            // Check for missing entity
            if (byEntity == null)
            {
                return;
            }

            // Get world by entity
            IWorldAccessor world = byEntity.World;

            // Check for missing world
            if (world == null)
            {
                return;
            }

            // Check for valid associated transport
            if (!Assets.Check(world, associatedTransport))
            {
                return;
            }

            // Get associated transport
            entityTransport = Entities.Get(world, associatedTransport);

            // Spawn associated transport
            if (!Entities.Spawn(world, blockSel, entityTransport))
            {
                return;
            }

            // Remove block type
            world.BlockAccessor.RemoveBlockEntity(blockSel.Position);
        }
    }
}