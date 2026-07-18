using System.Collections.Generic;

namespace UnityEngine
{
	public class TextEditor
	{
		public enum DblClickSnapping : byte
		{
			WORDS = 0,
			PARAGRAPHS = 1
		}

		private enum CharacterType
		{
			LetterLike = 0,
			Symbol = 1,
			Symbol2 = 2,
			WhiteSpace = 3
		}

		private enum TextEditOp
		{
			MoveLeft = 0,
			MoveRight = 1,
			MoveUp = 2,
			MoveDown = 3,
			MoveLineStart = 4,
			MoveLineEnd = 5,
			MoveTextStart = 6,
			MoveTextEnd = 7,
			MovePageUp = 8,
			MovePageDown = 9,
			MoveGraphicalLineStart = 10,
			MoveGraphicalLineEnd = 11,
			MoveWordLeft = 12,
			MoveWordRight = 13,
			MoveParagraphForward = 14,
			MoveParagraphBackward = 15,
			MoveToStartOfNextWord = 16,
			MoveToEndOfPreviousWord = 17,
			SelectLeft = 18,
			SelectRight = 19,
			SelectUp = 20,
			SelectDown = 21,
			SelectTextStart = 22,
			SelectTextEnd = 23,
			SelectPageUp = 24,
			SelectPageDown = 25,
			ExpandSelectGraphicalLineStart = 26,
			ExpandSelectGraphicalLineEnd = 27,
			SelectGraphicalLineStart = 28,
			SelectGraphicalLineEnd = 29,
			SelectWordLeft = 30,
			SelectWordRight = 31,
			SelectToEndOfPreviousWord = 32,
			SelectToStartOfNextWord = 33,
			SelectParagraphBackward = 34,
			SelectParagraphForward = 35,
			Delete = 36,
			Backspace = 37,
			DeleteWordBack = 38,
			DeleteWordForward = 39,
			DeleteLineBack = 40,
			Cut = 41,
			Copy = 42,
			Paste = 43,
			SelectAll = 44,
			SelectNone = 45,
			ScrollStart = 46,
			ScrollEnd = 47,
			ScrollPageUp = 48,
			ScrollPageDown = 49
		}

		public TouchScreenKeyboard keyboardOnScreen;

		public int pos;

		public int selectPos;

		public int controlID;

		public GUIContent content = new GUIContent();

		public GUIStyle style = GUIStyle.none;

		public Rect position;

		public bool multiline;

		public bool hasHorizontalCursorPos;

		public bool isPasswordField;

		internal bool m_HasFocus;

		public Vector2 scrollOffset = Vector2.zero;

		private bool m_TextHeightPotentiallyChanged;

		public Vector2 graphicalCursorPos;

		public Vector2 graphicalSelectCursorPos;

		private bool m_MouseDragSelectsWholeWords;

		private int m_DblClickInitPos;

		private DblClickSnapping m_DblClickSnap;

		private bool m_bJustSelected;

		private int m_iAltCursorPos = -1;

		private string oldText;

		private int oldPos;

		private int oldSelectPos;

		private static Dictionary<Event, TextEditOp> s_Keyactions;

		public bool hasSelection
		{
			get
			{
				return pos != selectPos;
			}
		}

		public string SelectedText
		{
			get
			{
				int length = content.text.Length;
				if (pos > length)
				{
					pos = length;
				}
				if (selectPos > length)
				{
					selectPos = length;
				}
				if (pos == selectPos)
				{
					return string.Empty;
				}
				if (pos < selectPos)
				{
					return content.text.Substring(pos, selectPos - pos);
				}
				return content.text.Substring(selectPos, pos - selectPos);
			}
		}

		private void ClearCursorPos()
		{
			hasHorizontalCursorPos = false;
			m_iAltCursorPos = -1;
		}

		public void OnFocus()
		{
			if (multiline)
			{
				pos = (selectPos = 0);
			}
			else
			{
				SelectAll();
			}
			m_HasFocus = true;
		}

		public void OnLostFocus()
		{
			m_HasFocus = false;
			scrollOffset = Vector2.zero;
		}

		private void GrabGraphicalCursorPos()
		{
			if (!hasHorizontalCursorPos)
			{
				graphicalCursorPos = style.GetCursorPixelPosition(position, content, pos);
				graphicalSelectCursorPos = style.GetCursorPixelPosition(position, content, selectPos);
				hasHorizontalCursorPos = false;
			}
		}

		public bool HandleKeyEvent(Event e)
		{
			InitKeyActions();
			EventModifiers modifiers = e.modifiers;
			e.modifiers &= ~EventModifiers.CapsLock;
			if (s_Keyactions.ContainsKey(e))
			{
				TextEditOp operation = s_Keyactions[e];
				PerformOperation(operation);
				e.modifiers = modifiers;
				UpdateScrollOffset();
				return true;
			}
			e.modifiers = modifiers;
			return false;
		}

