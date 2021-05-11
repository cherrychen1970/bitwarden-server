namespace Bit.Core.Enums
{
    public enum CollectionAccessType : byte
    {
        // Folder is deprecated
        //Folder = 0,
        All = 1,
        ManagerAbove = 2, // manager,admin,assignees
        AdminAbove = 3,  // admin, assignees      
    }
}
