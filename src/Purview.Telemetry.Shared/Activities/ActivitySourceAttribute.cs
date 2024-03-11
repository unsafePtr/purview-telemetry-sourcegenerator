﻿namespace Purview.Telemetry.Activities;

/// <summary>
/// Determines the default Activity Source name for generated Activities.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
sealed public class ActivitySourceAttribute : Attribute {
	/// <summary>
	/// Constructs a new <see cref="ActivitySourceAttribute"/>.
	/// </summary>
	/// <param name="name">The name of the activity source.</param>
	/// <exception cref="ArgumentNullException">If the <paramref name="name"/> is null, empty or whitespace.</exception>
	public ActivitySourceAttribute(string name) {
		if (string.IsNullOrWhiteSpace(name)) {
			throw new ArgumentNullException(nameof(name));
		}

		Name = name;
	}

	/// <summary>
	/// The default Activity Source name to use.
	/// </summary>
	public string Name { get; }
}
