using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.ViewModels;

public sealed class AdminLogItemViewModel {
    public AdminLogId Id { get; }
    public string Type { get; }
    public IReadOnlyList<string> Parameters { get; }
    public string Description { get; }
    public UserId UserId { get; }
    public Username Username { get; }
    public ZonedDateTime CreatedAt { get; }

    public AdminLogItemViewModel(AdminLog item) {
        Id = item.Id;
        Type = item.Type;
        Parameters = item.Parameters;
        Description = item.Description();
        UserId = item.UserId;
        Username = item.Username;
        CreatedAt = item.CreatedAt;
    }
}
