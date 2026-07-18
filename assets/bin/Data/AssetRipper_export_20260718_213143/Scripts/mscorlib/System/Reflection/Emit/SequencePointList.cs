using System.Diagnostics.SymbolStore;

namespace System.Reflection.Emit
{
	internal class SequencePointList
	{
		private const int arrayGrow = 10;

		private ISymbolDocumentWriter doc;

		private SequencePoint[] points;

		private int count;

		public ISymbolDocumentWriter Document
		{
			get
			{
				return doc;
			}
		}

		public int StartLine
		{
			get
			{
				return points[0].Line;
			}
		}

		public int EndLine
		{
			get
			{
				return points[count - 1].Line;
			}
		}

		public int StartColumn
		{
			get
			{
				return points[0].Col;
			}
		}

		public int EndColumn
		{
			get
			{
				return points[count - 1].Col;
			}
		}

		public SequencePointList(ISymbolDocumentWriter doc)
		{
			this.doc = doc;
		}

		public int[] GetOffsets()
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = points[i].Offset;
			}
			return array;
		}

		public int[] GetLines()
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = points[i].Line;
			}
			return array;
		}

		public int[] GetColumns()
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = points[i].Col;
			}
			return array;
		}

		public int[] GetEndLines()
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = points[i].EndLine;
			}
			return array;
		}

		public int[] GetEndColumns()
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = points[i].EndCol;
			}
			return array;
		}

		public void AddSequencePoint(int offset, int line, int col, int endLine, int endCol)
		{
			SequencePoint sequencePoint = new SequencePoint
			{
				Offset = offset,
				Line = line,
				Col = col,
				EndLine = endLine,
				EndCol = endCol
			};
			if (points == null)
			{
				points = new SequencePoint[10];
			}
			else if (count >= points.Length)
			{
				SequencePoint[] destinationArray = new SequencePoint[count + 10];
				Array.Copy(points, destinationArray, points.Length);
				points = destinationArray;
			}
			points[count] = sequencePoint;
			count++;
		}
	}
}
