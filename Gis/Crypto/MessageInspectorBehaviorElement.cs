using System;
using System.ServiceModel.Configuration;

namespace Gis.Crypto
{
	public class MessageInspectorBehaviorElement : BehaviorExtensionElement
	{
		public override Type BehaviorType => typeof(MessageInspectorBehavior);

		protected override object CreateBehavior()
		{
			return new MessageInspectorBehavior();
		}
	}
}