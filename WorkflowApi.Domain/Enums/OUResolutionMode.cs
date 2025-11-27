namespace WorkflowApi.Domain.Enums
{
    public enum OUResolutionMode
    {
        Fixed = 1,              // Use a fixed organizational unit
        FollowOrigin = 2,       // Follow the originator's organizational unit
        FollowInitiator = 3     // Follow the initiator's organizational unit
    }
}