		public bool DeleteLineBack()
		{
			if (hasSelection)
			{
				DeleteSelection();
				return true;
			}
			int num = pos;
			int num2 = num;
			while (num2-- != 0)
			{
				if (content.text[num2] == '\n')
				{
					num = num2 + 1;
					break;
				}
			}
			if (num2 == -1)
			{
				num = 0;
			}
			if (pos != num)
			{
				content.text = content.text.Remove(num, pos - num);
				selectPos = (pos = num);
				return true;
			}
			return false;
		}

		public bool DeleteWordBack()
		{
			if (hasSelection)
			{
				DeleteSelection();
				return true;
			}
			int num = FindEndOfPreviousWord(pos);
			if (pos != num)
			{
				content.text = content.text.Remove(num, pos - num);
				selectPos = (pos = num);
				return true;
			}
			return false;
		}

		public bool DeleteWordForward()
		{
			if (hasSelection)
			{
				DeleteSelection();
				return true;
			}
			int num = FindStartOfNextWord(pos);
			if (pos < content.text.Length)
			{
				content.text = content.text.Remove(pos, num - pos);
				return true;
			}
			return false;
		}

		public bool Delete()
		{
			if (hasSelection)
			{
				DeleteSelection();
				return true;
			}
			if (pos < content.text.Length)
			{
				content.text = content.text.Remove(pos, 1);
				return true;
			}
			return false;
		}

		public bool CanPaste()
		{
			return GUIUtility.systemCopyBuffer.Length != 0;
		}

		public bool Backspace()
		{
			if (hasSelection)
			{
				DeleteSelection();
				return true;
			}
			if (pos > 0)
			{
				content.text = content.text.Remove(pos - 1, 1);
				selectPos = --pos;
				ClearCursorPos();
				return true;
			}
			return false;
		}

		public void SelectAll()
		{
			pos = 0;
			selectPos = content.text.Length;
			ClearCursorPos();
		}

		public void SelectNone()
		{
			selectPos = pos;
			ClearCursorPos();
		}

		public bool DeleteSelection()
		{
			int length = content.text.Length;
			if (pos > length)
			{
				pos = length;
			}
			if (selectPos > length)
			{
				selectPos = length;
			}
			if (pos == selectPos)
			{
				return false;
			}
			if (pos < selectPos)
			{
				content.text = content.text.Substring(0, pos) + content.text.Substring(selectPos, content.text.Length - selectPos);
				selectPos = pos;
			}
			else
			{
				content.text = content.text.Substring(0, selectPos) + content.text.Substring(pos, content.text.Length - pos);
				pos = selectPos;
			}
			ClearCursorPos();
			return true;
		}

		public void ReplaceSelection(string replace)
		{
			DeleteSelection();
			content.text = content.text.Insert(pos, replace);
			selectPos = (pos += replace.Length);
			ClearCursorPos();
			UpdateScrollOffset();
			m_TextHeightPotentiallyChanged = true;
		}

		public void Insert(char c)
		{
			ReplaceSelection(c.ToString());
		}

		public void MoveSelectionToAltCursor()
		{
			if (m_iAltCursorPos != -1)
			{
				int iAltCursorPos = m_iAltCursorPos;
				string selectedText = SelectedText;
				content.text = content.text.Insert(iAltCursorPos, selectedText);
				if (iAltCursorPos < pos)
				{
					pos += selectedText.Length;
					selectPos += selectedText.Length;
				}
				DeleteSelection();
				selectPos = (pos = iAltCursorPos);
				ClearCursorPos();
				UpdateScrollOffset();
			}
		}

		public void MoveRight()
		{
			ClearCursorPos();
			if (selectPos == pos)
			{
				pos++;
				ClampPos();
				selectPos = pos;
			}
			else if (selectPos > pos)
			{
				pos = selectPos;
			}
			else
			{
				selectPos = pos;
			}
			UpdateScrollOffset();
		}

		public void MoveLeft()
		{
			if (selectPos == pos)
			{
				pos--;
				if (pos < 0)
				{
					pos = 0;
				}
				selectPos = pos;
			}
			else if (selectPos > pos)
			{
				selectPos = pos;
			}
			else
			{
				pos = selectPos;
			}
			ClearCursorPos();
			UpdateScrollOffset();
		}

		public void MoveUp()
		{
			if (selectPos < pos)
			{
				selectPos = pos;
			}
			else
			{
				pos = selectPos;
			}
			GrabGraphicalCursorPos();
			graphicalCursorPos.y -= 1f;
			pos = (selectPos = style.GetCursorStringIndex(position, content, graphicalCursorPos));
			if (pos <= 0)
			{
				ClearCursorPos();
			}
			UpdateScrollOffset();
		}

