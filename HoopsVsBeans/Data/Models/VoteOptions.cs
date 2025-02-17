using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HoopsVsBeans.Data.Models;

public class VoteOptions
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }
    public int Hoops { get; set; }
    public int Beans { get; set; }
}