using System;

namespace WinBot.Commands.Attributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
public sealed class CategoryAttribute : Attribute {
    public CategoryAttribute(Category category) {
        this.category = category;
    }

    /// <summary>
    ///     Gets the category of this command
    /// </summary>
    public Category category { get; }
}

public enum Category {
    Main,
    Fun,
    Misc,
    Staff,
    Owner,
    Images,
    NerdStuff
}
