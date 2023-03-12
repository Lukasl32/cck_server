using System.Text.Json.Serialization;

namespace Accessories.Models;

public class GeneralResult
{
    public long InjurieId { get; set; }
    public long TeamId { get; set; }
    public long RefereeId { get; set; }
    public Signature Signature { get; set; }
    [JsonPropertyName("tasks")]
    public ResultTask[] ResultTasks { get; set; }
}

public class Signature
{
    public string Referee { get; set; }
    public string Leader { get; set; }
    public DateTime Signed { get; set; }
}

public class ResultTask
{
    public long Id { get; set; }
    public int DeductedPoints { get; set; }
}

/*
 {
    "injurieId": 1,
    "teamId": 1,
    "refereeId": 1,
    "signature": {
        "referee": "564f6ba165c656d",
        "leader": "12c569ad56c96e485b2",
        "signed": "2023-4-21T15:13:55"
    },
    "tasks": [
       {
           "id": 1,
           "deductedPoints": 20
       },
       {
           "id": 2,
           "deductedPoints": 0
       }
    ]
}
*/