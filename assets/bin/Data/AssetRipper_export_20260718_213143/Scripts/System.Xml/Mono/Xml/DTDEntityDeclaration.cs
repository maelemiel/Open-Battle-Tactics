using System.Collections;
using System.Xml;

namespace Mono.Xml
{
	internal class DTDEntityDeclaration : DTDEntityBase
	{
		private string entityValue;

		private string notationName;

		private ArrayList ReferencingEntities = new ArrayList();

		private bool scanned;

		private bool recursed;

		private bool hasExternalReference;

		public string NotationName
		{
			get
			{
				return notationName;
			}
			set
			{
				notationName = value;
			}
		}

		public bool HasExternalReference
		{
			get
			{
				if (!scanned)
				{
					ScanEntityValue(new ArrayList());
				}
				return hasExternalReference;
			}
		}

		public string EntityValue
		{
			get
			{
				if (base.IsInvalid)
				{
					return string.Empty;
				}
				if (base.PublicId == null && base.SystemId == null && base.LiteralEntityValue == null)
				{
					return string.Empty;
				}
				if (entityValue == null)
				{
					if (NotationName != null)
					{
						entityValue = string.Empty;
					}
					else if (base.SystemId == null || base.SystemId == string.Empty)
					{
						entityValue = base.ReplacementText;
						if (entityValue == null)
						{
							entityValue = string.Empty;
						}
					}
					else
					{
						entityValue = base.ReplacementText;
					}
					ScanEntityValue(new ArrayList());
				}
				return entityValue;
			}
		}

		internal DTDEntityDeclaration(DTDObjectModel root)
			: base(root)
		{
		}

		public void ScanEntityValue(ArrayList refs)
		{
			string text = EntityValue;
			if (base.SystemId != null)
			{
				hasExternalReference = true;
			}
			if (recursed)
			{
				throw NotWFError("Entity recursion was found.");
			}
			recursed = true;
			if (scanned)
			{
				foreach (string @ref in refs)
				{
					if (ReferencingEntities.Contains(@ref))
					{
						throw NotWFError(string.Format("Nested entity was found between {0} and {1}", @ref, base.Name));
					}
				}
				recursed = false;
				return;
			}
			int length = text.Length;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				switch (text[i])
				{
				case '&':
					num = i + 1;
					break;
				case ';':
				{
					if (num == 0)
					{
						break;
					}
					string text3 = text.Substring(num, i - num);
					if (text3.Length == 0)
					{
						throw NotWFError("Entity reference name is missing.");
					}
					if (text3[0] == '#' || XmlChar.GetPredefinedEntity(text3) >= 0)
					{
						break;
					}
					ReferencingEntities.Add(text3);
					DTDEntityDeclaration dTDEntityDeclaration = base.Root.EntityDecls[text3];
					if (dTDEntityDeclaration != null)
					{
						if (dTDEntityDeclaration.SystemId != null)
						{
							hasExternalReference = true;
						}
						refs.Add(base.Name);
						dTDEntityDeclaration.ScanEntityValue(refs);
						foreach (string referencingEntity in dTDEntityDeclaration.ReferencingEntities)
						{
							ReferencingEntities.Add(referencingEntity);
						}
						refs.Remove(base.Name);
						text = text.Remove(num - 1, text3.Length + 2);
						text = text.Insert(num - 1, dTDEntityDeclaration.EntityValue);
						i -= text3.Length + 1;
						length = text.Length;
					}
					num = 0;
					break;
				}
				}
			}
			if (num != 0)
			{
				base.Root.AddError(new XmlException(this, BaseURI, "Invalid reference character '&' is specified."));
			}
			scanned = true;
			recursed = false;
		}
	}
}