		public void MoveDown()
		{
			if (selectPos > pos)
			{
				selectPos = pos;
			}
			else
			{
				pos = selectPos;
			}
			GrabGraphicalCursorPos();
			graphicalCursorPos.y += style.lineHeight + 5f;
			pos = (selectPos = style.GetCursorStringIndex(position, content, graphicalCursorPos));
			if (pos == content.text.Length)
			{
				ClearCursorPos();
			}
			UpdateScrollOffset();
		}

		public void MoveLineStart()
		{
			int num = ((selectPos >= pos) ? pos : selectPos);
			int num2 = num;
			while (num2-- != 0)
			{
				if (content.text[num2] == '\n')
				{
					selectPos = (pos = num2 + 1);
					return;
				}
			}
			selectPos = (pos = 0);
			UpdateScrollOffset();
		}

		public void MoveLineEnd()
		{
			int num = ((selectPos <= pos) ? pos : selectPos);
			int i = num;
			int length;
			for (length = content.text.Length; i < length; i++)
			{
				if (content.text[i] == '\n')
				{
					selectPos = (pos = i);
					return;
				}
			}
			selectPos = (pos = length);
			UpdateScrollOffset();
		}

		public void MoveGraphicalLineStart()
		{
			pos = (selectPos = GetGraphicalLineStart((pos >= selectPos) ? selectPos : pos));
			UpdateScrollOffset();
		}

		public void MoveGraphicalLineEnd()
		{
			pos = (selectPos = GetGraphicalLineEnd((pos <= selectPos) ? selectPos : pos));
			UpdateScrollOffset();
		}

		public void MoveTextStart()
		{
			selectPos = (pos = 0);
			UpdateScrollOffset();
		}

		public void MoveTextEnd()
		{
			selectPos = (pos = content.text.Length);
			UpdateScrollOffset();
		}

		public void MoveParagraphForward()
		{
			pos = ((pos <= selectPos) ? selectPos : pos);
			if (pos < content.text.Length)
			{
				selectPos = (pos = content.text.IndexOf('\n', pos + 1));
				if (pos == -1)
				{
					selectPos = (pos = content.text.Length);
				}
			}
			UpdateScrollOffset();
		}

		public void MoveParagraphBackward()
		{
			pos = ((pos >= selectPos) ? selectPos : pos);
			if (pos > 1)
			{
				selectPos = (pos = content.text.LastIndexOf('\n', pos - 2) + 1);
			}
			else
			{
				selectPos = (pos = 0);
			}
			UpdateScrollOffset();
		}

		public void MoveCursorToPosition(Vector2 cursorPosition)
		{
			selectPos = style.GetCursorStringIndex(position, content, cursorPosition + scrollOffset);
			if (!Event.current.shift)
			{
				pos = selectPos;
			}
			ClampPos();
			UpdateScrollOffset();
		}

		public void MoveAltCursorToPosition(Vector2 cursorPosition)
		{
			m_iAltCursorPos = style.GetCursorStringIndex(position, content, cursorPosition + scrollOffset);
			ClampPos();
			UpdateScrollOffset();
		}

		public bool IsOverSelection(Vector2 cursorPosition)
		{
			int cursorStringIndex = style.GetCursorStringIndex(position, content, cursorPosition + scrollOffset);
			return cursorStringIndex < Mathf.Max(pos, selectPos) && cursorStringIndex > Mathf.Min(pos, selectPos);
		}

		public void SelectToPosition(Vector2 cursorPosition)
		{
			if (!m_MouseDragSelectsWholeWords)
			{
				pos = style.GetCursorStringIndex(position, content, cursorPosition + scrollOffset);
			}
			else
			{
				int num = style.GetCursorStringIndex(position, content, cursorPosition + scrollOffset);
				if (m_DblClickSnap == DblClickSnapping.WORDS)
				{
					if (num < m_DblClickInitPos)
					{
						pos = FindEndOfClassification(num, -1);
						selectPos = FindEndOfClassification(m_DblClickInitPos, 1);
					}
					else
					{
						if (num >= content.text.Length)
						{
							num = content.text.Length - 1;
						}
						pos = FindEndOfClassification(num, 1);
						selectPos = FindEndOfClassification(m_DblClickInitPos - 1, -1);
					}
				}
				else if (num < m_DblClickInitPos)
				{
					if (num > 0)
					{
						pos = content.text.LastIndexOf('\n', num - 2) + 1;
					}
					else
					{
						pos = 0;
					}
					selectPos = content.text.LastIndexOf('\n', m_DblClickInitPos);
				}
				else
				{
					if (num < content.text.Length)
					{
						pos = content.text.IndexOf('\n', num + 1) + 1;
						if (pos <= 0)
						{
							pos = content.text.Length;
						}
					}
					else
					{
						pos = content.text.Length;
					}
					selectPos = content.text.LastIndexOf('\n', m_DblClickInitPos - 2) + 1;
				}
			}
			UpdateScrollOffset();
		}

