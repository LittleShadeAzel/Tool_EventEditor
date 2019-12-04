namespace LGP.EventEditor {
    /// <summary>
    /// Custom Trigger interface for the EventEditor. It is used to create a custom trigger wich can be implemented in all sorts of things like collision check, etc.
    /// </summary>
    public interface IEECustomTrigger {
        /// <summary>
        /// Is called by the EventEditor when the page trigger is defined as "Custom."
        /// </summary>
        /// <returns>Determiens if the custom trigger is active or not.</returns>
        bool CustomTrigger();
    }
}