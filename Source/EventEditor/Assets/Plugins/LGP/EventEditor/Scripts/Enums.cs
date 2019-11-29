namespace LGP.EventEditor {
    /// <summary>
    /// Determines which way a page triggers.
    /// </summary>
    public enum ETriggerMode {
        Interaction,
        Autorun,
        Collision,
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