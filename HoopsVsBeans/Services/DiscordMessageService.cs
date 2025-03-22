using HoopsVsBeans.Data;
using HoopsVsBeans.Data.Models;
using HoopsVsBeans.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HoopsVsBeans.Services
{
    public class DiscordMessageService(IOptions<DiscordWebhookOptions> options, HoopsVsBeansContext context)
    {
        private readonly HttpClient _httpClient = new();

        public async Task SendVoteMessage(Vote vote)
        {
            var currentVote = await context.VoteOptions.FirstAsync();
            
            var messagePayload = new
            {
                content = $"A new vote was cast for **{vote.OptionVoted}**!\n" +
                          $"Current totals:\n" +
                          $"Hoops: **{currentVote.Hoops}**\n" +
                          $"Beans: **{currentVote.Beans}**"
            };
            
            var response = await _httpClient.PostAsJsonAsync(options.Value.Url, messagePayload);
        }
    }
}