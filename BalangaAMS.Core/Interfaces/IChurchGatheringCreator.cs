using System;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;

namespace BalangaAMS.Core.Interfaces
{
    public interface IChurchGatheringManager
    {
        void CreateGathering(Gatherings gatherings, DateTime datestarted);
        GatheringSession ReturnNewlyCreatedGathering();
        void UpdateGathering(GatheringSession gathering);
        void RemoveGathering(GatheringSession gathering);
        bool IsRemovingSuccessful();
    }
}