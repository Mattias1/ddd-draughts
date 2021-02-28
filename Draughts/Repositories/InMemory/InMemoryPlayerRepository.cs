using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryPlayerRepository : InMemoryRepository<Player, PlayerId>, IPlayerRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryPlayerRepository(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        protected override IList<Player> GetBaseQuery() => GameDatabase.PlayersTable.Select(p => p.ToDomainModel()).ToList();

        public override void Save(Player entity) {
            //  Note: the real fix is to remove this repository, as it should be part of the game aggregate's save operation
            throw new InvalidOperationException("Can't use this method, as we need a gameId to save a player :(");
        }

        public void Save(Player entity, GameId gameId) {
            var player = DbPlayer.FromDomainModel(entity, gameId);
            _unitOfWork.Store(player, GameDatabase.TempPlayersTable);
        }
    }
}
