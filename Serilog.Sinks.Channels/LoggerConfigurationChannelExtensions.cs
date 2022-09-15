using System;
using Serilog.Sinks.Channels;
using Serilog.Configuration;
using Serilog.Events;

namespace Serilog
{
	/// <summary>
	/// Extends <see cref="LoggerConfiguration"/> with methods for configuring asynchronous logging using channels.
	/// </summary>
	public static class LoggerConfigurationChannelExtensions
	{
		/// <summary>
		/// Configure a sink to be invoked asynchronously, using a channel.
		/// </summary>
		/// <param name="loggerSinkConfiguration">The <see cref="LoggerSinkConfiguration"/> being configured.</param>
		/// <param name="configure">An action that configures the wrapped sink.</param>
		/// <returns>A <see cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
		public static LoggerConfiguration Channel(
			this LoggerSinkConfiguration loggerSinkConfiguration,
			Action<LoggerSinkConfiguration> configure)
		{
			return LoggerSinkConfiguration.Wrap(
				loggerSinkConfiguration,
				wrappedSink => new ChannelSink(wrappedSink),
				configure,
				LevelAlias.Minimum,
				null);
		}
	}
}