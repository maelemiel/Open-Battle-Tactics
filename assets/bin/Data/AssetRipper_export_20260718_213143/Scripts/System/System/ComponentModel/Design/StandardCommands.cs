namespace System.ComponentModel.Design
{
	public class StandardCommands
	{
		public static readonly CommandID AlignBottom;

		public static readonly CommandID AlignHorizontalCenters;

		public static readonly CommandID AlignLeft;

		public static readonly CommandID AlignRight;

		public static readonly CommandID AlignToGrid;

		public static readonly CommandID AlignTop;

		public static readonly CommandID AlignVerticalCenters;

		public static readonly CommandID ArrangeBottom;

		public static readonly CommandID ArrangeIcons;

		public static readonly CommandID ArrangeRight;

		public static readonly CommandID BringForward;

		public static readonly CommandID BringToFront;

		public static readonly CommandID CenterHorizontally;

		public static readonly CommandID CenterVertically;

		public static readonly CommandID Copy;

		public static readonly CommandID Cut;

		public static readonly CommandID Delete;

		public static readonly CommandID F1Help;

		public static readonly CommandID Group;

		public static readonly CommandID HorizSpaceConcatenate;

		public static readonly CommandID HorizSpaceDecrease;

		public static readonly CommandID HorizSpaceIncrease;

		public static readonly CommandID HorizSpaceMakeEqual;

		public static readonly CommandID LineupIcons;

		public static readonly CommandID LockControls;

		public static readonly CommandID MultiLevelRedo;

		public static readonly CommandID MultiLevelUndo;

		public static readonly CommandID Paste;

		public static readonly CommandID Properties;

		public static readonly CommandID PropertiesWindow;

		public static readonly CommandID Redo;

		public static readonly CommandID Replace;

		public static readonly CommandID SelectAll;

		public static readonly CommandID SendBackward;

		public static readonly CommandID SendToBack;

		public static readonly CommandID ShowGrid;

		public static readonly CommandID ShowLargeIcons;

		public static readonly CommandID SizeToControl;

		public static readonly CommandID SizeToControlHeight;

		public static readonly CommandID SizeToControlWidth;

		public static readonly CommandID SizeToFit;

		public static readonly CommandID SizeToGrid;

		public static readonly CommandID SnapToGrid;

		public static readonly CommandID TabOrder;

		public static readonly CommandID Undo;

		public static readonly CommandID Ungroup;

		public static readonly CommandID VerbFirst;

		public static readonly CommandID VerbLast;

		public static readonly CommandID VertSpaceConcatenate;

		public static readonly CommandID VertSpaceDecrease;

		public static readonly CommandID VertSpaceIncrease;

		public static readonly CommandID VertSpaceMakeEqual;

		public static readonly CommandID ViewGrid;

		public static readonly CommandID DocumentOutline;

		public static readonly CommandID ViewCode;

		static StandardCommands()
		{
			Guid menuGroup = new Guid("5efc7975-14bc-11cf-9b2b-00aa00573819");
			Guid menuGroup2 = new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7");
			AlignBottom = new CommandID(menuGroup, 1);
			AlignHorizontalCenters = new CommandID(menuGroup, 2);
			AlignLeft = new CommandID(menuGroup, 3);
			AlignRight = new CommandID(menuGroup, 4);
			AlignToGrid = new CommandID(menuGroup, 5);
			AlignTop = new CommandID(menuGroup, 6);
			AlignVerticalCenters = new CommandID(menuGroup, 7);
			ArrangeBottom = new CommandID(menuGroup, 8);
			ArrangeIcons = new CommandID(menuGroup2, 12298);
			ArrangeRight = new CommandID(menuGroup, 9);
			BringForward = new CommandID(menuGroup, 10);
			BringToFront = new CommandID(menuGroup, 11);
			CenterHorizontally = new CommandID(menuGroup, 12);
			CenterVertically = new CommandID(menuGroup, 13);
			Copy = new CommandID(menuGroup, 15);
			Cut = new CommandID(menuGroup, 16);
			Delete = new CommandID(menuGroup, 17);
			F1Help = new CommandID(menuGroup, 377);
			Group = new CommandID(menuGroup, 20);
			HorizSpaceConcatenate = new CommandID(menuGroup, 21);
			HorizSpaceDecrease = new CommandID(menuGroup, 22);
			HorizSpaceIncrease = new CommandID(menuGroup, 23);
			HorizSpaceMakeEqual = new CommandID(menuGroup, 24);
			LineupIcons = new CommandID(menuGroup2, 12299);
			LockControls = new CommandID(menuGroup, 369);
			MultiLevelRedo = new CommandID(menuGroup, 30);
			MultiLevelUndo = new CommandID(menuGroup, 44);
			Paste = new CommandID(menuGroup, 26);
			Properties = new CommandID(menuGroup, 28);
			PropertiesWindow = new CommandID(menuGroup, 235);
			Redo = new CommandID(menuGroup, 29);
			Replace = new CommandID(menuGroup, 230);
			SelectAll = new CommandID(menuGroup, 31);
			SendBackward = new CommandID(menuGroup, 32);
			SendToBack = new CommandID(menuGroup, 33);
			ShowGrid = new CommandID(menuGroup, 103);
			ShowLargeIcons = new CommandID(menuGroup2, 12300);
			SizeToControl = new CommandID(menuGroup, 35);
			SizeToControlHeight = new CommandID(menuGroup, 36);
			SizeToControlWidth = new CommandID(menuGroup, 37);
			SizeToFit = new CommandID(menuGroup, 38);
			SizeToGrid = new CommandID(menuGroup, 39);
			SnapToGrid = new CommandID(menuGroup, 40);
			TabOrder = new CommandID(menuGroup, 41);
			Undo = new CommandID(menuGroup, 43);
			Ungroup = new CommandID(menuGroup, 45);
			VerbFirst = new CommandID(menuGroup2, 8192);
			VerbLast = new CommandID(menuGroup2, 8448);
			VertSpaceConcatenate = new CommandID(menuGroup, 46);
			VertSpaceDecrease = new CommandID(menuGroup, 47);
			VertSpaceIncrease = new CommandID(menuGroup, 48);
			VertSpaceMakeEqual = new CommandID(menuGroup, 49);
			ViewGrid = new CommandID(menuGroup, 125);
			DocumentOutline = new CommandID(menuGroup, 239);
			ViewCode = new CommandID(menuGroup, 333);
		}
	}
}
