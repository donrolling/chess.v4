using Models.Application;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Data.Repository.Dapper {
	public class GameDapperRepository : GameDapperBaseRepository, IGameRepository {	
		public GameDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory) { }
	}
}