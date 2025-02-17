using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HoopsVsBeans.Data.Models;

public class Vote
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }
    public DateTime VoteTime { get; set; }
    public required string OptionVoted { get; set; } 
}