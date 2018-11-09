using Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities {
	public class Game : Entity<long> {
			[Required]
		[Display(Name = "Is Finished")]
		public bool IsFinished { get; set; }		
		[StringLength(50, ErrorMessage = "Event cannot be longer than 50 characters.")]
		[Display(Name = "Event")]
		public string Event { get; set; }
		[StringLength(50, ErrorMessage = "Site cannot be longer than 50 characters.")]
		[Display(Name = "Site")]
		public string Site { get; set; }
		[StringLength(50, ErrorMessage = "Date cannot be longer than 50 characters.")]
		[Display(Name = "Date")]
		public string Date { get; set; }
		[StringLength(50, ErrorMessage = "Round cannot be longer than 50 characters.")]
		[Display(Name = "Round")]
		public string Round { get; set; }
		[StringLength(50, ErrorMessage = "White cannot be longer than 50 characters.")]
		[Display(Name = "White")]
		public string White { get; set; }
		[StringLength(50, ErrorMessage = "Black cannot be longer than 50 characters.")]
		[Display(Name = "Black")]
		public string Black { get; set; }
		[StringLength(8, ErrorMessage = "Result cannot be longer than 8 characters.")]
		[Display(Name = "Result")]
		public string Result { get; set; }
		[Display(Name = "E C O")]
		public int? ECO { get; set; }		
		[Display(Name = "White Elo")]
		public int? WhiteElo { get; set; }		
		[Display(Name = "Black Elo")]
		public int? BlackElo { get; set; }		
		[StringLength(50, ErrorMessage = "Natural Key cannot be longer than 50 characters.")]
		[Display(Name = "Natural Key")]
		public string NaturalKey { get; set; }
		[StringLength(150, ErrorMessage = "File Name cannot be longer than 150 characters.")]
		[Display(Name = "File Name")]
		public string FileName { get; set; }
		[StringLength(50, ErrorMessage = "Annotator cannot be longer than 50 characters.")]
		[Display(Name = "Annotator")]
		public string Annotator { get; set; }
		[StringLength(50, ErrorMessage = "Source cannot be longer than 50 characters.")]
		[Display(Name = "Source")]
		public string Source { get; set; }
		[Display(Name = "Remark")]
		public string Remark { get; set; }
		[StringLength(1000, ErrorMessage = "P G N cannot be longer than 1000 characters.")]
		[Display(Name = "P G N")]
		public string PGN { get; set; }
		[Required]
		[StringLength(100, ErrorMessage = "F E N cannot be longer than 100 characters.")]
		[Display(Name = "F E N")]
		public string FEN { get; set; }
	}

	public class Game_Properties : Entity_Properties {
		public const string Id = "Id";
		public const string IsFinished = "IsFinished";
		public const string Event = "Event";
		public const string Site = "Site";
		public const string Date = "Date";
		public const string Round = "Round";
		public const string White = "White";
		public const string Black = "Black";
		public const string Result = "Result";
		public const string ECO = "ECO";
		public const string WhiteElo = "WhiteElo";
		public const string BlackElo = "BlackElo";
		public const string NaturalKey = "NaturalKey";
		public const string FileName = "FileName";
		public const string Annotator = "Annotator";
		public const string Source = "Source";
		public const string Remark = "Remark";
		public const string PGN = "PGN";
		public const string FEN = "FEN";
	}
}