		public void SelectLeft()
		{
			if (m_bJustSelected && pos > selectPos)
			{
				int num = pos;
				pos = selectPos;
				selectPos = num;
			}
			m_bJustSelected = false;
			pos--;
			if (pos < 0)
			{
				pos = 0;
			}
			UpdateScrollOffset();
		}

		public void SelectRight()
		{
			if (m_bJustSelected && pos < selectPos)
			{
				int num = pos;
				pos = selectPos;
				selectPos = num;
			}
			m_bJustSelected = false;
			pos++;
			int length = content.text.Length;
			if (pos > length)
			{
				pos = length;
			}
			UpdateScrollOffset();
		}

		public void SelectUp()
		{
			GrabGraphicalCursorPos();
			graphicalCursorPos.y -= 1f;
			pos = style.GetCursorStringIndex(position, content, graphicalCursorPos);
			UpdateScrollOffset();
		}

		public void SelectDown()
		{
			GrabGraphicalCursorPos();
			graphicalCursorPos.y += style.lineHeight + 5f;
			pos = style.GetCursorStringIndex(position, content, graphicalCursorPos);
			UpdateScrollOffset();
		}

		public void SelectTextEnd()
		{
			pos = content.text.Length;
			UpdateScrollOffset();
		}

		public void SelectTextStart()
		{
			pos = 0;
			UpdateScrollOffset();
		}

		public void MouseDragSelectsWholeWords(bool on)
		{
			m_MouseDragSelectsWholeWords = on;
			m_DblClickInitPos = pos;
		}

		public void DblClickSnap(DblClickSnapping snapping)
		{
			m_DblClickSnap = snapping;
		}

		private int GetGraphicalLineStart(int p)
		{
			Vector2 cursorPixelPosition = style.GetCursorPixelPosition(position, content, p);
			cursorPixelPosition.x = 0f;
			return style.GetCursorStringIndex(position, content, cursorPixelPosition);
		}

		private int GetGraphicalLineEnd(int p)
		{
			Vector2 cursorPixelPosition = style.GetCursorPixelPosition(position, content, p);
			cursorPixelPosition.x += 5000f;
			return style.GetCursorStringIndex(position, content, cursorPixelPosition);
		}

		private int FindNextSeperator(int startPos)
		{
			int length = content.text.Length;
			while (startPos < length && !isLetterLikeChar(content.text[startPos]))
			{
				startPos++;
			}
			while (startPos < length && isLetterLikeChar(content.text[startPos]))
			{
				startPos++;
			}
			return startPos;
		}

		private static bool isLetterLikeChar(char c)
		{
			return char.IsLetterOrDigit(c) || c == '\'';
		}

		private int FindPrevSeperator(int startPos)
		{
			startPos--;
			while (startPos > 0 && !isLetterLikeChar(content.text[startPos]))
			{
				startPos--;
			}
			while (startPos >= 0 && isLetterLikeChar(content.text[startPos]))
			{
				startPos--;
			}
			return startPos + 1;
		}

		public void MoveWordRight()
		{
			pos = ((pos <= selectPos) ? selectPos : pos);
			pos = (selectPos = FindNextSeperator(pos));
			ClearCursorPos();
			UpdateScrollOffset();
		}

		public void MoveToStartOfNextWord()
		{
			ClearCursorPos();
			if (pos != selectPos)
			{
				MoveRight();
				return;
			}
			pos = (selectPos = FindStartOfNextWord(pos));
			UpdateScrollOffset();
		}

		public void MoveToEndOfPreviousWord()
		{
			ClearCursorPos();
			if (pos != selectPos)
			{
				MoveLeft();
				return;
			}
			pos = (selectPos = FindEndOfPreviousWord(pos));
			UpdateScrollOffset();
		}

		public void SelectToStartOfNextWord()
		{
			ClearCursorPos();
			pos = FindStartOfNextWord(pos);
			UpdateScrollOffset();
		}

		public void SelectToEndOfPreviousWord()
		{
			ClearCursorPos();
			pos = FindEndOfPreviousWord(pos);
			UpdateScrollOffset();
		}

		private CharacterType ClassifyChar(char c)
		{
			if (char.IsWhiteSpace(c))
			{
				return CharacterType.WhiteSpace;
			}
			if (char.IsLetterOrDigit(c) || c == '\'')
			{
				return CharacterType.LetterLike;
			}
			return CharacterType.Symbol;
		}

