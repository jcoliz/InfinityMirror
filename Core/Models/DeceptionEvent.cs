using System.Text.Json.Serialization;

namespace InfinityMirror.Core.Models;

/// <summary>
/// Infinity Mirror Deception Event (InfinityMirrorDeceptionEvent_CL)
/// </summary>
/// <remarks>
/// Infinity Mirror is a fictional startup building a deception-based
/// security solution. The platform deploys decoys, lures, and
/// honeytokens to detect attackers inside networks with high fidelity.
///
/// Our solution creates a “hall of mirrors” within the enterprise
/// environment – planting decoy assets (servers, credentials, files)
/// that lure attackers into revealing themselves. When an attacker
/// interacts with a decoy, our system not only raises an immediate
/// alert, but also automatically correlates related signals via
/// Microsoft Sentinel.
/// </remarks>
public record DeceptionEvent
{
    /// <summary>
    /// UTC time the message was generated on the client side.
    /// </summary>
    public DateTimeOffset TimeOnClient { get; set; }

    /// <summary>
    /// Unique identifier GUID for the event. This is a string to match the template format.
    /// </summary>
    public string? Id { get; set; } = null;

    /// <summary>
    /// Detailed description of the event (might include attacker actions)
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Dynamic properties associated with the message
    /// </summary>
    public MessageProperties Properties { get; set; } = new();

    /// <summary>
    /// Category of event (policy or trap type) associated with the message (e.g. "Network", "Host", "Application")
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// How severe of an attack is indicated by this event
    /// </summary>
    public SeverityLevel? Severity { get; set; }

    /// <summary>
    /// Campaign name associated with the event (if applicable). This can be used to group related events together.
    /// </summary>
    public string? Campaign { get; set; }

    /// <summary>
    /// Information about the decoy asset that generated the event. This includes the decoy name, type, and any associated comments.
    /// </summary>
    public DecoyInfo Decoy { get; set; } = new();

    /// <summary>
    /// Describes  the attacker activity which was detected, leading to the event.
    /// </summary>
    public string? DeviceEventClass { get; set; }

    /// <summary>
    /// Attacker IP address
    /// </summary>
    public string? SourceAddress { get; set; }

    /// <summary>
    /// Attacker host name (if resolved)
    /// </summary>
    public string? SourceHostName { get; set; }

    /// <summary>
    /// Attacker machine unique identifier GUID (if available from the decoy system)
    /// </summary>
    public string? SourceHostId { get; set; } // GUID changed to string to match template format

    /// <summary>
    /// Decoy asset IP that was targeted
    /// </summary>
    public string? DestinationAddress { get; set; }

    /// <summary>
    /// Decoy service port targeted (if network connection)
    /// </summary>
    public string? DestinationPort { get; set; }

    /// <summary>
    /// MITRE ATT&CK technique associated with the event (if applicable). This includes the tactic and technique name and ID.
    /// </summary>
    public MitreTechnique MitreTechnique { get; set; } = new();

    /// <summary>
    /// Hash of any malware or tool captured during the event (if applicable). This can be used to identify known malicious files.
    /// </summary>
    public string? FileHash { get; set; }

    /// <summary>
    /// User account name associated with the event (if applicable). This can help identify which user was involved in the activity.
    /// </summary>
    public string? User { get; set; }
}

public record MitreTechnique
{
    public MitreTactic Tactic { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public record MitreTactic
{
    public MitreTacticName Name { get; set; }
    public string Id { get; set; } = string.Empty;
}

public record DecoyInfo
{
    public string Name { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DecoyType Type { get; set; }
    public string? Id { get; set; } = null; // GUID changed to string to match template format
}

public record MessageProperties
{
    public Guid? SessionId { get; set; }
    public string? Comment { get; set; }
    public int SequenceNumber { get; set; }
    public MessageGenerationProperties? Generation { get; set; }
}

public record MessageGenerationProperties
{
    public int MessagesPerInterval { get; set; } = 10;
    public GenerationInterval Interval { get; set; } = GenerationInterval.Cycle;
}

[JsonConverter(typeof(JsonStringEnumConverter<SeverityLevel>))]
public enum SeverityLevel
{
    Unknown,
    Debug,
    Low,
    Medium,
    High,
    Critical
}

[JsonConverter(typeof(JsonStringEnumConverter<DecoyType>))]
public enum DecoyType
{
    VirtualMachine,
    NetworkDevice,
    Application,
    Cluster,
    Database,
    File,
    FileShare,
    Identity,
    IoTSensor,
    Other
}

[JsonConverter(typeof(JsonStringEnumConverter<MitreTacticName>))]
public enum MitreTacticName
{
    Unknown,
    Reconnaissance,
    ResourceDevelopment,
    InitialAccess,
    Execution,
    Persistence,
    PrivilegeEscalation,
    DefenseEvasion,
    CredentialAccess,
    Discovery,
    LateralMovement,
    Collection,
    Exfiltration,
    CommandAndControl,
    Impact
}

[JsonConverter(typeof(JsonStringEnumConverter<GenerationInterval>))]
public enum GenerationInterval
{
    Never,
    Session,
    Cycle,
}
