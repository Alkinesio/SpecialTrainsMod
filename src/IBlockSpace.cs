using System.Collections.Generic;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

public interface IBlockSpace
{
    IBlockSpace Parent { get; }

    ref readonly RigidTransformD Transform { get; }
    ISpacePhysicsResolver Physics { get; }

    T GetOrCreateSystem<T>(string className = null) where T : class;

    void GetBlock(Vec3f pos, int layer, out int id, out IBlockEntity entity);
    void SetBlock(Vec3f pos, int layer, int id, IBlockEntityInternal entity);
}

public interface IBlockEntity
{
    IBlockSpace Space { get; }
    Vec3f SpacePosition { get; }
}

public interface IBlockEntityInternal : IBlockEntity
{
    new IBlockSpace Space { set; }
    new Vec3f SpacePosition { set; }

    void OnInitialize();

    void FromAttribute(IAttribute tree);

    IAttribute ToAttribute();

    void OnStoreCollectibleMappings(IDictionary<int, AssetLocation> blockIdMapping, IDictionary<int, AssetLocation> itemIdMapping);

    void OnLoadCollectibleMappings(IDictionary<int, AssetLocation> oldBlockIdMapping, IDictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed);
}