using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HumanPlayer))]
public class MoveSelector : MonoBehaviour
{
	public bool IsMoveSelected => _selectedMove.HasValue;

	MoveData? _selectedMove;

	Piece _selectedPiece;

	Square _startSquare;
	Square _endSquare;

	uint _selectedSamePieceCounter;

	bool _dragSelectedPieceWithCursor;

	bool _pickingPromotion;

	HumanPlayer _player;

	PlayerManager _players;
	Board _board;

	void Awake()
	{
		_players = PlayerManager.Instance;
		_board = Board.Instance;

		_player = GetComponent<HumanPlayer>();
	}

	void Update()
	{
		if (_dragSelectedPieceWithCursor)
		{
			DragSelectedPieceAfterCursor();
		}
	}

	void DragSelectedPieceAfterCursor()
	{
		Vector2 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) * Vector2.one;
		_selectedPiece.transform.position = newPosition;
	}

	public MoveData GetSelectedMove()
	{
		MoveData selectedMoveCopy = _selectedMove.Value;
		_selectedMove = null;
		return selectedMoveCopy;
	}

	public IEnumerator OnSquareSelected(Square selectedSquare) // called when OnMouseDown triggered on Square
	{
		bool pieceWasntSelected = _selectedPiece == null;
		if (pieceWasntSelected)
		{
			if (selectedSquare.IsOccupied)
			{
				_selectedPiece = selectedSquare.Piece;
				_startSquare = selectedSquare;
				_dragSelectedPieceWithCursor = true;
				_selectedSamePieceCounter++;
				_startSquare.DisplayLastMoveIndicator();

				bool selectedCurrentPlayerPiece = _selectedPiece.Color == _players.CurrentPlayer.Color;
				if (selectedCurrentPlayerPiece)
				{
					_board.DisplayLegalMovesIndicators(_selectedPiece.LegalMoves);
				}
			}
		}
		else // piece wasn't yet selected
		{
			if (!selectedSquare.IsOccupied)
			{
				bool isSelectedSquareValidForMove = _selectedPiece.LegalMoves.Exists(x => x.OldSquare == _startSquare && x.NewSquare == selectedSquare);
				if (isSelectedSquareValidForMove)
				{
					bool selectedCurrentPlayerPiece = _selectedPiece.Color == _players.CurrentPlayer.Color;
					if (selectedCurrentPlayerPiece)
					{
						_endSquare = selectedSquare;
						yield return PickMove();
					}
					else // selected opponent piece
					{
						yield break;
					}
				}
				else // selected illegal for move square
				{
					_board.HideLegalMovesIndicators(_selectedPiece.LegalMoves);
					_startSquare.HideLastMoveIndicator();
					_selectedSamePieceCounter = 0;
					_selectedPiece = null;
					_startSquare = null;
				}
			}
			else // square not empty
			{
				bool isSelectedSquareValidForMove = _selectedPiece.LegalMoves.Exists(x => x.OldSquare == _startSquare && x.NewSquare == selectedSquare);
				if (isSelectedSquareValidForMove)
				{
					bool selectedCurrentPlayerPiece = _selectedPiece.Color == _players.CurrentPlayer.Color;
					if (selectedCurrentPlayerPiece)
					{
						_endSquare = selectedSquare;
						yield return PickMove();
					}
					else // selected opponent piece
					{
						yield break;
					}
				}
				else // selected illegal for move square
				{
					bool isSelectedSquareAnOldSquare = _startSquare == selectedSquare;
					if (isSelectedSquareAnOldSquare)
					{
						_selectedSamePieceCounter++;
						_dragSelectedPieceWithCursor = true;
					}
					else // selected square is different than previously selected square
					{
						_board.HideLegalMovesIndicators(_selectedPiece.LegalMoves);
						_startSquare.HideLastMoveIndicator();
						_selectedSamePieceCounter = 0;

						_selectedPiece = selectedSquare.Piece;
						_startSquare = selectedSquare;
						_dragSelectedPieceWithCursor = true;
						_selectedSamePieceCounter++;
						_startSquare.DisplayLastMoveIndicator();

						bool selectedMyPiece = _selectedPiece.Color == _players.CurrentPlayer.Color;
						if (selectedMyPiece)
						{
							_board.DisplayLegalMovesIndicators(_selectedPiece.LegalMoves);
						}
					}
				}
			}
		}
	}

	public IEnumerator OnPieceDrop() // called when OnMouseUp triggered on Square
	{
		bool pieceWasntSelected = _selectedPiece == null;
		if (pieceWasntSelected)
		{
			yield break;
		}

		Vector2Int roundedSelectedPieceLastPosition = Vector2Int.RoundToInt(_selectedPiece.transform.position);

		_dragSelectedPieceWithCursor = false;

		Square selectedSquare;
		try
		{
			selectedSquare = _board.Squares[roundedSelectedPieceLastPosition.x, roundedSelectedPieceLastPosition.y];
		}
		catch (IndexOutOfRangeException) // piece dropped outside board
		{
			_selectedPiece.transform.position = new Vector3(_startSquare.Position.x, _startSquare.Position.y);
			yield break;
		}

		bool isSelectedSquareValidForMove = _selectedPiece.LegalMoves.Exists(x => x.OldSquare == _startSquare && x.NewSquare == selectedSquare);
		if (isSelectedSquareValidForMove)
		{
			bool selectedCurrentPlayerPiece = _selectedPiece.Color == _players.CurrentPlayer.Color;
			if (selectedCurrentPlayerPiece)
			{
				_endSquare = selectedSquare;
				yield return PickMove();
			}
			else // selected opponent piece
			{
				_selectedPiece.transform.position = new Vector3(_startSquare.Position.x, _startSquare.Position.y);
				yield break;
			}
		}
		else // dropped piece on illegal square
		{
			_selectedPiece.transform.position = new Vector3(_startSquare.Position.x, _startSquare.Position.y);

			bool mouseUpOnSameSquare = selectedSquare == _startSquare;
			if (mouseUpOnSameSquare)
			{
				if (_selectedSamePieceCounter >= 2)
				{
					_startSquare.HideLastMoveIndicator();
					_board.HideLegalMovesIndicators(_selectedPiece.LegalMoves);
					_selectedPiece = null;
					_startSquare = null;
					_selectedSamePieceCounter = 0;
				}
			}
		}
	}

	IEnumerator PickMove()
	{
		if (_pickingPromotion)
		{
			yield break;
		}

		_pickingPromotion = true;

		_board.HideLegalMovesIndicators(_selectedPiece.LegalMoves);
		_endSquare.DisplayLastMoveIndicator();

		bool isPromotionMove = (_selectedPiece is Pawn) && (_player.Color == ColorType.White ? _endSquare.OnTopRank : _endSquare.OnBottomRank);
		if (isPromotionMove)
		{
			_endSquare.DisplayPromotionPanel();

			_selectedPiece.transform.position = new Vector3(_endSquare.Position.x, _endSquare.Position.y);

			yield return new WaitUntil(() => _endSquare.PromotionPanel.IsPromotionSelected());

			_selectedMove = new MoveData(_selectedPiece, _startSquare, _endSquare, _endSquare.Piece, _endSquare.PromotionPanel.PromotionType);

			_endSquare.HidePromotionPanel();
		}
		else
		{
			_selectedMove = new MoveData(_selectedPiece, _startSquare, _endSquare, _endSquare.Piece);
		}

		_selectedSamePieceCounter = 0;
		_selectedPiece = null;
		_startSquare = null;
		_endSquare = null;
		_dragSelectedPieceWithCursor = false;
		_pickingPromotion = false;
	}
}
