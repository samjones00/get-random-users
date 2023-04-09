namespace MyHomework.Interfaces
{
    internal interface IUserService
    {
        Task GetAndSaveUsers(CancellationToken cancellationToken);
    }
}