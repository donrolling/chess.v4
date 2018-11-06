using Data.Interfaces;
using Models.Entities;
using System;

namespace Data.Repository.Interfaces {
	public interface IGameRepository : IEntityDapperRepository<Game, long> {
	}
}