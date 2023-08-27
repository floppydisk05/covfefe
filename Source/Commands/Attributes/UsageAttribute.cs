using System;

namespace WinBot.Commands.Attributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
public sealed class UsageAttribute : Attribute {
    public UsageAttribute(string usage) {
        Usage = usage;
    }

    /// <summary>
    ///     Gets the usage of this command
    /// </summary>
    public string Usage { get; }
}
