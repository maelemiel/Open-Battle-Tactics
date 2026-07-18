using System;
using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdParticleStateManager
	{
		private Hashtable table;

		private XmlSchemaContentProcessing processContents;

		public XmlSchemaElement CurrentElement;

		public Stack ContextStack = new Stack();

		public XsdValidationContext Context = new XsdValidationContext();

		public XmlSchemaContentProcessing ProcessContents
		{
			get
			{
				return processContents;
			}
		}

		public XsdParticleStateManager()
		{
			table = new Hashtable();
			processContents = XmlSchemaContentProcessing.Strict;
		}

		public void PushContext()
		{
			ContextStack.Push(Context.Clone());
		}

		public void PopContext()
		{
			Context = (XsdValidationContext)ContextStack.Pop();
		}

		internal void SetProcessContents(XmlSchemaContentProcessing value)
		{
			processContents = value;
		}

		public XsdValidationState Get(XmlSchemaParticle xsobj)
		{
			XsdValidationState xsdValidationState = table[xsobj] as XsdValidationState;
			if (xsdValidationState == null)
			{
				xsdValidationState = Create(xsobj);
			}
			return xsdValidationState;
		}

		public XsdValidationState Create(XmlSchemaObject xsobj)
		{
			switch (xsobj.GetType().Name)
			{
			case "XmlSchemaElement":
				return AddElement((XmlSchemaElement)xsobj);
			case "XmlSchemaSequence":
				return AddSequence((XmlSchemaSequence)xsobj);
			case "XmlSchemaChoice":
				return AddChoice((XmlSchemaChoice)xsobj);
			case "XmlSchemaAll":
				return AddAll((XmlSchemaAll)xsobj);
			case "XmlSchemaAny":
				return AddAny((XmlSchemaAny)xsobj);
			case "EmptyParticle":
				return AddEmpty();
			default:
				throw new InvalidOperationException("Should not occur.");
			}
		}

		internal XsdValidationState MakeSequence(XsdValidationState head, XsdValidationState rest)
		{
			if (head is XsdEmptyValidationState)
			{
				return rest;
			}
			return new XsdAppendedValidationState(this, head, rest);
		}

		private XsdElementValidationState AddElement(XmlSchemaElement element)
		{
			return new XsdElementValidationState(element, this);
		}

		private XsdSequenceValidationState AddSequence(XmlSchemaSequence sequence)
		{
			return new XsdSequenceValidationState(sequence, this);
		}

		private XsdChoiceValidationState AddChoice(XmlSchemaChoice choice)
		{
			return new XsdChoiceValidationState(choice, this);
		}

		private XsdAllValidationState AddAll(XmlSchemaAll all)
		{
			return new XsdAllValidationState(all, this);
		}

		private XsdAnyValidationState AddAny(XmlSchemaAny any)
		{
			return new XsdAnyValidationState(any, this);
		}

		private XsdEmptyValidationState AddEmpty()
		{
			return new XsdEmptyValidationState(this);
		}
	}
}
