namespace LGP.EventEditor {
    /// <summary>
    /// Determines which way a page triggers.
    /// Interaction => Triggers only with the "GameEvent.Interact(GameEvent event)" call.
    /// Autorun => Automaticly triggers after the setup of the active page.
    /// Custom => Triggers only when the custom trigger implemented by the interface CheckCustomTrigger returns true.
    /// </summary>
    public enum ETriggerMode {
        Interaction,
        Autorun,
        Custom,
    }

    /// <summary>
    /// Numerical operation mode.
    /// </summary>
    public enum ENummeralCondition {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterOrEvenThan,
        LesserThan,
        LesserOrEvenThan
    }

    /// <summary>
    /// Boolean operation modes.
    /// </summary>
    public enum EBoolConditionMode {
        Same,
        NotSame
    }

    /// <summary>
    /// String operation modes.
    /// </summary>
    public enum EStringConditionMode {
        Same,
        NotSame
    }

    /// <summary>
    /// Condition types.
    /// </summary>
    public enum EConditionObjectType {
        Boolean,
        Integer,
        Float,
        String
    }

    public enum EConditionType {
        LocalSwtich,
        GlobalSwtich,
        GameObject
    }
}