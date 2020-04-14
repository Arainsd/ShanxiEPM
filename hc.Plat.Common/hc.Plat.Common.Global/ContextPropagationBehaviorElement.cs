using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;

namespace hc.Plat.Common.Global
{
    public class ContextPropagationBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ContextPropagationBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new ContextPropagationBehavior();
        }
    }
    public class ContextPropagationCBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ContextPropagationCBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new ContextPropagationCBehavior();
        }
    }
}
