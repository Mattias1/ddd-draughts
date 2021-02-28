using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.ViewModels {
    public class AdminLogViewModel {
        public AdminLogId Id { get; }
        public string Type { get; }
        public IReadOnlyList<string> Parameters { get; }
        public string Description { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public Permission Permission { get; }
        public ZonedDateTime CreatedAt { get; }

        public AdminLogViewModel(AdminLog adminLog) {
            Id = adminLog.Id;
            Type = adminLog.Type;
            Parameters = adminLog.Parameters;
            Description = adminLog.Description();
            UserId = adminLog.UserId;
            Username = adminLog.Username;
            Permission = adminLog.Permission;
            CreatedAt = adminLog.CreatedAt;
        }
    }
}
