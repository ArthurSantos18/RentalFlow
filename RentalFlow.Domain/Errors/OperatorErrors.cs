namespace RentalFlow.Domain.Errors;

public static class OperatorErrors
{
    public static readonly Error OperatorNotFound = new(404, "Operator not found.");
    public static readonly Error OperatorNotInTeam = new(403, "You can only manage operators in your own team.");
    public static readonly Error CannotMoveToDifferentTeam = new(403, "Managers can only move operators to their own team.");
    public static readonly Error ManagerCannotManageAdmin = new(403, "Managers cannot manage administrators.");
    public static readonly Error CannotPromoteToAdmin = new(403, "Managers cannot promote operators to administrators.");
    public static readonly Error CannotDemoteLastAdmin = new(400, "Cannot demote the last administrator.");
    public static readonly Error CannotDeleteSelf = new(400, "You cannot delete your own account.");
    public static readonly Error CannotDeleteLastAdmin = new(400, "Cannot delete the last administrator.");
    public static readonly Error CannotDeactivateLastAdmin = new(400, "Cannot deactivate the last administrator.");
}