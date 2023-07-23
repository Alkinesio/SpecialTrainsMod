using SuperTrains.Utilities;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Blocks = SuperTrains.Utilities.Blocks;


namespace SuperTrains
{
    public class SimpleRailsBlock : Block
    {

        /// <summary>Try and place the block.</summary>
        /// <returns>True if the blocked is correctly placed.</returns>
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            // Check if can place the block (by default, returns with error in the case)
            if (!this.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return false;
            }

            // Cannot place rails on other rails
            if (Rails.countRailsOnUD(world, blockSel.Position) > 0)
            {
                return false;
            }

            // Set orientation (flip if horizontal/vertical)
            BlockFacing targetFacing = Block.SuggestedHVOrientation(byPlayer, blockSel)[0];

            // Check for curves
            if (this.TryPlaceCurve(world, byPlayer, blockSel.Position))
            {
                return true;
            }

            // Check for raised
            if (this.TryToPlaceRaised(world, byPlayer, blockSel.Position))
            {
                return true;
            }

            // Place the block as flat with got values
            world.GetBlock(base.CodeWithParts(targetFacing.Axis == EnumAxis.Z ? "flat_ns" : "flat_we"))
                .DoPlaceBlock(world, byPlayer, blockSel, itemstack);
            return true;
        }

        public override bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1)
        {
            // Call default method
            if (!base.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, 0))
            {
                return false;
            }

            // Set block position
            BlockPos position = blockSel.Position;

            // Set player
            IPlayer player = world.PlayerByUid((byEntity as EntityPlayer).PlayerUID);

            // Update linked rails (set horizontal/vertical orientation)
            return updateCloseRails(null, world, player, position);
        }

        /// <summary>
        /// Try to place rails as curve type if possible, else nothing will happen.
        /// </summary>
        /// <returns>True if placed as curve.</returns>
        private bool TryPlaceCurve(IWorldAccessor world, IPlayer byPlayer, BlockPos position)
        {
            // Set the curve type
            String targetCurve = setCurve(world, byPlayer, position);

            // If the set of the curve is invalid then returns false
            if (targetCurve == null)
            {
                return false;
            }

            // Place the curve and returns true if everything is fine
            Block curveToPlace = world.GetBlock(base.CodeWithParts("curved_" + targetCurve));
            if (curveToPlace != null)
            {
                // If placed correctly the curve
                if (this.placeIfSuitable(world, byPlayer, curveToPlace, position))
                {
                    // Update linked rails (set horizontal/vertical orientation)
                    return updateCloseRails(targetCurve, world, byPlayer, position);
                }
            }

            return false;
        }

        /// <summary>
        /// Try to place rails as raised type if possible, else nothing will happen.
        /// </summary>
        /// <returns>True if placed as raised.</returns>
        private bool TryToPlaceRaised(IWorldAccessor world, IPlayer byPlayer, BlockPos position)
        {
            // Set the raised type
            String targetRaised = setRaised(world, position);

            // If the set of the raise is invalid then returns false
            if (targetRaised == null)
            {
                return false;
            }

            // Place the curve and returns true if everything is fine
            Block raiseToPlace = world.GetBlock(base.CodeWithParts("raised_" + targetRaised));
            if (raiseToPlace != null)
            {
                return this.placeIfSuitable(world, byPlayer, raiseToPlace, position);
            }

            return false;
        }

        private bool placeIfSuitable(IWorldAccessor world, IPlayer byPlayer, Block block, BlockPos pos)
        {
            string failureCode = "";
            BlockSelection blockSel = new BlockSelection
            {
                Position = pos,
                Face = BlockFacing.UP
            };
            if (block.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return block.DoPlaceBlock(world, byPlayer, blockSel, null);
            }
            return false;
        }

        /// <returns>The string that represents the curve (that can be 'ne', 'nw', 'sw' or 'se').
        /// <br/> Null if not set correctly. Be sure that position represents right coordinates where to set the curve.
        /// <br/> This method does not place any block in the world.
        /// </returns>
        private String setCurve(IWorldAccessor world, IPlayer placer, BlockPos position)
        {
            // Set counting on oblique faces (NE, SE, SW, NW)
            int NERails = Rails.countRailsOnNE(world, position);
            int SERails = Rails.countRailsOnSE(world, position);
            int SWRails = Rails.countRailsOnSW(world, position);
            int NWRails = Rails.countRailsOnNW(world, position);

            // Returns if there are not enough rails on oblique faces to make a curve
            // For example if there are two rails (on North and East faces) then can make the curve
            if (NERails < 2 && SERails < 2 && SWRails < 2 && NWRails < 2)
                return null;

            // Set the target curve (that can be NE, SE, SW, NW)
            string targetCurve = null;
            // Get first player orientation (used for close rails count greater than 2 pieces)
            string playerFacing = Blocks.DirectionToString(Blocks.ObliqueFromAngle(GameMath.Mod(placer.Entity.Pos.Yaw, (float)Math.PI * 2f)));
            /* Additional info: 
                    - x: placing curve;
                    - X: rails;
                    - -: not rails;
            */
            /* Case NESW:
             * - X -
             * X x X    => Will depend from player facing
             * - X -
             */
            if (NERails >= 2 && NWRails >= 2 && SERails >= 2 && SWRails >= 2)
            {
                targetCurve = playerFacing;
            }
            /* Case NEW:
            * - X -
            * X x X    => Will depend from player facing
            * - - -
            */
            else if (NERails >= 2 && NWRails >= 2)
            {
                if (playerFacing == "sw")
                {
                    targetCurve = "nw";
                }
                else if (playerFacing == "se")
                {
                    targetCurve = "ne";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case NES:
            * - X -
            * - x X    => Will depend from player facing
            * - X -
            */
            else if (NERails >= 2 && SERails >= 2)
            {
                if (playerFacing == "nw")
                {
                    targetCurve = "ne";
                }
                else if (playerFacing == "sw")
                {
                    targetCurve = "se";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case ESW:
            * - - -
            * X x X    => Will depend from player facing
            * - X -
            */
            else if (SERails >= 2 && SWRails >= 2)
            {
                if (playerFacing == "ne")
                {
                    targetCurve = "se";
                }
                else if (playerFacing == "nw")
                {
                    targetCurve = "sw";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case NSW:
            * - X -
            * X x -    => Will depend from player facing
            * - X -
            */
            else if (NWRails >= 2 && SWRails >= 2)
            {
                if (playerFacing == "ne")
                {
                    targetCurve = "nw";
                }
                else if (playerFacing == "se")
                {
                    targetCurve = "sw";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case NW:
            * - X -
            * X x -    => Will be NW
            * - - -
            */
            else if (NWRails >= 2)
            {
                targetCurve = "nw";
            }
            /* Case NE:
            * - X -
            * - x X    => Will be NE
            * - - -
            */
            else if (NERails >= 2)
            {
                targetCurve = "ne";
            }
            /* Case SE:
            * - - -
            * - x X    => Will be SE
            * - X -
            */
            else if (SERails >= 2)
            {
                targetCurve = "se";
            }
            /* Case SW:
            * - - -
            * X x -    => Will be SW
            * - X -
            */
            else if (SWRails >= 2)
            {
                targetCurve = "sw";
            }

            return targetCurve;
        }

        /// <returns>The string that represents the raise (that can be 'e', 'n', 'w' or 's').
        /// <br/> Null if not set correctly. Be sure that position represents right coordinates where to set the raise.
        /// <br/> This method does not place any block in the world.
        /// </returns>
        private String setRaised(IWorldAccessor world, BlockPos position)
        {
            // Get close blocks
            Block EBlock = Blocks.getBlockToEast(world, position);
            Block NBlock = Blocks.getBlockToNorth(world, position);
            Block WBlock = Blocks.getBlockToWest(world, position);
            Block SBlock = Blocks.getBlockToSouth(world, position);

            // Set flag on faces (E, N, W, S)
            bool ERails = Rails.isBlockRailsBlock(EBlock);
            bool NRails = Rails.isBlockRailsBlock(NBlock);
            bool WRails = Rails.isBlockRailsBlock(WBlock);
            bool SRails = Rails.isBlockRailsBlock(SBlock);

            // Set required directions
            bool EOrientation = Rails.getFlatRailsDirection(EBlock) == "we";
            bool NOrientation = Rails.getFlatRailsDirection(NBlock) == "ns";
            bool WOrientation = Rails.getFlatRailsDirection(WBlock) == "we";
            bool SOrientation = Rails.getFlatRailsDirection(SBlock) == "ns";

            // Set the target raised (that can be E, N, S or W)
            if (ERails && !WRails && EOrientation && WBlock.SideSolid[BlockFacing.EAST.Index])
                return "e";
            if (NRails && !SRails && NOrientation && SBlock.SideSolid[BlockFacing.NORTH.Index])
                return "n";
            if (SRails && !NRails && SOrientation && NBlock.SideSolid[BlockFacing.SOUTH.Index])
                return "s";
            if (WRails && !ERails && WOrientation && EBlock.SideSolid[BlockFacing.WEST.Index])
                return "w";

            return null;
        }

        /// <summary>
        /// Update close rails to a curve (if set to null then the close curves will be set to their original shape).
        /// </summary>
        /// <returns>True if correctly updated close rails.</returns>
        private bool updateCloseRails(string targetCurve, IWorldAccessor world, IPlayer placer, BlockPos position)
        {
            if (targetCurve == null)
            {
                if (Rails.isThereRailBlockToEast(world, position) && Rails.isCurveRailBlock(Blocks.getBlockToEast(world, position)))
                {
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.EAST)));
                }
                if (Rails.isThereRailBlockToNorth(world, position) && Rails.isCurveRailBlock(Blocks.getBlockToNorth(world, position)))
                {
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.NORTH)));
                }
                if (Rails.isThereRailBlockToWest(world, position) && Rails.isCurveRailBlock(Blocks.getBlockToWest(world, position)))
                {
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.WEST)));
                }
                if (Rails.isThereRailBlockToSouth(world, position) && Rails.isCurveRailBlock(Blocks.getBlockToSouth(world, position)))
                {
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.SOUTH)));
                }

                return true;
            }

            // Check given curve type is valid
            BlockFacing[] orientation = Blocks.StringToDirection(targetCurve);
            if (orientation == null || (!Directions.isValidObliqueDirection(orientation)))
            {
                return false;
            }

            // Update linked rails (set horizontal/vertical orientation)
            switch (targetCurve)
            {
                case "ne":
                    orientation[0] = BlockFacing.NORTH;
                    orientation[1] = BlockFacing.EAST;
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    break;
                case "nw":
                    orientation[0] = BlockFacing.NORTH;
                    orientation[1] = BlockFacing.WEST;
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    break;
                case "sw":
                    orientation[0] = BlockFacing.SOUTH;
                    orientation[1] = BlockFacing.WEST;
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    break;
                case "se":
                    orientation[0] = BlockFacing.SOUTH;
                    orientation[1] = BlockFacing.EAST;
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                    world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    break;
            }

            return true;
        }
    }
}