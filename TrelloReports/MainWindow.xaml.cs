using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chello.Core;

namespace TrelloReports
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//These two (Key&Token) are for a specific user, allow user to enter these as Settings
		private const string cAuthKey = "44f745f18b33a5d556a8af895bcb8923";
		private const string cUserToken = "e4e7545f2b2184b81c8d2b5489da955d8976a69e954da4619d13618a87798e06";

		ObservableCollection<TrelloBoard> listOfBoards = new ObservableCollection<TrelloBoard>();

		public MainWindow()
		{
			InitializeComponent();

			treeviewTrelloBoards.ItemsSource = listOfBoards;
		}

		private void Window_Loaded_1(object sender, RoutedEventArgs e)
		{
			var chelloClient = new ChelloClient(cAuthKey, cUserToken);

			//var boards = chello.Members.PinnedBoards("francoishill");
			var boards = chelloClient.Boards.ForUser("francoishill");
			foreach (var b in boards)
				listOfBoards.Add(new TrelloBoard(chelloClient, b));
		}
	}

	public class TrelloBoard
	{
		private Chello.Core.ChelloClient _chelloClient;
		private Chello.Core.Board _chelloBoard;

		public string BoardName { get { return _chelloBoard.Name; } }

		private List<TrelloCardUpdate> _boardactivities = null;
		public List<TrelloCardUpdate> BoardActivities
		{
			get
			{
				if (_boardactivities == null)
				{
					_boardactivities = new List<TrelloCardUpdate>();
					var updates = _chelloClient.CardUpdates.ForBoard(_chelloBoard.Id, new TrelloCardupdatesArguments(1000));
					foreach (var upd in updates)
						_boardactivities.Add(new TrelloCardUpdate(this._chelloClient, upd));
				}
				return _boardactivities;
			}
		}

		public TrelloBoard(Chello.Core.ChelloClient chelloClient, Chello.Core.Board chelloBoard)
		{
			this._chelloClient = chelloClient;
			this._chelloBoard = chelloBoard;
		}

		public override string ToString()
		{
			return this.BoardName;
		}

		public class TrelloCardUpdate
		{
			private Chello.Core.ChelloClient _chelloClient;
			private Chello.Core.CardUpdateAction _cardUpdateAction;

			public DateTime CardupdateDateString { get { return _cardUpdateAction.Date; } }
			public bool CardClosed { get { return _cardUpdateAction.Data.Card != null ? _cardUpdateAction.Data.Card.Closed : false; } }
			public string CardName { get { return _cardUpdateAction.Data.Card != null ? _cardUpdateAction.Data.Card.Name : ""; } }
			public string CardDescription { get { return _cardUpdateAction.Data.Card != null ? _cardUpdateAction.Data.Card.Desc : ""; } }
			public string CardLabels { get { return _cardUpdateAction.Data.Card != null && _cardUpdateAction.Data.Card.Labels != null ? string.Join(", ", _cardUpdateAction.Data.Card.Labels.Select(l => l.Color)) : ""; } }
			public string CardupdateDataListBefore { get { return _cardUpdateAction.Data.ListBefore != null ? _cardUpdateAction.Data.ListBefore.Name : ""; } }
			public string CardupdateDataListAfter { get { return _cardUpdateAction.Data.ListAfter != null ? _cardUpdateAction.Data.ListAfter.Name : ""; } }

			public TrelloCardUpdate(Chello.Core.ChelloClient chelloClient, Chello.Core.CardUpdateAction cardUpdateAction)
			{
				this._chelloClient = chelloClient;
				this._cardUpdateAction = cardUpdateAction;
			}

			public override string ToString()
			{
				return _cardUpdateAction.Date.ToString(@"yyyy-MM-dd HH\hmm:ss");
			}
		}

		public class TrelloCardupdatesArguments
		{
			public int limit { get; private set; }
			public TrelloCardupdatesArguments(int limit)
			{
				this.limit = limit;
			}
		}
	}
}
