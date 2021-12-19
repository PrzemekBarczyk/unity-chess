using Backend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frontend
{
	public class MoveSelector : MonoSingleton<MoveSelector>
	{
		[Header("External Dependencies")]
		[SerializeField] GraphicalBoard _board;
		[SerializeField] PiecesSprites _piecesSprites;

		[Header("Dragged Piece")]
		[SerializeField] SpriteRenderer _draggedPieceSpriteRenderer;

		List<Move> _legalMoves;

		bool _isMoveSelected;
		Move _selectedMove;

		GraphicalSquare _selectedPieceSquare; // every occupied square can be selected
		GraphicalSquare _startSquare; // only legal
		GraphicalSquare _endSquare; // only legal

		byte _droppedPiecInSameSquareCounter = 0;

		bool _pickingPromotion;

		public void SetLegalMoves(List<Move> legalMoves)
		{
			_legalMoves = legalMoves;
		}

		public bool IsMoveSelected()
		{
			return _isMoveSelected;
		}

		public Move GetSelectedMove()
		{
			Move selectedMoveCopy = _selectedMove;
			_isMoveSelected = false;
			return selectedMoveCopy;
		}

		public IEnumerator OnSquareSelected(GraphicalSquare selectedSquare) // called when OnMouseDown triggered on Square
		{
			if (_pickingPromotion)
			{
				yield break;
			}

			// piece selected and selected legal square
			bool pieceIsSelected = _selectedPieceSquare != null;
			bool startSquareIsSelected = _startSquare != null;
			if (pieceIsSelected && startSquareIsSelected)
			{
				bool selectedLegalSquare = _legalMoves.Exists(
					m => m.OldSquare.Position == Vector2Int.RoundToInt(_startSquare.transform.position) &&
					m.NewSquare.Position == Vector2Int.RoundToInt(selectedSquare.transform.position)
				);
				if (selectedLegalSquare)
				{
					StartCoroutine(PickPromotion(selectedSquare)); // make move
					yield break;
				}
			}

			bool selectedGraphicalSquareIsOccupied = selectedSquare.PieceSprite != null;
			if (selectedGraphicalSquareIsOccupied) // selecting occupied square pick ups piece and display indicator
			{
				bool isSamePieceSelected = selectedSquare == _selectedPieceSquare;
				if (pieceIsSelected && !isSamePieceSelected) // if selected different piece
				{
					_droppedPiecInSameSquareCounter = 0;

					// deselect square and hide indicator
					_selectedPieceSquare.HideSelectionIndicator();
					_selectedPieceSquare = null;

					// hide legal squares and delete start square (if it was legal square)
					if (startSquareIsSelected)
					{
						HideLegalMovesIndicators(_legalMoves.FindAll(
							m => m.OldSquare.Position.x == _startSquare.transform.position.x &&
							m.OldSquare.Position.y == _startSquare.transform.position.y
						));
						_startSquare = null;
					}
				}

				// select square and display indicator
				_selectedPieceSquare = selectedSquare;
				_selectedPieceSquare.DisplaySelectionIndicator();

				// select sprite to drag and hide static
				_draggedPieceSpriteRenderer.sprite = selectedSquare.PieceSprite;
				_draggedPieceSpriteRenderer.enabled = true;
				selectedSquare.PieceSprite = null;

				var legalMovesForSelectedGraphicalSquare = _legalMoves.FindAll(
					m => m.OldSquare.Position == Vector2Int.RoundToInt(selectedSquare.transform.position)
				);
				if (legalMovesForSelectedGraphicalSquare.Count > 0) // selected piece is legal
				{
					// display legal moves indicators and save start square
					DisplayLegalMovesIndicators(legalMovesForSelectedGraphicalSquare);
					_startSquare = selectedSquare;
				}
			}
			else // selected square is not occupied -> remove old indicators
			{
				if (pieceIsSelected)
				{
					_droppedPiecInSameSquareCounter = 0;

					// deselect square and hide indicator
					_selectedPieceSquare.HideSelectionIndicator();
					_selectedPieceSquare = null;

					// hide legal squares and delete start square
					if (startSquareIsSelected)
					{
						HideLegalMovesIndicators(_legalMoves.FindAll(
							m => m.OldSquare.Position == Vector2Int.RoundToInt(_startSquare.transform.position)
						));
						_startSquare = null;
					}
				}
			}
		}

		void Update()
		{
			if (_draggedPieceSpriteRenderer.enabled)
			{
				DragSelectedPieceAfterCursor();
			}
		}

		void DragSelectedPieceAfterCursor()
		{
			Vector2 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) * Vector2.one;
			_draggedPieceSpriteRenderer.transform.position = newPosition;
		}

		public IEnumerator OnPieceDrop() // called when OnMouseUp triggered on Square
		{
			if (_pickingPromotion)
			{
				yield break;
			}

			bool isPieceSelected = _selectedPieceSquare != null;
			if (!isPieceSelected)
			{
				yield break;
			}

			_draggedPieceSpriteRenderer.enabled = false;

			Vector2Int roundedDraggedPieceLastPosition = Vector2Int.RoundToInt(_draggedPieceSpriteRenderer.transform.position);

			GraphicalSquare selectedGraphicalSquare;
			try
			{
				selectedGraphicalSquare = _board.Squares[roundedDraggedPieceLastPosition.x, roundedDraggedPieceLastPosition.y];
			}
			catch (IndexOutOfRangeException) // piece dropped outside board
			{
				_selectedPieceSquare.PieceSprite = _draggedPieceSpriteRenderer.sprite;
				yield break;
			}

			bool selectedLegalSquare = _legalMoves.Exists(
				m => (m.OldSquare.Position == Vector2Int.RoundToInt(_selectedPieceSquare.transform.position)) &&
				(m.NewSquare.Position == Vector2Int.RoundToInt(selectedGraphicalSquare.transform.position))
			);
			if (selectedLegalSquare) // piece dropped on legal square
			{
				StartCoroutine(PickPromotion(selectedGraphicalSquare));
				yield break;
			}

			_selectedPieceSquare.PieceSprite = _draggedPieceSpriteRenderer.sprite; // dropped piece on illegal square

			bool droppedPieceOnSameSquare = selectedGraphicalSquare.transform.position == _selectedPieceSquare.transform.position;
			if (droppedPieceOnSameSquare)
			{
				_droppedPiecInSameSquareCounter++;
				if (_droppedPiecInSameSquareCounter > 1)
				{
					_selectedPieceSquare.HideSelectionIndicator();
					_selectedPieceSquare = null;

					if (_startSquare)
						HideLegalMovesIndicators(_legalMoves.FindAll(
							m => m.OldSquare.Position == Vector2Int.RoundToInt(_startSquare.transform.position)
					));
					_startSquare = null;

					_droppedPiecInSameSquareCounter = 0;
				}
			}
		}

		IEnumerator PickPromotion(GraphicalSquare endSquare)
		{
			_pickingPromotion = true;

			_endSquare = endSquare;

			HideLegalMovesIndicators(_legalMoves.FindAll(
				m => m.OldSquare.Position == Vector2Int.RoundToInt(_startSquare.transform.position)
			));

			Move selectedMove = _legalMoves.Find(
				m => m.OldSquare.Position == Vector2Int.RoundToInt(_startSquare.transform.position) &&
				m.NewSquare.Position == Vector2Int.RoundToInt(_endSquare.transform.position)
			);
			if (selectedMove.IsPromotion)
			{
				_endSquare.DisplayPromotionPanel();

				yield return new WaitUntil(() => _endSquare.PromotionPanel.IsPromotionSelected());

				_selectedMove = _legalMoves.Find(
					m => m.OldSquare.Position == Vector2Int.RoundToInt(_startSquare.transform.position) &&
					m.NewSquare.Position == Vector2Int.RoundToInt(_endSquare.transform.position) &&
					m.Type == _endSquare.PromotionPanel.PromotionType);

				_endSquare.HidePromotionPanel();
			}
			else
			{
				_selectedMove = _legalMoves.Find(
					m => m.OldSquare.Position == Vector2Int.RoundToInt(_startSquare.transform.position) &&
					m.NewSquare.Position == Vector2Int.RoundToInt(_endSquare.transform.position)
				);
			}

			_selectedPieceSquare.HideSelectionIndicator();
			_selectedPieceSquare = null;
			_startSquare = null;
			_endSquare = null;
			_draggedPieceSpriteRenderer.enabled = false;
			_droppedPiecInSameSquareCounter = 0;
			_pickingPromotion = false;
			_isMoveSelected = true;
		}

		public void DisplayLegalMovesIndicators(List<Move> legalMoves)
		{
			foreach (Move move in legalMoves)
			{
				if (move.EncounteredPiece != null)
					_board.Squares[move.NewSquare.Position.x, move.NewSquare.Position.y].DisplayValidForAttackIndicator();
				else
					_board.Squares[move.NewSquare.Position.x, move.NewSquare.Position.y].DisplayValidForMoveIndicator();
			}
		}

		public void HideLegalMovesIndicators(List<Move> legalMoves)
		{
			foreach (Move move in legalMoves)
				_board.Squares[move.NewSquare.Position.x, move.NewSquare.Position.y].HideValidMovementIndicators();
		}
	}
}
