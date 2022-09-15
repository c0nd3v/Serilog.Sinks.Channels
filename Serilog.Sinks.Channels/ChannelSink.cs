using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Core;

namespace Serilog.Sinks.Channels
{
	internal sealed class ChannelSink : ILogEventSink, IDisposable
	{
		private ILogEventSink _wrappedSink;
		private Channel<LogEvent> _logChannel;
		private Task _workerTask;

		public ChannelSink(ILogEventSink wrappedSink)
		{
			_wrappedSink = wrappedSink ?? throw new ArgumentNullException(nameof(wrappedSink));
			_logChannel = Channel.CreateUnbounded<LogEvent>();
			_workerTask = PumpAsync();
		}

		public void Emit(LogEvent logEvent)
		{
			_logChannel.Writer.TryWrite(logEvent);
		}

		public void Dispose()
		{
			// Prevent any more events from being added
			_logChannel.Writer.TryComplete();

			// Allow queued events to be flushed
			_workerTask.Wait();

			(_wrappedSink as IDisposable)?.Dispose();
		}

		private async Task PumpAsync()
		{
			await foreach (var logEvent in _logChannel.Reader.ReadAllAsync())
				_wrappedSink.Emit(logEvent);
		}
	}
}