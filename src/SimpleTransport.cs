// Ignore Spelling: Sel

using SuperTrains.Utilities;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace SuperTrains
{
    public abstract class SimpleTransport : Item
    {
        private SimpleRailsBlock requiredRails;
        private BlockPos requiredRailsPos;
        private Entity transport;

        /// <summary>
        /// Could be changed by children because of variations
        /// </summary>
        private AssetLocation assetLocation;

        /// <summary>
        /// Check and spawn the transport on interact (you can <see cref="SetAssetLocation">set asset location</see> before to call this method to set different code for variants).
        /// </summary>
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            CheckAndSpawn(slot, byEntity, blockSel, ref handHandling);
        }

        public void SetAssetLocation(AssetLocation assetLocation)
        {
            this.assetLocation = assetLocation;
        }

        public bool CheckAndSpawn(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, ref EnumHandHandling handHandling)
        {
            if (CanBeSpawned(slot, byEntity, blockSel))
            {
                Spawn(byEntity.World, ref handHandling);
                Debug.WriteLine("Should be placed!");
                return true;
            }
            else
            {
                Debug.WriteLine("Not should be placed!");
            }

            return false;
        }

        public bool CanBeSpawned(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel)
        {
            if (!CheckForRails(byEntity, blockSel))
            {
                return false;
            }

            if (blockSel == null)
            {
                return false;
            }

            EntityPlayer ePlayer = null;
            if (byEntity is EntityPlayer)
                ePlayer = byEntity as EntityPlayer;

            IPlayer player = byEntity.World.PlayerByUid((ePlayer).PlayerUID);
            if (!byEntity.World.Claims.TryAccess(player, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
            {
                return false;
            }

            if (ePlayer == null || player.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                slot.TakeOut(1);
                slot.MarkDirty();
            }

            EntityProperties entityType = byEntity.World.GetEntityType(assetLocation);
            if (entityType == null)
            {
                byEntity.World.Logger.Error("ItemTransport: No such entity - {0}", assetLocation);
                if (api.World.Side == EnumAppSide.Client)
                {
                    (api as ICoreClientAPI).TriggerIngameError(this, "nosuchentity", "No such entity loaded - '" + assetLocation + "'.");
                }

                return false;
            }

            transport = byEntity.World.ClassRegistry.CreateEntity(entityType);
            if (transport == null)
            {
                return false;
            }

            transport.ServerPos.X = (float)(blockSel.Position.X + ((!blockSel.DidOffset) ? blockSel.Face.Normali.X : 0)) + 0.5f;
            transport.ServerPos.Y = blockSel.Position.Y + ((!blockSel.DidOffset) ? blockSel.Face.Normali.Y : 0);
            transport.ServerPos.Z = (float)(blockSel.Position.Z + ((!blockSel.DidOffset) ? blockSel.Face.Normali.Z : 0)) + 0.5f;
            transport.ServerPos.Yaw = (float)byEntity.World.Rand.NextDouble() * 2f * (float)Math.PI;
            transport.Pos.SetFrom(transport.ServerPos);
            transport.PositionBeforeFalling.Set(transport.ServerPos.X, transport.ServerPos.Y, transport.ServerPos.Z);
            transport.Attributes.SetString("origin", "playerplaced");

            if (Attributes != null && Attributes.IsTrue("setGuardedEntityAttribute"))
            {
                transport.WatchedAttributes.SetLong("guardedEntityId", byEntity.EntityId);
                if (ePlayer != null)
                {
                    transport.WatchedAttributes.SetString("guardedPlayerUid", ePlayer.PlayerUID);
                }
            }

            return true;
        }

        private void Spawn(IWorldAccessor world, ref EnumHandHandling handHandling)
        {
            if (world == null || transport == null)
                return;

            world.SpawnEntity(transport);
            handHandling = EnumHandHandling.PreventDefaultAction;
        }

        public Entity GetTransport()
        {
            return transport;
        }

        public SimpleRailsBlock GetDownsideRails()
        {
            return requiredRails;
        }

        public BlockPos GetDownsideRailsPos()
        {
            return requiredRailsPos;
        }

        private bool CheckForRails(EntityAgent byEntity, BlockSelection blockSel)
        {
            Debug.WriteLine("Start!");

            // Get coordinates as block position
            BlockPos position = blockSel.Position;

            // Get world by entity
            IWorldAccessor world = byEntity.World;

            Debug.WriteLine("POS : " + position);

            // Must place on simple rails
            if (!Rails.IsThereRailBlock(world, position))
            {
                return false;
            }

            Debug.WriteLine("Yea, there is a rails downside!");

            // Set required rails parameters
            requiredRails = Rails.GetRailBlock(world, position);
            requiredRailsPos = position - new BlockPos(Blocks.FaceToCoordinates(BlockFacing.DOWN));

            Debug.WriteLine("Conditions for simple transport correctly initialized!");
            return true;
        }
    }
}