		public int FindStartOfNextWord(int p)
		{
			int length = content.text.Length;
			if (p == length)
			{
				return p;
			}
			char c = content.text[p];
			CharacterType characterType = ClassifyChar(c);
			if (characterType != CharacterType.WhiteSpace)
			{
				p++;
				while (p < length && ClassifyChar(content.text[p]) == characterType)
				{
					p++;
				}
			}
			else if (c == '\t' || c == '\n')
			{
				return p + 1;
			}
			if (p == length)
			{
				return p;
			}
			c = content.text[p];
			if (c == ' ')
			{
				while (p < length && char.IsWhiteSpace(content.text[p]))
				{
					p++;
				}
			}
			else if (c == '\t' || c == '\n')
			{
				return p;
			}
			return p;
		}

		private int FindEndOfPreviousWord(int p)
		{
			if (p == 0)
			{
				return p;
			}
			p--;
			while (p > 0 && content.text[p] == ' ')
			{
				p--;
			}
			CharacterType characterType = ClassifyChar(content.text[p]);
			if (characterType != CharacterType.WhiteSpace)
			{
				while (p > 0 && ClassifyChar(content.text[p - 1]) == characterType)
				{
					p--;
				}
			}
			return p;
		}

		public void MoveWordLeft()
		{
			pos = ((pos >= selectPos) ? selectPos : pos);
			pos = FindPrevSeperator(pos);
			selectPos = pos;
			UpdateScrollOffset();
		}

		public void SelectWordRight()
		{
			ClearCursorPos();
			int num = selectPos;
			if (pos < selectPos)
			{
				selectPos = pos;
				MoveWordRight();
				selectPos = num;
				pos = ((pos >= selectPos) ? selectPos : pos);
			}
			else
			{
				selectPos = pos;
				MoveWordRight();
				selectPos = num;
				UpdateScrollOffset();
			}
		}

		public void SelectWordLeft()
		{
			ClearCursorPos();
			int num = selectPos;
			if (pos > selectPos)
			{
				selectPos = pos;
				MoveWordLeft();
				selectPos = num;
				pos = ((pos <= selectPos) ? selectPos : pos);
			}
			else
			{
				selectPos = pos;
				MoveWordLeft();
				selectPos = num;
				UpdateScrollOffset();
			}
		}

		public void ExpandSelectGraphicalLineStart()
		{
			ClearCursorPos();
			if (pos < selectPos)
			{
				pos = GetGraphicalLineStart(pos);
			}
			else
			{
				int num = pos;
				pos = GetGraphicalLineStart(selectPos);
				selectPos = num;
			}
			UpdateScrollOffset();
		}

		public void ExpandSelectGraphicalLineEnd()
		{
			ClearCursorPos();
			if (pos > selectPos)
			{
				pos = GetGraphicalLineEnd(pos);
			}
			else
			{
				int num = pos;
				pos = GetGraphicalLineEnd(selectPos);
				selectPos = num;
			}
			UpdateScrollOffset();
		}

		public void SelectGraphicalLineStart()
		{
			ClearCursorPos();
			pos = GetGraphicalLineStart(pos);
			UpdateScrollOffset();
		}

		public void SelectGraphicalLineEnd()
		{
			ClearCursorPos();
			pos = GetGraphicalLineEnd(pos);
			UpdateScrollOffset();
		}

		public void SelectParagraphForward()
		{
			ClearCursorPos();
			bool flag = pos < selectPos;
			if (pos < content.text.Length)
			{
				pos = content.text.IndexOf('\n', pos + 1);
				if (pos == -1)
				{
					pos = content.text.Length;
				}
				if (flag && pos > selectPos)
				{
					pos = selectPos;
				}
			}
			UpdateScrollOffset();
		}

		public void SelectParagraphBackward()
		{
			ClearCursorPos();
			bool flag = pos > selectPos;
			if (pos > 1)
			{
				pos = content.text.LastIndexOf('\n', pos - 2) + 1;
				if (flag && pos < selectPos)
				{
					pos = selectPos;
				}
			}
			else
			{
				selectPos = (pos = 0);
			}
			UpdateScrollOffset();
		}

		public void SelectCurrentWord()
		{
			ClearCursorPos();
			int length = content.text.Length;
			selectPos = pos;
			if (length != 0)
			{
				if (pos >= length)
				{
					pos = length - 1;
				}
				if (selectPos >= length)
				{
					selectPos--;
				}
				if (pos < selectPos)
				{
					pos = FindEndOfClassification(pos, -1);
					selectPos = FindEndOfClassification(selectPos, 1);
				}
				else
				{
					pos = FindEndOfClassification(pos, 1);
					selectPos = FindEndOfClassification(selectPos, -1);
				}
				m_bJustSelected = true;
				UpdateScrollOffset();
			}
		}

		private int FindEndOfClassification(int p, int dir)
		{
			int length = content.text.Length;
			if (p >= length || p < 0)
			{
				return p;
			}
			CharacterType characterType = ClassifyChar(content.text[p]);
			do
			{
				p += dir;
				if (p < 0)
				{
					return 0;
				}
				if (p >= length)
				{
					return length;
				}
			}
			while (ClassifyChar(content.text[p]) == characterType);
			if (dir == 1)
			{
				return p;
			}
			return p + 1;
		}

