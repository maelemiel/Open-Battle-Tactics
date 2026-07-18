using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdValidationContext
	{
		private object xsi_type;

		internal XsdValidationState State;

		private Stack element_stack = new Stack();

		public object XsiType
		{
			get
			{
				return xsi_type;
			}
			set
			{
				xsi_type = value;
			}
		}

		public XmlSchemaElement Element
		{
			get
			{
				return (element_stack.Count <= 0) ? null : (element_stack.Peek() as XmlSchemaElement);
			}
		}

		public object ActualType
		{
			get
			{
				if (element_stack.Count == 0)
				{
					return null;
				}
				if (XsiType != null)
				{
					return XsiType;
				}
				return (Element == null) ? null : Element.ElementType;
			}
		}

		public XmlSchemaType ActualSchemaType
		{
			get
			{
				object actualType = ActualType;
				if (actualType == null)
				{
					return null;
				}
				XmlSchemaType xmlSchemaType = actualType as XmlSchemaType;
				if (xmlSchemaType == null)
				{
					xmlSchemaType = XmlSchemaType.GetBuiltInSimpleType(((XmlSchemaDatatype)actualType).TypeCode);
				}
				return xmlSchemaType;
			}
		}

		public bool IsInvalid
		{
			get
			{
				return State == XsdValidationState.Invalid;
			}
		}

		public void PushCurrentElement(XmlSchemaElement element)
		{
			element_stack.Push(element);
		}

		public void PopCurrentElement()
		{
			element_stack.Pop();
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void EvaluateStartElement(string localName, string ns)
		{
			State = State.EvaluateStartElement(localName, ns);
		}

		public bool EvaluateEndElement()
		{
			return State.EvaluateEndElement();
		}
	}
}
