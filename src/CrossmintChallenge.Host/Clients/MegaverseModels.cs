namespace CrossmintChallenge.Clients;

public record MapGoalResponse(List<List<GoalItem>> Goal);

public enum GoalItem
{
    SPACE,
    POLYANET,
    BLUE_SOLOON,
    PURPLE_SOLOON,
    RED_SOLOON,
    WHITE_SOLOON,
    UP_COMETH,
    DOWN_COMETH,
    LEFT_COMETH,
    RIGHT_COMETH,
}

public record MapResponse(Map Map);

public record Map(
    string _id,
    List<List<CellContent?>> Content,
    string CandidateId,
    int Phase,
    int __v
);

// 0 => polyanet
// 1 => soloon
//      colors: purple, red white, blue
// 2 => cometh
//      direction: up, right, down, left

public enum ColorEnum
{
    purple,
    red,
    white,
    blue,
}

public enum DirectionEnum
{
    up,
    right,
    down,
    left,
}

public enum MegaverseStarsEnum
{
    polyanets,
    soloons,
    comeths,
}

public record CellContent(int Type, DirectionEnum? Direction, ColorEnum? Color);
