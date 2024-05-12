using DragaliaAPI.Database;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Update to grant chapter 10 completion rewards to players that previously cleared chapter 10 and did not receive rewards.
/// </summary>
[UsedImplicitly]
public class V21Update(
    ApiContext apiContext,
    IPresentService presentService,
    ILogger<V20Update> logger
) : ISavefileUpdate
{
    private const int StoryId = 1001009;

    public int SavefileVersion => 21;

    public async Task Apply()
    {
        bool playerCompletedChapter10 = await apiContext
            .PlayerStoryState.Where(x =>
                x.StoryType == StoryTypes.Quest
                && x.StoryId == StoryId
                && x.State == StoryState.Read
            )
            .AnyAsync();

        logger.LogDebug(
            "Player completed chapter 10: {PlayerCompletedChapter10}",
            playerCompletedChapter10
        );

        if (playerCompletedChapter10)
        {
            logger.LogInformation(
                "Detected that chapter 10 was completed. Granting completion rewards."
            );

            if (
                MasterAsset.QuestStoryRewardInfo.TryGetValue(
                    StoryId,
                    out QuestStoryRewardInfo? rewardInfo
                )
            )
            {
                foreach (QuestStoryReward reward in rewardInfo.Rewards)
                {
                    if (reward.Type is EntityTypes.Material or EntityTypes.HustleHammer)
                    {
                        presentService.AddPresent(
                            new Present.Present(
                                PresentMessage.Chapter10Clear,
                                (EntityTypes)reward.Type,
                                reward.Id,
                                reward.Quantity
                            )
                        );
                    }
                }
            }
        }
    }
}