		public void SelectCurrentParagraph()
		{
			ClearCursorPos();
			int length = content.text.Length;
			if (pos < length)
			{
				pos = content.text.IndexOf('\n', pos);
				if (pos == -1)
				{
					pos = content.text.Length;
				}
				else
				{
					pos++;
				}
			}
			if (selectPos != 0)
			{
				selectPos = content.text.LastIndexOf('\n', selectPos - 1) + 1;
			}
			UpdateScrollOffset();
		}

		public void UpdateScrollOffsetIfNeeded()
		{
			if (m_TextHeightPotentiallyChanged)
			{
				UpdateScrollOffset();
				m_TextHeightPotentiallyChanged = false;
			}
		}

		private void UpdateScrollOffset()
		{
			int cursorStringIndex = pos;
			graphicalCursorPos = style.GetCursorPixelPosition(new Rect(0f, 0f, position.width, position.height), content, cursorStringIndex);
			Rect rect = style.padding.Remove(position);
			Vector2 vector = new Vector2(style.CalcSize(content).x, style.CalcHeight(content, position.width));
			if (vector.x < position.width)
			{
				scrollOffset.x = 0f;
			}
			else
			{
				if (graphicalCursorPos.x + 1f > scrollOffset.x + rect.width)
				{
					scrollOffset.x = graphicalCursorPos.x - rect.width;
				}
				if (graphicalCursorPos.x < scrollOffset.x + (float)style.padding.left)
				{
					scrollOffset.x = graphicalCursorPos.x - (float)style.padding.left;
				}
			}
			if (vector.y < rect.height)
			{
				scrollOffset.y = 0f;
			}
			else
			{
				if (graphicalCursorPos.y + style.lineHeight > scrollOffset.y + rect.height + (float)style.padding.top)
				{
					scrollOffset.y = graphicalCursorPos.y - rect.height - (float)style.padding.top + style.lineHeight;
				}
				if (graphicalCursorPos.y < scrollOffset.y + (float)style.padding.top)
				{
					scrollOffset.y = graphicalCursorPos.y - (float)style.padding.top;
				}
			}
			if (scrollOffset.y > 0f && vector.y - scrollOffset.y < rect.height)
			{
				scrollOffset.y = vector.y - rect.height - (float)style.padding.top - (float)style.padding.bottom;
			}
			scrollOffset.y = ((!(scrollOffset.y < 0f)) ? scrollOffset.y : 0f);
		}

		public void DrawCursor(string text)
		{
			string text2 = content.text;
			int num = pos;
			if (Input.compositionString.Length > 0)
			{
				content.text = text.Substring(0, pos) + Input.compositionString + text.Substring(selectPos);
				num += Input.compositionString.Length;
			}
			else
			{
				content.text = text;
			}
			graphicalCursorPos = style.GetCursorPixelPosition(new Rect(0f, 0f, position.width, position.height), content, num);
			Vector2 contentOffset = style.contentOffset;
			style.contentOffset -= scrollOffset;
			style.Internal_clipOffset = scrollOffset;
			Input.compositionCursorPos = graphicalCursorPos + new Vector2(position.x, position.y + style.lineHeight) - scrollOffset;
			if (Input.compositionString.Length > 0)
			{
				style.DrawWithTextSelection(position, content, controlID, pos, pos + Input.compositionString.Length, true);
			}
			else
			{
				style.DrawWithTextSelection(position, content, controlID, pos, selectPos);
			}
			if (m_iAltCursorPos != -1)
			{
				style.DrawCursor(position, content, controlID, m_iAltCursorPos);
			}
			style.contentOffset = contentOffset;
			style.Internal_clipOffset = Vector2.zero;
			content.text = text2;
		}

