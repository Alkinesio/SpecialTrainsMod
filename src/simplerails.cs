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
    public class RaiseType
    {
        private String type;
        private bool continuous;

        public RaiseType(string type, bool continuous)
        {
            setType(type);
            setContinuous(continuous);
        }

        public void setType(string type)
        {
            this.type = type;
        }

        public void setContinuous(bool continuous)
        {
            this.continuous = continuous;
        }

        public String getType()
        {
            return type;
        }

        public bool getContinuous()
        {
            return continuous;
        }
    }

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
            world.GetBlock(CodeWithPath(FirstCodePart() + (targetFacing.Axis == EnumAxis.Z ? "-flat_ns" : "-flat_we")))
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
            return updateCloseRails(null, world, position);
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
            Block curveToPlace = world.GetBlock(CodeWithPath(FirstCodePart() + "-curved_" + targetCurve));
            if (curveToPlace != null)
            {
                // If placed correctly the curve
                if (this.placeIfSuitable(world, byPlayer, curveToPlace, position))
                {
                    // Update linked rails (set horizontal/vertical orientation)
                    return updateCloseRails(targetCurve, world, position);
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
            RaiseType target = setRaised(world, position);

            // If the set of the raise is invalid then returns false
            if (target.getType() == null)
            {
                return false;
            }

            // Place the curve and returns true if everything is fine
            Block raiseToPlace = world.GetBlock(CodeWithPath(FirstCodePart() + "-raised_" + (target.getContinuous() ? "continuous_" : null) + target.getType()));
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

        /// <returns>A composite tuple (defined as RaiseType)
        /// <br/> • A string that represents the raise (that can be 'e', 'n', 'w' or 's').
        /// <br/> • A boolean that represents if the raise is continuous or not.
        /// <br/>
        /// <br/> Type null and boolean false if not set correctly. Be sure that position represents right coordinates where to set the raise.
        /// <br/> This method does not place any block in the world.
        /// </returns>
        private RaiseType setRaised(IWorldAccessor world, BlockPos position)
        {
            // Get close blocks
            Block EBlock = Blocks.getBlockToEast(world, position);
            Block NBlock = Blocks.getBlockToNorth(world, position);
            Block WBlock = Blocks.getBlockToWest(world, position);
            Block SBlock = Blocks.getBlockToSouth(world, position);
            Block UEBlock = Blocks.getBlockToUE(world, position);
            Block UNBlock = Blocks.getBlockToUN(world, position);
            Block UWBlock = Blocks.getBlockToUW(world, position);
            Block USBlock = Blocks.getBlockToUS(world, position);
            Block DEBlock = Blocks.getBlockToDE(world, position);
            Block DNBlock = Blocks.getBlockToDN(world, position);
            Block DWBlock = Blocks.getBlockToDW(world, position);
            Block DSBlock = Blocks.getBlockToDS(world, position);

            // Set flag on faces (E, N, W, S)
            bool ERails = Rails.isBlockRailsBlock(EBlock);
            bool NRails = Rails.isBlockRailsBlock(NBlock);
            bool WRails = Rails.isBlockRailsBlock(WBlock);
            bool SRails = Rails.isBlockRailsBlock(SBlock);

            // Set flag on up faces (UE, UN, UW, US)
            bool UERails = Rails.isBlockRailsBlock(UEBlock);
            bool UNRails = Rails.isBlockRailsBlock(UNBlock);
            bool UWRails = Rails.isBlockRailsBlock(UWBlock);
            bool USRails = Rails.isBlockRailsBlock(USBlock);

            // Set flag on down faces (DE, DN, DW, DS)
            bool DERails = Rails.isBlockRailsBlock(DEBlock);
            bool DNRails = Rails.isBlockRailsBlock(DNBlock);
            bool DWRails = Rails.isBlockRailsBlock(DWBlock);
            bool DSRails = Rails.isBlockRailsBlock(DSBlock);

            // Set required directions
            bool EOrientation = Rails.getFlatRailsDirection(EBlock) == "we" || Rails.getRaisedRailsDirection(UEBlock) == "e" || Rails.getRaisedRailsDirection(DEBlock) == "e";
            bool NOrientation = Rails.getFlatRailsDirection(NBlock) == "ns" || Rails.getRaisedRailsDirection(UNBlock) == "n" || Rails.getRaisedRailsDirection(DNBlock) == "n";
            bool WOrientation = Rails.getFlatRailsDirection(WBlock) == "we" || Rails.getRaisedRailsDirection(UWBlock) == "w" || Rails.getRaisedRailsDirection(DWBlock) == "w";
            bool SOrientation = Rails.getFlatRailsDirection(SBlock) == "ns" || Rails.getRaisedRailsDirection(USBlock) == "s" || Rails.getRaisedRailsDirection(DSBlock) == "s";

            // Set the target raised (that can be E, N, W or S)
            if (EOrientation && WBlock.SideSolid[BlockFacing.EAST.Index])
            {
                if (ERails && !WRails)
                    return new RaiseType("e", false);
                if ((UWRails && !UERails) || (DERails && !DWRails))
                    return new RaiseType("e", true);
            }
            if (NOrientation && SBlock.SideSolid[BlockFacing.NORTH.Index])
            {
                if (NRails && !SRails)
                    return new RaiseType("n", false);
                if ((USRails && !UNRails) || (DNRails && !DSRails))
                    return new RaiseType("n", true);
            }
            if (WOrientation && EBlock.SideSolid[BlockFacing.WEST.Index])
            {
                if (WRails && !ERails)
                    return new RaiseType("w", false);
                if ((UERails && !UWRails) || (DWRails && !DERails))
                    return new RaiseType("w", true);
            }
            if (SOrientation && NBlock.SideSolid[BlockFacing.SOUTH.Index])
            {
                if (SRails && !NRails)
                    return new RaiseType("s", false);
                if ((UNRails && !USRails) || (DSRails && !DNRails))
                    return new RaiseType("s", true);
            }

            return new RaiseType(null, false);
        }

        /// <summary>
        /// Update close rails to a curve (if set to null then the close curves will be set to their original shape).
        /// </summary>
        /// <returns>True if correctly updated close rails.</returns>
        private bool updateCloseRails(string targetCurve, IWorldAccessor world, BlockPos position)
        {
            IBlockAccessor wba = world.BlockAccessor;

            Block NBlock = Blocks.getBlockToNorth(world, position);
            Block EBlock = Blocks.getBlockToEast(world, position);
            Block WBlock = Blocks.getBlockToWest(world, position);
            Block SBlock = Blocks.getBlockToSouth(world, position);

            if (targetCurve == null)
            {
                // East
                if (Rails.isThereRailBlockToEast(world, position))
                {
                    // There is a curved rail block
                    if (Rails.isCurvedRailBlock(EBlock))
                        wba.ExchangeBlock(
                            wba.GetBlock(EBlock.CodeWithPath(EBlock.FirstCodePart() + "-flat_ns")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.EAST))
                            );
                    // There is a raised rail block
                    else if (Rails.isRaisedRailBlock(EBlock))
                        wba.ExchangeBlock(
                            wba.GetBlock(EBlock.CodeWithPath(EBlock.FirstCodePart() + "-flat_we")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.EAST))
                            );
                }
                // North
                if (Rails.isThereRailBlockToNorth(world, position))
                {
                    // There is a curved rail block
                    if (Rails.isCurvedRailBlock(NBlock))
                        wba.ExchangeBlock(
                            wba.GetBlock(NBlock.CodeWithPath(NBlock.FirstCodePart() + "-flat_we")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.NORTH))
                            );
                    // There is a raised rail block
                    else if (Rails.isRaisedRailBlock(NBlock))
                        wba.ExchangeBlock(
                                wba.GetBlock(NBlock.CodeWithPath(NBlock.FirstCodePart() + "-flat_ns")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.NORTH))
                            );
                }
                // West
                if (Rails.isThereRailBlockToWest(world, position))
                {
                    // There is a curved rail block
                    if (Rails.isCurvedRailBlock(WBlock))
                        wba.ExchangeBlock(
                            wba.GetBlock(WBlock.CodeWithPath(WBlock.FirstCodePart() + "-flat_ns")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.WEST))
                            );
                    // There is a raised rail block
                    else if (Rails.isRaisedRailBlock(WBlock))
                        wba.ExchangeBlock(
                            wba.GetBlock(WBlock.CodeWithPath(WBlock.FirstCodePart() + "-flat_we")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.WEST))
                            );
                }
                // South
                if (Rails.isThereRailBlockToSouth(world, position))
                {
                    // There is a curved rail block
                    if (Rails.isCurvedRailBlock(SBlock))
                        wba.ExchangeBlock(
                            wba.GetBlock(SBlock.CodeWithPath(SBlock.FirstCodePart() + "-flat_we")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.SOUTH))
                            );
                    // There is a raised rail block
                    else if (Rails.isRaisedRailBlock(SBlock))
                        wba.ExchangeBlock(
                            wba.GetBlock(SBlock.CodeWithPath(SBlock.FirstCodePart() + "-flat_ns")).Id,
                            position + new BlockPos(Blocks.FaceToCoordinates(BlockFacing.SOUTH))
                            );
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

                    Block NEBlock = Blocks.getBlockToNE(world, position);

                    orientation[0] = BlockFacing.NORTH;
                    orientation[1] = BlockFacing.EAST;

                    if (Rails.isThereRailBlockToNE(world, position) && Rails.isCurvedRailBlock(NEBlock) && Rails.getCurvedRailBlockDirection(NEBlock) == "sw")
                    {
                        wba.ExchangeBlock(wba.GetBlock(NBlock.CodeWithPath(NBlock.FirstCodePart() + "-curved_se")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(EBlock.CodeWithPath(EBlock.FirstCodePart() + "-curved_nw")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    else
                    {
                        wba.ExchangeBlock(wba.GetBlock(NBlock.CodeWithPath(NBlock.FirstCodePart() + "-flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(EBlock.CodeWithPath(EBlock.FirstCodePart() + "-flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    break;

                case "nw":

                    Block NWBlock = Blocks.getBlockToNW(world, position);

                    orientation[0] = BlockFacing.NORTH;
                    orientation[1] = BlockFacing.WEST;

                    if (Rails.isThereRailBlockToNW(world, position) && Rails.isCurvedRailBlock(NWBlock) && Rails.getCurvedRailBlockDirection(NWBlock) == "se")
                    {
                        wba.ExchangeBlock(wba.GetBlock(NBlock.CodeWithPath(NBlock.FirstCodePart() + "-curved_sw")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(WBlock.CodeWithPath(WBlock.FirstCodePart() + "-curved_ne")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    else
                    {
                        wba.ExchangeBlock(wba.GetBlock(NBlock.CodeWithPath(NBlock.FirstCodePart() + "-flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(WBlock.CodeWithPath(WBlock.FirstCodePart() + "-flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    break;

                case "sw":

                    Block SWBlock = Blocks.getBlockToSW(world, position);

                    orientation[0] = BlockFacing.SOUTH;
                    orientation[1] = BlockFacing.WEST;

                    if (Rails.isThereRailBlockToSW(world, position) && Rails.isCurvedRailBlock(SWBlock) && Rails.getCurvedRailBlockDirection(SWBlock) == "ne")
                    {
                        wba.ExchangeBlock(wba.GetBlock(SBlock.CodeWithPath(SBlock.FirstCodePart() + "-curved_nw")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(WBlock.CodeWithPath(WBlock.FirstCodePart() + "-curved_se")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    else
                    {
                        wba.ExchangeBlock(wba.GetBlock(SBlock.CodeWithPath(SBlock.FirstCodePart() + "-flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(WBlock.CodeWithPath(WBlock.FirstCodePart() + "-flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    break;

                case "se":

                    Block SEBlock = Blocks.getBlockToSE(world, position);

                    orientation[0] = BlockFacing.SOUTH;
                    orientation[1] = BlockFacing.EAST;

                    if (Rails.isThereRailBlockToSE(world, position) && Rails.isCurvedRailBlock(SEBlock) && Rails.getCurvedRailBlockDirection(SEBlock) == "nw")
                    {
                        wba.ExchangeBlock(wba.GetBlock(SBlock.CodeWithPath(SBlock.FirstCodePart() + "-curved_ne")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(EBlock.CodeWithPath(EBlock.FirstCodePart() + "-curved_sw")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    else
                    {
                        wba.ExchangeBlock(wba.GetBlock(SBlock.CodeWithPath(SBlock.FirstCodePart() + "-flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                        wba.ExchangeBlock(wba.GetBlock(EBlock.CodeWithPath(EBlock.FirstCodePart() + "-flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                    }
                    break;

            }

            return true;
        }

        /// <summary>
        /// Get material of the rails (example: "wooden", "iron", "golden")
        /// </summary>
        /// <returns>Null if invalid, else a string that represents the material.</returns>
        private String getMaterial()
        {
            return FirstCodePart().Split('_')[0];
        }

    }
}