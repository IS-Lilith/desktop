using Gecko.Net;
using Gecko.Observers;
using System;
using System.Collections.Generic;

namespace AlipayBank.Web.utils
{
	public class MyObserver : BaseHttpModifyRequestObserver
	{
		public delegate void TicketLoadedEventHandler(HttpChannel p_HttpChannel, object sender, EventArgs e);

		public List<string> targetUrls = new List<string>();

		public event TicketLoadedEventHandler TicketLoadedEvent;

		protected override void ObserveRequest(HttpChannel p_HttpChannel)
		{
			if (p_HttpChannel != null)
			{
				TraceableChannel traceableChannel = p_HttpChannel.CastToTraceableChannel();
				StreamListenerTee streamListenerTee = new StreamListenerTee();
				streamListenerTee.Completed += delegate(object sender, EventArgs e)
				{
					this.TicketLoadedEvent(p_HttpChannel, sender, e);
				};
				traceableChannel.SetNewListener(streamListenerTee);
			}
		}
	}
}