		private bool PerformOperation(TextEditOp operation)
		{
			switch (operation)
			{
			case TextEditOp.MoveLeft:
				MoveLeft();
				break;
			case TextEditOp.MoveRight:
				MoveRight();
				break;
			case TextEditOp.MoveUp:
				MoveUp();
				break;
			case TextEditOp.MoveDown:
				MoveDown();
				break;
			case TextEditOp.MoveLineStart:
				MoveLineStart();
				break;
			case TextEditOp.MoveLineEnd:
				MoveLineEnd();
				break;
			case TextEditOp.MoveWordRight:
				MoveWordRight();
				break;
			case TextEditOp.MoveToStartOfNextWord:
				MoveToStartOfNextWord();
				break;
			case TextEditOp.MoveToEndOfPreviousWord:
				MoveToEndOfPreviousWord();
				break;
			case TextEditOp.MoveWordLeft:
				MoveWordLeft();
				break;
			case TextEditOp.MoveTextStart:
				MoveTextStart();
				break;
			case TextEditOp.MoveTextEnd:
				MoveTextEnd();
				break;
			case TextEditOp.MoveParagraphForward:
				MoveParagraphForward();
				break;
			case TextEditOp.MoveParagraphBackward:
				MoveParagraphBackward();
				break;
			case TextEditOp.MoveGraphicalLineStart:
				MoveGraphicalLineStart();
				break;
			case TextEditOp.MoveGraphicalLineEnd:
				MoveGraphicalLineEnd();
				break;
			case TextEditOp.SelectLeft:
				SelectLeft();
				break;
			case TextEditOp.SelectRight:
				SelectRight();
				break;
			case TextEditOp.SelectUp:
				SelectUp();
				break;
			case TextEditOp.SelectDown:
				SelectDown();
				break;
			case TextEditOp.SelectWordRight:
				SelectWordRight();
				break;
			case TextEditOp.SelectWordLeft:
				SelectWordLeft();
				break;
			case TextEditOp.SelectToEndOfPreviousWord:
				SelectToEndOfPreviousWord();
				break;
			case TextEditOp.SelectToStartOfNextWord:
				SelectToStartOfNextWord();
				break;
			case TextEditOp.SelectTextStart:
				SelectTextStart();
				break;
			case TextEditOp.SelectTextEnd:
				SelectTextEnd();
				break;
			case TextEditOp.ExpandSelectGraphicalLineStart:
				ExpandSelectGraphicalLineStart();
				break;
			case TextEditOp.ExpandSelectGraphicalLineEnd:
				ExpandSelectGraphicalLineEnd();
				break;
			case TextEditOp.SelectParagraphForward:
				SelectParagraphForward();
				break;
			case TextEditOp.SelectParagraphBackward:
				SelectParagraphBackward();
				break;
			case TextEditOp.SelectGraphicalLineStart:
				SelectGraphicalLineStart();
				break;
			case TextEditOp.SelectGraphicalLineEnd:
				SelectGraphicalLineEnd();
				break;
			case TextEditOp.Delete:
				return Delete();
			case TextEditOp.Backspace:
				return Backspace();
			case TextEditOp.Cut:
				return Cut();
			case TextEditOp.Copy:
				Copy();
				break;
			case TextEditOp.Paste:
				return Paste();
			case TextEditOp.SelectAll:
				SelectAll();
				break;
			case TextEditOp.SelectNone:
				SelectNone();
				break;
			case TextEditOp.DeleteWordBack:
				return DeleteWordBack();
			case TextEditOp.DeleteLineBack:
				return DeleteLineBack();
			case TextEditOp.DeleteWordForward:
				return DeleteWordForward();
			default:
				Debug.Log("Unimplemented: " + operation);
				break;
			}
			return false;
		}

		public void SaveBackup()
		{
			oldText = content.text;
			oldPos = pos;
			oldSelectPos = selectPos;
		}

		public void Undo()
		{
			content.text = oldText;
			pos = oldPos;
			selectPos = oldSelectPos;
			UpdateScrollOffset();
		}

		public bool Cut()
		{
			if (isPasswordField)
			{
				return false;
			}
			Copy();
			return DeleteSelection();
		}

		public void Copy()
		{
			if (selectPos != pos && !isPasswordField)
			{
				string systemCopyBuffer = ((pos >= selectPos) ? content.text.Substring(selectPos, pos - selectPos) : content.text.Substring(pos, selectPos - pos));
				GUIUtility.systemCopyBuffer = systemCopyBuffer;
			}
		}

		public bool Paste()
		{
			string systemCopyBuffer = GUIUtility.systemCopyBuffer;
			if (systemCopyBuffer != string.Empty)
			{
				ReplaceSelection(systemCopyBuffer);
				return true;
			}
			return false;
		}

		private static void MapKey(string key, TextEditOp action)
		{
			s_Keyactions[Event.KeyboardEvent(key)] = action;
		}

