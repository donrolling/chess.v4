using Common;
using Common.Models;
using Dapper;
using models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Repository.Dapper {
	public class GameRepository {
		public string ConnectionString { get; }

		public GameRepository(string connectionString) {
			ConnectionString = connectionString;
		}

		public Envelope<long> Insert(Game game) {
			var sql = @"INSERT INTO [dbo].[Game](
						   [IsFinished]
						   , [Event]
						   , [Site]
						   , [Date]
						   , [Round]
						   , [White]
						   , [Black]
						   , [Result]
						   , [ECO]
						   , [WhiteElo]
						   , [BlackElo]
						   , [NaturalKey]
						   , [FileName]
						   , [Annotator]
						   , [Source]
						   , [Remark]
						   , [PGN]
						   , [FEN]
						   , [IsActive]
						   , [CreatedById]
						   , [CreatedDate]
						   , [UpdatedById]
						   , [UpdatedDate]
					 ) VALUES (
						   @IsFinished
						   , @Event
						   , @Site
						   , @Date
						   , @Round
						   , @White
						   , @Black
						   , @Result
						   , @ECO
						   , @WhiteElo
						   , @BlackElo
						   , @NaturalKey
						   , @FileName
						   , @Annotator
						   , @Source
						   , @Remark
						   , @PGN
						   , @FEN
						   , @IsActive
						   , @CreatedById
						   , @CreatedDate
						   , @UpdatedById
						   , @UpdatedDate)
					   select cast(scope_identity() as bigint)";
			long result = 0;
			using (var cnn = new SqlConnection(this.ConnectionString)) {
				result = cnn.Query<long>(sql, game).First();
			}
			return Envelope<long>.Ok(result);
		}

		public Envelope<Game> SelectById(long id) {
			Game result;
			using (var cnn = new SqlConnection(this.ConnectionString)) {
				result = cnn.Query<Game>("select * from Game where Id = @id", new { id = id }).First();
			}
			return Envelope<Game>.Ok(result);
		}

		public IEnumerable<Game> SelectAll() {
			using (var cnn = new SqlConnection(this.ConnectionString)) {
				return cnn.Query<Game>("select * from Game");
			}
		}
	}
}