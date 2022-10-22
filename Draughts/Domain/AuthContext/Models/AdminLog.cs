using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.AuthContext.Models;

public sealed class AdminLog : AggregateRoot<AdminLog, AdminLogId> {
    public override AdminLogId Id { get; }
    public string Type { get; }
    public IReadOnlyList<string> Parameters { get; }
    public UserId UserId { get; }
    public Username Username { get; }
    public ZonedDateTime CreatedAt { get; }

    public AdminLog(AdminLogId id, string type, IReadOnlyList<string> parameters,
            UserId userId, Username username, ZonedDateTime createdAt) {
        Id = id;
        UserId = userId;
        Username = username;
        Type = type;
        Parameters = parameters;
        CreatedAt = createdAt;

        try {
            Description();
        }
        catch (Exception e) {
            throw new InvalidOperationException($"Invalid admin log type ({type}).", e);
        }
    }

    public string Description() {
        string userId, username = "";
        switch (Type) {
            case "role.create":
                var (roleId, rolename) = Parameters.UnpackDuo();
                return $"Created a new role '{roleId} - {rolename}'";
            case "role.edit":
                (roleId, rolename) = Parameters.UnpackDuo();
                return $"Edited the role '{roleId} - {rolename}'";
            case "role.delete":
                (roleId, rolename) = Parameters.UnpackDuo();
                return $"Deleted the role '{roleId} - {rolename}'";
            case "role.gain":
                (roleId, rolename, userId, username) = Parameters.UnpackQuad();
                return $"Assigned the role '{roleId} - {rolename}' to '{userId} - {username}'";
            case "role.lose":
                (roleId, rolename, userId, username) = Parameters.UnpackQuad();
                return $"Removed the role '{roleId} - {rolename}' from '{userId} - {username}'";
            case "events.sync":
                AssertNoParams();
                return "Synced the event queue status";
            case "events.dispatch":
                AssertNoParams();
                return "Dispatched the unhandled events";
            default:
                throw new InvalidOperationException("Unknown admin log type.");
        }
    }

    private void AssertNoParams() {
        if (Parameters.Count != 0) {
            throw new InvalidOperationException("There should not be parameters for this admin log.");
        }
    }
}