		private void InitKeyActions()
		{
			if (s_Keyactions == null)
			{
				s_Keyactions = new Dictionary<Event, TextEditOp>();
				MapKey("left", TextEditOp.MoveLeft);
				MapKey("right", TextEditOp.MoveRight);
				MapKey("up", TextEditOp.MoveUp);
				MapKey("down", TextEditOp.MoveDown);
				MapKey("#left", TextEditOp.SelectLeft);
				MapKey("#right", TextEditOp.SelectRight);
				MapKey("#up", TextEditOp.SelectUp);
				MapKey("#down", TextEditOp.SelectDown);
				MapKey("delete", TextEditOp.Delete);
				MapKey("backspace", TextEditOp.Backspace);
				MapKey("#backspace", TextEditOp.Backspace);
				if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.OSXEditor)
				{
					MapKey("^left", TextEditOp.MoveGraphicalLineStart);
					MapKey("^right", TextEditOp.MoveGraphicalLineEnd);
					MapKey("&left", TextEditOp.MoveWordLeft);
					MapKey("&right", TextEditOp.MoveWordRight);
					MapKey("&up", TextEditOp.MoveParagraphBackward);
					MapKey("&down", TextEditOp.MoveParagraphForward);
					MapKey("%left", TextEditOp.MoveGraphicalLineStart);
					MapKey("%right", TextEditOp.MoveGraphicalLineEnd);
					MapKey("%up", TextEditOp.MoveTextStart);
					MapKey("%down", TextEditOp.MoveTextEnd);
					MapKey("#home", TextEditOp.SelectTextStart);
					MapKey("#end", TextEditOp.SelectTextEnd);
					MapKey("#^left", TextEditOp.ExpandSelectGraphicalLineStart);
					MapKey("#^right", TextEditOp.ExpandSelectGraphicalLineEnd);
					MapKey("#^up", TextEditOp.SelectParagraphBackward);
					MapKey("#^down", TextEditOp.SelectParagraphForward);
					MapKey("#&left", TextEditOp.SelectWordLeft);
					MapKey("#&right", TextEditOp.SelectWordRight);
					MapKey("#&up", TextEditOp.SelectParagraphBackward);
					MapKey("#&down", TextEditOp.SelectParagraphForward);
					MapKey("#%left", TextEditOp.ExpandSelectGraphicalLineStart);
					MapKey("#%right", TextEditOp.ExpandSelectGraphicalLineEnd);
					MapKey("#%up", TextEditOp.SelectTextStart);
					MapKey("#%down", TextEditOp.SelectTextEnd);
					MapKey("%a", TextEditOp.SelectAll);
					MapKey("%x", TextEditOp.Cut);
					MapKey("%c", TextEditOp.Copy);
					MapKey("%v", TextEditOp.Paste);
					MapKey("^d", TextEditOp.Delete);
					MapKey("^h", TextEditOp.Backspace);
					MapKey("^b", TextEditOp.MoveLeft);
					MapKey("^f", TextEditOp.MoveRight);
					MapKey("^a", TextEditOp.MoveLineStart);
					MapKey("^e", TextEditOp.MoveLineEnd);
					MapKey("&delete", TextEditOp.DeleteWordForward);
					MapKey("&backspace", TextEditOp.DeleteWordBack);
					MapKey("%backspace", TextEditOp.DeleteLineBack);
				}
				else
				{
					MapKey("home", TextEditOp.MoveGraphicalLineStart);
					MapKey("end", TextEditOp.MoveGraphicalLineEnd);
					MapKey("%left", TextEditOp.MoveWordLeft);
					MapKey("%right", TextEditOp.MoveWordRight);
					MapKey("%up", TextEditOp.MoveParagraphBackward);
					MapKey("%down", TextEditOp.MoveParagraphForward);
					MapKey("^left", TextEditOp.MoveToEndOfPreviousWord);
					MapKey("^right", TextEditOp.MoveToStartOfNextWord);
					MapKey("^up", TextEditOp.MoveParagraphBackward);
					MapKey("^down", TextEditOp.MoveParagraphForward);
					MapKey("#^left", TextEditOp.SelectToEndOfPreviousWord);
					MapKey("#^right", TextEditOp.SelectToStartOfNextWord);
					MapKey("#^up", TextEditOp.SelectParagraphBackward);
					MapKey("#^down", TextEditOp.SelectParagraphForward);
					MapKey("#home", TextEditOp.SelectGraphicalLineStart);
					MapKey("#end", TextEditOp.SelectGraphicalLineEnd);
					MapKey("^delete", TextEditOp.DeleteWordForward);
					MapKey("^backspace", TextEditOp.DeleteWordBack);
					MapKey("%backspace", TextEditOp.DeleteLineBack);
					MapKey("^a", TextEditOp.SelectAll);
					MapKey("^x", TextEditOp.Cut);
					MapKey("^c", TextEditOp.Copy);
					MapKey("^v", TextEditOp.Paste);
					MapKey("#delete", TextEditOp.Cut);
					MapKey("^insert", TextEditOp.Copy);
					MapKey("#insert", TextEditOp.Paste);
				}
			}
		}

		public void ClampPos()
		{
			if (m_HasFocus && controlID != GUIUtility.keyboardControl)
			{
				OnLostFocus();
			}
			if (!m_HasFocus && controlID == GUIUtility.keyboardControl)
			{
				OnFocus();
			}
			if (pos < 0)
			{
				pos = 0;
			}
			else if (pos > content.text.Length)
			{
				pos = content.text.Length;
			}
			if (selectPos < 0)
			{
				selectPos = 0;
			}
			else if (selectPos > content.text.Length)
			{
				selectPos = content.text.Length;
			}
			if (m_iAltCursorPos > content.text.Length)
			{
				m_iAltCursorPos = content.text.Length;
			}
		}
	}
}
