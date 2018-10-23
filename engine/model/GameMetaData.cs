using chess.v4.engine.enumeration;
using System.Collections.Generic;

namespace chess.v4.engine.model {

	public class GameMetaData {
		public string Annotator { get; set; }
		public string Black { get; set; }
		public string BlackELO { get; set; }
		public string Date { get; set; }
		public string ECO { get; set; }
		public string Event { get; set; }
		public string Filename { get; set; }
		public string ID { get; set; }
		public SortedList<int, string> Moves { get; set; }
		public string Remark { get; set; }
		public string Result { get; set; }
		public string Round { get; set; }
		public string Site { get; set; }
		public string Source { get; set; }
		public string White { get; set; }
		public string WhiteELO { get; set; }

		public string GetValue(MetaType metaType) {
			var value = this.GetType().GetProperty(metaType.ToString()).GetValue(this, null);
			return value.ToString();
		}

		public string GetValue(string propertyName) {
			string retval = string.Empty;
			var propInfo = this.GetType().GetProperty(propertyName);
			try {
				var value = propInfo.GetValue(this, null);
				retval = value.ToString();
			} catch {
			}
			return retval;
		}

		public void SetValue(MetaType metaType, string value) {
			switch (metaType) {
				case MetaType.Event:
					this.Event = value;
					break;

				case MetaType.Site:
					this.Site = value;
					break;

				case MetaType.Date:
					this.Date = value;
					break;

				case MetaType.Round:
					this.Round = value;
					break;

				case MetaType.White:
					this.White = value;
					break;

				case MetaType.Black:
					this.Black = value;
					break;

				case MetaType.Result:
					this.Result = value;
					break;

				case MetaType.WhiteElo:
					this.WhiteELO = value;
					break;

				case MetaType.BlackElo:
					this.BlackELO = value;
					break;

				case MetaType.ECO:
					this.ECO = value;
					break;

				case MetaType.Annotator:
					this.Annotator = value;
					break;

				case MetaType.Source:
					this.Source = value;
					break;

				case MetaType.Remark:
					this.Remark = value;
					break;

				case MetaType.Filename:
					this.Filename = value;
					break;

				case MetaType.ID:
					this.ID = value;
					break;
			}
		}
	}
}