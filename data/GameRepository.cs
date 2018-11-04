using common;
using System;

namespace data {
	public class GameRepository {
		public Envelope<long> Insert(GameRepository gam) {
			p.Id = cnn.Query<int>(@"insert Products(Name,Description) 
values (@Name,@Description) 
select cast(scope_identity() as int)", p).First();

			var sql = @"INSERT INTO [dbo].[Game](
							[GameId]
						   ,[IsFinished]
						   ,[Event]
						   ,[Site]
						   ,[Date]
						   ,[Round]
						   ,[White]
						   ,[Black]
						   ,[Result]
						   ,[ECO]
						   ,[WhiteElo]
						   ,[BlackElo]
						   ,[NaturalKey]
						   ,[FileName]
						   ,[Annotator]
						   ,[Source]
						   ,[Remark]
						   ,[PGN]
						   ,[FEN]
						   ,[IsActive]
						   ,[CreatedById]
						   ,[CreatedDate]
						   ,[UpdatedById]
						   ,[UpdatedDate])
					 VALUES (
							@GameId, bigint,>
						   ,@IsFinished, bit,>
						   ,@Event, nvarchar(50),>
						   ,@Site, nvarchar(50),>
						   ,@Date, nvarchar(50),>
						   ,@Round, nvarchar(50),>
						   ,@White, nvarchar(50),>
						   ,@Black, nvarchar(50),>
						   ,@Result, nvarchar(5),>
						   ,@ECO, int,>
						   ,@WhiteElo, int,>
						   ,@BlackElo, int,>
						   ,@NaturalKey, nvarchar(50),>
						   ,@FileName, nvarchar(150),>
						   ,@Annotator, nvarchar(50),>
						   ,@Source, nvarchar(50),>
						   ,@Remark, nvarchar(max),>
						   ,@PGN,
						   ,@FEN,
						   ,@IsActive,
						   ,@CreatedById,
						   ,@CreatedDate,
						   ,@UpdatedById,
						   ,@UpdatedDate)";
			return Envelope<long>.Ok(0);
		}
	}
}
