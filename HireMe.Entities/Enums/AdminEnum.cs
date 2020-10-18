namespace HireMe.Entities.Enums
{
    public enum PostState
    {
     Unapproved,
     Approved 
    }

    public enum PostType : int
    {
        Company = 0,
        Contestant = 1,
        Job = 2
    }
}