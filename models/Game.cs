using System;

namespace models {
	public class Game {
		public string Annotator { get; set; }
		public string Black { get; set; }
		public int BlackElo { get; set; }
		public long CreatedById { get; set; }
		public DateTime CreatedDate { get; set; }
		public string Date { get; set; }
		public int ECO { get; set; }
		public string Event { get; set; }
		public string FEN { get; set; }
		public string FileName { get; set; }
		public long Id { get; set; }
		public bool IsActive { get; set; }
		public bool IsFinished { get; set; }
		public string NaturalKey { get; set; }
		public string PGN { get; set; }
		public string Remark { get; set; }
		public string Result { get; set; }
		public string Round { get; set; }
		public string Site { get; set; }
		public string Source { get; set; }
		public long UpdatedById { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string White { get; set; }
		public int WhiteElo { get; set; }
	}
}